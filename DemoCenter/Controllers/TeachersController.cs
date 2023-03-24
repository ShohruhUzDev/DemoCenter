using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using DemoCenter.Services.Foundations.Teachers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using RESTFulSense.Controllers;

namespace DemoCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : RESTFulController
    {
        private readonly ITeacherService teacherService;

        public TeachersController(ITeacherService teacherService) =>
            this.teacherService = teacherService;

        [HttpPost]
        public async ValueTask<ActionResult<Teacher>> PostTeacherAsync(Teacher teacher)
        {
            try
            {
                return Created(await this.teacherService.AddTeacherAsync(teacher));
            }

            catch (TeacherValidationException TeacherValidationExpection)
            {
                return BadRequest(TeacherValidationExpection.InnerException);
            }
            catch (TeacherDependencyValidationException TeacherDependencyValidationException)
                when (TeacherDependencyValidationException.InnerException is AlreadyExistTeacherException)
            {
                return Conflict(TeacherDependencyValidationException.InnerException);
            }
            catch (TeacherDependencyValidationException TeacherDependencyValidationException)
            {
                return BadRequest(TeacherDependencyValidationException.InnerException);
            }
            catch (TeacherDependencyException TeacherDependencyException)
            {
                return InternalServerError(TeacherDependencyException.InnerException);
            }
            catch (TeacherServiceException TeacherServiceException)
            {
                return InternalServerError(TeacherServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Teacher>> GetAllTeachers()
        {
            try
            {
                IQueryable<Teacher> teachers = this.teacherService.RetrieveAllTeachers();

                return Ok(teachers);
            }
            catch (TeacherDependencyException teacherDependencyException)
            {
                return InternalServerError(teacherDependencyException);
            }
            catch (TeacherServiceException teacherServiceException)
            {
                return InternalServerError(teacherServiceException);
            }
        }

        [HttpGet("{teacherId}")]
        public async ValueTask<ActionResult<Teacher>> GetTeacherByIdAsync(Guid teacherId)
        {
            try
            {
                Teacher teacher = await this.teacherService.RetrieveTeacherByIdAsync(teacherId);

                return Ok(teacher);
            }
            catch (TeacherValidationException teacherValidationException)
                when (teacherValidationException.InnerException is NotFoundTeacherException)
            {
                return NotFound(teacherValidationException.InnerException);
            }
            catch (TeacherValidationException teacherValidationException)
            {
                return BadRequest(teacherValidationException.InnerException);
            }
            catch (TeacherDependencyException teacherDependencyException)
            {
                return InternalServerError(teacherDependencyException);
            }
            catch (TeacherServiceException teacherServiceException)
            {
                return InternalServerError(teacherServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Teacher>> PutTeacherAsync(Teacher teacher)
        {
            try
            {
                teacher.UpdatedDate = DateTimeOffset.Now;
                Teacher modifiedTeacher = await this.teacherService.ModifyTeacherAsync(teacher);

                return Ok(modifiedTeacher);
            }
            catch (TeacherValidationException teacherValidationException)
                when (teacherValidationException.InnerException is NotFoundTeacherException)
            {
                return NotFound(teacherValidationException.InnerException);
            }
            catch (TeacherValidationException teacherValidationException)
            {
                return BadRequest(teacherValidationException.InnerException);
            }
            catch (TeacherDependencyValidationException teacherDependencyValidationException)
            {
                return BadRequest(teacherDependencyValidationException.InnerException);
            }
            catch (TeacherDependencyException teacherDependencyException)
            {
                return InternalServerError(teacherDependencyException.InnerException);
            }
            catch (TeacherServiceException teacherServiceException)
            {
                return InternalServerError(teacherServiceException.InnerException);
            }

        }

        [HttpDelete("{teacherId}")]
        public async ValueTask<ActionResult<Teacher>> DeleteTeacherAsync(Guid teacherId)
        {
            try
            {
                Teacher deletedTeacher = await this.teacherService.RemoveTeacherByIdAsync(teacherId);

                return Ok(deletedTeacher);
            }
            catch (TeacherValidationException teacherValidationException)
                when(teacherValidationException.InnerException is NotFoundTeacherException)
            {
                return NotFound(teacherValidationException.InnerException);
            }
            catch(TeacherValidationException teacherValidationException)
            {
                return BadRequest(teacherValidationException.InnerException);
            }
            catch(TeacherDependencyValidationException teaacherDependencyValidationException)
                when(teaacherDependencyValidationException.InnerException is LockedTeacherException)
            {
                return Locked(teaacherDependencyValidationException.InnerException);
            }
            catch(TeacherDependencyValidationException teacherDependencyValidationException)
            {
                return BadRequest(teacherDependencyValidationException.InnerException);
            }
            catch(TeacherDependencyException teacherDependencyException)
            {
                return InternalServerError(teacherDependencyException.InnerException);
            }
            catch(TeacherServiceException teacherServiceException)
            {
                return InternalServerError(teacherServiceException.InnerException);
            }
        }
    }
}

