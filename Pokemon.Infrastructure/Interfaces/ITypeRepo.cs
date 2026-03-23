using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.Infrastructure.Interfaces
{
    public interface ITypeRepo
    {
        public Task<string> GetTypeSprite(string name);
        public Task<RequestTypeModel> GetTypeData(string name);

        public Task SaveTypeData(RequestTypeModel requestType);
    }
}
