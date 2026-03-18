using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interface;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Domain.Services
{
    public class TypeDataService
    {
        private readonly ITypeModelFactory _typeFactory;

        //Namn och typ värdena
        private readonly Dictionary<string, TypeModel> _typeDataDic = new();

        public TypeDataService(ITypeModelFactory typeModelFactory)
        {
            _typeFactory = typeModelFactory;
        }

        public void AddType(RequestTypeModel type)
        {
            var actualType = _typeFactory.Create(type);
            //finns inte "grass"
            if (!_typeDataDic.ContainsKey(type.Name))
            {
                //ny nyckel som heter "grass", med alla värden för gräs typen
                _typeDataDic[type.Name] = actualType;
                Console.WriteLine(type);
                Console.WriteLine(_typeDataDic);
            }
           
        }
        public TypeModel GetTypeModel(string name)
        {
            Console.WriteLine(name);
            if (_typeDataDic.ContainsKey(name))
            {
                return _typeDataDic[name];
            }
            else
            {
                throw new KeyNotFoundException("Key not found in types yaow");
            }
        }

        public Dictionary<string, TypeModel> GetAllTypeData()
        {
            return _typeDataDic;
        }

        public double GetTypeAttackMultiplier(string attackTypeName, string defenderTypeName, string? defenderOtherTypeName)
        {
            var attackType = GetTypeModel(attackTypeName);
            var firstDefenderType = GetTypeModel(defenderTypeName);
            //Hämta typ om andra typ strängen inte är null, är den null skippa delarna där vi kollar andra typen
            TypeModel? secondDefenderType = defenderOtherTypeName != null ? GetTypeModel(defenderOtherTypeName) : null;

            double multiplier = 1;

            // Check immunities for both types first
            if (firstDefenderType.Immunities?.Any(x => x.Trim().ToLower() == attackType.Name.Trim().ToLower()) ?? false)
            {
                return 0;
            }
            if (secondDefenderType != null)
            {
                if (secondDefenderType.Immunities?.Any(x => x.Trim().ToLower() == attackType.Name.Trim().ToLower()) ?? false)
                {
                    return 0;
                }
                   
            } 
            // First type effectiveness
            if (attackType.Effectivnesses?.Any(x => x.Trim().ToLower() == firstDefenderType.Name.Trim().ToLower()) ?? false)
            {
                multiplier *= 2;
            }
                

            if (attackType.TypesResisting?.Any(x => x.Trim().ToLower() == firstDefenderType.Name.Trim().ToLower()) ?? false)
            {
                multiplier *= 0.5;
            }
  

            // Second type effectiveness
            if (secondDefenderType != null)
            {
                if (attackType.Effectivnesses?.Any(x => x.Trim().ToLower() == secondDefenderType.Name.Trim().ToLower()) ?? false)
                {
                    multiplier *= 2;
                }
                  

                if (attackType.TypesResisting?.Any(x => x.Trim().ToLower() == secondDefenderType.Name.Trim().ToLower()) ?? false)
                {
                    multiplier *= 0.5;
                }
                
            }

            return multiplier;
        }
    }
}
