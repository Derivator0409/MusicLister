using Feleves_Feladat_FZW0D1.ViewModels;

namespace Feleves_Feladat_FZW0D1.Views;

public partial class EditSongPage : ContentPage
{
    public EditSongPage(EditSongViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}