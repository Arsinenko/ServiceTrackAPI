using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class JobTypeController : ControllerBase
{
    private readonly IJobTypeService _jobTypeService;

    public JobTypeController(IJobTypeService jobTypeService)
    {
        _jobTypeService = jobTypeService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobTypeDto>> GetById(Guid id)
    {
        var jobType = await _jobTypeService.GetByIdAsync(id);
        if (jobType == null)
            return NotFound();
        return Ok(jobType);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<JobTypeDto>> GetByName(string name)
    {
        var jobType = await _jobTypeService.GetBeyNameAsync(name);
        if (jobType == null)
            return NotFound();
        return Ok(jobType);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobTypeDto>>> GetAll()
    {
        var jobTypes = await _jobTypeService.GetAllAsync();
        return Ok(jobTypes);
    }

    [HttpPost]
    public async Task<ActionResult<JobTypeDto>> Create(CreateJobTypeDto jobTypeDto)
    {
        var  jobType = await _jobTypeService.CreateAsync(jobTypeDto);
        return Ok(jobType);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<JobTypeDto>> Update(Guid id, UpdateJobTypeDto jobTypeDto)
    {
        var jobType = await _jobTypeService.UpdateAsync(id, jobTypeDto);
        if (jobType == null)
        {
            return NotFound();
        }
        return Ok(jobType);
    }
    
}