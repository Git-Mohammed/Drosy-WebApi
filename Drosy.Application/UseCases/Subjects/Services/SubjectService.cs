using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Subjects.DTOs;
using Drosy.Application.UseCases.Subjects.Interfaces;
using Drosy.Domain.Entities;
using Drosy.Domain.Interfaces.Common.Uow;
using Drosy.Domain.Interfaces.Repository;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using Drosy.Domain.Shared.ErrorComponents.Subjects;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubjectService> _logger;

    public SubjectService(
        ISubjectRepository subjectRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger<SubjectService> logger)
    {
        _subjectRepository = subjectRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DataResult<SubjectDTO>>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var subjects = await _subjectRepository.GetAllAsync(ct);
            if (subjects is null)
            {
                return Result.Failure<DataResult<SubjectDTO>>(SubjectErrors.SubjectNotFound);
            }

            DataResult<SubjectDTO> dataResult = new DataResult<SubjectDTO>
            {
                Data = _mapper.Map<IEnumerable<Subject>, IEnumerable<SubjectDTO>>(subjects),
                TotalRecordsCount = subjects.Count()
            };

            return Result.Success(dataResult);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return Result.Failure<DataResult<SubjectDTO>>(CommonErrors.Unexpected);
        }
    }
    public async Task<Result<SubjectDTO>> GetByIdAsync(int id, CancellationToken ct)
    {
        try
        {
            var subject = await _subjectRepository.GetByIdAsync(id, ct);
            if (subject == null)
                return Result.Failure<SubjectDTO>(SubjectErrors.SubjectNotFound);

            var dto = _mapper.Map<Subject, SubjectDTO>(subject);
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving subject: {Message}", ex.Message);
            return Result.Failure<SubjectDTO>(SubjectErrors.SubjectFailure);
        }
    }

    public async Task<Result<SubjectDTO>> CreateAsync(CreateSubjectDTO dto, CancellationToken ct)
    {
        try
        {
            var subjects = await _subjectRepository.GetAllAsync(ct);
            var isDuplicate = subjects.Any(x => string.Equals(x.Name, dto.Name, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
                return Result.Failure<SubjectDTO>(SubjectErrors.IsDuplicate);

            var subject = _mapper.Map<CreateSubjectDTO, Subject>(dto);
            await _subjectRepository.AddAsync(subject, ct);

            var saved = await _unitOfWork.SaveChangesAsync(ct);
            if (!saved)
                return Result.Failure<SubjectDTO>(SubjectErrors.SubjectSaveFailure);

            var subjectDto = _mapper.Map<Subject, SubjectDTO>(subject);
            return Result.Success(subjectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating subject: {Message}", ex.Message);
            return Result.Failure<SubjectDTO>(SubjectErrors.SubjectFailure);
        }
    }

    public async Task<Result> UpdateAsync(UpdateSubjectDTO dto, int id, CancellationToken ct)
    {
        try
        {
            var subject = await _subjectRepository.GetByIdAsync(id, ct);
            if (subject == null)
                return Result.Failure(SubjectErrors.SubjectNotFound);

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure(SubjectErrors.NameRequired);

            _mapper.Map(dto, subject);
            await _subjectRepository.UpdateAsync(subject, ct);

            var saved = await _unitOfWork.SaveChangesAsync(ct);
            return saved ? Result.Success() : Result.Failure(SubjectErrors.SubjectSaveFailure);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error updating subject: {Message}", ex.Message);
            return Result.Failure(SubjectErrors.SubjectFailure);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            var subject = await _subjectRepository.GetByIdAsync(id, ct);
            if (subject == null)
                return Result.Failure(SubjectErrors.SubjectNotFound);

            await _subjectRepository.DeleteAsync(subject, ct);

            var saved = await _unitOfWork.SaveChangesAsync(ct);
            return saved ? Result.Success() : Result.Failure(SubjectErrors.SubjectDeleteFailure);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting subject: {Message}", ex.Message);
            return Result.Failure(SubjectErrors.SubjectFailure);
        }
    }
}
