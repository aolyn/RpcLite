using Microsoft.EntityFrameworkCore;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Dal
{
	public class ServiceDataContext : DbContext
	{
		public DbSet<Service> Services { get; set; }
		public DbSet<ServiceProducer> ServiceProducers { get; set; }
		public DbSet<ServiceGroup> ServiceGroups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//set table names
			modelBuilder.Entity<Service>().ToTable(nameof(Service));
			modelBuilder.Entity<ServiceProducer>().ToTable(nameof(ServiceProducer));
			modelBuilder.Entity<ServiceGroup>().ToTable(nameof(ServiceGroup));

			modelBuilder.Entity<Service>().HasKey(b => b.Id);
			modelBuilder.Entity<Service>().Property(it => it.Name).HasMaxLength(64).IsRequired();

			modelBuilder.Entity<ServiceProducer>().HasKey(b => b.Id);
			modelBuilder.Entity<ServiceProducer>().Property(it => it.Data).IsRequired();
			modelBuilder.Entity<ServiceProducer>().Property(it => it.Group).HasMaxLength(64).IsRequired();
			modelBuilder.Entity<ServiceProducer>().Property(it => it.Address).HasMaxLength(255).IsRequired();

			modelBuilder.Entity<ServiceGroup>().Property(it => it.Id).HasMaxLength(64).HasColumnName("Name").IsRequired();

			modelBuilder.Entity<ServiceGroup>()
				.HasMany(it => it.ServiceProducers)
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
			//optionsBuilder.Options.FindExtension<
			//optionsBuilder.use
			optionsBuilder.UseNpgsql("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseMySQL("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseSqlite("Data Source=rpclite.db;Version=3;");
			//optionsBuilder.UseSqlServer("server=localhost;user id=chris;password=chris123;database=RpcLite");
			//optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RpcLite;Integrated Security=True");
			base.OnConfiguring(optionsBuilder);
		}
	}
}
