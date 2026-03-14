using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;

namespace Domain.Services
{
    public class TypeDataService
    {
        //Namn och typ värdena
        private readonly Dictionary<string, TypeModel> _typeDataDic = new();
        public void AddType(TypeModel type)
        {
            //finns inte "grass"
            if (!_typeDataDic.ContainsKey(type.Name))
            {
                //ny nyckel som heter "grass", med alla värden för gräs typen
                _typeDataDic[type.Name] = type;
                Console.WriteLine(type);
                Console.WriteLine(_typeDataDic);
            }
           
        }
        public TypeModel GetTypeModel(string name)
        {
            if (_typeDataDic.ContainsKey(name))
            {
                return _typeDataDic[name];
            }
            else
            {
                throw new KeyNotFoundException("Key not found in types yaow");
            }
        }

        public double GetTypeAttackMultiplier(string attackTypeName, string defenderTypeName, string? defenderOtherTypeName)
        {
            var attacktType = GetTypeModel(attackTypeName);
            var firstDefenderType = GetTypeModel(defenderTypeName);

            //Om defender har en andra typ, ta med den, annars inte
            TypeModel? secondDefenderType = defenderOtherTypeName != null ? GetTypeModel(defenderTypeName) : null;

            double multiplier = 1;
            //OM attackens effektivtet är emot defendertyps första typ, bra multiplier läggs till
            //Eld mot gräs t.ex.
            if(attacktType.Effectivnesses?.Any(x=>x.ToLower()==defenderTypeName.ToLower()) ?? false)
            {
                multiplier *= 2;
            }

            //Hälften så effektiv här
            //Eld mot vatten
            if(attacktType.Resistances?.Any(x=>x.ToLower() == defenderTypeName.ToLower()) ?? false)
            {
                multiplier *= 0.5;
            }

            //defender är immun
            //Poison attack på Steel => immun
            if(attacktType.Immunities?.Any(x=>x.ToLower()==defenderTypeName.ToLower())  ?? false)
            {
                multiplier = 0;
            }

            //Andra typen om den inte är null
            if(secondDefenderType != null)
            {
                if (attacktType.Effectivnesses?.Any(x => x.ToLower() == secondDefenderType.Name.ToLower()) ?? false)
                {
                    multiplier *= 2;
                }

                //Hälften så effektiv här
                //Eld mot vatten
                if (attacktType.Resistances?.Any(x => x.ToLower() == secondDefenderType.Name.ToLower()) ?? false)
                {
                    multiplier *= 0.5;
                }

                //defender är immun
                //Poison attack på Steel => immun
                if (attacktType.Immunities?.Any(x => x.ToLower() == secondDefenderType.Name.ToLower()) ?? false)
                {
                    multiplier = 0;
                }
            }
            return multiplier;
        }
    }
}
