using DemoCenter.Models.Students;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCenter.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Student> InsertStudentAsync(Student student);
        ValueTask<Student> UpdateStudentAsync(Student student);
        ValueTask<Student> DeleteStudentAsync(Student student);
        ValueTask<Student> SelectStudentByIdAsync(Guid id);
        IQueryable<Student> SelectAllStudents();
    }
}
