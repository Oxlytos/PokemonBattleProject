using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.AppServices.Interfaces;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.AppServices.Factories
{
    public class MoveModelFactory
    {
     
        public MoveModel Create(RequestMoveModel reqMove)
        {
            MoveModel model = new MoveModel();
            model.Name=reqMove.Move.Name;

            return model;
        }
        public  List<MoveModel> CreateList(RequestMoveModel[] reqMoves)
        {
            List<MoveModel> moves = new List<MoveModel>();
            foreach (var reqMove in reqMoves)
            {
                moves.Add(Create(reqMove));
            }
            return moves;
        }
    }
}
