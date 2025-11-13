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
        public Task CreateArtistAsync(Artist newArtist)
        {
            throw new NotImplementedException();
        }

        public Task DeleteArtistAsync(int ID)
        {
            throw new NotImplementedException();
        }

        public Task<List<Artist>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Artist> GetAsync(int ID)
        {
            throw new NotImplementedException();
        }

        public Task UpdateArtistAsync(int ID, Artist UpdateArtist)
        {
            throw new NotImplementedException();
        }
    }
}
