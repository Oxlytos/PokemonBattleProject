using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Models.Base;
using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;
using Pokemon.Infrastructure.Interfaces;

namespace Pokemon.Infrastructure.Services
{
    public class DeseralizerService : IDeseralizerService
    {
        public async Task<RequestMoveModel> DeseralizeMoveModel(string json)
        {
            var move = JsonSerializer.Deserialize<RequestMoveModel>(json);

            var moveInfo = JsonSerializer.Deserialize<Move>(json);
            var moveType = JsonSerializer.Deserialize<MoveType>(json);

            return move;
        }

        public async Task<RequestPokeonModel> FairyProcuationHandler(RequestPokeonModel normalPokemon)
        {
            List<TypeRequest> oldTypes = new List<TypeRequest>();
            foreach (var oldType in normalPokemon.OldTypes)
            {
                var oldies = oldType.OldTypesInfo.ToList();
                oldTypes.AddRange(oldies);
            }

            normalPokemon.Types = oldTypes.ToArray();
            return normalPokemon;
        }

        public async Task<RequestTypeModel> DeserializeTypeModel(string jsonResponse)
        {
            var typeJson = JsonSerializer.Deserialize<RequestTypeModel>(jsonResponse);
            if (typeJson.Name == "fairy")
            {
                return null;
            }
            else
            {
                return typeJson;
            }

        }
        public async Task<RequestPokeonModel> DeseralizePokemonModel(string json)
        {
            var normalPokemon = JsonSerializer.Deserialize<RequestPokeonModel>(json);

            if (normalPokemon.Types.Any(e => e.Types.Name == "fairy"))
            {
                normalPokemon = await FairyProcuationHandler(normalPokemon);
            }
            normalPokemon.Name = char.ToUpper(normalPokemon.Name[0]) + normalPokemon.Name.Substring(1);
            var sprites = JsonSerializer.Deserialize<SpriteCollection>(json);
            normalPokemon.Sprites = sprites;
            var pokemonMoves = JsonSerializer.Deserialize<MoveRequestCollection>(json);
            Console.WriteLine(pokemonMoves);
            normalPokemon.Moves = pokemonMoves;
            Console.WriteLine(normalPokemon.Sprites);
            //Gör till den fånigt långa och komplicerade spritecollectionen
            return normalPokemon;
        }
        public async Task<RequestMoveModel> DeserializeMoveModel(string json)
        {
            var move = JsonSerializer.Deserialize<RequestMoveModel>(json);
            var moveInfo = JsonSerializer.Deserialize<Move>(json);
            Console.WriteLine(moveInfo);
            move.Move = moveInfo;

            return move;
        }
        public async Task<TypeModel> DeseralizeTypeModel(string jsonContet)
        {
            Console.WriteLine(jsonContet);
            var type = JsonSerializer.Deserialize<TypeModel>(jsonContet);
            Console.WriteLine(type);

            return type;
        }
        public async Task<MoveModel> GetMoveModelDeseralized(string jsonContet)
        {
            var move = JsonSerializer.Deserialize<MoveModel>(jsonContet);
            return move;
        }
        public async Task<BasePokemon> GetBasePokemonDeseralized(string jsonContet)
        {
            var poke = JsonSerializer.Deserialize<BasePokemon>(jsonContet);
            return poke;
        }


    }
}
