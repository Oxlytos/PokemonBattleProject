namespace PokemonBattle
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Routing för att hitta korrekt och inte krascha
            Routing.RegisterRoute(nameof(TeamBuilderPage), typeof(TeamBuilderPage));
        }
    }
}
