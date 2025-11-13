using Microsoft.EntityFrameworkCore;
using MusicLister.Infrastructure;
using MusicLister.Models;
using MusicLister.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feleves_Feladat_FZW0D1.Services
{
    public class DbArtistServices(DatabaseContext ctx) : IArtistService
    {
        public async Task CreateArtistAsync(Artist newArtist)
        {
            ctx.Artists.AddAsync(newArtist);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteArtistAsync(int ID)
        {
            Artist artistdelete=ctx.Artists.FirstOrDefault(x => x.ID==ID);
            if (artistdelete != null)
            {
                ctx.Artists.Remove(artistdelete);
                await ctx.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public async Task<List<Artist>> GetAllAsync()
        {
           return await ctx.Artists.Include(x=>x.Name).Include(x=>x.ID).ToListAsync();
        }

        public async Task<Artist> GetAsync(int ID)
        {
            return await ctx.Artists.FindAsync(ID)?? throw new KeyNotFoundException();
        }

        public async Task UpdateArtistAsync(int ID, Artist UpdateArtist)
        {
            Artist artist= await ctx.Artists.FirstOrDefaultAsync(y=>y.ID==ID)?? throw new KeyNotFoundException();

            artist.Name = UpdateArtist.Name;
            artist.ID = UpdateArtist.ID;
            await ctx.SaveChangesAsync();


        }
    }
}
