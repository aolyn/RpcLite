using Microsoft.EntityFrameworkCore;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Dal
{
	public class ServiceDataContext : DbContext
	{
		public DbSet<Service> Services { get; set; }
		public DbSet<ServiceMapping> ServiceMappings { get; set; }
		public DbSet<ServiceEnvironment> ServiceEnvironments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//set table names
			modelBuilder.Entity<Service>().ToTable(nameof(Service));
			modelBuilder.Entity<ServiceMapping>().ToTable(nameof(ServiceMapping));
			modelBuilder.Entity<ServiceEnvironment>().ToTable(nameof(ServiceEnvironment));

			modelBuilder.Entity<Service>().HasKey(b => b.Id);
			modelBuilder.Entity<Service>().Property(it => it.Name).HasMaxLength(64).IsRequired();

			modelBuilder.Entity<ServiceMapping>().HasKey(b => b.Id);
			modelBuilder.Entity<ServiceMapping>().Property(it => it.Namespace).HasMaxLength(128).IsRequired();
			modelBuilder.Entity<ServiceMapping>().Property(it => it.Environment).HasMaxLength(64).IsRequired();
			modelBuilder.Entity<ServiceMapping>().Property(it => it.Address).HasMaxLength(255).IsRequired();

			modelBuilder.Entity<ServiceEnvironment>().Property(it => it.Id).HasMaxLength(64).HasColumnName("Name").IsRequired();

			modelBuilder.Entity<ServiceEnvironment>()
				.HasMany(it => it.ServiceMappings)
				.WithOne(t => t.ServiceEnvironment)
				.HasForeignKey(it => it.Environment);

			//modelBuilder.Entity<ServiceMapping>()
			//	.Property(e => e.ServiceId)
			//	.HasAnnotation(
			//		IndexAnnotation.AnnotationName,
			//		new IndexAnnotation(new IndexAttribute("ServiceMapping_UK_ServiceId_Environment_Namespace", 2)));

			//TODO: add index ServiceMapping_UK_ServiceId_Environment_Namespace

			//modelBuilder.Entity<Post>()
			//	.HasOne(p => p.Blog)
			//	.WithMany(b => b.Posts);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//optionsBuilder.use
			optionsBuilder.UseNpgsql("server=localhost;user id=chris;password=chris123;database=ShareVideo");
			//optionsBuilder.UseSqlite("Data Source=rpclite.db;Version=3;");
			//optionsBuilder.UseSqlServer("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RpcLite;Integrated Security=True");
			base.OnConfiguring(optionsBuilder);
		}
	}
}
