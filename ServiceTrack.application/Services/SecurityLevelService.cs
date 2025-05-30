using AuthApp.application.DTOs;
using AuthApp.domain.Entities;
using ServiceTrack.application.Interfaces;
using ServiceTrack.application.Exceptions;

namespace AuthApp.application.Services;

public class SecurityLevelService : ISecurityLevelService
{
    private readonly ISecurityLevelRepository _repository;

    public SecurityLevelService(ISecurityLevelRepository repository)
    {
        _repository = repository;
    }

    public async Task<SecurityLevelDto?> GetByIdAsync(int id)
    {
        var securityLevel = await _repository.GetByIdAsync(id);
        return securityLevel == null ? null : MapToDto(securityLevel);
    }

    public async Task<IEnumerable<SecurityLevelDto>> GetAllAsync()
    {
        var securityLevels = await _repository.GetAllAsync();
        return securityLevels.Select(MapToDto);
    }

    public async Task<SecurityLevelDto> CreateAsync(CreateSecurityLevelDto createDto)
    {
        if (await _repository.ExistsByCodeAsync(createDto.Code))
        {
            throw new SecurityLevelAlreadyExistsException($"Уровень безопасности с кодом '{createDto.Code}' уже существует");
        }

        if (await _repository.ExistsByNameAsync(createDto.Name))
        {
            throw new SecurityLevelAlreadyExistsException($"Уровень безопасности с именем '{createDto.Name}' уже существует");
        }

        var securityLevel = new SecurityLevel
        {
            Code = createDto.Code,
            Name = createDto.Name,
            Description = createDto.Description,
            IsAlive = createDto.IsAlive
        };

        await _repository.CreateAsync(securityLevel);
        return MapToDto(securityLevel);
    }

    public async Task<SecurityLevelDto> UpdateAsync(int id, UpdateSecurityLevelDto updateDto)
    {
        var securityLevel = await _repository.GetByIdAsync(id);
        if (securityLevel == null)
            throw new KeyNotFoundException($"Уровень безопасности с ID {id} не найден");

        if (await _repository.ExistsByCodeAsync(updateDto.Code, id))
        {
            throw new SecurityLevelAlreadyExistsException($"Уровень безопасности с кодом '{updateDto.Code}' уже существует");
        }

        if (await _repository.ExistsByNameAsync(updateDto.Name, id))
        {
            throw new SecurityLevelAlreadyExistsException($"Уровень безопасности с именем '{updateDto.Name}' уже существует");
        }

        securityLevel.Code = updateDto.Code;
        securityLevel.Name = updateDto.Name;
        securityLevel.Description = updateDto.Description;
        securityLevel.IsAlive = updateDto.IsAlive;

        await _repository.UpdateAsync(securityLevel);
        return MapToDto(securityLevel);
    }

    public async Task DeleteAsync(int id)
    {
        var securityLevel = await _repository.GetByIdAsync(id);
        if (securityLevel == null)
            throw new KeyNotFoundException($"Уровень безопасности с ID {id} не найден");

        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SecurityLevelDto>> BulkCreateAsync(BulkCreateSecurityLevelDto bulkCreateDto)
    {
        var createdSecurityLevels = new List<SecurityLevelDto>();
        var errors = new List<string>();

        foreach (var item in bulkCreateDto.Items)
        {
            try
            {
                var created = await CreateAsync(item);
                createdSecurityLevels.Add(created);
            }
            catch (Exception ex)
            {
                errors.Add($"Ошибка при создании уровня безопасности '{item.Name}': {ex.Message}");
            }
        }

        if (errors.Any())
        {
            throw new AggregateException("Ошибки при массовом создании уровней безопасности", errors.Select(e => new Exception(e)));
        }

        return createdSecurityLevels;
    }

    public async Task<IEnumerable<SecurityLevelDto>> BulkUpdateAsync(BulkUpdateSecurityLevelDto bulkUpdateDto)
    {
        var updatedSecurityLevels = new List<SecurityLevelDto>();
        var errors = new List<string>();

        foreach (var item in bulkUpdateDto.Items)
        {
            try
            {
                var updateDto = new UpdateSecurityLevelDto
                {
                    Code = item.Code,
                    Name = item.Name,
                    Description = item.Description,
                    IsAlive = item.IsAlive
                };
                var updated = await UpdateAsync(item.Id, updateDto);
                updatedSecurityLevels.Add(updated);
            }
            catch (Exception ex)
            {
                errors.Add($"Ошибка при обновлении уровня безопасности с ID {item.Id}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            throw new AggregateException("Ошибки при массовом обновлении уровней безопасности", errors.Select(e => new Exception(e)));
        }

        return updatedSecurityLevels;
    }

    private static SecurityLevelDto MapToDto(SecurityLevel securityLevel)
    {
        return new SecurityLevelDto
        {
            Id = securityLevel.Id,
            Code = securityLevel.Code,
            Name = securityLevel.Name,
            Description = securityLevel.Description,
            IsAlive = securityLevel.IsAlive
        };
    }
} 