using DemoCenter.Models.Subjects;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Subject> Subjects { get; set; }
    }
}
