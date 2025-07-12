using Drosy.Application.Interfaces.Common;

namespace Drosy.Infrastructure.Mapping.Configs
{
    public class MappingServiceAdapter(MapsterMapper.IMapper mapper) : IMapper
    {
        private readonly MapsterMapper.IMapper _mapper = mapper;

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            _mapper.Map(source, destination);
        }
    }
}
