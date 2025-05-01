using AuthApp.application.DTOs;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;

namespace AuthApp.application.Services;

public class ServiceRequestService : IServiceRequestService
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly IUserRepository _userRepository;

    public ServiceRequestService(
        IServiceRequestRepository serviceRequestRepository,
        IUserRepository userRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _userRepository = userRepository;
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
        var request = new ServiceRequest
        {
            ContractId = createDto.ContractId,
            Customer = createDto.Customer,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false,
            UserServiceRequests = new List<UserServiceRequest>()
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

        await _serviceRequestRepository.CreateAsync(request);
        return ServiceRequestDto.FromServiceRequest(request);
    }

    public async Task<ServiceRequestDto?> UpdateAsync(int id, UpdateServiceRequestDto updateDto)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(id);
        if (request == null)
            return null;

        request.Customer = updateDto.Customer;
        request.Description = updateDto.Description;
        request.IsCompleted = updateDto.IsCompleted;
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
} 