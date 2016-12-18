using System;
using EventStorePoc.Lib;
using EventStorePoc.Lib.EventStoreFacade;

namespace EventStorePoc.Domain.AccountConfiguration
{
    public class AccountConfigurationSnapshot : EventInfo
    {
        public bool IsActive { get; set; }

        public Guid Id { get; set; }

        public AccountConfigurationSnapshot(Guid entityId) 
            : base(entityId, "Snapshot")
        {
        }
    }
}
