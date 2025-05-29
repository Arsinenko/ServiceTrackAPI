using AuthApp.application.Services;
using AuthApp.application.Interfaces;
using AuthApp.domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;
using System.IO;
using System.Text;

namespace ServiceTrack.Tests.Application;

public class EquipmentAttachmentServiceTests
{
    private readonly Mock<IEquipmentAttachmentRepository> _mockAttachmentRepository;
    private readonly Mock<IHostEnvironment> _mockEnvironment;
    private readonly EquipmentAttachmentService _service;
    private readonly string _testUploadDirectory;

    public EquipmentAttachmentServiceTests()
    {
        _mockAttachmentRepository = new Mock<IEquipmentAttachmentRepository>();
        _mockEnvironment = new Mock<IHostEnvironment>();
        _testUploadDirectory = Path.Combine(Path.GetTempPath(), "equipment_attachments_test");
        
        _mockEnvironment.Setup(e => e.ContentRootPath)
            .Returns(Path.GetTempPath());
        _mockEnvironment.Setup(e => e.EnvironmentName)
            .Returns("Test");

        _service = new EquipmentAttachmentService(_mockAttachmentRepository.Object, _mockEnvironment.Object);
        
        // Clean up test directory before each test
        if (Directory.Exists(_testUploadDirectory))
        {
            Directory.Delete(_testUploadDirectory, true);
        }
        Directory.CreateDirectory(_testUploadDirectory);
    }

    [Fact]
    public async Task SaveAttachmentAsync_WhenValidFile_ShouldSaveFileAndCreateAttachment()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var fileName = "test.txt";
        var fileContent = "Test content";
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        var equipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Model = "Test Model",
            SerialNumber = "123",
            Manufacturer = "Test Manufacturer",
            Category = 1,
            SecurityLevels = new List<EquipmentSecurityLevel>(),
            InspectionMethods = new List<EquipmentInspectionMethod>()
        };

        var expectedAttachment = new EquipmentAttachment
        {
            Id = 1,
            EquipmentId = equipmentId,
            FileName = fileName,
            FileSize = fileContent.Length,
            FilePath = It.IsAny<string>(),
            FileType = "text/plain",
            Description = "Test description",
            UploadDate = DateTime.UtcNow,
            Equipment = equipment
        };

        _mockAttachmentRepository.Setup(r => r.CreateAsync(It.IsAny<EquipmentAttachment>()))
            .ReturnsAsync((EquipmentAttachment a) => a);

        // Act
        var result = await _service.SaveAttachmentAsync(formFile, equipmentId, "Test description");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(equipmentId, result.EquipmentId);
        Assert.Equal(fileName, result.FileName);
        Assert.Equal(fileContent.Length, result.FileSize);
        Assert.Equal("text/plain", result.FileType);
        Assert.Equal("Test description", result.Description);
        
        // Verify file was saved to disk
        var savedFilePath = Path.Combine(_testUploadDirectory, result.FilePath);
        Assert.True(File.Exists(savedFilePath));
        
        // Cleanup
        if (File.Exists(savedFilePath))
        {
            File.Delete(savedFilePath);
        }
    }

    [Fact]
    public async Task SaveAttachmentsAsync_WhenValidFiles_ShouldSaveAllFilesAndCreateAttachments()
    {
        // Arrange
        var equipmentId = Guid.NewGuid();
        var files = new List<IFormFile>();
        var fileContents = new[] { "Content 1", "Content 2" };
        var fileNames = new[] { "test1.txt", "test2.txt" };

        var equipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Model = "Test Model",
            SerialNumber = "123",
            Manufacturer = "Test Manufacturer",
            Category = 1,
            SecurityLevels = new List<EquipmentSecurityLevel>(),
            InspectionMethods = new List<EquipmentInspectionMethod>()
        };

        for (int i = 0; i < fileContents.Length; i++)
        {
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents[i]));
            var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", fileNames[i])
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };
            files.Add(formFile);
        }

        var expectedAttachments = files.Select((f, i) => new EquipmentAttachment
        {
            Id = i + 1,
            EquipmentId = equipmentId,
            FileName = fileNames[i],
            FileSize = fileContents[i].Length,
            FilePath = It.IsAny<string>(),
            FileType = "text/plain",
            Description = "Test description",
            UploadDate = DateTime.UtcNow,
            Equipment = equipment
        }).ToList();

        _mockAttachmentRepository.Setup(r => r.CreateAsync(It.IsAny<EquipmentAttachment>()))
            .ReturnsAsync((EquipmentAttachment a) => a);

        // Act
        var results = await _service.SaveAttachmentsAsync(files, equipmentId, "Test description");

        // Assert
        Assert.NotNull(results);
        Assert.Equal(files.Count, results.Count);
        
        for (int i = 0; i < results.Count; i++)
        {
            Assert.Equal(equipmentId, results[i].EquipmentId);
            Assert.Equal(fileNames[i], results[i].FileName);
            Assert.Equal(fileContents[i].Length, results[i].FileSize);
            Assert.Equal("text/plain", results[i].FileType);
            Assert.Equal("Test description", results[i].Description);
            
            // Verify file was saved to disk
            var savedFilePath = Path.Combine(_testUploadDirectory, results[i].FilePath);
            Assert.True(File.Exists(savedFilePath));
            
            // Cleanup
            if (File.Exists(savedFilePath))
            {
                File.Delete(savedFilePath);
            }
        }
    }

    [Fact]
    public async Task DeleteAttachmentAsync_WhenAttachmentExists_ShouldDeleteFileAndAttachment()
    {
        // Arrange
        var attachmentId = 1;
        var equipmentId = Guid.NewGuid();
        var fileName = "test.txt";
        var filePath = "test_path.txt";
        
        var equipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Model = "Test Model",
            SerialNumber = "123",
            Manufacturer = "Test Manufacturer",
            Category = 1,
            SecurityLevels = new List<EquipmentSecurityLevel>(),
            InspectionMethods = new List<EquipmentInspectionMethod>()
        };
        
        var attachment = new EquipmentAttachment
        {
            Id = attachmentId,
            EquipmentId = equipmentId,
            FileName = fileName,
            FilePath = filePath,
            FileSize = 100,
            FileType = "text/plain",
            Description = "Test description",
            UploadDate = DateTime.UtcNow,
            Equipment = equipment
        };

        _mockAttachmentRepository.Setup(r => r.GetByIdAsync(attachmentId))
            .ReturnsAsync(attachment);
        _mockAttachmentRepository.Setup(r => r.DeleteAsync(attachment))
            .Returns(Task.CompletedTask);

        // Create a test file
        var testFilePath = Path.Combine(_testUploadDirectory, filePath);
        var directoryPath = Path.GetDirectoryName(testFilePath);
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(testFilePath, "Test content");
        }

        // Act
        await _service.DeleteAttachmentAsync(attachmentId);

        // Assert
        _mockAttachmentRepository.Verify(r => r.DeleteAsync(attachment), Times.Once);
        Assert.False(File.Exists(testFilePath));
    }

    [Fact]
    public async Task DeleteAttachmentsAsync_WhenAttachmentsExist_ShouldDeleteAllFilesAndAttachments()
    {
        // Arrange
        var attachmentIds = new List<int> { 1, 2 };
        var equipmentId = Guid.NewGuid();
        var attachments = new List<EquipmentAttachment>();
        
        var equipment = new Equipment
        {
            Id = equipmentId,
            Name = "Test Equipment",
            Model = "Test Model",
            SerialNumber = "123",
            Manufacturer = "Test Manufacturer",
            Category = 1,
            SecurityLevels = new List<EquipmentSecurityLevel>(),
            InspectionMethods = new List<EquipmentInspectionMethod>()
        };
        
        for (int i = 0; i < attachmentIds.Count; i++)
        {
            var fileName = $"test{i}.txt";
            var filePath = $"test_path{i}.txt";
            
            var attachment = new EquipmentAttachment
            {
                Id = attachmentIds[i],
                EquipmentId = equipmentId,
                FileName = fileName,
                FilePath = filePath,
                FileSize = 100,
                FileType = "text/plain",
                Description = "Test description",
                UploadDate = DateTime.UtcNow,
                Equipment = equipment
            };
            attachments.Add(attachment);

            // Create test files
            var testFilePath = Path.Combine(_testUploadDirectory, filePath);
            var directoryPath = Path.GetDirectoryName(testFilePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(testFilePath, "Test content");
            }
        }

        _mockAttachmentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => attachments.First(a => a.Id == id));
        _mockAttachmentRepository.Setup(r => r.DeleteBulkAsync(It.IsAny<List<EquipmentAttachment>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAttachmentsAsync(attachmentIds);

        // Assert
        _mockAttachmentRepository.Verify(r => r.DeleteBulkAsync(It.IsAny<List<EquipmentAttachment>>()), Times.Once);
        
        foreach (var attachment in attachments)
        {
            var testFilePath = Path.Combine(_testUploadDirectory, attachment.FilePath);
            Assert.False(File.Exists(testFilePath));
        }
    }
} 