using System.Threading.Tasks;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using DemoCenter.Services.Foundations.Teachers;
using Microsoft.AspNetCore.Mvc;
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
        
    }
}
