using Microsoft.EntityFrameworkCore;
using RpcLite.Monitor.Service.Models;

namespace RpcLite.Monitor.Service.Dal
{
	public class ServiceDataContext : DbContext
	{
		public DbSet<InvokeItem> Invoke { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql("server=localhost;user id=chris;password=chris123;database=RpcMonitor");
			//optionsBuilder.UseSqlite("Data Source=rpclite.db;Version=3;");
			//optionsBuilder.UseSqlServer("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RpcLite;Integrated Security=True");
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//set table names
			modelBuilder.Entity<InvokeItem>().ToTable("Invoke");

			modelBuilder.Entity<InvokeItem>().HasKey(b => b.Id);
			modelBuilder.Entity<InvokeItem>().Property(it => it.Service).HasMaxLength(64).IsRequired();
			modelBuilder.Entity<InvokeItem>().Property(it => it.StartDate).IsRequired();

			base.OnModelCreating(modelBuilder);
		}

	}
}
