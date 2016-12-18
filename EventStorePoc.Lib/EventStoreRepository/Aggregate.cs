using System;
using System.Collections;
using System.Collections.Generic;
using EventStorePoc.Lib.EventStoreFacade;
using EventInfo = EventStorePoc.Lib.EventStoreFacade.EventInfo;

namespace EventStorePoc.Lib.EventStoreRepository
{
    public abstract class Aggregate : IAggregate
    {
        private readonly List<EventInfo> _changes = new List<EventInfo>();

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; protected set; }

        /// <summary>
        /// Gets the uncommitted events.
        /// </summary>
        /// <returns></returns>
        public ICollection GetUncommittedEvents()
        {
            return _changes;
        }

        /// <summary>
        /// Clears the uncommitted events.
        /// </summary>
        public void ClearUncommittedEvents()
        {
            _changes.Clear();
        }

        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <returns></returns>
        public abstract EventInfo GetSnapshot();

        /// <summary>
        /// Marks the changes as committed.
        /// </summary>
        public void MarkChangesAsCommitted()
        {
            ClearUncommittedEvents();
        }

        /// <summary>
        /// Loads from history.
        /// </summary>
        /// <param name="history">The history.</param>
        public void LoadFromHistory(IEnumerable<EventInfo> history)
        {
            foreach (var e in history)
                ApplyEvent((dynamic) e, false);
        }

        /// <summary>
        /// Applies the event.
        /// </summary>
        /// <param name="event">The event.</param>
        public void ApplyEvent(EventInfo @event)
        {
            ApplyEvent(@event, true);
        }
        
        private void ApplyEvent(EventInfo @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);

            if (isNew)
                _changes.Add(@event);
        }
    }
}
