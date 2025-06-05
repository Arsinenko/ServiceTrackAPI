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
    private readonly ICustomerRepository _customerRepository;
    private readonly IInspectionMethodRepository _methodRepository;

    public ServiceRequestService(
        IServiceRequestRepository serviceRequestRepository,
        IUserRepository userRepository,
        IEquipmentRepository equipmentRepository,
        IJobTypeRepository jobTypeRepository,
        ICustomerRepository customerRepository,
        IInspectionMethodRepository methodRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _userRepository = userRepository;
        _equipmentRepository = equipmentRepository;
        _jobTypeRepository = jobTypeRepository;
        _customerRepository = customerRepository;
        _methodRepository = methodRepository;
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
        // Проверка на дублирующийся ContractId
        var existingRequest = (await _serviceRequestRepository.GetAllAsync())
            .FirstOrDefault(r => r.ContractId == createDto.ContractId);
        if (existingRequest != null)
            throw new ArgumentException($"ContractId {createDto.ContractId} already exists");

        var jobType = await _jobTypeRepository.GetByIdAsync(createDto.JobTypeId);
        if (jobType == null)
            throw new ArgumentException("Invalid job type ID");

        var customer = await _customerRepository.GetByIdAsync(createDto.CustomerId);
        if (customer == null)
            throw new ArgumentException("Invalid customer ID");

        var request = new ServiceRequest
        {
            ContractId = createDto.ContractId,
            CustomerId = createDto.CustomerId,
            Customer = customer,
            Reasons = createDto.Reasons,
            JobTypeId = createDto.JobTypeId,
            JobType = jobType,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false,
            UserServiceRequests = new List<UserServiceRequest>(),
            ServiceRequestEquipments = new List<ServiceRequestEquipment>(),
            RequestNumber = createDto.RequestNumber,
            PlannedCompletionDate = createDto.PlannedCompletionDate,
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

    public async Task<List<ServiceRequestDto>> CreateBulkAsync(CreateServiceRequestBulkDto createDto)
    {
        // Check for duplicate ContractIds in the input
        var contractIds = createDto.ServiceRequests.Select(r => r.ContractId).ToList();
        var duplicateContractIds = contractIds.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateContractIds.Any())
        {
            throw new ArgumentException($"Duplicate ContractIds found: {string.Join(", ", duplicateContractIds)}");
        }

        // Check for existing ContractIds in the database
        var existingRequests = await _serviceRequestRepository.GetAllAsync();
        var existingContractIds = existingRequests.Select(r => r.ContractId).ToList();
        var conflictingContractIds = contractIds.Intersect(existingContractIds).ToList();

        if (conflictingContractIds.Any())
        {
            throw new ArgumentException($"ContractIds already exist in the database: {string.Join(", ", conflictingContractIds)}");
        }

        var requests = new List<ServiceRequest>();
        
        foreach (var requestDto in createDto.ServiceRequests)
        {
            var jobType = await _jobTypeRepository.GetByIdAsync(requestDto.JobTypeId);
            if (jobType == null)
                throw new ArgumentException($"Invalid job type ID for request: {requestDto.Reasons}");

            var customer = await _customerRepository.GetByIdAsync(requestDto.CustomerId);
            if (customer == null)
                throw new ArgumentException($"Invalid customer ID for request: {requestDto.ContractId}");

            var request = new ServiceRequest
            {
                ContractId = requestDto.ContractId,
                CustomerId = requestDto.CustomerId,
                RequestNumber = requestDto.RequestNumber,
                PlannedCompletionDate = requestDto.PlannedCompletionDate,
                Customer = customer,
                Reasons = requestDto.Reasons,
                JobTypeId = requestDto.JobTypeId,
                JobType = jobType,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false,
                UserServiceRequests = new List<UserServiceRequest>(),
                ServiceRequestEquipments = new List<ServiceRequestEquipment>()
            };

            // Add initial user assignments
            foreach (var assignment in requestDto.InitialAssignments)
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
            foreach (var equipment in requestDto.InitialEquipment)
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

            requests.Add(request);
        }

        var createdIds = await _serviceRequestRepository.CreateBulkAsync(requests);
        
        // Load the created requests with their related entities
        var createdRequests = new List<ServiceRequest>();
        foreach (var id in createdIds)
        {
            var request = await _serviceRequestRepository.GetByIdAsync(id);
            if (request != null)
            {
                createdRequests.Add(request);
            }
        }
        
        return createdRequests.Select(ServiceRequestDto.FromServiceRequest).ToList();
    }

    public async Task<ServiceRequestDto?> UpdateAsync(int id, UpdateServiceRequestDto updateDto)
    {
        var request = await _serviceRequestRepository.GetByIdAsync(id);
        if (request == null)
            return null;

        var jobType = await _jobTypeRepository.GetByIdAsync(updateDto.JobTypeId);
        if (jobType == null)
            throw new ArgumentException("Invalid job type ID");

        var customer = await _customerRepository.GetByIdAsync(updateDto.CustomerId);
        if (customer == null)
            throw new ArgumentException("Invalid customer ID");

        request.CustomerId = updateDto.CustomerId;
        request.Customer = customer;
        request.Reasons = updateDto.Description;
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

    public async Task<DeleteServiceRequestBulkResult> DeleteBulkAsync(IEnumerable<int> requestIds)
    {
        var deletedRequests = new List<ServiceRequestDto>();
        var failedRequestIds = new List<int>();
        var failureReasons = new List<string>();

        foreach (var requestId in requestIds)
        {
            try
            {
                var request = await _serviceRequestRepository.GetByIdAsync(requestId);
                if (request == null)
                {
                    failedRequestIds.Add(requestId);
                    failureReasons.Add("Request not found");
                    continue;   
                }
                await _serviceRequestRepository.DeleteAsync(requestId);
                deletedRequests.Add(ServiceRequestDto.FromServiceRequest(request));
            }
            catch (Exception ex)
            {
                failedRequestIds.Add(requestId);
                failureReasons.Add(ex.Message);
            }
        }

        return new DeleteServiceRequestBulkResult
        {
            DeletedServiceRequests = deletedRequests,
            FailedServiceRequestIds = failedRequestIds,
            FailureReasons = failureReasons
        };
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

    public async Task<List<ServiceRequestDto>> CreateBulkWithNewEquipmentAsync(CreateServiceRequestWithNewEquipmentBulkDto createDto)
    {
        // Check for duplicate ContractIds in the input
        var contractIds = createDto.ServiceRequests.Select(r => r.ContractId).ToList();
        var duplicateContractIds = contractIds.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateContractIds.Any())
        {
            throw new ArgumentException($"Duplicate ContractIds found: {string.Join(", ", duplicateContractIds)}");
        }

        // Check for existing ContractIds in the database
        var existingRequests = await _serviceRequestRepository.GetAllAsync();
        var existingContractIds = existingRequests.Select(r => r.ContractId).ToList();
        var conflictingContractIds = contractIds.Intersect(existingContractIds).ToList();

        if (conflictingContractIds.Any())
        {
            throw new ArgumentException($"ContractIds already exist in the database: {string.Join(", ", conflictingContractIds)}");
        }

        var requests = new List<ServiceRequest>();
        
        foreach (var requestDto in createDto.ServiceRequests)
        {
            var jobType = await _jobTypeRepository.GetByIdAsync(requestDto.JobTypeId);
            if (jobType == null)
                throw new ArgumentException($"Invalid job type ID for request: {requestDto.Reasons}");

            var customer = await _customerRepository.GetByIdAsync(requestDto.CustomerId);
            if (customer == null)
                throw new ArgumentException($"Invalid customer ID for request: {requestDto.ContractId}");

            var request = new ServiceRequest
            {
                ContractId = requestDto.ContractId,
                CustomerId = requestDto.CustomerId,
                RequestNumber = requestDto.RequestNumber,
                PlannedCompletionDate = requestDto.PlannedCompletionDate,
                Customer = customer,
                Reasons = requestDto.Reasons,
                JobTypeId = requestDto.JobTypeId,
                JobType = jobType,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false,
                UserServiceRequests = new List<UserServiceRequest>(),
                ServiceRequestEquipments = new List<ServiceRequestEquipment>()
            };

            // Add initial user assignments
            foreach (var assignment in requestDto.InitialAssignments)
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

            // Create and add new equipment
            foreach (var equipmentDto in requestDto.NewEquipment)
            {
                // Validate ExecutorId exists
                var executor = await _userRepository.GetByIdAsync(equipmentDto.ExecutorId);
                if (executor == null)
                {
                    throw new ArgumentException($"Invalid executor ID for equipment: {equipmentDto.Name}");
                }

                var equipment = new Equipment
                {
                    Id = Guid.NewGuid(),
                    Name = equipmentDto.Name,
                    Model = equipmentDto.Model,
                    SerialNumber = equipmentDto.SerialNumber,
                    Manufacturer = equipmentDto.Manufacturer,
                    Category = equipmentDto.Category,
                    Quantity = equipmentDto.Quantity,
                    ExecutorId = equipmentDto.ExecutorId,
                    SecurityLevelId = equipmentDto.SecurityLevelId,
                    SZZ = equipmentDto.SZZ,
                    Description = equipmentDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    EquipmentInspectionMethods = new List<EquipmentInspectionMethod>(),
                    Attachments = new List<EquipmentAttachment>()
                };

                // Create components recursively if they exist
                if (equipmentDto.Components != null && equipmentDto.Components.Any())
                {
                    equipment.Components = new List<Equipment>();
                    foreach (var componentDto in equipmentDto.Components)
                    {
                        // Validate ExecutorId exists for component
                        var componentExecutor = await _userRepository.GetByIdAsync(componentDto.ExecutorId);
                        if (componentExecutor == null)
                        {
                            throw new ArgumentException($"Invalid executor ID for component: {componentDto.Name}");
                        }

                        var component = new Equipment
                        {
                            Id = Guid.NewGuid(),
                            Name = componentDto.Name,
                            Model = componentDto.Model,
                            SerialNumber = componentDto.SerialNumber,
                            Manufacturer = componentDto.Manufacturer,
                            Category = componentDto.Category,
                            Quantity = componentDto.Quantity,
                            ExecutorId = componentDto.ExecutorId,
                            SecurityLevelId = componentDto.SecurityLevelId,
                            SZZ = componentDto.SZZ,
                            Description = componentDto.Description,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            EquipmentInspectionMethods = new List<EquipmentInspectionMethod>(),
                            Attachments = new List<EquipmentAttachment>()
                        };

                        // Add inspection methods for component
                        foreach (var method in componentDto.Methods)
                        {
                            var inspectionMethod = await _methodRepository.GetByIdAsync(method.InspectionMethodId);
                            if (inspectionMethod == null)
                            {
                                throw new ArgumentException($"Invalid inspection method ID for component: {componentDto.Name}");
                            }
                            component.EquipmentInspectionMethods.Add(new EquipmentInspectionMethod
                            {
                                EquipmentId = component.Id,
                                InspectionMethodId = inspectionMethod.Id
                            });
                        }

                        // Save the component
                        await _equipmentRepository.CreateAsync(component);
                        equipment.Components.Add(component);
                    }
                }

                // Save the equipment
                await _equipmentRepository.CreateAsync(equipment);

                // Add equipment to service request
                request.ServiceRequestEquipments.Add(new ServiceRequestEquipment
                {
                    EquipmentId = equipment.Id,
                    ServiceRequestId = request.Id,
                    AddedAt = DateTime.UtcNow,
                    Notes = null
                });
            }

            requests.Add(request);
        }

        var createdIds = await _serviceRequestRepository.CreateBulkAsync(requests);
        
        // Load the created requests with their related entities
        var createdRequests = new List<ServiceRequest>();
        foreach (var id in createdIds)
        {
            var request = await _serviceRequestRepository.GetByIdAsync(id);
            if (request != null)
            {
                createdRequests.Add(request);
            }
        }
        
        return createdRequests.Select(ServiceRequestDto.FromServiceRequest).ToList();
    }
} 