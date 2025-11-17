using Feleves_Feladat_FZW0D1.ViewModels;

namespace Feleves_Feladat_FZW0D1.Views;

public partial class SpotifyImportPage : ContentPage
{
	public SpotifyImportPage(SpotifyImportViewModel viewModel)
	{
		InitializeComponent();
		BindingContext=viewModel;
	}
}