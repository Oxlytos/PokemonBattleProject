using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Models;
using Domain.Models;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IMoveService
    {
        Task<MoveRequestModel[]> AddMove(PokemonModel pokemon, MoveRequestModel newMove);
        Task<MoveRequestModel[]> RemoveMove(PokemonModel pokemon, MoveRequestModel move);
    }

}
