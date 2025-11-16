using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicLister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feleves_Feladat_FZW0D1.ViewModels
{
    [QueryProperty(nameof(ArtistToEdit), "Artist")]

    public partial class EditArtistViewModel:ObservableObject
    {
        [ObservableProperty]
        Artist artistToEdit;
        public EditArtistViewModel()
        {
            
        }

        [RelayCommand]
        async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(ArtistToEdit.Name))
            {
                WeakReferenceMessenger.Default.Send($"Hiba: Az elődadó megadása kötelező!");
                return;
            }

            var param = new ShellNavigationQueryParameters
            {
                { "SavedArtist",ArtistToEdit }
            };
            await Shell.Current.GoToAsync("..",param);
        }


        [RelayCommand]
        async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

    }
}
