using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.RequestModels;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface IMoveService
    {
        Task<RequestMoveModel[]> AddMove(RequestPokeonModel pokemon, RequestMoveModel newMove);
        Task<RequestMoveModel[]> RemoveMove(RequestPokeonModel pokemon, RequestMoveModel move);
    }

}
