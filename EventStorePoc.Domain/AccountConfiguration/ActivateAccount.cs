using System;
using EventStorePoc.Lib;
using EventStorePoc.Lib.EventStoreFacade;

namespace EventStorePoc.Domain.AccountConfiguration
{
    public class ActivateAccount : EventInfo
    {
        public ActivateAccount(Guid id, string reason)
            : base(id, reason)
        {
        }
    }
}