using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Regions.DTOs;
using Drosy.Application.UseCases.Regions.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.EFCore;

public class RegionService : IRegionService
{
    private readonly IRegionRepository _regionRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegionService> _logger;

    public RegionService(
        IRegionRepository regionRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger<RegionService> logger)
    {
        _regionRepository = regionRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DataResult<RegionDTO>>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var regions = await _regionRepository.GetAllAsync(ct);
            if (regions is null || !regions.Any())
                return Result.Failure<DataResult<RegionDTO>>(CommonErrors.NotFound);

            var dataResult = new DataResult<RegionDTO>
            {
                Data = _mapper.Map<IEnumerable<Region>, IEnumerable<RegionDTO>>(regions),
                TotalRecordsCount = regions.Count()
            };

            return Result.Success(dataResult);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving regions: {Message}", ex.Message);
            return Result.Failure<DataResult<RegionDTO>>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<RegionDTO>> GetByIdAsync(int id, CancellationToken ct)
    {
        try
        {
            var region = await _regionRepository.GetByIdAsync(id, ct);
            if (region == null)
                return Result.Failure<RegionDTO>(CommonErrors.NotFound);

            var dto = _mapper.Map<Region, RegionDTO>(region);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving region by ID: {Message}", ex.Message);
            return Result.Failure<RegionDTO>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result<RegionDTO>> CreateAsync(CreateRegionDTO dto, CancellationToken ct)
    {
        try
        {
            var existing = await _regionRepository.GetAllAsync(ct);
            var isDuplicate = existing.Any(x => string.Equals(x.Name, dto.Name, StringComparison.OrdinalIgnoreCase));
            if (isDuplicate)
                return Result.Failure<RegionDTO>(CommonErrors.BusinessRule);

            var region = _mapper.Map<CreateRegionDTO, Region>(dto);
            await _regionRepository.AddAsync(region, ct);

            var saved = await _unitOfWork.SaveChangesAsync(ct);
            if (!saved)
                return Result.Failure<RegionDTO>(EfCoreErrors.CanNotSaveChanges);

            var regionDto = _mapper.Map<Region, RegionDTO>(region);
            return Result.Success(regionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating region: {Message}", ex.Message);
            return Result.Failure<RegionDTO>(CommonErrors.Unexpected);
        }
    }

    public async Task<Result> UpdateAsync(UpdateRegionDTO dto, int id, CancellationToken ct)
    {
        try
        {
            var region = await _regionRepository.GetByIdAsync(id, ct);
            if (region == null)
                return Result.Failure(CommonErrors.NotFound);


            _mapper.Map(dto, region);
            await _regionRepository.UpdateAsync(region, ct);

            var saved = await _unitOfWork.SaveChangesAsync(ct);
            return saved ? Result.Success() : Result.Failure(EfCoreErrors.CanNotSaveChanges);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error updating region: {Message}", ex.Message);
            return Result.Failure(CommonErrors.Unexpected);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            var region = await _regionRepository.GetByIdAsync(id, ct);
            if (region == null)
                return Result.Failure(CommonErrors.NotFound);

            var unused = await _regionRepository.IsRegionUnusedAsync(id, ct);
            if (!unused)
                return Result.Failure(CommonErrors.BusinessRule);

            await _regionRepository.DeleteAsync(region, ct);

            var saved = await _unitOfWork.SaveChangesAsync(ct);
            return saved ? Result.Success() : Result.Failure(EfCoreErrors.CanNotSaveChanges);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting region: {Message}", ex.Message);
            return Result.Failure(CommonErrors.Unexpected);
        }
    }
}
