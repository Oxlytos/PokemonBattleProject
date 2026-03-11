using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.RequestModels;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IApplicationState
    {
        public ObservableCollection<RequestPokeonModel> TeamPokemon {  get; }
    }
}
