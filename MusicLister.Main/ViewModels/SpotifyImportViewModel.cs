using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicLister.Models;
using MusicLister.Services;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Feleves_Feladat_FZW0D1.ViewModels
{
    public partial class SpotifyImportViewModel:ObservableObject
    {
        private readonly IServiceScopeFactory scopeFactory;

        [ObservableProperty]
        string playlistUrl;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotImporting))]
        bool isImporting;

        public bool IsNotImporting => IsImporting==false;

        [ObservableProperty]
        string statusMessage;

        public SpotifyImportViewModel(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            statusMessage= "Kérlek, add meg a Spotify playlist URL-jét vagy ID-jét.";
        }

        [RelayCommand]
        async Task ImportPlayListAsync()
        {
            if (string.IsNullOrWhiteSpace(PlaylistUrl))
            {
                StatusMessage = "Hiba: A mező nem lehet üres";
                return;
            }
            IsImporting = true;
            StatusMessage = "Importálás folyamatban:..";

            try
            {
                StatusMessage = "Spotify-hoz csatlakozás...";
                string clientId = "613b6502662249be984583601d2cded3";
                string clientSecret = "8ce73e7501c547ddbce6193699aa9c91";

                var config = SpotifyClientConfig.CreateDefault();
                var request = new ClientCredentialsRequest(clientId, clientSecret);
                var response = await new OAuthClient(config).RequestToken(request);
                var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

                string playlistId = ExtractPlaylistId(PlaylistUrl);
                var tracksPaging = await spotify.Playlists.GetItems(playlistId);
                var allTracks = await spotify.PaginateAll(tracksPaging);

                StatusMessage = "Adatok feldolgozása...";
                var allArtists = allTracks
                    .Select(item => item.Track as FullTrack)
                    .Where(t => t != null)
                    .SelectMany(t => t.Artists ?? Enumerable.Empty<SimpleArtist>())
                    .Where(a => a != null);

                var uniqueArtists = allArtists
                    .GroupBy(a => a.Id)
                    .Select(g => g.First());

                var xml = new XElement("MusicImport",
                    new XElement("Artists",
                        uniqueArtists.Select(artist =>
                            new XElement("Artist",
                                new XElement("SpotifyId", artist.Id),
                                new XElement("Name", artist.Name)
                            )
                        )
                    ),
                    new XElement("Songs",
                        allTracks.Select(item =>
                        {
                            var t = item.Track as FullTrack;
                            if (t == null || !t.Artists.Any()) return null;
                            var firstArtist = t.Artists.First();
                            var duration = TimeSpan.FromMilliseconds(t.DurationMs);

                            return new XElement("Song",
                                new XElement("Title", t.Name),
                                new XElement("Length", duration.ToString("c")),
                                new XElement("ArtistSpotifyId", firstArtist.Id)
                            );
                        }).Where(el => el != null)
                    )
                );
                StatusMessage = "Adatbázis mentése...";
                await SaveToDatabaseAsync(xml);

                StatusMessage = "Importálás kész! Visszanavigálás...";
                await Task.Delay(1500); // Kis késleltetés, hogy az üzenet olvasható legyen

                // Visszanavigálunk a főoldalra, és átadjuk a "Refresh=true" paramétert
                await Shell.Current.GoToAsync("..", new ShellNavigationQueryParameters
                {
                    { "Refresh", true }
                });




            }
            catch (Exception ex)
            {
                StatusMessage = $"Hiba történt: {ex.Message}";
            }




        }

        private async Task SaveToDatabaseAsync(XElement xml)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var artistService = scope.ServiceProvider.GetRequiredService<IArtistService>();
                var songService = scope.ServiceProvider.GetRequiredService<ISongService>();

                var existingArtists = await artistService.GetAllAsync();
                var artistMap = new Dictionary<string, int>(); // SpotifyID -> Adatbázis ID

                // 1. Előadók mentése (duplikátum-ellenőrzéssel)
                foreach (var artistEl in xml.Element("Artists").Elements("Artist"))
                {
                    string spotifyId = artistEl.Element("SpotifyId").Value;
                    string name = artistEl.Element("Name").Value;

                    var existing = existingArtists.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (existing != null)
                    {
                        artistMap[spotifyId] = existing.ID; // Már létezik, csak a map-be tesszük
                    }
                    else
                    {
                        var newArtist = new Artist { Name = name };
                        await artistService.CreateArtistAsync(newArtist); // Létrehozás
                        artistMap[spotifyId] = newArtist.ID; // Az új, generált ID-t tároljuk
                    }
                }

                // 2. Dalok mentése (duplikátum-ellenőrzéssel)
                var existingSongs = await songService.GetAllAsync();
                foreach (var songEl in xml.Element("Songs").Elements("Song"))
                {
                    string title = songEl.Element("Title").Value;
                    string artistSpotifyId = songEl.Element("ArtistSpotifyId").Value;

                    // Csak akkor mentsük, ha az előadóját is sikeresen importáltuk
                    if (artistMap.TryGetValue(artistSpotifyId, out int dbArtistId))
                    {
                        // Ellenőrizzük, hogy ez a dal (cím + előadó ID) létezik-e már
                        bool songExists = existingSongs.Any(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase) && s.ArtistID == dbArtistId);

                        if (!songExists)
                        {
                            var newSong = new Song
                            {
                                Title = title,
                                Length = TimeSpan.Parse(songEl.Element("Length").Value),
                                ArtistID = dbArtistId
                            };
                            await songService.CreateSongAsync(newSong);
                        }
                    }
                }
            }
        }

        // Korábbi ExtractPlaylistId metódus
        static string ExtractPlaylistId(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            input = input.Trim();

            if (input.StartsWith("spotify:playlist:"))
            {
                var parts = input.Split(':');
                if (parts.Length == 3)
                    return parts[2];
            }

            if (input.Contains("open.spotify.com/playlist/"))
            {
                var uri = new Uri(input);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length == 2 && segments[0] == "playlist")
                {
                    return segments[1].Split('?')[0]; // URL paraméterek (pl. ?si=...) levágása
                }
            }

            // A te http://googleusercontent.com/spotify.com/5 linkjeid kezelése
            if (input.Contains("http://googleusercontent.com/spotify.com/6"))
            {
                var uri = new Uri(input);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && segments[0] == "playlist")
                {
                    return segments[1].Split('?')[0];
                }
            }

            return input; // Fallback: Tegyük fel, hogy a felhasználó közvetlenül az ID-t írta be
        }



    }
}
