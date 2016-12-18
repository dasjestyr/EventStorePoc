using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using EventStorePoc.Domain;
using EventStorePoc.Domain.AccountConfiguration;
using EventStorePoc.Lib;
using EventStorePoc.Lib.EventStoreFacade;
using EventStorePoc.Lib.EventStoreRepository;

namespace EventStorePoc.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly IRepository<AccountConfiguration> _accountRepo;

        public AccountController()
        {
            var store = new EventStore("tcp://admin:changeit@winsrv01:1113");
            _accountRepo = new AccountConfigurationRepository(store);
        }

        [Route(""), HttpPost]
        public async Task<IHttpActionResult> CreateAccount()
        {
            var id = Guid.Parse("97ba83fe-e649-4f17-a7e6-9c3347569156");
            var account = new AccountConfiguration();

            account.Create(id, "Call to API");
            account.ActivateAccount("Call to API");
            
            await _accountRepo.Save(account);

            return Created($"/api/Account/{id}", account);
        }

        [Route("{id:Guid}"), HttpGet]
        public async Task<IHttpActionResult> GetAccount(Guid id)
        {
            var account = await _accountRepo.GetById(id, true);
            return Ok(account);
        }

        [Route("{id:Guid"), HttpPost]
        public async Task<IHttpActionResult> DeactiveAccount(Guid id)
        {
            var account = await _accountRepo.GetById(id, true);
            account.DeactivateAccount("Call to API");

            await _accountRepo.Save(account);

            return StatusCode(HttpStatusCode.Accepted);
        }
    }
}
