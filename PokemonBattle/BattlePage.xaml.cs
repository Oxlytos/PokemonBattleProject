using Pokemon.Services.Interfaces;
using PokemonBattle.ViewModels;

namespace PokemonBattle;

public partial class BattlePage : ContentPage
{
	public BattlePage(BattleViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}