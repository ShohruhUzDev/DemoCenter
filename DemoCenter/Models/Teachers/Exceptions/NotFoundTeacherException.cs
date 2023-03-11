using System;
using Xeptions;

namespace DemoCenter.Models.Teachers.Exceptions
{
    public class NotFoundTeacherException : Xeption
    {
        public NotFoundTeacherException(Guid teacherId):
            base(message:$"Couldn't found teacher with id: {teacherId}")
        {            
        }
    }
}
