using MusicLister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLister.Services
{
    public  interface IArtistService
    {
        public Task CreateArtistAsync(Artist newArtist);

        public Task DeleteArtistAsync(int ID);

        public Task UpdateArtistAsync(int ID, Artist UpdateArtist);

        public Task<List<Artist>> GetAllAsync();

        public Task<Artist> GetAsync(int ID);
    }
}
