﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Students;
using DemoCenter.Models.Foundations.Students.Exceptions;
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
            catch (StudentServiceException studentServiceException)
            {
                return InternalServerError(studentServiceException.InnerException);
            }
        }

        [HttpGet("{studentId}")]
        public async ValueTask<ActionResult<Student>> GetStudentByIdAsync(Guid studentId)
        {
            try
            {
                Student student = await this.studentService.RetrieveStudentByIdAsync(studentId);

                return Ok(student);
            }
            catch (StudentDependencyException studentDependencyException)
            {
                return InternalServerError(studentDependencyException.InnerException);
            }
            catch (StudentValidationException studentValidationException)
                when (studentValidationException.InnerException is InvalidStudentException)
            {
                return BadRequest(studentValidationException.InnerException);
            }
            catch (StudentValidationException studentValidationException)
                when (studentValidationException.InnerException is NotFoundStudentException)
            {
                return BadRequest(studentValidationException.InnerException);
            }
            catch (StudentServiceException studentServiceException)
            {
                return InternalServerError(studentServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Student>> PutStudentAsync(Student student)
        {
            try
            {
                Student updatedStudent = await this.studentService.ModifyStudentAsync(student);

                return Ok(updatedStudent);
            }
            catch (StudentValidationException studentValidationException)
                when (studentValidationException.InnerException is NotFoundStudentException)
            {
                return NotFound(studentValidationException.InnerException);
            }
            catch (StudentValidationException studentValidationException)
            {
                return BadRequest(studentValidationException.InnerException);
            }
            catch (StudentDependencyValidationException studentDependencyValidationException)
            {
                return BadRequest(studentDependencyValidationException);
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

        [HttpDelete("{studentId}")]
        public async ValueTask<ActionResult<Student>> DeleteStudentAsync(Guid studentId)
        {
            try
            {
                Student deletedStudent = await this.studentService.RemoveStudentByIdAsync(studentId);

                return Ok(deletedStudent);
            }
            catch (StudentValidationException studentValidationException)
                when (studentValidationException.InnerException is NotFoundStudentException)
            {
                return NotFound(studentValidationException.InnerException);
            }
            catch (StudentValidationException studentValidationException)
            {
                return BadRequest(studentValidationException.InnerException);
            }
            catch (StudentDependencyValidationException studentDependencyValidationException)
                when (studentDependencyValidationException.InnerException is LockedStudentException)
            {
                return Locked(studentDependencyValidationException.InnerException);
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
    }
}
