using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interface;
using Domain.Models.Game;

namespace Domain.Services
{
    public class TypeDataService
    {
        //Name "fire" then all data with typemodel
        private readonly Dictionary<string, TypeModel> _typeDataDic = new();

        public void AddType(TypeModel type)
        {
            //New typing
            if (!_typeDataDic.ContainsKey(type.Name))
            {
                _typeDataDic[type.Name] = type;
            }
           
        }
        public void AddTypes(TypeModel[] types)
        {
            foreach (TypeModel type in types)
            {
                AddType(type);
            }
        }
        //get gamedata model of "fire" type
        public TypeModel GetTypeModel(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
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

        //If ever needed, get all data
        public Dictionary<string, TypeModel> GetAllTypeData()
        {
            return _typeDataDic;
        }

        public double GetTypeAttackMultiplier(string attackTypeName, string defenderTypeName, string? defenderOtherTypeName)
        {
            Console.WriteLine(attackTypeName);
            Console.WriteLine(defenderTypeName);
            Console.WriteLine(defenderOtherTypeName);
            var attackType = GetTypeModel(attackTypeName);
            if (attackType == null)
            {
                return 0;
            }
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
