using DemoCenter.Models.Students;
using DemoCenter.Models.Students.Exceptions;

namespace DemoCenter.Services.Foundations.Students
{
    public partial class StudentService
    {
        private static void ValidationStudentNotNull(Student student)
        {
            if (student == null)
            {
                throw new NullStudentException();
            }
        }
    }
}
