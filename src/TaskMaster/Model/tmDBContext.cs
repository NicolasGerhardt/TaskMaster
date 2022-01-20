using System.Data.Entity;

namespace TaskMaster.Model
{
    internal class tmDBContext : DbContext
    {
        public DbSet<Status> Status { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}
