using System.Data.Entity;

namespace PSD_Project.EntityFramework
{
    public partial class Raamen : DbContext
    {
        public Raamen()
            : base("name=Raamen")
        {
        }

        public virtual DbSet<Detail> Details { get; set; }
        public virtual DbSet<Header> Headers { get; set; }
        public virtual DbSet<Meat> Meats { get; set; }
        public virtual DbSet<Raman> Ramen { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Header>()
                .HasMany(e => e.Details)
                .WithRequired(e => e.Header)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Meat>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Raman>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Raman>()
                .Property(e => e.Borth)
                .IsUnicode(false);

            modelBuilder.Entity<Raman>()
                .Property(e => e.Price)
                .IsUnicode(false);

            modelBuilder.Entity<Raman>()
                .HasMany(e => e.Details)
                .WithRequired(e => e.Raman)
                .HasForeignKey(e => e.Ramenid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Gender)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Headers)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.CustomerId);
        }
    }
}
