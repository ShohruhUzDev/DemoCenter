using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.GroupStudents.Exceptions;
using DemoCenter.Services.Foundations.GroupStudents;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace DemoCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupStudentsController : RESTFulController
    {
        private readonly IGroupStudentService groupStudentService;

        public GroupStudentsController(IGroupStudentService groupStudentService) =>
         this.groupStudentService = groupStudentService;

        [HttpPost]
        public async ValueTask<ActionResult<GroupStudent>> PostGroupStudentAsync(GroupStudent groupStudent)
        {
            try
            {
                
                GroupStudent createdGroupStudent =
                    await this.groupStudentService.AddGroupStudentAsync(groupStudent);

                return Created(createdGroupStudent);
            }
            catch (GroupStudentValidationException groupStudentValidationException)
            {
                return BadRequest(groupStudentValidationException.InnerException);
            }
            catch (GroupStudentDependencyValidationException groupStudentDependencyValidationException)
                when (groupStudentDependencyValidationException.InnerException is InvalidGroupStudentReferenceException)
            {
                return FailedDependency(groupStudentDependencyValidationException.InnerException);
            }
            catch (GroupStudentDependencyValidationException groupDependencyValidationException)
                when (groupDependencyValidationException.InnerException is AlreadyExistsGroupStudentException)
            {
                return Conflict(groupDependencyValidationException.InnerException);
            }
            catch (GroupStudentDependencyException groupStudentDependencyException)
            {
                return InternalServerError(groupStudentDependencyException.InnerException);
            }
            catch (GroupStudentServiceException groupStudentServiceException)
            {
                return InternalServerError(groupStudentServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<GroupStudent>> GetAllGroupStudents()
        {
            try
            {
                IQueryable<GroupStudent> allGroupStudents =
                    this.groupStudentService.RetrieveAllGroupStudents();

                return Ok(allGroupStudents);
            }
            catch (GroupStudentDependencyException groupStudentDependencyException)
            {
                return InternalServerError(groupStudentDependencyException.InnerException);
            }
            catch (GroupStudentServiceException groupStudentServiceException)
            {
                return InternalServerError(groupStudentServiceException.InnerException);
            }

        }

        [HttpGet("groupStudentId")]
        public async ValueTask<ActionResult<GroupStudent>> GetGroupStudentByIdAsync(Guid groupId, Guid studentId)
        {
            try
            {
                GroupStudent groupStudent =
                    await this.groupStudentService.RetrieveGroupStudentByIdAsync(groupId, studentId);

                return Ok(groupStudent);
            }
            catch (GroupStudentValidationException groupStudentValidationException)
                when (groupStudentValidationException.InnerException is NotFoundGroupStudentException)
            {
                return NotFound(groupStudentValidationException.InnerException);
            }
            catch (GroupStudentValidationException groupStudentValidationException)
            {
                return BadRequest(groupStudentValidationException.InnerException);
            }
            catch (GroupStudentDependencyException groupStudentDependencyException)
            {
                return InternalServerError(groupStudentDependencyException.InnerException);
            }
            catch (GroupStudentServiceException groupStudentServiceException)
            {
                return InternalServerError(groupStudentServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<GroupStudent>> PutGroupStudentAsync(GroupStudent groupStudent)
        {
            try
            {
                GroupStudent updatedGroupStudent = 
                    await this.groupStudentService.ModifyGroupStudentAsync(groupStudent);

                return Ok(updatedGroupStudent); 
            }
            catch (GroupStudentValidationException groupStudentValidationException)
                when(groupStudentValidationException.InnerException is NotFoundGroupStudentException)
            {
                return NotFound(groupStudentValidationException.InnerException);
            }
            catch(GroupStudentValidationException groupStudentValidationException)
            {
                return BadRequest(groupStudentValidationException.InnerException);
            }
            catch(GroupStudentDependencyValidationException groupStudentDependencyValidationException)  
                when(groupStudentDependencyValidationException.InnerException is InvalidGroupStudentReferenceException)
            {
                return FailedDependency(groupStudentDependencyValidationException.InnerException);
            }
            catch(GroupStudentDependencyValidationException groupDependencyValidationException)
                when(groupDependencyValidationException.InnerException is AlreadyExistsGroupStudentException)
            {
                return Conflict(groupDependencyValidationException.InnerException);
            }
            catch(GroupStudentDependencyException groupStudentDependencyException)
            {
                return InternalServerError(groupStudentDependencyException.InnerException);
            }
            catch(GroupStudentServiceException groupStudentServiceException)
            {
                return InternalServerError(groupStudentServiceException.InnerException);    
            }
        }


        [HttpDelete("groupStudentId")]
        public async ValueTask<ActionResult<GroupStudent>> DeleteGroupStudentAsync(Guid groupId, Guid studentId)
        {
            try
            {
                GroupStudent deletedGroupStudent =
                    await this.groupStudentService.RemoveGroupStudentByIdAsync(groupId, studentId);

                return Ok(deletedGroupStudent);
            }
            catch (GroupStudentValidationException groupStudentValidationException)
                when( groupStudentValidationException.InnerException is NotFoundGroupStudentException)
            {
                return NotFound(groupStudentValidationException.InnerException);               
            }
            catch(GroupStudentValidationException groupStudentValidationException)
            {
                return BadRequest(groupStudentValidationException.InnerException);
            }
            catch(GroupStudentDependencyValidationException groupStudentDependencyValidationException)
                when(groupStudentDependencyValidationException.InnerException is LockedGroupStudentException)
            {
                return Locked(groupStudentDependencyValidationException.InnerException);
            }
            catch(GroupStudentDependencyValidationException groupStudentDependencyValidationException)
            {
                return BadRequest(groupStudentDependencyValidationException.InnerException);
            }
            catch(GroupStudentDependencyException groupStudentDependencyException)
            {
                return InternalServerError(groupStudentDependencyException.InnerException); 
            }
            catch(GroupStudentServiceException groupStudentServiceException)
            {
                return InternalServerError(groupStudentServiceException.InnerException);
            }
           
        }
    }
}
