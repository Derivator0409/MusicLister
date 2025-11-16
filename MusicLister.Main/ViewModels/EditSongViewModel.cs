using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicLister.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Feleves_Feladat_FZW0D1.ViewModels
{
    [QueryProperty(nameof(SongToEdit), "Song")]
    public partial class EditSongViewModel:ObservableObject
    {
        [ObservableProperty]
        Song songToEdit;

        public EditSongViewModel()
        {
            
        }
        [ObservableProperty]
        double songLengthInSeconds;

       
        partial void OnSongToEditChanged(Song value)
        {
            if (value != null)
            {
               
                SongLengthInSeconds = value.Length.TotalSeconds;
            }
        }

        [RelayCommand]
        async Task SaveAsync()
        {
            
            if (string.IsNullOrWhiteSpace(SongToEdit.Title) || SongLengthInSeconds <= 0)
            {
                WeakReferenceMessenger.Default.Send("Hiba: A Dal minden mezőjét kötelező kitölteni!");
                return;
            }
            if (SongToEdit != null)
            {
                SongToEdit.Length = TimeSpan.FromSeconds(SongLengthInSeconds);
            }

            var param = new ShellNavigationQueryParameters
            {
                { "SavedSong", SongToEdit }
            };

            await Shell.Current.GoToAsync("..", param);

        }

        [RelayCommand]
        async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }


    }
}
