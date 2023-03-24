using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using DemoCenter.Services.Foundations.Subjects;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace DemoCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : RESTFulController
    {
        private readonly ISubjectService subjectService;

        public SubjectsController(ISubjectService subjectService) =>
               this.subjectService = subjectService;

        [HttpPost]
        public async ValueTask<ActionResult<Subject>> PostSubjectAsync(Subject subject)
        {
            try
            {
                return await this.subjectService.AddSubjectAsync(subject);
            }
            catch (SubjectValidationException subjectValidationException)
            {
                return BadRequest(subjectValidationException.InnerException);
            }
            catch (SubjectDependencyValidationException subjectDependencyValidationException)
                when (subjectDependencyValidationException.InnerException is AlreadyExistsSubjectException)
            {
                return Conflict(subjectDependencyValidationException.InnerException);
            }
            catch (SubjectDependencyValidationException subjectDependencyValidationException)
            {
                return BadRequest(subjectDependencyValidationException.InnerException);
            }
            catch (SubjectDependencyException subjectDepedencyException)
            {
                return InternalServerError(subjectDepedencyException.InnerException);
            }
            catch (SubjectServiceException subjectServiceException)
            {
                return InternalServerError(subjectServiceException.InnerException);
            }

        }

        [HttpGet]
        public ActionResult<IQueryable<Subject>> GetAllSubjects()
        {
            try
            {
                IQueryable<Subject> allSubjects = this.subjectService.RetrieveAllSubjects();

                return Ok(allSubjects);
            }
            catch (SubjectDependencyException subjectDependencyException)
            {
                return InternalServerError(subjectDependencyException.InnerException);
            }
            catch (SubjectServiceException subjectServiceException)
            {
                return InternalServerError(subjectServiceException.InnerException);
            }
        }

        [HttpGet("{subjectId}")]
        public async ValueTask<ActionResult<Subject>> GetSubjectByIdAsync(Guid subjectId)
        {
            try
            {
                Subject subject = await this.subjectService.RetrieveSubjectByIdAsync(subjectId);

                return Ok(subject);
            }
            catch (SubjectDependencyException subjectDependencyException)
            {
                return InternalServerError(subjectDependencyException.InnerException);
            }
            catch (SubjectValidationException subjectValidationException)
                when (subjectValidationException.InnerException is InvalidSubjectException)
            {
                return BadRequest(subjectValidationException.InnerException);
            }
            catch (SubjectValidationException subjectValidationException)
                when (subjectValidationException.InnerException is NotFoundSubjectException)
            {
                return NotFound(subjectValidationException.InnerException);
            }
            catch (SubjectServiceException subjectServiceException)
            {
                return InternalServerError(subjectServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Subject>> PutSubjectAsync(Subject subject)
        {
            try
            {
                subject.UpdatedDate = DateTimeOffset.Now;
                Subject modifiedSubject =
                    await this.subjectService.ModifySubjectAsync(subject);

                return Ok(modifiedSubject);
            }
            catch (SubjectValidationException subjectValidationException)
                when (subjectValidationException.InnerException is NotFoundSubjectException)
            {
                return NotFound(subjectValidationException.InnerException);
            }
            catch (SubjectValidationException subjectValidationException)
            {
                return BadRequest(subjectValidationException.InnerException);
            }
            catch (SubjectDependencyValidationException subjectDependencyValidationException)
            {
                return BadRequest(subjectDependencyValidationException.InnerException);
            }
            catch (SubjectDependencyException subjectDependencyException)
            {
                return InternalServerError(subjectDependencyException.InnerException);
            }
            catch (SubjectServiceException subjectServiceException)
            {
                return InternalServerError(subjectServiceException.InnerException);
            }
        }

        [HttpDelete("{subjectId}")]
        public async ValueTask<ActionResult<Subject>> DeleteSubjectAsync(Guid subjectId)
        {
            try
            {
                Subject deletedSubject = await
                    this.subjectService.RemoveSubjectByIdAsync(subjectId);

                return Ok(deletedSubject);
            }
            catch (SubjectValidationException subjectValidationException)
                when (subjectValidationException.InnerException is NotFoundSubjectException)
            {
                return NotFound(subjectValidationException.InnerException);
            }
            catch (SubjectValidationException subjectValidationException)
            {
                return BadRequest(subjectValidationException.InnerException);
            }
            catch (SubjectDependencyValidationException subjectDependencyValidationException)
                when (subjectDependencyValidationException.InnerException is LockedSubjectException)
            {
                return Locked(subjectDependencyValidationException.InnerException);
            }
            catch (SubjectDependencyValidationException subjectDependencyValidationException)
            {
                return BadRequest(subjectDependencyValidationException.InnerException);
            }
            catch(SubjectDependencyException subjectDependencyException)
            {
                return InternalServerError(subjectDependencyException.InnerException);
            }
            catch (SubjectServiceException subjectServiceException)
            {
                return InternalServerError(subjectServiceException.InnerException);
            }
        }
    }
}
