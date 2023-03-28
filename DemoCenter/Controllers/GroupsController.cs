using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Groups.Exceptions;
using DemoCenter.Services.Foundations.Groups;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace DemoCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : RESTFulController
    {
        private readonly IGroupService groupService;

        public GroupsController(IGroupService groupService) =>
            this.groupService = groupService;

        [HttpPost]
        public async ValueTask<ActionResult<Group>> PostGroupAsync(Group group)
        {
            try
            {
                return Created(await this.groupService.AddGroupAsync(group));
            }
            catch (GroupValidationException groupValidationException)
            {
                return BadRequest(groupValidationException.InnerException);
            }
            catch (GroupDependencyValidationException groupDependencyValidationException)
                when (groupDependencyValidationException.InnerException is InvalidGroupReferenceException)
            {
                return FailedDependency(groupDependencyValidationException.InnerException);
            }
            catch (GroupDependencyValidationException groupDependencyException)
                when (groupDependencyException.InnerException is AlreadyExistGroupException)
            {
                return Conflict(groupDependencyException.InnerException);
            }
            catch (GroupDependencyValidationException groupDependencyValidationException)
            {
                return BadRequest(groupDependencyValidationException.InnerException);
            }
            catch (GroupDependencyException groupDependencyException)
            {
                return InternalServerError(groupDependencyException.InnerException);
            }
            catch (GroupServiceException groupServiceException)
            {
                return InternalServerError(groupServiceException.InnerException);
            }

        }

        [HttpGet]
        public ActionResult<IQueryable<Group>> GetAllGroups()
        {
            try
            {
                IQueryable<Group> allGroups = this.groupService.RetrieveAllGroups();

                return Ok(allGroups);
            }
            catch (GroupDependencyException groupDepdencyException)
            {
                return InternalServerError(groupDepdencyException.InnerException);
            }
            catch(GroupServiceException groupServiceException)
            {
                return InternalServerError(groupServiceException.InnerException);   
            }
        }

        
    }
}
