using DemoCenter.Models.Teachers;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Teacher> Teachers { get; set; }
    }
}
