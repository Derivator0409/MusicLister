using MusicLister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLister.Services
{
    public interface ISongService
    {
        public Task CreateSongAsync(Song newSong);

        public Task DeleteSongAsync(int ID);

        public Task UpdateSongAsync(int ID, Song UpdateSong);

        public Task <List<Song>> GetAllAsync();

        public Task<Song> GetAsync(int ID);
    
    }
}
