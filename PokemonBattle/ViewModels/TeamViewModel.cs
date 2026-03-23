using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Domain.Models.Game;
using Pokemon.AppServices.Factories;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;
using PokemonBattle.Facades;
using PokemonBattle.Interfaces;
using PokemonBattle.ListModel;

namespace PokemonBattle.ViewModels
{
    public class TeamViewModel : ITeamViewModel, INotifyPropertyChanged
    {

        //Handle clicking, adding and removing team members in this VM
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
        private string _errorName;

        public string ErrorName
        {
            get { return _errorName; }
            set
            {
                if (_errorName != value)
                {
                    _errorName = value;
                    OnPropertyChanged(nameof(ErrorName));
                }
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

        public ICommand UpdateSelectedImageCommand { get; }
        public ICommand SaveTeamForAiCommand { get; }

        public ICommand ShinyCommand { get; }

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
        private ListPokemonDisplayModel _teamPokemonModelSelected;
        public ListPokemonDisplayModel TeamPokemonModelSelected
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
                _=UpdateSelectedImage(_pokemonName);
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


            //All comands, that run async
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
            UpdateSelectedImageCommand = new Command<string>(async (name) => await UpdateSelectedImage(name));
            ShinyCommand = new Command<ListPokemonDisplayModel>(async (poke) => await ShinyOrRegular(poke));
        }
        //Change shiny status
        private async Task ShinyOrRegular(ListPokemonDisplayModel poke)
        {
            if(poke == null) return;
            if (poke.IsShiny)
            {
                poke.PartyPoke.IsShiny = false;
            }
            else
            {
                poke.PartyPoke.IsShiny = true;
            }

            await RebuildTeamDisplay();
        }

        //Saving team for AI is just the current team on the screen => Create a "Fire Opponent" that just has fire pokemon => Simulate a fire gym leader or specialist
        //Might also be for simplyfing this process, future, randomize?
        private async Task SaveTeamForAi()
        {
            await _uiFacade.SaveTeamForAI();
        }

        //refresh display
        public async Task RebuildTeamDisplay()
        {
            DisplayTeamPokemon.Clear();
            var pokemon = await _uiFacade.GetPokemonTeam();
            foreach (var pokemin in pokemon)
            {
                var display = await _displayModelFactory.CreateFrontFacingSprite(pokemin);
                OnPropertyChanged(nameof(Pokemon));
                DisplayTeamPokemon.Add(display);
            }
        }

        //Save current player teams, overwrites old one
        private async Task SaveTeam()
        {
            Microsoft.Maui.Controls.Application.Current?.MainPage?.Unfocus();
            await _uiFacade.SaveTeam();
        }
        //get team, rebuild
        public async Task LoadTeamAsync()
        {
            await _uiFacade.LoadTeamAsync();
            await RebuildTeamDisplay();
        }

        //Either just empty, or retrieves what the API got
        public async Task GetPokemonRequestModelMoves()
        {
            if(SelectedPokemonModel != null)
            {
                var movesList = SelectedPokemonModel.Moves?.Moves ?? new RequestMoveModel[1];
                BasicMovesModels = new ObservableCollection<RequestMoveModel>(movesList);

            }
        }
        //Gotta have a pokemon to assign moves to etc
        public async Task GoToMoveAssignerPage(ListPokemonDisplayModel listmodel)
        {
            if (listmodel != null)
            {
                var moveView = App.Current.Handler.MauiContext.Services.GetService<MoveViewModel>();

                var partyMember = await _uiFacade.GetPartyPokemon(listmodel, DisplayTeamPokemon);
                if (partyMember == null)
                {
                    ErrorName = "Something went wrong with retrieving party member...";
                    await Task.Delay(2000);
                    ErrorName = "";
                    return;
                }

                moveView.ActualPokemon = partyMember;
                PokemonImage = await _uiFacade.LoadPokemonFrontSpritePathAsync(listmodel.Name);
                moveView.PokemonImage = PokemonImage;
                var page = new MoveAssignerPage(moveView);
                await Shell.Current.Navigation.PushAsync(page);
            }
           
        }
        //Just check if there's a player team loaded and AI has a team
        public async Task GoToBattlePage()
        {
            if (!await _uiFacade.CanUserGoToBattlePage())
            {
                ErrorName = "Can't enter battle, either a pokemon has no moves, or there's no AI teams";
                await Task.Delay(5000);
                ErrorName = "";
                return ;
            }
            var battleView = App.Current.Handler.MauiContext.Services.GetService<BattleViewModel>();
            var page = new BattlePage(battleView);
            await Shell.Current.Navigation.PushAsync(page);
        }
        //Find by index, remove that
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
        
        public async Task UpdateSelectedImage(string name)
        {
            var sprite = await _uiFacade.LoadPokemonFrontSpritePathAsync(name);
            if (sprite != null)
            {
                PokemonImage = ImageSource.FromFile(sprite);

            }
        }
        public async Task LoadPokemonSpriteAsync()
        {
            //har vi tryckt på en pokemon i listan, annars returnera
            if (SelectedPokemonModel == null) return;

            var path = await _uiFacade.LoadPokemonFrontSpritePathAsync(SelectedPokemonModel.Name);
            if (path != null)
            {
                PokemonImage = ImageSource.FromFile(path);

            }
        }
        public async Task LoadSpriteForPokemonListItemAsync(ListPokemonDisplayModel listItem)
        {
            if (listItem == null) return;

            listItem.SpriteTypePaths = await _uiFacade.LoadTypeSpritePaths(listItem.Name);
            PokemonImage = listItem.SpriteImage;

        }
        //Creates a patymember model in the facade, then UI display
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
            PokemonImage = uiMember.SpriteImage;
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
