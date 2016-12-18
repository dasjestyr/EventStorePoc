using System;
using EventStorePoc.Lib;
using EventStorePoc.Lib.EventStoreFacade;

namespace EventStorePoc.Domain.AccountConfiguration
{
    public class DeactivateAccount : EventInfo
    {
        public DeactivateAccount(Guid id, string reason) 
            : base(id, reason)
        {
        }
    }
}