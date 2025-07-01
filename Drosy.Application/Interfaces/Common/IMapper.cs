namespace Drosy.Application.Interfaces.Common
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }
}