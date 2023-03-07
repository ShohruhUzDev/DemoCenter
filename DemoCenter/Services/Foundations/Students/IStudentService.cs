using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Students;

namespace DemoCenter.Services.Foundations.Students
{
    public interface IStudentService
    {
        ValueTask<Student> AddStudentAsync(Student student);
        IQueryable<Student> RetrieveAllStudents();
        ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId);
        ValueTask<Student> ModifyStudentAsync(Student student);
        ValueTask<Student> RemoveStudentByIdAsync(Guid studentId);
    }
}