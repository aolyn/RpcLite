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

			modelBuilder.Entity<ServiceMapping>().HasKey(b => b.Id);
			//modelBuilder.Entity<ServiceMapping>().Property(it => it.Name).HasMaxLength(128).IsRequired();
			//modelBuilder.Entity<ServiceMapping>().Property(it => it.Url).HasMaxLength(128).IsRequired();

			//modelBuilder.Entity<Category>().HasKey(b => b.Id);
			//modelBuilder.Entity<Category>().Property(it => it.Name).HasMaxLength(128).IsRequired();
			//modelBuilder.Entity<Category>().Property(it => it.Url).HasMaxLength(128).IsRequired();

			////modelBuilder.Entity<Video>().HasMany(it => it.VideoCategories)
			////	.WithOne(it => it.Video)
			////	.HasForeignKey(it => it.VideoId);
			//modelBuilder.Entity<Video>().HasKey(b => b.Id);
			//modelBuilder.Entity<Video>().Property(it => it.Name).HasMaxLength(128).IsRequired();
			//modelBuilder.Entity<Video>().Property(it => it.NameCn).HasMaxLength(128).IsRequired();
			//modelBuilder.Entity<Video>().Property(it => it.NameEn).HasMaxLength(255);
			//modelBuilder.Entity<Video>().Property(it => it.NameCnAlias).HasMaxLength(255);
			//modelBuilder.Entity<Video>().Property(it => it.Url).HasMaxLength(128).IsRequired();

			//modelBuilder.Entity<Video>().HasIndex(it => it.DateCreated);


			//modelBuilder.Entity<Actor>().Property(it => it.Name).HasMaxLength(128).IsRequired();
			//modelBuilder.Entity<Actor>().Property(it => it.Url).HasMaxLength(128).IsRequired();


			//modelBuilder.Entity<Director>().Property(it => it.Name).HasMaxLength(128).IsRequired();
			//modelBuilder.Entity<Director>().Property(it => it.Url).HasMaxLength(128).IsRequired();


			//modelBuilder.Entity<Tag>().Property(it => it.Name).HasMaxLength(128).IsRequired();
			//modelBuilder.Entity<Tag>().Property(it => it.Url).HasMaxLength(128).IsRequired();

			//modelBuilder.Entity<VideoTag>().HasAlternateKey(it => new { it.VideoId, it.TagId });
			//modelBuilder.Entity<VideoActor>().HasAlternateKey(it => new { it.VideoId, it.ActorId });
			//modelBuilder.Entity<VideoCategory>().HasAlternateKey(it => new { it.VideoId, it.CategoryId });

			//modelBuilder.Entity<CategoryType>()
			//	.HasMany(it => it.)
			//	.WithOne();

			//modelBuilder.Entity<Post>()
			//	.HasOne(p => p.Blog)
			//	.WithMany(b => b.Posts);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//optionsBuilder.use
			optionsBuilder.UseNpgsql("server=localhost;user id=chris;password=chris123;database=ShareVideo");
			//optionsBuilder.UseSqlite("Data Source=filename;Version=3;");
			//optionsBuilder.UseSqlServer("server=localhost;user id=chris;password=chris123;database=Blog");
			base.OnConfiguring(optionsBuilder);
		}
	}
}
