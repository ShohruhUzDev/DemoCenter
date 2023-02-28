using DemoCenter.Models.Groups;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Group> Groups { get; set; }
    }
}
