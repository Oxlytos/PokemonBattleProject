using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.Infrastructure.Interfaces;
using Pokemon.Infrastructure.Repositories;

namespace Pokemon.Infrastructure.Factories
{
    public class MoveModelFactory
    {
        private readonly IFetchRepository _pokeFetchRepository;
        public MoveModelFactory(IFetchRepository pokeFetchRepository)
        {
            _pokeFetchRepository = pokeFetchRepository;
        }
        public async Task<MoveModel> Create(RequestMoveModel reqMove)
        {
            MoveModel model = new MoveModel();
            var requestMove = await _pokeFetchRepository.GetMoveModelAsync(reqMove.Move.Name);

            model = await _pokeFetchRepository.GetSerialisedMoveModelAsync(requestMove.Move.Name);

            Console.WriteLine(model);
            var typeInfo = await _pokeFetchRepository.GetTypeModelAsync(requestMove.MoveTypeInfo.Name);

            model.Type = await _pokeFetchRepository.GetSerialisedTypeModelAsync(requestMove.MoveTypeInfo.Name);

            return model;
        }
        public async Task<List<MoveModel>> CreateList(RequestMoveModel[] reqMoves)
        {
            List<MoveModel> moves = new List<MoveModel>();
            foreach (var reqMove in reqMoves)
            {
                MoveModel model = new MoveModel();
                var requestMove = await _pokeFetchRepository.GetMoveModelAsync(reqMove.Move.Name);

                model = await _pokeFetchRepository.GetSerialisedMoveModelAsync(requestMove.Move.Name);

                model.Type = await _pokeFetchRepository.GetSerialisedTypeModelAsync(requestMove.MoveTypeInfo.Name);

                moves.Add(model);
            }
           

            return moves;
        }
    }
}
