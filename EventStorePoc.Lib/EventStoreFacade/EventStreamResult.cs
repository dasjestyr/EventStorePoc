using System.Collections.Generic;

namespace EventStorePoc.Lib.EventStoreFacade
{
    public class EventStreamResult
    {
        public List<EventInfo> Events { get; set; }

        public bool NeedsSnapshot { get; set; }
    }
}