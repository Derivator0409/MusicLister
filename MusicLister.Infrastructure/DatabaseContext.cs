using MusicLister.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicLister.Infrastructure
{
    public class DatabaseContext(DbContextOptions options):DbContext(options)
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; } 
    
    
    }
}
