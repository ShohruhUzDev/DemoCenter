using System;
using Xeptions;

namespace DemoCenter.Models.Students.Exceptions
{
    public class NotFoundStudentException : Xeption
    {
        public NotFoundStudentException(Guid studentId):
            base(message:$"Couldn't found student with id: {studentId}")
        {           
        }
    }
}
