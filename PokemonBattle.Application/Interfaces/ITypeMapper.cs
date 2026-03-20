using Pokemon.ContractDTOs.RequestModel;

namespace Pokemon.AppServices.Interfaces
{
    public interface ITypeMapper
    {
        List<string> MapTypes(RequestPokeonModel request);
    }
}