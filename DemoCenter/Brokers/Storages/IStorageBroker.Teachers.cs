using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Teachers;

namespace DemoCenter.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Teacher> InsertTeacherAsync(Teacher teacher);
        IQueryable<Teacher> SelectAllTeachers();
        ValueTask<Teacher> SelectTeacherByIdAsync(Guid id);
        ValueTask<Teacher> UpdateTeacherAsync(Teacher teacher);
        ValueTask<Teacher> DeleteTeacherAsync(Teacher teacher);
    }
}
