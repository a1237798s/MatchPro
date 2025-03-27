using Microsoft.EntityFrameworkCore;

namespace BackMange.Models
{
    public partial class GameCaseContext : DbContext
    {
        public GameCaseContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration Config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

                optionsBuilder.UseSqlServer(Config.GetConnectionString("GameCase"));
            }
        }

    }
}
