using Feleves_Feladat_FZW0D1.ViewModels;
using Feleves_Feladat_FZW0D1.Views;

namespace Feleves_Feladat_FZW0D1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("editartist", typeof(EditArtistPage));
        }
    }
}
