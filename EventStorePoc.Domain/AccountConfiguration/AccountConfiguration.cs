using System;
using EventStorePoc.Lib.EventStoreFacade;
using EventStorePoc.Lib.EventStoreRepository;

namespace EventStorePoc.Domain.AccountConfiguration
{
    public class AccountConfiguration : Aggregate
    {
        private bool _isActive;
        
        public void Create(Guid id, string reason)
        {
            ApplyEvent(new CreateAccount(id, reason));
        }

        public void ActivateAccount(string reason)
        {
            if(_isActive)
                throw new InvalidOperationException("Account is already active.");

            ApplyEvent(new ActivateAccount(Id, reason));
        }

        public void DeactivateAccount(string reason)
        {
            if(!_isActive)
                throw  new InvalidOperationException("Account is already inactive");

            ApplyEvent(new DeactivateAccount(Id, reason));
        }

        #region -- Apply Events

        protected void Apply(ActivateAccount e)
        {
            _isActive = true;
        }

        protected void Apply(DeactivateAccount e)
        {
            _isActive = false;
        }

        protected void Apply(CreateAccount e)
        {
            Id = e.Id;
        }

        protected void Apply(AccountConfigurationSnapshot e)
        {
            Id = e.Id;
            _isActive = e.IsActive;
        }

        #endregion

        public override EventInfo GetSnapshot()
        {
            return new AccountConfigurationSnapshot(Id)
            {
                IsActive = _isActive
            };
        }
    }
}
