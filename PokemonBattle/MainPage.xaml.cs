using System.Threading.Tasks;
using Pokemon.Services.Interfaces;
using PokemonBattle.ViewModels;

namespace PokemonBattle
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }


        private async void OnClickTeamBuilder(object sender, EventArgs e)
        {
            //await Shell.Current.Navigation.PushAsync(new TeamBuilderPage());
        }

        private async void OnClickBattleButton(object sender, EventArgs e)
        {
            //await Shell.Current.Navigation.PushAsync(new BattlePage());
        }
    }

}
