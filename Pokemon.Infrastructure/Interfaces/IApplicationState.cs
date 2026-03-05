using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IApplicationState
    {
        public ObservableCollection<PokemonModel> TeamPokemon {  get; }
    }
}
