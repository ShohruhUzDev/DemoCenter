using DemoCenter.Models.Foundations.Users.Exceptions;
using DemoCenter.Models.Orchestrations.Exceptions;
using DemoCenter.Models.Orchestrations.UserTokens;
using DemoCenter.Services.Orchestrations;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace DemoCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountsController : RESTFulController
    {
        private readonly IUserSecurityOrchestrationService userSecurityOrchestrationService;

        public AccountsController(IUserSecurityOrchestrationService userSecurityOrchestrationService) =>
            this.userSecurityOrchestrationService = userSecurityOrchestrationService;

        [HttpGet]
        public ActionResult<UserToken> Login(string email, string password)
        {
            try
            {
                return this.userSecurityOrchestrationService.CreateUserToken(email, password);
            }
            catch (UserTokenOrchestrationValidationException userTokenOrchestrationValidationException)
                when (userTokenOrchestrationValidationException.InnerException is InvalidUserException)

            {
                return BadRequest(userTokenOrchestrationValidationException.InnerException);
            }
            catch (UserTokenOrchestrationValidationException userTokenOrchestrationValidationException)
                when (userTokenOrchestrationValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userTokenOrchestrationValidationException.InnerException);
            }
            catch (UserTokenOrchestrationDependencyException userTokenOrchestrationDependencyException)
            {
                return InternalServerError(userTokenOrchestrationDependencyException.InnerException);
            }
            catch (UserTokenOrchestrationServiceException userTokenOrchestrationServiceException)
            {
                return InternalServerError(userTokenOrchestrationServiceException.InnerException);
            }
        }
    }
}
