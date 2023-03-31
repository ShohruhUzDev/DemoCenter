using DemoCenter.Models.Foundations.Groups;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddGroupConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasOne(group => group.Teacher)
                .WithMany(teacher => teacher.Groups)
                .HasForeignKey(group => group.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Group>()
                .HasOne(group => group.Subject)
                .WithMany(subject => subject.Groups)
                .HasForeignKey(group => group.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
