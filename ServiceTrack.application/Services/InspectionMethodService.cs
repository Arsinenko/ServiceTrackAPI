using AuthApp.application.DTOs;
using AuthApp.application.Exceptions;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using ServiceTrack.application.Interfaces;
using CreateInspectionMethodItemDto = AuthApp.application.DTOs.CreateInspectionMethodItemDto;

namespace AuthApp.application.Services;

public class InspectionMethodService : IInspectionMethodService
{
    private readonly IInspectionMethodRepository _repository;

    public InspectionMethodService(IInspectionMethodRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<InspectionMethodDto>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync();
        return result.Select(InspectionMethodDto.FroMethodDto);

    }

    public async Task<InspectionMethodDto?> GetByIdAsync(int id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
        {
            return null;
        }
        return InspectionMethodDto.FroMethodDto(result);
    }

    public async Task<InspectionMethodDto?> GetByNameAsync(string code)
    {
        var result = await _repository.GetByNameAsync(code);
        if (result == null)
        {
            return null;
        }
        return InspectionMethodDto.FroMethodDto(result);
    }

    public async Task<InspectionMethodDto> CreateAsync(CreateInspectionMethodItemDto inspectionMethod)
    {
        // check name for duplicates 
        var codeExist = await _repository.GetByNameAsync(inspectionMethod.Code);
        if (codeExist != null)
        {
            throw new InspectionMethodNameAlreadyExistsException($"Inspection method with code {inspectionMethod.Code} already exists");
        }

        var method = new InspectionMethod
        {
            Code = inspectionMethod.Code,
            Name = inspectionMethod.Name,
            Description = inspectionMethod.Description,
            IsAlive = true
        };
        var result = await _repository.CreateAsync(method);
        return InspectionMethodDto.FroMethodDto(result);

    }

    public async Task<IEnumerable<InspectionMethodDto>> CreateBulkAsync(CreateInspectionMethodsDto inspectionMethods)
    {
        var inspectionMethodsDto = inspectionMethods.InspectionMethodItems.ToList();
        var methods = new List<InspectionMethod>();
        foreach (var inspectionMethod in inspectionMethodsDto)
        {
            methods.Add(new InspectionMethod
            {
                Code = inspectionMethod.Code,
                Name = inspectionMethod.Name,
                Description = inspectionMethod.Description,
                IsAlive = true
            });
        }
        var result = await _repository.CreateBulkAsync(methods);
        return result.Select(InspectionMethodDto.FroMethodDto);
    }

    public async Task<InspectionMethodDto> UpdateAsync(UpdateInspectionMethodItemDto inspectionMethod)
    {
        var method = await _repository.GetByIdAsync(inspectionMethod.Id);
        method!.Code = inspectionMethod.Code;
        method.Name = inspectionMethod.Name;
        method.Description = inspectionMethod.Description;
        method.IsAlive = inspectionMethod.IsAlive;
        await _repository.UpdateBulkAsync(new List<InspectionMethod>(){method});
        return InspectionMethodDto.FroMethodDto(method);
        
    }

    public async Task<List<InspectionMethodDto>> UpdateBulkAsync(UpdateInspectionMethodsDto inspectionMethods)
    {
        var methodsDto = inspectionMethods.InspectionMethodItems.ToList();
        var methods = new List<InspectionMethod>();
        foreach (var inspectionMethod in methodsDto)
        {
            methods.Add(new InspectionMethod
            {
                Id = inspectionMethod.Id,
                Code = inspectionMethod.Code,
                Name = inspectionMethod.Name,
                Description = inspectionMethod.Description,
                IsAlive = true
            });
        }
        var result = await _repository.UpdateBulkAsync(methods);
        return result.Select(InspectionMethodDto.FroMethodDto).ToList();
    }

    public Task<List<InspectionMethodDto>> DeleteBulkAsync(List<InspectionMethod> inspectionMethods)
    {
        throw new NotImplementedException();
    }
}