using Pokemon.Services.Interfaces;
using PokemonBattle.ViewModels;

namespace PokemonBattle;

public partial class BattlePage : ContentPage
{
	private MainViewModel _mainViewModel;
	public BattlePage()
	{
		InitializeComponent();
	}
}