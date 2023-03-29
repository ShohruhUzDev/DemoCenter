using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Groups;
using DemoCenter.Models.GroupStudents.Exceptions;
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
            catch (GroupServiceException groupServiceException)
            {
                return InternalServerError(groupServiceException.InnerException);
            }
        }

        [HttpGet("{groupId}")]
        public async ValueTask<ActionResult<Group>> GetGroupByIdAsync(Guid groupId)
        {
            try
            {
                Group group = await this.groupService.RetrieveGroupByIdAsync(groupId);

                return Ok(group);
            }
            catch (GroupValidationException groupValidationException)
                when (groupValidationException.InnerException is NotFoundGroupException)
            {
                return NotFound(groupValidationException.InnerException);
            }
            catch (GroupValidationException groupValidationException)
            {
                return BadRequest(groupValidationException.InnerException);
            }
            catch (GroupDependencyException groupDepedencyException)
            {
                return InternalServerError(groupDepedencyException.InnerException);
            }
            catch (GroupServiceException groupServiceException)
            {
                return InternalServerError(groupServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Group>> PutGroupAsync(Group group)
        {
            try
            {
                Group updatedGroup = await this.groupService.ModifyGroupAsync(group);

                return Ok(updatedGroup);
            }
            catch (GroupValidationException groupValidationException)
                when (groupValidationException.InnerException is NotFoundGroupException)
            {
                return NotFound(groupValidationException.InnerException);
            }
            catch (GroupValidationException groupValidationException)
            {
                return BadRequest(groupValidationException.InnerException);
            }
            catch (GroupDependencyValidationException groupDepedencyValidationException)
                when (groupDepedencyValidationException.InnerException is InvalidGroupReferenceException)
            {
                return FailedDependency(groupDepedencyValidationException.InnerException);
            }
            catch (GroupDependencyValidationException groupDepedencyValidationException)
                when (groupDepedencyValidationException.InnerException is AlreadyExistGroupException)
            {
                return Conflict(groupDepedencyValidationException.InnerException);
            }
            catch (GroupDependencyException groupDepedencyException)
            {
                return InternalServerError(groupDepedencyException.InnerException);
            }
            catch (GroupServiceException groupServiceException)
            {
                return InternalServerError(groupServiceException.InnerException);
            }


        }

        [HttpDelete("{groupId}")]
        public async ValueTask<ActionResult<Group>> DeleteGroupAsync(Guid groupId)
        {
            try
            {
                Group deletedGroup = await this.groupService.RemoveGroupByIdAsync(groupId);

                return Ok(deletedGroup);
            }
            catch (GroupValidationException groupValidationException)
                when (groupValidationException.InnerException is NotFoundGroupException)
            {
                return NotFound(groupValidationException.InnerException);
            }
            catch (GroupValidationException groupValidationException)
            {
                return BadRequest(groupValidationException.InnerException);
            }
            catch (GroupDependencyValidationException groupDependencyValidationException)
                when (groupDependencyValidationException.InnerException is LockedGroupException)
            {
                return Locked(groupDependencyValidationException.InnerException);
            }
            catch (GroupDependencyValidationException groupDepdencyValidationException)
            {
                return BadRequest(groupDepdencyValidationException.InnerException);
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

    }
}
