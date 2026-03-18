using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.AppServices.Interfaces
{
    public interface IGeneralMapper
    {
        PartyPokemonModel MapBasic(RequestPokeonModel model);
    }

}
