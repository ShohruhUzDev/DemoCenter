using System.Threading.Tasks;
using DemoCenter.Models.Users;
using DemoCenter.Services.Foundations.Users;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;

namespace DemoCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : RESTFulController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService) =>
            this.userService = userService;

        [HttpPost]
        public async ValueTask<ActionResult<User>> PostUserAsync(User user)
        {
            try
            {
                User createdUser = await this.userService.AddUserAsync(user);

                return Created(createdUser);
            }
            catch (UserValidationException userValidationException)
            {
                return BadRequest(userValidationException.InnerException);
            }
            catch(UserDependencyValidationException userDependencyValidationException)
                when(userDependencyValidationException.InnerException is AlreadyExistsUserException)
            { 
                return Conflict(userDependencyValidationException.InnerException);        
            }
            catch(UserDependencyValidationException userDepedencyValidationException)
            {
                return BadRequest(userDepedencyValidationException.InnerException); 
            }
            catch(UserDependencyException userDepedencyException)
            {
                return InternalServerError(userDepedencyException.InnerException);
            }
            catch(UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException.InnerException);
            }
        }
    }
}
