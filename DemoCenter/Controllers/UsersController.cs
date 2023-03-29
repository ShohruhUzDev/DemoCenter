using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Users;
using DemoCenter.Services.Foundations.Users;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using DemoCenter.Models.Users.Exceptions;

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
            catch (UserDependencyValidationException userDependencyValidationException)
                when (userDependencyValidationException.InnerException is AlreadyExistsUserException)
            {
                return Conflict(userDependencyValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDepedencyValidationException)
            {
                return BadRequest(userDepedencyValidationException.InnerException);
            }
            catch (UserDependencyException userDepedencyException)
            {
                return InternalServerError(userDepedencyException.InnerException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<User>> GetAllUsers()
        {
            try
            {
                IQueryable<User> allUsers = this.userService.RetrieveAllUsers();

                return Ok(allUsers);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException.InnerException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException.InnerException);
            }
        }

        [HttpGet("{userId}")]
        public async ValueTask<ActionResult<User>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                User user = await this.userService.RetrieveUserByIdAsync(userId);

                return Ok(user);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError($"{userDependencyException.InnerException}");
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is InvalidUserException)
            {
                return BadRequest(userValidationException.InnerException);

            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userValidationException.InnerException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<User>> PutUserAsync(User user)
        {
            try
            {
                User updatedUser = await this.userService.ModifyUserAsync(user);

                return Ok(updatedUser);
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userValidationException.InnerException);
            }
            catch (UserValidationException userValidationException)
            {
                return BadRequest(userValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDepedencyValidationException)
            {
                return BadRequest(userDepedencyValidationException.InnerException);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException.InnerException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException.InnerException);
            }
        }

        [HttpDelete("{userId}")]
        public async ValueTask<ActionResult<User>> DeleteUserAsync(Guid userId)
        {
            try
            {
                User deletedUser = await this.userService.RemoveUserByIdAsync(userId);

                return Ok(deletedUser);
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userValidationException.InnerException);
            }
            catch (UserValidationException userValidationException)
            {
                return BadRequest(userValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDepedencyValidationException)
                when (userDepedencyValidationException.InnerException is LockedUserException)
            {
                return Locked(userDepedencyValidationException.InnerException);
            }
            catch (UserDependencyValidationException userDepedencyValidationException)
            {
                return Locked(userDepedencyValidationException.InnerException);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException.InnerException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException.InnerException);
            }

        }
    }
}
