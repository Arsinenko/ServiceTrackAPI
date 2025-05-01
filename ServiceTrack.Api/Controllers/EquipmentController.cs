using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentServiceservice;

    public EquipmentController(IEquipmentService equipmentServiceservice)
    {
        _equipmentServiceservice = equipmentServiceservice;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentDto>>> GetAllEquipment()
    {
        var equipment = await _equipmentServiceservice.GetAllAsync();
        return Ok(equipment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EquipmentDto>> GetEquipmentById(Guid id)
    {
        var equipment = await _equipmentServiceservice.GetByIdAsync(id);
        return Ok(equipment);
    }
    
    [HttpPost]
    public async Task<ActionResult<EquipmentDto>> CreateEquipment(CreateEquipmentDto createEquipmentDto)
    {
        var equipment = await _equipmentServiceservice.CreateAsync(createEquipmentDto);
        return CreatedAtAction(nameof(GetEquipmentById), new { id = equipment.Id }, equipment);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EquipmentDto>> UpdateEquipment(Guid id, UpdateEquipmentDto updateEquipmentDto)
    {
        var equipment = await _equipmentServiceservice.GetByIdAsync(id);
        if (equipment == null)
        {
            return NotFound();
        }
        return Ok(equipment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEquipment(Guid id)
    {
        await _equipmentServiceservice.DeleteAsync(id);
        return NoContent();
    }
    
}