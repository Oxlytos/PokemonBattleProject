using System.Threading.Tasks;
using Domain.Models.RequestModels;
using Pokemon.Services.Interfaces;
using PokemonBattle.Interfaces;
using PokemonBattle.ViewModels;

namespace PokemonBattle;

public partial class MoveAssignerPage : ContentPage
{
	private RequestPokeonModel _pokemonModel;
	public MoveAssignerPage(MoveViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private async void GoBackButtonClicked(object sender, EventArgs e)
    {
		await Shell.Current.Navigation.PopAsync();
    }

    private async void GoToMainMenuButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}