using MusicLister.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicLister.Infrastructure
{
    public class DatabaseContext(DbContextOptions options):DbContext(options)
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Artist>().HasMany(e => e.Songs).WithOne().HasForeignKey(x => x.ArtistID);
        }

    }
}
