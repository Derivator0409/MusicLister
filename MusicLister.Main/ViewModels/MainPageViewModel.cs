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
    public partial class MainPageViewModel:ObservableObject
    {
        private readonly ISongService songService;
        private readonly IArtistService artistService;

        [ObservableProperty]
        ObservableCollection<Artist> artists = new ObservableCollection<Artist>();

        [ObservableProperty]
        ObservableCollection<Song> songs = new ObservableCollection<Song>();

        [ObservableProperty]
        Artist selectedArtist;

        [ObservableProperty]
        Song selectedSong;

        public MainPageViewModel(IArtistService artistService,ISongService songService)
        {
           this.songService = songService;
            this.artistService = artistService;
        }
        [RelayCommand]
        public async Task AddNewArtist()
        {
            SelectedArtist = null;
            var newArtist=new Artist();
            var param = new ShellNavigationQueryParameters { {"Artist",newArtist } };
            await Shell.Current.GoToAsync("editartist",param);
        
        }
        [RelayCommand]
        public async Task AddNewSong()
        {
            SelectedSong = null;
            var newSong=new Song();
            var param = new ShellNavigationQueryParameters { {"Song",newSong } };
            await Shell.Current.GoToAsync("editsong",param);
        }

        [RelayCommand]
        async Task EditSongAsync()
        {
            if(SelectedArtist==null)
            {
                WeakReferenceMessenger.Default.Send("Hiba: Nincs kiválasztva Zeneszám");
                return;
            }
            var param = new ShellNavigationQueryParameters { { "Song",SelectedSong} };
            await Shell.Current.GoToAsync("editsong", param);
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
            bool response = await Shell.Current.DisplayAlert("Törlés",$"Biztosan szeretnéd törölni a {SelectedArtist.Name} előadót","igen","nem");
            if(response)
            {
                await artistService.DeleteArtistAsync(SelectedArtist.ID);
                Artists.Remove(SelectedArtist);
                SelectedArtist = null;

            }

        }


        async partial void OnSelectedArtistChanged(Artist value)
        {
            if (value != null)
            {
                await artistService.UpdateArtistAsync(SelectedArtist.ID, value);

                if (SelectedArtist != null)
                {
                    var oldArtist = Artists.Where(x => x.ID == SelectedArtist.ID).FirstOrDefault();
                    if (oldArtist != null)
                    {
                        Artists.Remove(oldArtist);
                    }
                    Artists.Add(value);
                }
                else 
                {
                    Artists.Add(value);
                }
            }
            else
            {
                return;
            }

        }
            
        



        [RelayCommand]
        public async Task LoadAll()
        {
            var Artistdb = await artistService.GetAllAsync();
            var Songdb = await songService.GetAllAsync();

            Artists.Clear();
            Songs.Clear();
            
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
