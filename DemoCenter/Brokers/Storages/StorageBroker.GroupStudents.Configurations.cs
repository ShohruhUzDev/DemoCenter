using DemoCenter.Models.GroupStudents;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddGroupStudentConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupStudent>()
                .HasKey(groupStudent =>
                new { groupStudent.GroupId, groupStudent.StudentId });

            modelBuilder.Entity<GroupStudent>()
                .HasOne(groupStudent => groupStudent.Student)
                .WithMany(student => student.GroupStudents)
                .HasForeignKey(groupStudent => groupStudent.GroupId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GroupStudent>()
                .HasOne(groupStudent => groupStudent.Group)
                .WithMany(group => group.GroupStudents)
                .HasForeignKey(groupStudents => groupStudents.GroupId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
