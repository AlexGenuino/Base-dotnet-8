namespace Infra.Persistence
{
    public class TasksDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<ListCard> ListsCards { get; set; }
        public DbSet<Card> Cards {  get; set; } 
    }
}
