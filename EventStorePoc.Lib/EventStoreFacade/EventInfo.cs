using System;
using System.Text;
using Newtonsoft.Json;

namespace EventStorePoc.Lib.EventStoreFacade
{
    public abstract class EventInfo
    {
        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <value>
        /// The type of the event.
        /// </value>
        public string EventType => GetType().AssemblyQualifiedName;

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventInfo" /> class.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="reason">The reason.</param>
        protected EventInfo(Guid entityId, string reason)
        {
            EventId = Guid.NewGuid();
            EntityId = entityId;
            Reason = reason;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
