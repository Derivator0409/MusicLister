using CommunityToolkit.Mvvm.ComponentModel;
using MusicLister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feleves_Feladat_FZW0D1.ViewModels
{
    [QueryProperty(nameof(ArtistToEdit), "SavedSubject")]

    public partial class EditArtistViewModel:ObservableObject
    {
        [ObservableProperty]
        Artist artistToEdit;
        public EditArtistViewModel()
        {
            
        }





    }
}
