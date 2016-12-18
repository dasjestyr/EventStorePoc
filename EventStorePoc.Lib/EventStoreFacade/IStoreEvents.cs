using System;
using System.Threading.Tasks;

namespace EventStorePoc.Lib.EventStoreFacade
{
    public interface IStoreEvents : IDisposable
    {
        Task AppendEvents(string streamName, params EventInfo[] infos);

        Task<EventStreamResult> GetEvents(string streamname, int startingPosition);

        Task<EventInfo> GetSnapshot(string streamName);
        
        Task Connect();

        void Disconnect();
    }
}