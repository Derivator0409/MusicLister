using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Messaging;
using Feleves_Feladat_FZW0D1.ViewModels;
namespace Feleves_Feladat_FZW0D1
{
    public partial class MainPage : ContentPage
    {
       private readonly MainPageViewModel mainPageViewModel;

        public MainPage(MainPageViewModel mainPageView)
        {
            InitializeComponent();
            this.mainPageViewModel = mainPageView;
           BindingContext = this.mainPageViewModel;
            WeakReferenceMessenger.Default.Register<string>(this, async (recipient, message) => { await DisplayAlert("alert", message, "Ok"); });

        }
        private async void OnLoaded(object? sender, EventArgs e)
        {
            await mainPageViewModel.LoadAllCommand.ExecuteAsync(null);
        }

        
        
    }
}
