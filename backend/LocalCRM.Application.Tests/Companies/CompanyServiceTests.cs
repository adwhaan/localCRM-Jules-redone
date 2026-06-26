using AutoMapper;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Application.Companies;
using LocalCRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LocalCRM.Application.Tests.Companies;

public class CompanyServiceTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public CompanyServiceTests()
    {
        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<CompanyMappingProfile>();
        });
        _mapper = config.CreateMapper();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(x => x.Username).Returns("testuser");
    }

    [Fact]
    public async Task CreateCompanyAsync_ShouldCreateCompanyAndAuditLog()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LocalCRMContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new LocalCRMContext(options);
        var service = new CompanyService(context, _mapper, _currentUserServiceMock.Object);

        var command = new CreateCompanyCommand
        {
            Name = "Test Company",
            CompanyRef = "TC001",
            City = "Test City",
            CompanyType = "Client",
            Rating = 3
        };

        // Act
        var result = await service.CreateCompanyAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Company", result.Name);
        Assert.Equal(1, await context.Companies.CountAsync());
        Assert.Equal(1, await context.AuditLogs.CountAsync());
    }
}
