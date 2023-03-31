using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.GroupStudents;

namespace DemoCenter.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<GroupStudent> InsertGroupStudentAsync(GroupStudent GroupStudent);
        IQueryable<GroupStudent> SelectAllGroupStudents();
        ValueTask<GroupStudent> SelectGroupStudentByIdAsync(Guid groupId, Guid studentId);
        ValueTask<GroupStudent> UpdateGroupStudentAsync(GroupStudent groupStudent);
        ValueTask<GroupStudent> DeleteGroupStudentAsync(GroupStudent groupStudent);
    }
}
