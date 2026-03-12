using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Mappers;
using Pokemon.Shared.Extensions;

namespace Pokemon.AppServices.Factories;

public static class PartyPokemonFactory
{
    public static PartyPokemonModel Create(RequestPokeonModel request)
    {
        Console.WriteLine(request);
        var partyPoke = GeneralMapper.MapBasic(request);
        Console.WriteLine(partyPoke.Nickname);
        Console.WriteLine(request);
        //Map stats
        partyPoke.Stats=StatMapper.MapStats(request);

        //Map type
        partyPoke.Types = TypeMapper.MapTypes(request);


        return partyPoke;
       
    }
}
