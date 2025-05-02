using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class ServiceRequestService : IServiceRequestService
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IJobTypeRepository _jobTypeRepository;

    public ServiceRequestService(
        IServiceRequestRepository serviceRequestRepository,
        IUserRepository userRepository,
        IEquipmentRepository equipmentRepository,
        IJobTypeRepository jobTypeRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _userRepository = userRepository;
        _equipmentRepository = equipmentRepository;
        _jobTypeRepository = jobTypeRepository;
    }

    public async Task<ServiceRequestDto?> GetByIdAsync(int id)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(id);
        return request != null ? ServiceRequestDto.FromServiceRequest(request) : null;
    }

    public async Task<IEnumerable<ServiceRequestDto>> GetAllAsync()
    {
        var requests = await _serviceRequestRepository.GetAllAsync();
        return requests.Select(r => ServiceRequestDto.FromServiceRequest(r));
    }

    public async Task<IEnumerable<ServiceRequestDto>> GetByUserIdAsync(Guid userId)
    {
        var requests = await _serviceRequestRepository.GetByUserIdAsync(userId);
        return requests.Select(r => ServiceRequestDto.FromServiceRequest(r));
    }

    public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto createDto)
    {
        var jobType = await _jobTypeRepository.GetByIdAsync(createDto.JobTypeId);
        if (jobType == null)
            throw new ArgumentException("Invalid job type ID");

        var request = new ServiceRequest
        {
            ContractId = createDto.ContractId,
            Customer = createDto.Customer,
            Description = createDto.Description,
            JobTypeId = createDto.JobTypeId,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false,
            UserServiceRequests = new List<UserServiceRequest>(),
            ServiceRequestEquipments = new List<ServiceRequestEquipment>()
        };

        // Add initial user assignments
        foreach (var assignment in createDto.InitialAssignments)
        {
            var user = await _userRepository.GetByIdAsync(assignment.UserId);
            if (user != null)
            {
                request.UserServiceRequests.Add(new UserServiceRequest
                {
                    UserId = user.Id,
                    ServiceRequestId = request.Id,
                    AssignedAt = DateTime.UtcNow,
                    IsPrimaryAssignee = assignment.IsPrimaryAssignee
                });
            }
        }

        // Add initial equipment assignments
        foreach (var equipment in createDto.InitialEquipment)
        {
            var eq = await _equipmentRepository.GetByIdAsync(equipment.EquipmentId);
            if (eq != null)
            {
                request.ServiceRequestEquipments.Add(new ServiceRequestEquipment
                {
                    EquipmentId = eq.Id,
                    ServiceRequestId = request.Id,
                    AddedAt = DateTime.UtcNow,
                    Notes = equipment.Notes
                });
            }
        }

        await _serviceRequestRepository.CreateAsync(request);
        return ServiceRequestDto.FromServiceRequest(request);
    }

    public async Task<ServiceRequestDto?> UpdateAsync(int id, UpdateServiceRequestDto updateDto)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(id);
        if (request == null)
            return null;

        var jobType = await _jobTypeRepository.GetByIdAsync(updateDto.JobTypeId);
        if (jobType == null)
            throw new ArgumentException("Invalid job type ID");

        request.Customer = updateDto.Customer;
        request.Description = updateDto.Description;
        request.IsCompleted = updateDto.IsCompleted;
        request.JobTypeId = updateDto.JobTypeId;
        request.UpdatedAt = DateTime.UtcNow;

        // Update user assignments
        if (updateDto.UserAssignments != null)
        {
            // Remove existing assignments
            request.UserServiceRequests.Clear();

            // Add new assignments
            foreach (var assignment in updateDto.UserAssignments)
            {
                var user = await _userRepository.GetByIdAsync(assignment.UserId);
                if (user != null)
                {
                    request.UserServiceRequests.Add(new UserServiceRequest
                    {
                        UserId = user.Id,
                        ServiceRequestId = request.Id,
                        AssignedAt = DateTime.UtcNow,
                        IsPrimaryAssignee = assignment.IsPrimaryAssignee
                    });
                }
            }
        }

        // Update equipment assignments
        if (updateDto.EquipmentAssignments != null)
        {
            // Remove existing assignments
            request.ServiceRequestEquipments.Clear();

            // Add new assignments
            foreach (var assignment in updateDto.EquipmentAssignments)
            {
                var equipment = await _equipmentRepository.GetByIdAsync(assignment.EquipmentId);
                if (equipment != null)
                {
                    request.ServiceRequestEquipments.Add(new ServiceRequestEquipment
                    {
                        EquipmentId = equipment.Id,
                        ServiceRequestId = request.Id,
                        AddedAt = DateTime.UtcNow,
                        Notes = assignment.Notes
                    });
                }
            }
        }

        await _serviceRequestRepository.UpdateAsync(request);
        return ServiceRequestDto.FromServiceRequest(request);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _serviceRequestRepository.DeleteAsync(id);
    }

    public async Task<ServiceRequestDto?> AssignUserAsync(int requestId, Guid userId, bool isPrimary = false)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(requestId);
        if (request == null)
            return null;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        // If this is a primary assignee, remove primary status from other users
        if (isPrimary)
        {
            foreach (var usr in request.UserServiceRequests)
            {
                usr.IsPrimaryAssignee = false;
            }
        }

        // Add new assignment
        request.UserServiceRequests.Add(new UserServiceRequest
        {
            UserId = user.Id,
            ServiceRequestId = request.Id,
            AssignedAt = DateTime.UtcNow,
            IsPrimaryAssignee = isPrimary
        });

        await _serviceRequestRepository.UpdateAsync(request);
        return ServiceRequestDto.FromServiceRequest(request);
    }

    public async Task<ServiceRequestDto?> UnassignUserAsync(int requestId, Guid userId)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(requestId);
        if (request == null)
            return null;

        var assignment = request.UserServiceRequests.FirstOrDefault(usr => usr.UserId == userId);
        if (assignment != null)
        {
            request.UserServiceRequests.Remove(assignment);
            await _serviceRequestRepository.UpdateAsync(request);
        }

        return ServiceRequestDto.FromServiceRequest(request);
    }

    public async Task<ServiceRequestDto?> AssignEquipmentAsync(int requestId, Guid equipmentId, string? notes = null)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(requestId);
        if (request == null)
            return null;

        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);
        if (equipment == null)
            return null;

        // Add new assignment
        request.ServiceRequestEquipments.Add(new ServiceRequestEquipment
        {
            EquipmentId = equipment.Id,
            ServiceRequestId = request.Id,
            AddedAt = DateTime.UtcNow,
            Notes = notes
        });

        await _serviceRequestRepository.UpdateAsync(request);
        return ServiceRequestDto.FromServiceRequest(request);
    }

    public async Task<ServiceRequestDto?> UnassignEquipmentAsync(int requestId, Guid equipmentId)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(requestId);
        if (request == null)
            return null;

        var assignment = request.ServiceRequestEquipments.FirstOrDefault(sre => sre.EquipmentId == equipmentId);
        if (assignment != null)
        {
            request.ServiceRequestEquipments.Remove(assignment);
            await _serviceRequestRepository.UpdateAsync(request);
        }

        return ServiceRequestDto.FromServiceRequest(request);
    }
} 