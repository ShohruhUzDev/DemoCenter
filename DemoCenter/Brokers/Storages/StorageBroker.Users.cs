using DemoCenter.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }
    }
}
