using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventStorePoc.Lib.EventStoreFacade
{
    public class EventStore : IStoreEvents
    {
        private const int MaxFetchSize = 1000;

        private readonly IEventStoreConnection _connection;

        public EventStore(string host)
        {
            var settings = ConnectionSettings
                .Create()
                .EnableVerboseLogging()
                .UseCustomLogger(new TraceLogger());

            _connection = EventStoreConnection.Create(settings, new Uri(host));
        }

        public async Task Connect()
        {
            await _connection
                .ConnectAsync()
                .ConfigureAwait(false);
        }

        public void Disconnect()
        {
            _connection.Close();
        }

        public async Task AppendEvents(string streamName, params EventInfo[] infos)
        {
            if(infos == null || !infos.Any())
                throw new ArgumentNullException(nameof(infos));
            
            var first = infos[0];

            if(infos.Any(i => i.EntityId != first.EntityId))
                throw new InvalidOperationException("All events must belong to the same entity.");
            
            var stream = infos
                .Select(info => new EventData(info.EventId, info.EventType, true, info.GetData(), null))
                .ToList();

            await _connection
                .AppendToStreamAsync(streamName, ExpectedVersion.Any, stream)
                .ConfigureAwait(false);
        }

        public async Task<EventStreamResult> GetEvents(string streamName, int startingPosition)
        {
            // collect all the slices from the starting point on
            StreamEventsSlice slice;
            do
            {
                slice = await _connection
                    .ReadStreamEventsForwardAsync(
                        streamName,
                        startingPosition,
                        MaxFetchSize,
                        true)
                    .ConfigureAwait(false);

            } while (!slice.IsEndOfStream);
            
            var events = new List<EventInfo>();
            foreach (var @event in slice.Events)
            {
                var instance = DeserializeEvent<EventInfo>(@event.Event);
                instance.Version = @event.Event.EventNumber;
                events.Add(instance);
            }

            var result = new EventStreamResult
            {
                Events = events,
                NeedsSnapshot = slice.Events.Length > MaxFetchSize
            };

            return result;
        }

        public async Task<EventInfo> GetSnapshot(string streamName)
        {
            var snapshot = await _connection
                .ReadStreamEventsBackwardAsync(
                    streamName, 
                    StreamPosition.Start, 
                    1, 
                    true)
                .ConfigureAwait(false);

            if (snapshot.Events == null || !snapshot.Events.Any())
                return null;

            var lastEvent = snapshot.Events[0];
            var state = DeserializeEvent<EventInfo>(lastEvent.Event);
            
            return state;
        }

        private static T DeserializeEvent<T>(RecordedEvent e) where T : class
        {
            var asJson = Encoding.UTF8.GetString(e.Data);
            var objectType = Type.GetType(e.EventType, true);
            var instance = JsonConvert.DeserializeObject(asJson, objectType);
            return instance as T;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return; 

            _connection.Close();
            _connection.Dispose();
        }
    }
}