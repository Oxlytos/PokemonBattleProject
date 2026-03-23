using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.Infrastructure.Mapper
{
    public static class TypeMapper
    {
        //Fire request to Fire model
        public static TypeModel MapType(RequestTypeModel request)
        {
            TypeModel model = new TypeModel();
            model.Name = request.Name;

            MapSource(model.Effectivnesses, request.DamageRelations?.Effectivnesses);

            MapSource(model.Immunities, request.DamageRelations?.Immunities);

            MapSource(model.TypesImmune, request.DamageRelations?.TypesImmune);

            MapSource(model.Weaknesses, request.DamageRelations?.Weaknesses);

            MapSource(model.Resistances, request.DamageRelations?.Resistances);

            MapSource(model.TypesResisting, request.DamageRelations?.TypesResisting);

            if (request.DamageTypeClass.DamageType == "special")
            {
                model.IsSpecialDamage = true;
            }
            else
            {
                model.IsSpecialDamage = false;
            }

            return model;
        }
        //List X to array Y
        public static void MapSource(List<string> toThis, TypeData[] fromThis)
        {
            if (fromThis == null)
            {
                return;
            }
            toThis.AddRange(fromThis.Select(x => x.TypeName));
        }
    }
}
