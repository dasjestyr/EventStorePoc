using System;
using System.Linq;
using System.Threading.Tasks;
using EventStorePoc.Lib.EventStoreFacade;

namespace EventStorePoc.Lib.EventStoreRepository
{
    public abstract class Repository<T> : IRepository<T> 
        where T : Aggregate, new()
    {
        private readonly IStoreEvents _eventStore;
        private readonly string _streamNameFormat;
        private readonly string _streamSnapshotFormat;

        protected Repository(string streamName, IStoreEvents eventStore)
        {
            _eventStore = eventStore;
            _streamNameFormat = $"{streamName}:{{0}}";
            _streamSnapshotFormat = $"{_streamNameFormat}-snapshot";
        }

        public async Task Save(T entity)
        {
            var events = entity.GetUncommittedEvents();

            if (events.Count == 0)
                return;

            await _eventStore
                .Connect()
                .ConfigureAwait(false);

            var streamName = string.Format(_streamNameFormat, entity.Id);

            await _eventStore
                .AppendEvents(
                    streamName, 
                    events.Cast<EventInfo>()
                    .ToArray())
                .ConfigureAwait(false);

            _eventStore.Disconnect();

            entity.MarkChangesAsCommitted();
        }

        public async Task<T> GetById(Guid id, bool useSnapshot)
        {
            var snapshotName = string.Format(_streamSnapshotFormat, id);

            await _eventStore
                .Connect()
                .ConfigureAwait(false);

            var startPosition = 0;
            EventInfo snapshot = null;
            if (useSnapshot)
            {
                snapshot = await _eventStore
                    .GetSnapshot(snapshotName)
                    .ConfigureAwait(false);

                // if we have a snapshot, start on the next event
                // otherwise, we need everything to get a good state
                startPosition = snapshot?.Version + 1 ?? 0;
            }

            var result = await _eventStore
                .GetEvents(
                    string.Format(_streamNameFormat, id),
                    startPosition)
                .ConfigureAwait(false);

            var events = result.Events;
            var entity = new T();

            // snapshot must load first
            if(snapshot != null)
                events.Insert(0, snapshot);

            entity.LoadFromHistory(events);
            
            if (!result.NeedsSnapshot)
                return entity;

            // save a new snapshot
            var newSnapshot = entity.GetSnapshot();
            newSnapshot.Version = events.Last().Version;

            await _eventStore
                .AppendEvents(snapshotName, newSnapshot)
                .ConfigureAwait(false);

            _eventStore.Disconnect();
            return entity;
        }
    }
}
