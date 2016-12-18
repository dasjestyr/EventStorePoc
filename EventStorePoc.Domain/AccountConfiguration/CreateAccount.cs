using System;
using EventStorePoc.Lib;
using EventStorePoc.Lib.EventStoreFacade;

namespace EventStorePoc.Domain.AccountConfiguration
{
    public class CreateAccount : EventInfo
    {
        public Guid Id { get; set; }
        
        public CreateAccount(Guid id, string reason) 
            : base(id, reason)
        {
            Id = id;
            Reason = reason;
        }
    }
}