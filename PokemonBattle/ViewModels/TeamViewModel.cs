using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Factories;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Repository.Interfaces;
using Pokemon.Services.Interfaces;
using PokemonBattle.Facades;
using PokemonBattle.Interfaces;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class TeamViewModel : ITeamViewModel, INotifyPropertyChanged
    {

        private UIFacade _uiFacade;
        private readonly ListPokemonDisplayModelFactory _displayModelFactory;
        public ObservableCollection<RequestPokeonModel> AllPokemon { get; }
        public ObservableCollection<ListPokemonDisplayModel> DisplayTeamPokemon { get;  } = new();
        private ObservableCollection<RequestMoveModel> _moves;
        public ObservableCollection<RequestMoveModel>? BasicMovesModels
        {
            get
            {
                return _moves;
            }
            set
            {
                _moves = value;
                OnPropertyChanged(nameof(BasicMovesModels));

            }
        }
        public ICommand GetPokemonCommand { get; }
        public ICommand AddToTeamCommand { get; }
        public ICommand RemoveFromTeamCommand { get; }
        public ICommand GoToHomeMenuCommand { get; }
        public ICommand GoToMoveAssignerPageCommand { get; }
        public ICommand GetMovesCommand { get; }

        public ICommand SaveTeamCommand { get; }
        public ICommand LoadTeamCommand { get; }
        public ICommand GoToBattlePageCommand { get; }

        public ICommand SaveTeamForAiCommand { get; }

        private ImageSource _pokemonImage;
        public ImageSource PokemonImage
        {
            get
            {
                return _pokemonImage;
            }
            set
            {
                if (_pokemonImage != value)
                {
                    _pokemonImage = value;
                    OnPropertyChanged(nameof(PokemonImage));
                }
            }
        }
       

        public event PropertyChangedEventHandler? PropertyChanged;
        private string _pokemonName { get; set; }
        public string PokemonName
        {
            get { return _pokemonName; }
        }
        private RequestPokeonModel _teamPokemonModelSelected;
        public RequestPokeonModel TeamPokemonModelSelected
        {
            get { return _teamPokemonModelSelected; }
            set
            {
                _teamPokemonModelSelected = value;
                OnPropertyChanged(nameof(TeamPokemonModelSelected));
            }
        }
        private RequestPokeonModel _selectedPokemonModel;
        public RequestPokeonModel SelectedPokemonModel
        {
            get { return _selectedPokemonModel; }
            set
            {
                _selectedPokemonModel = value;
                _pokemonName = value?.Name;
                OnPropertyChanged(nameof(SelectedPokemonModel));
                OnPropertyChanged(nameof(PokemonName));

                ((Command)AddToTeamCommand).ChangeCanExecute();
                //Command load image?
                _ = LoadPokemonSpriteAsync();
            }
        }
        public TeamViewModel
            (
            ListPokemonDisplayModelFactory displayModelFactory,
            UIFacade uIFacade
            )
        {
         
            _uiFacade = uIFacade;
            _displayModelFactory = displayModelFactory;
            AllPokemon = new ObservableCollection<RequestPokeonModel>();
            GetPokemonCommand = new Command(async () => await LoadPokemonAsync());
            AddToTeamCommand = new Command(async () => await AddToTeam(), () => SelectedPokemonModel != null);
            RemoveFromTeamCommand = new Command<ListPokemonDisplayModel>(async (poke) => await RemoveFromTeam(poke));
            GoToMoveAssignerPageCommand = new Command<ListPokemonDisplayModel>(async (poke) => await GoToMoveAssignerPage(poke));
            SaveTeamCommand = new Command(async() => await SaveTeam());
            LoadTeamCommand = new Command(async () => await LoadTeamAsync());
            GetMovesCommand = new Command(async () => await GetPokemonRequestModelMoves());
            GoToBattlePageCommand = new Command(async () => await GoToBattlePage());
            SaveTeamForAiCommand = new Command(async () => await SaveTeamForAi());
        }

        private async Task SaveTeamForAi()
        {
            await _uiFacade.SaveTeamForAI();
        }

        public async Task RebuildTeamDisplay()
        {
            DisplayTeamPokemon.Clear();
            var pokemon = await _uiFacade.GetPokemonTeamAsync();
            foreach (var pokemin in pokemon)
            {
                var display = await _displayModelFactory.CreateFrontFacingSprite(pokemin);
                OnPropertyChanged(nameof(Pokemon));
                DisplayTeamPokemon.Add(display);
            }
        }

        private async Task SaveTeam()
        {
            Microsoft.Maui.Controls.Application.Current?.MainPage?.Unfocus();
            await _uiFacade.SaveTeam();
        }
        public async Task LoadTeamAsync()
        {
            await _uiFacade.LoadTeamAsync();
            await RebuildTeamDisplay();
        }

        public async Task GetPokemonRequestModelMoves()
        {
            if(SelectedPokemonModel != null)
            {
                var movesList = SelectedPokemonModel.Moves?.Moves ?? new RequestMoveModel[1];
                BasicMovesModels = new ObservableCollection<RequestMoveModel>(movesList);

            }
        }
        public async Task GoToMoveAssignerPage(ListPokemonDisplayModel listmodel)
        {
            if (listmodel != null)
            {
                var moveView = App.Current.Handler.MauiContext.Services.GetService<MoveViewModel>();

                var partyMember = await _uiFacade.GetPartyPokemon(listmodel, DisplayTeamPokemon);
                if (partyMember == null)
                {
                    return;
                }

                moveView.ActualPokemon = partyMember;
                PokemonImage = await _uiFacade.LoadPokemonFrontSpritePathAsync(listmodel.Name);
                moveView.PokemonImage = PokemonImage;
                var page = new MoveAssignerPage(moveView);
                await Shell.Current.Navigation.PushAsync(page);
            }
           
        }
        public async Task GoToBattlePage()
        {
            if (!await _uiFacade.CanUserGoToBattlePage())
            {
                return ;
            }
            var battleView = App.Current.Handler.MauiContext.Services.GetService<BattleViewModel>();
            var page = new BattlePage(battleView);
            await Shell.Current.Navigation.PushAsync(page);
        }
        public async Task RemoveFromTeam(ListPokemonDisplayModel listpokmeon)
        {

            if (listpokmeon == null)
            {
                return;
            }
            var toRemove = await _uiFacade.RemoveFromPartyAndUITeam(listpokmeon, DisplayTeamPokemon);
            if (toRemove == null)
            {
                return ;
            }
            DisplayTeamPokemon.RemoveAt(toRemove.Value);
            
        }
        public async Task LoadPokemonSpriteAsync()
        {
            //har vi tryckt på en pokemon i listan, annars returnera
            if (SelectedPokemonModel == null) return;

            var path = await _uiFacade.LoadPokemonFrontSpritePathAsync(SelectedPokemonModel.Name);

            PokemonImage = ImageSource.FromFile(path);
           

        }
        public async Task LoadSpriteForPokemonListItemAsync(ListPokemonDisplayModel listItem)
        {
            if (listItem == null) return;

            listItem.SpriteTypePaths = await _uiFacade.LoadTypeSpritePaths(listItem.Name);

        }
        public async Task AddToTeam()
        {
            //lägg inte till null
            if (_selectedPokemonModel == null)
            {
                return;
            }

            var partyPokemon = await _uiFacade.AddToPartyTeam(_selectedPokemonModel);

            var uiMember = await _displayModelFactory.CreateFrontFacingSprite(partyPokemon);

            await LoadSpriteForPokemonListItemAsync(uiMember);
            DisplayTeamPokemon.Add(uiMember);

            SelectedPokemonModel = null;
          
        }
        public async Task LoadPokemonAsync()
        {
            var pokemon = await _uiFacade.GetAllPokemonAsync();
            AllPokemon.Clear();
            foreach (var item in pokemon)
            {
                AllPokemon.Add(item);
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      
    }
}
