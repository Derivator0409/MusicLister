using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicLister.Models;
using MusicLister.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feleves_Feladat_FZW0D1.ViewModels
{
    [QueryProperty(nameof(SongToSave), "SavedSong")]
    [QueryProperty(nameof(ArtistToSave), "SavedArtist")]
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public MainPageViewModel(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        [ObservableProperty]
        ObservableCollection<Song> filteredSongs = new ObservableCollection<Song>();

        [ObservableProperty]
        ObservableCollection<Artist> artists = new ObservableCollection<Artist>();

        [ObservableProperty]
        ObservableCollection<Song> songs = new ObservableCollection<Song>();

        [ObservableProperty]
        Artist artistToSave;

        [ObservableProperty]
        Song songToSave;

        [ObservableProperty]
        Artist selectedArtist;

        [ObservableProperty]
        Song selectedSong;

        
        async partial void OnSelectedArtistChanged(Artist value)
        {
           
            FilteredSongs.Clear();
            SelectedSong = null;

            if (value == null)
            {
                return;
            }

        
            var songsForThisArtist = Songs.Where(s => s.ArtistID == value.ID);

            foreach (var song in songsForThisArtist)
            {
                FilteredSongs.Add(song);
            }
        }

        [RelayCommand]
        public async Task AddNewArtist()
        {
            SelectedArtist = null;
            var newArtist = new Artist();
            var param = new ShellNavigationQueryParameters { { "Artist", newArtist } };
            await Shell.Current.GoToAsync("editartist", param);
        }

        [RelayCommand]
        public async Task AddNewSong()
        {
            if (SelectedArtist == null)
            {
                WeakReferenceMessenger.Default.Send("Hiba: Először válassz ki egy előadót, akihez a dalt hozzáadod!");
                return;
            }

            SelectedSong = null;

            var newSong = new Song
            {
                ArtistID = SelectedArtist.ID
            };

            var param = new ShellNavigationQueryParameters { { "Song", newSong } };
            await Shell.Current.GoToAsync("editsong", param);
        }

        [RelayCommand]
        async Task EditSongAsync()
        {
            if (SelectedSong == null)
            {
                WeakReferenceMessenger.Default.Send("Hiba: Nincs kiválasztva Zeneszám");
                return;
            }
            var param = new ShellNavigationQueryParameters { { "Song", SelectedSong } };
            await Shell.Current.GoToAsync("editsong", param);
        }

        [RelayCommand]
        async Task DeleteSong()
        {
            if (SelectedSong == null)
            {
                WeakReferenceMessenger.Default.Send("Hiba: Nincs semmi kijelölve törléshez");
                return;
            }
            bool response = await Shell.Current.DisplayAlert("Törlés", $"Biztosan szeretnéd törölni a {SelectedSong.Title} dalt", "igen", "nem");
            if (response)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var songService = scope.ServiceProvider.GetRequiredService<ISongService>();
                    await songService.DeleteSongAsync(SelectedSong.ID);
                }

                Songs.Remove(SelectedSong);
                FilteredSongs.Remove(SelectedSong);

                SelectedSong = null;
            }
        }

        [RelayCommand]
        async Task EditArtistAsync()
        {
            if (SelectedArtist == null)
            {
                WeakReferenceMessenger.Default.Send("Hiba: Nincs kiválasztva Előadó");
                return;
            }
            var param = new ShellNavigationQueryParameters { { "Artist", SelectedArtist } };
            await Shell.Current.GoToAsync("editartist", param);
        }

        [RelayCommand]
        async Task DeleteArtist()
        {
            if (SelectedArtist == null)
            {
                WeakReferenceMessenger.Default.Send("Hiba: Nincs semmi kijelölve törléshez");
                return;
            }
            bool response = await Shell.Current.DisplayAlert("Törlés", $"Biztosan szeretnéd törölni a {SelectedArtist.Name} előadót", "igen", "nem");

            if (response)
            {
                int deletedArtistId = SelectedArtist.ID;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var artistService = scope.ServiceProvider.GetRequiredService<IArtistService>();
                    await artistService.DeleteArtistAsync(deletedArtistId);
                }

                Artists.Remove(SelectedArtist);

                var songsToRemove = Songs.Where(s => s.ArtistID == deletedArtistId).ToList();
                foreach (var song in songsToRemove)
                {
                    Songs.Remove(song);
                }

                SelectedArtist = null;
            }
        }

        async partial void OnSongToSaveChanged(Song value)
        {
            if (value == null)
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                var songService = scope.ServiceProvider.GetRequiredService<ISongService>();

                if (value.ID == 0)
                {
                    await songService.CreateSongAsync(value);
                    Songs.Add(value);

                    if (SelectedArtist != null && value.ArtistID == SelectedArtist.ID)
                    {
                        FilteredSongs.Add(value);
                    }
                }
                else 
                {
                    await songService.UpdateSongAsync(value.ID, value);

                    var oldSongMaster = Songs.FirstOrDefault(a => a.ID == value.ID);
                    if (oldSongMaster != null) Songs[Songs.IndexOf(oldSongMaster)] = value;

                 
                    var oldSongFiltered = FilteredSongs.FirstOrDefault(a => a.ID == value.ID);
                    if (oldSongFiltered != null) FilteredSongs[FilteredSongs.IndexOf(oldSongFiltered)] = value;
                }
            }

            SongToSave = null;
        }

        async partial void OnArtistToSaveChanged(Artist value)
        {
            if (value == null)
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                var artistService = scope.ServiceProvider.GetRequiredService<IArtistService>();

                if (value.ID == 0)
                {
                    await artistService.CreateArtistAsync(value);
                    Artists.Add(value);
                }
                else
                {
                    await artistService.UpdateArtistAsync(value.ID, value);

                    var oldArtist = Artists.FirstOrDefault(a => a.ID == value.ID);
                    if (oldArtist != null)
                    {
                        var index = Artists.IndexOf(oldArtist);
                        Artists[index] = value;
                    }
                }
            }
            ArtistToSave = null;
        }

        [RelayCommand]
        public async Task LoadAll()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var artistService = scope.ServiceProvider.GetRequiredService<IArtistService>();
                var songService = scope.ServiceProvider.GetRequiredService<ISongService>();

                var Artistdb = await artistService.GetAllAsync();
                var Songdb = await songService.GetAllAsync();

                Artists.Clear();
                Songs.Clear();
                FilteredSongs.Clear();

                foreach (var Artist in Artistdb)
                {
                    Artists.Add(Artist);
                }
                foreach (var Song in Songdb)
                {
                    Songs.Add(Song);
                }
            }
        }
    }
}