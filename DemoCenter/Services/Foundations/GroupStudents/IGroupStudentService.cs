using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.GroupStudents;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public interface IGroupStudentService
    {
        ValueTask<GroupStudent> AddGroupStudentAsync(GroupStudent student);
        IQueryable<GroupStudent> RetrieveAllGroupStudents();
        ValueTask<GroupStudent> RetrieveGroupStudentByIdAsync(Guid groupId, Guid studentId);
        ValueTask<GroupStudent> ModifyGroupStudentAsync(GroupStudent groupStudent);
        ValueTask<GroupStudent> RemoveGroupStudentByIdAsync(Guid groupId, Guid studentId);
    }
}
