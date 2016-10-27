using System;
using Aolyn.Config;
using Microsoft.EntityFrameworkCore;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Dal
{
	public class ServiceDataContext : DbContext
	{
		public DbSet<Service> Services { get; set; }
		public DbSet<ServiceProvider> ServiceProviders { get; set; }
		public DbSet<ServiceGroup> ServiceGroups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//set table names
			modelBuilder.Entity<Service>().ToTable(nameof(Service));
			modelBuilder.Entity<ServiceProvider>().ToTable(nameof(ServiceProvider));
			modelBuilder.Entity<ServiceGroup>().ToTable(nameof(ServiceGroup));

			modelBuilder.Entity<Service>().HasKey(b => b.Id);
			modelBuilder.Entity<Service>().Property(it => it.Name).HasMaxLength(64).IsRequired();

			modelBuilder.Entity<ServiceProvider>().HasKey(b => b.Id);
			modelBuilder.Entity<ServiceProvider>().Property(it => it.Group).HasMaxLength(64).IsRequired();
			modelBuilder.Entity<ServiceProvider>().Property(it => it.Address).HasMaxLength(255).IsRequired();
			modelBuilder.Entity<ServiceProvider>().Property(it => it.Data).HasMaxLength(2048);

			modelBuilder.Entity<ServiceGroup>().Property(it => it.Id).HasMaxLength(64).HasColumnName("Name").IsRequired();

			modelBuilder.Entity<ServiceGroup>()
				.HasMany(it => it.ServiceProviders)
				.WithOne(t => t.ServiceGroup)
				.HasForeignKey(it => it.Group);

			//modelBuilder.Entity<ServiceMapping>()
			//	.Property(e => e.ServiceId)
			//	.HasAnnotation(
			//		IndexAnnotation.AnnotationName,
			//		new IndexAnnotation(new IndexAttribute("ServiceMapping_UK_ServiceId_Environment_Namespace", 2)));

			//modelBuilder.HasDefaultSchema("public");

			//TODO: add index ServiceMapping_UK_ServiceId_Environment_Namespace

			//modelBuilder.Entity<Post>()
			//	.HasOne(p => p.Blog)
			//	.WithMany(b => b.Posts);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			ConfigManager.Default.Entity.Use(optionsBuilder, "RegistryDb");

			//optionsBuilder.UseNpgsql("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseMySQL("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseSqlite("Data Source=rpclite.db;Version=3;");
			//optionsBuilder.UseSqlServer("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RpcLite;Integrated Security=True");
			base.OnConfiguring(optionsBuilder);
		}

	}
}
