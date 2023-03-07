using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;

namespace DemoCenter.Services.Foundations.Teachers
{
    public partial class TeacherService
    {
        private static void ValidationTeacherNotNull(Teacher teacher)
        {
            if (teacher is null)
            {
                throw new NullTeacherException();

            }
        }
    }
}
