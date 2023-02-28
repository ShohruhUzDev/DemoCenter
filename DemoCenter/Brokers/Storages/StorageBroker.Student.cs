using DemoCenter.Models.Students;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Student> Students { get; set; }
    }
}
