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
    public class DbSongServices(DatabaseContext ctx) : ISongService
    {
        public async Task CreateSongAsync(Song newSong)
        {
            ctx.Songs.AddAsync(newSong);
            await ctx.SaveChangesAsync();
        }

        public Task DeleteSongAsync(int ID)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Song>> GetAllAsync()
        {
            return await ctx.Songs.Include(x => x.Title)
                .Include(x => x.Length)
                .Include(x => x.ArtistID)
                .ToListAsync();
        }

        public async Task<Song> GetAsync(int ID)
        {
            return await ctx.Songs.FindAsync(ID)?? throw new KeyNotFoundException();
        }

        public async Task UpdateSongAsync(int ID, Song UpdateSong)
        {
           Song song= await ctx.Songs.FirstOrDefaultAsync(x => x.ID == ID);
            if (song != null)
            {
                song.Length = UpdateSong.Length;
                song.Title = UpdateSong.Title;
                song.ArtistID = UpdateSong.ArtistID;
                
                
                await ctx.SaveChangesAsync();
            }
            else 
            {
                throw new KeyNotFoundException();
            }
                
        }
    }
}
