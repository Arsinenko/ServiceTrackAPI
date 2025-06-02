using System.ComponentModel.DataAnnotations;
using AuthApp.domain.Entities;

namespace AuthApp.application.DTOs;

public class JobTypeDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt  { get; set; }

    public static JobTypeDto FromJobType(JobType jobType)
    {
        return new JobTypeDto
        {
            Id = jobType.Id,
            Name = jobType.Name,
            Description = jobType.Description,
            CreatedAt = jobType.CreatedAt.ToLocalTime(),
            UpdatedAt = jobType.UpdatedAt?.ToLocalTime(),
        };
    }
}

public class CreateJobTypeDto
{
    [Required(ErrorMessage = "Name field is requred!")]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Reasons field is required!")]
    public required string Description { get; set; }
}

public class UpdateJobTypeDto : CreateJobTypeDto;

public class CreateJobTypeBulkDto
{
    public required ICollection<CreateJobTypeDto> JobTypes { get; set; }
}

public class UpdateJobTypeBulkItemDto : CreateJobTypeDto
{
    public required Guid Id { get; set; }
}

public class UpdateJobTypeBulkDto
{
    public required ICollection<UpdateJobTypeBulkItemDto> JobTypes { get; set; }
}