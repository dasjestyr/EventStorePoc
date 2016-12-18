using EventStorePoc.Lib;
using EventStorePoc.Lib.EventStoreFacade;
using EventStorePoc.Lib.EventStoreRepository;

namespace EventStorePoc.Domain.AccountConfiguration
{
    public class AccountConfigurationRepository : Repository<AccountConfiguration>
    {
        public AccountConfigurationRepository(IStoreEvents eventStore) 
            : base("AccountConfiguration", eventStore)
        {
        }
    }
}