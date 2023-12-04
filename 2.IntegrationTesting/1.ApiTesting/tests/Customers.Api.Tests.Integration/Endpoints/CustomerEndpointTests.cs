using System.Net;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Customers.Api.Tests.Integration.Endpoints;

[Collection("Shared collection")]
public class CustomerEndpointTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDb;
    private readonly Action<string> _setupGitHubUser;
    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, f => f.Person.Email)
        .RuleFor(x => x.FullName, f => f.Person.FullName)
        .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.Date)
        .RuleFor(x => x.GitHubUsername, f => f.Person.UserName.Replace(".", "").Replace("-", "").Replace("_", ""))
        .UseSeed(100);

    public CustomerEndpointTests(CustomerApiFactory waf)
    {
        _client = waf.HttpClient;
        _resetDb = waf.ResetDatabaseAsync;
        _setupGitHubUser = waf.RegisterGitHubUser;
    }
    
    [Fact]
    public async Task Create_ShouldCreateCustomer_WhenDetailsAreValid()
    {
        // Arrange
        var request = _customerGenerator.Generate();
        _setupGitHubUser(request.GitHubUsername);
        
        var expectedResponse = new CustomerResponse
        {
            Email = request.Email,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            GitHubUsername = request.GitHubUsername
        };

        // Act
        var response = await _client.PostAsJsonAsync("customers", request);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().Be($"https://localhost:5001/customers/{customerResponse.Id}");
        customerResponse.Should().BeEquivalentTo(expectedResponse, x => x.Excluding(e => e.Id));
        customerResponse.Id.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenTheEmailIsInvalid()
    {
        // Arrange
        var request = _customerGenerator.Clone()
            .RuleFor(x => x.Email, _ => "fhsiudsnidsi")
            .Generate();
        _setupGitHubUser(request.GitHubUsername);

        // Act
        var response = await _client.PostAsJsonAsync("customers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors["Email"].Should().Equal("nick is not a valid email address");
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnAllCustomers_WhenCustomersExist()
    {
        // Arrange
        var request = new CustomerRequest
        {
            Email = "nick@chapsas.com",
            FullName = "Nick Chapsas",
            DateOfBirth = new DateTime(1993, 01, 01),
            GitHubUsername = "nickchapsas"
        };
    
        var createCustomerHttpResponse = await _client.PostAsJsonAsync("customers", request);
        var createdCustomer = await createCustomerHttpResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        // Act
        var response = await _client.GetAsync("customers");

        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<GetAllCustomersResponse>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        customerResponse!.Customers.Should().ContainEquivalentOf(createdCustomer).And.HaveCount(1);
    }
    
    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_ShouldUpdateCustomerDetails_WhenDetailsAreValid()
    {
        // Arrange
        var createRequest = new CustomerRequest
        {
            Email = "nick@chapsas.com",
            FullName = "Nick Chapsas",
            DateOfBirth = new DateTime(1993, 01, 01),
            GitHubUsername = "nickchapsas"
        };
    
        var createCustomerHttpResponse = await _client.PostAsJsonAsync("customers", createRequest);
        var createdCustomer = await createCustomerHttpResponse.Content.ReadFromJsonAsync<CustomerResponse>();
    
        var updateRequest = new CustomerRequest
        {
            Email = "chapsas@nick.com",
            FullName = "Nick Chapsas",
            DateOfBirth = new DateTime(1993, 01, 01),
            GitHubUsername = "nickchapsas"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"customers/{createdCustomer!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);    
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customerResponse.Should().BeEquivalentTo(updateRequest);
    }
    
    [Fact]
    public async Task Delete_ShouldDeleteCustomer_WhenCustomerExists()
    {
        // Arrange
        var createRequest = new CustomerRequest
        {
            Email = "nick@chapsas.com",
            FullName = "Nick Chapsas",
            DateOfBirth = new DateTime(1993, 01, 01),
            GitHubUsername = "nickchapsas"
        };
    
        var createCustomerHttpResponse = await _client.PostAsJsonAsync("customers", createRequest);
        var createdCustomer = await createCustomerHttpResponse.Content.ReadFromJsonAsync<CustomerResponse>();

        // Act
        var response = await _client.DeleteAsync($"customers/{createdCustomer!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _resetDb();
    }
}
