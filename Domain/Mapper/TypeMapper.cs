using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;

namespace Pokemon.Infrastructure.Mappers
{
    public static class TypeMapper
    {
        //Fire request till Fire model more or less
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
        //Lista till X från Y, exakt samma för varje del av typemodel och request
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
