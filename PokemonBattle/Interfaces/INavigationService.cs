using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonBattle.Interfaces
{
    internal interface INavigationService
    {
        Task NavigateToBattlePageAsync();
        Task NavigateToTeamBuilderPageAsync();
        Task GoBackPageAsync();
    }
}
