using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Factories;
using Domain.Models.Game;
using Domain.Models.RequestModels;
using Pokemon.AppServices.Interfaces;
using Pokemon.AppServices.Mappers;
using Pokemon.Shared.Extensions;

namespace Pokemon.AppServices.Factories;

public class PartyPokemonFactory
{
    private IGeneralMapper _generalMapper;
    private IStatMapper _statMapper;
    private ITypeMapper _typeMapper;
    public PartyPokemonFactory(IGeneralMapper generalMapper, ITypeMapper typeMapper, IStatMapper statMapper)
    {
        _typeMapper = typeMapper;
        _generalMapper = generalMapper;
        _statMapper = statMapper;
    }
    public PartyPokemonModel Create(RequestPokeonModel request)
    {
        var partyPoke = _generalMapper.MapBasic(request);
        //Map stats
        partyPoke.Stats= _statMapper.MapStats(request);

        //Calc for assigning effective stats from base stats
        var calc = StatCalculatorFactory.GetCalculator();
        partyPoke.Stats = calc.CalculateEffectiveStats(partyPoke.Stats);
        //Map type
        partyPoke.Types = _typeMapper.MapTypes(request);

        return partyPoke;
       
    }
}
