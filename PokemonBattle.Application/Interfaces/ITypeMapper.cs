using Domain.Models.RequestModels;

namespace Pokemon.AppServices.Interfaces
{
    public interface ITypeMapper
    {
        List<string> MapTypes(RequestPokeonModel request);
    }
}