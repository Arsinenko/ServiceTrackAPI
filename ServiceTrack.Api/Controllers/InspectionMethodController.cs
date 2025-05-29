using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InspectionMethodController : ControllerBase
{
    private readonly IInspectionMethodService _inspectionMethodService;

    public InspectionMethodController(IInspectionMethodService inspectionMethodService)
    {
        _inspectionMethodService = inspectionMethodService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InspectionMethodDto>> Get(int id)
    {
        var result = await _inspectionMethodService.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<InspectionMethodDto>>> GetAll()
    {
        var result = await _inspectionMethodService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<InspectionMethodDto>> GetByName(string name)
    {
        var result =  await _inspectionMethodService.GetByNameAsync(name);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<InspectionMethodDto>> Create(CreateInspectionMethodItemDto inspectionMethod)
    {
        var result = await _inspectionMethodService.CreateAsync(inspectionMethod);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<List<InspectionMethodDto>>> BulkCreate(CreateInspectionMethodsDto inspectionMethods)
    {
        var result = await _inspectionMethodService.CreateBulkAsync(inspectionMethods);
        return Ok(result);
    }

    [HttpPut("bulk")]
    public async Task<ActionResult<List<InspectionMethodDto>>> BulkUpdate(UpdateInspectionMethodsDto inspectionMethods)
    {
        var result = await _inspectionMethodService.UpdateBulkAsync(inspectionMethods);
        return Ok(result);
    }
    
}