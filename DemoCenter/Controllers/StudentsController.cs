using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;
using DemoCenter.Services.Foundations.Students;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace DemoCenter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : RESTFulController
    {
        private readonly IStudentService studentService;

        public StudentsController(IStudentService studentService) =>
            this.studentService = studentService;

        [HttpPost]
        public async ValueTask<ActionResult<Student>> PostStudentAsync(Student student)
        {
            try
            {
                Student createdStudent = await this.studentService.AddStudentAsync(student);

                return Created(createdStudent);
            }
            catch (StudentValidationException studentValidationException)
            {
                return BadRequest(studentValidationException.InnerException);
            }
            catch (StudentDependencyValidationException studentValidationException)
                when (studentValidationException.InnerException is AlreadyExistsStudentException)
            {
                return Conflict(studentValidationException.InnerException);
            }
            catch (StudentDependencyValidationException studentDependencyValidationException)
            {
                return BadRequest(studentDependencyValidationException.InnerException);
            }
            catch (StudentDependencyException studentDependencyException)
            {
                return InternalServerError(studentDependencyException.InnerException);
            }
            catch (StudentServiceException studentServiceException)
            {
                return InternalServerError(studentServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Student>> GetAllStudents()
        {
            try
            {
                IQueryable<Student> allStudents = this.studentService.RetrieveAllStudents();

                return Ok(allStudents);
            }
            catch (StudentDependencyException studentDependencyException)
            {
                return InternalServerError(studentDependencyException.InnerException);
            }
            catch(StudentServiceException studentServiceException)
            {
                return InternalServerError(studentServiceException.InnerException); 
            }
        }

    }
}
