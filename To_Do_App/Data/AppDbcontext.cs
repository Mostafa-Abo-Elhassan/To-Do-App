using Microsoft.EntityFrameworkCore;
using To_Do_App.Models;

namespace To_Do_App.Data
{
    public class AppDbcontext : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Status> statuses { get; set; }

        public DbSet<Todo> todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Category entity
            modelBuilder.Entity<Category>().HasData(

                new Category { Id = "work", Name = "Work" },
                new Category { Id = "home", Name = "Home" },
                new Category { Id = "study", Name = "Study" },
                new Category { Id = "school", Name = "School" }

                );

            modelBuilder.Entity<Status>().HasData(

               new Status { Id = "opend", Name = "Opend" },
               new Status { Id = "close", Name = "Compeleted" }


               );
        }


    }
}
