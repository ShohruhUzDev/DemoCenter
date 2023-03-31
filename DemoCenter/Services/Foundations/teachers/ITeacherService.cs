using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Teachers;

namespace DemoCenter.Services.Foundations.Teachers
{
    public interface ITeacherService
    {
        ValueTask<Teacher> AddTeacherAsync(Teacher teacher);
        IQueryable<Teacher> RetrieveAllTeachers();
        ValueTask<Teacher> RetrieveTeacherByIdAsync(Guid teacherId);
        ValueTask<Teacher> ModifyTeacherAsync(Teacher teacher);
        ValueTask<Teacher> RemoveTeacherByIdAsync(Guid teacherid);
    }
}
