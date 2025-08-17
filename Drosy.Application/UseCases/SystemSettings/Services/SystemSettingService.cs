using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.SystemSettings.DTOs;
using Drosy.Application.UseCases.SystemSettings.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Microsoft.AspNetCore.Http;

namespace Drosy.Application.UseCases.SystemSettings.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemSettingService> _logger;
        private readonly IImageService _imageService;
        public SystemSettingService(
            ISystemSettingRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SystemSettingService> logger,
            IImageService imageService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _imageService = imageService;
        }

        public async Task<Result<SystemSettingDTO>> GetAsync(CancellationToken ct)
        {
            try
            {
                var setting = await _repository.GetAsync(ct);
                if (setting == null)
                    return Result.Failure<SystemSettingDTO>(CommonErrors.NotFound);

                var dto = _mapper.Map<SystemSetting,SystemSettingDTO>(setting);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving system settings: {Message}", ex.Message);
                return Result.Failure<SystemSettingDTO>(CommonErrors.Unexpected);
            }
        }

        public async Task<Result<SystemSettingDTO>> UpdateAsync(UpdateSystemSettingDTO dto, CancellationToken ct)
        {
            try
            {
                var setting = await _repository.GetAsync(ct);
                if (setting == null)
                    return Result.Failure<SystemSettingDTO>(CommonErrors.NotFound);

                setting.WebName = dto.WebName;
                setting.DefaultCurrency = dto.DefaultCurrency;

                if (dto.LogoFile != null)
                {
                    // Assume logo is saved and path is returned
                    var logoPath = await _imageService.SaveImageAsync(dto.LogoFile, "System");
                    setting.LogoPath = logoPath;
                }

                await _repository.UpdateAsync(setting, ct);
                var saved = await _unitOfWork.SaveChangesAsync(ct);

                return saved
                    ? Result.Success(_mapper.Map<SystemSetting, SystemSettingDTO>(setting))
                    : Result.Failure<SystemSettingDTO>(CommonErrors.Unexpected);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating system settings: {Message}", ex.Message);
                return Result.Failure<SystemSettingDTO>(CommonErrors.Unexpected);
            }
        }

        private async Task<string> SaveLogoAsync(IFormFile logoFile)
        {
            // Stubbed for brevity — implement actual file saving logic
            var fileName = $"{Guid.NewGuid()}_{logoFile.FileName}";
            var path = Path.Combine("wwwroot", "logos", fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await logoFile.CopyToAsync(stream);
            return $"/logos/{fileName}";
        }
    }
}
