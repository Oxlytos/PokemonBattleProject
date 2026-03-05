using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.State
{
    public class ApplicationState:IApplicationState
    {
        public ObservableCollection<PokemonModel> TeamPokemon { get; } = new();
    }
}
