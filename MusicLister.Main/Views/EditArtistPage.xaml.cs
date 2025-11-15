using Feleves_Feladat_FZW0D1.ViewModels;

namespace Feleves_Feladat_FZW0D1.Views;

public partial class EditArtistPage : ContentPage
{
	public EditArtistPage(EditArtistViewModel viewModel)
	{
		InitializeComponent();
		BindingContext=viewModel;
	}
}