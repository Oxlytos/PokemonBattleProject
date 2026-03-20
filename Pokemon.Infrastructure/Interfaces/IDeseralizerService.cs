using Domain.Models.Base;
using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IDeseralizerService
    {
        Task<RequestMoveModel> DeseralizeMoveModel(string json);
        Task<RequestPokeonModel> DeseralizePokemonModel(string json);
        Task<TypeModel> DeseralizeTypeModel(string jsonContet);
        Task<RequestMoveModel> DeserializeMoveModel(string json);
        Task<RequestTypeModel> DeserializeTypeModel(string jsonResponse);
        Task<RequestPokeonModel> FairyProcuationHandler(RequestPokeonModel normalPokemon);
        Task<BasePokemon> GetBasePokemonDeseralized(string jsonContet);
        Task<MoveModel> GetMoveModelDeseralized(string jsonContet);
    }
}