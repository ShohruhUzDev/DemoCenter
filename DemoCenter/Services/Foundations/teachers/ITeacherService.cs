using System.Threading.Tasks;
using DemoCenter.Models.Teachers;

namespace DemoCenter.Services.Foundations.Teachers
{
    public interface ITeacherService
    {
        ValueTask<Teacher> AddTeacherAsync(Teacher teacher);
    }
}
