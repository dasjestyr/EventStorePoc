using System;
using System.Collections;
using EventStorePoc.Lib.EventStoreFacade;

namespace EventStorePoc.Lib.EventStoreRepository
{
    public interface IAggregate
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid Id { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        int Version { get; }

        /// <summary>
        /// Applies the event.
        /// </summary>
        /// <param name="event">The event.</param>
        void ApplyEvent(EventInfo @event);

        /// <summary>
        /// Gets the uncommitted events.
        /// </summary>
        /// <returns></returns>
        ICollection GetUncommittedEvents();

        /// <summary>
        /// Clears the uncommitted events.
        /// </summary>
        void ClearUncommittedEvents();

        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <returns></returns>
        EventInfo GetSnapshot();
    }
}