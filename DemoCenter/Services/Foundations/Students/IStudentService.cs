using System.Threading.Tasks;
using DemoCenter.Models.Students;

namespace DemoCenter.Services.Foundations.Students
{
    public interface IStudentService
    {
        ValueTask<Student> AddStudentAsync(Student student);
    }
}