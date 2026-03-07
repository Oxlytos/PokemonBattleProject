using System.Threading.Tasks;
using Pokemon.Services.Interfaces;
using PokemonBattle.Interfaces;
using PokemonBattle.ViewModels;

namespace PokemonBattle;

public partial class TeamBuilderPage : ContentPage
{
    public TeamBuilderPage(ITeamViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private async void OnClickBattleButton(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new BattlePage());
    }

    private void OnClickGetPokemonAsyncButton(object sender, EventArgs e)
    {

    }

    private async void OnClickGoToHomeMenu(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}