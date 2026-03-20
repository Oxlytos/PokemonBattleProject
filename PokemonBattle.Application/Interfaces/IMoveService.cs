using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.AppServices.Interfaces
{
    public interface IMoveService
    {
        Task<List<string>> AddMove(PartyPokemonModel pokemon, MoveModel newMove);
        Task<List<string>> RemoveMove(PartyPokemonModel pokemon, MoveModel move);
        Task<bool> CanWeAddAMove(PartyPokemonModel partyPokemonModel);
        Task<List<MoveModel>> GetMoveModels(List<string> moves);

    }
}
