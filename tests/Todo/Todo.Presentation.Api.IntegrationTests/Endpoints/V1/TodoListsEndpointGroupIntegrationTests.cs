using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SharedCore.Presentation.Dtos;
using Todo.Presentation.Api.Dtos.V1.TodoLists;
using Todo.Presentation.Api.Dtos.V1.TodoLists.Create;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Create;
using Todo.Presentation.Api.Dtos.V1.TodoLists.TodoItems.Update;
using Todo.Presentation.Api.Dtos.V1.TodoLists.Update;
using Todo.Presentation.Api.IntegrationTests.Fixtures;

namespace Todo.Presentation.Api.IntegrationTests.Endpoints.V1;

public class TodoListsEndpointGroupIntegrationTests : BaseIntegrationTests
{
    private const string TodoListsPathBase = "api/todoLists";
    private const string TodoItemsPathSegment = "todoItems";

    public TodoListsEndpointGroupIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateTodoListAsync_ShouldSucceed()
    {
        // Arrange
        var request = new CreateTodoListApiDto { Name = "Name" };

        // Act
        var response = await Client.PostAsJsonAsync(TodoListsPathBase, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var todoListResponse =
            await response.Content.ReadFromJsonAsync<TodoListApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(todoListResponse);
        Assert.Equal(request.Name, todoListResponse.Name);
    }

    [Fact]
    public async Task CreateTodoListAsync_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateTodoListApiDto { Name = string.Empty };

        // Act
        var response = await Client.PostAsJsonAsync(TodoListsPathBase, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task CreateTodoListAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, TodoListsPathBase);
        httpRequestMessage.Headers.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoListByIdAsync_ShouldSucceed()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();

        // Act
        var response =
            await Client.GetAsync($"{TodoListsPathBase}/{todoListId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todoListResponse =
            await response.Content.ReadFromJsonAsync<TodoListApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(todoListResponse);
        Assert.Equal(todoListId, todoListResponse.Id);
    }

    [Fact]
    public async Task GetTodoListByIdAsync_WhenUnknownId_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = Guid.NewGuid();

        // Act
        var response =
            await Client.GetAsync($"{TodoListsPathBase}/{todoListId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task GetTodoListByIdAsync_WhenIdFromOtherUser_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = await CreateTodoListForOtherUserAsync();

        // Act
        var response =
            await Client.GetAsync($"{TodoListsPathBase}/{todoListId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task GetTodoListByIdAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{TodoListsPathBase}/{todoListId}");
        httpRequestMessage.Headers.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoListsAsync_ShouldSucceed()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();

        // Act
        var response = await Client.GetAsync($"{TodoListsPathBase}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todoListsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedQueryResultApiDto<TodoListApiDto>>(TestContext.Current
                .CancellationToken);
        Assert.NotNull(todoListsResponse);
        Assert.Contains(todoListsResponse.Records, l => l.Id == todoListId);
    }

    [Fact]
    public async Task GetTodoListsAsync_ShouldNotHaveOtherUserTodoList()
    {
        // Arrange
        var idFromOtherUser = await CreateTodoListForOtherUserAsync();

        // Act
        var response = await Client.GetAsync($"{TodoListsPathBase}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todoListsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedQueryResultApiDto<TodoListApiDto>>(TestContext.Current
                .CancellationToken);
        Assert.NotNull(todoListsResponse);
        Assert.DoesNotContain(todoListsResponse.Records, l => l.Id == idFromOtherUser);
    }

    [Fact]
    public async Task GetTodoListsAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{TodoListsPathBase}");
        httpRequestMessage.Headers.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodoListAsync_ShouldSucceed()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();
        var request = new UpdateTodoListApiDto { Name = "NewName" };

        // Act
        var response = await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}", request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todoListResponse =
            await response.Content.ReadFromJsonAsync<TodoListApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(todoListResponse);
        Assert.Equal(request.Name, todoListResponse.Name);
    }

    [Fact]
    public async Task UpdateTodoListAsync_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        var request = new UpdateTodoListApiDto { Name = string.Empty };

        // Act
        var response = await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}", request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateTodoListAsync_WhenUnknownId_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        var request = new UpdateTodoListApiDto { Name = "NewName" };

        // Act
        var response = await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}", request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateTodoListAsync_WhenIdFromOtherUser_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = await CreateTodoListForOtherUserAsync();
        var request = new UpdateTodoListApiDto { Name = "NewName" };

        // Act
        var response = await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}", request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateTodoListAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, $"{TodoListsPathBase}/{todoListId}");
        httpRequestMessage.Headers.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodoListAsync_ShouldSucceed()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();

        // Act
        var response =
            await Client.DeleteAsync($"{TodoListsPathBase}/{todoListId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todoListResponse =
            await response.Content.ReadFromJsonAsync<TodoListApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(todoListResponse);
        Assert.Equal(todoListId, todoListResponse.Id);
    }

    [Fact]
    public async Task DeleteTodoListAsync_WhenUnknownId_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = Guid.NewGuid();

        // Act
        var response =
            await Client.DeleteAsync($"{TodoListsPathBase}/{todoListId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task DeleteTodoListAsync_WhenIdFromOtherUser_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = await CreateTodoListForOtherUserAsync();

        // Act
        var response =
            await Client.DeleteAsync($"{TodoListsPathBase}/{todoListId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task DeleteTodoListAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{TodoListsPathBase}/{todoListId}");
        httpRequestMessage.Headers.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodoItemAsync_ShouldSucceed()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();
        var request = new CreateTodoItemApiDto
        {
            Title = "Title",
            Description = "Description",
            Schedule = new TodoItemScheduleApiDto(DateTimeOffset.UtcNow.AddDays(1))
        };

        // Act
        var response =
            await Client.PostAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}", request,
                TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var todoItemResponse =
            await response.Content.ReadFromJsonAsync<TodoItemApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(todoItemResponse);
        Assert.Equal(request.Title, todoItemResponse.Title);
        Assert.Equal(request.Description, todoItemResponse.Description);
        Assert.Equal(request.Schedule.DueDate, todoItemResponse.Schedule.DueDate);
        Assert.False(todoItemResponse.IsDone);
    }

    [Fact]
    public async Task CreateTodoItemAsync_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        var request = new CreateTodoItemApiDto { Title = string.Empty };

        // Act
        var response =
            await Client.PostAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}", request,
                TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task CreateTodoItemAsync_WhenUnknownTodoList_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        var request = new CreateTodoItemApiDto { Title = "Title" };

        // Act
        var response =
            await Client.PostAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}", request,
                TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateTodoItemAsync_ShouldSucceed()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();
        var todoItemId = await CreateTodoItemAsync(todoListId);
        var request = new UpdateTodoItemApiDto
        {
            Title = "NewTitle",
            Description = "NewDescription",
            Schedule = new TodoItemScheduleApiDto(DateTimeOffset.UtcNow.AddDays(1)),
            IsDone = true
        };

        // Act
        var response =
            await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}/{todoItemId}",
                request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todoItemResponse =
            await response.Content.ReadFromJsonAsync<TodoItemApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(todoItemResponse);
        Assert.Equal(request.Title, todoItemResponse.Title);
        Assert.Equal(request.Description, todoItemResponse.Description);
        Assert.Equal(request.Schedule.DueDate, todoItemResponse.Schedule.DueDate);
        Assert.Equal(request.IsDone, todoItemResponse.IsDone);
    }

    [Fact]
    public async Task UpdateTodoItemAsync_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        var todoItemId = Guid.NewGuid();
        var request = new UpdateTodoItemApiDto { Title = string.Empty };

        // Act
        var response =
            await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}/{todoItemId}",
                request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateTodoItemAsync_WhenUnknownTodoList_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        var todoItemId = Guid.NewGuid();
        var request = new UpdateTodoItemApiDto { Title = "Title" };

        // Act
        var response =
            await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}/{todoItemId}",
                request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateTodoItemAsync_WhenUnknownTodoItem_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();
        var todoItemId = Guid.NewGuid();
        var request = new UpdateTodoItemApiDto { Title = "Title" };

        // Act
        var response =
            await Client.PatchAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}/{todoItemId}",
                request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task DeleteTodoItemAsync_ShouldSucceed()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();
        var todoItemId = await CreateTodoItemAsync(todoListId);

        // Act
        var response =
            await Client.DeleteAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}/{todoItemId}",
                TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todoItemResponse =
            await response.Content.ReadFromJsonAsync<TodoItemApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(todoItemResponse);
    }

    [Fact]
    public async Task DeleteTodoItemAsync_WhenUnknownTodoList_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = Guid.NewGuid();
        var todoItemId = Guid.NewGuid();

        // Act
        var response =
            await Client.DeleteAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}/{todoItemId}",
                TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    [Fact]
    public async Task DeleteTodoItemAsync_WhenUnknownTodoItem_ShouldReturnNotFound()
    {
        // Arrange
        var todoListId = await CreateTodoListAsync();
        var todoItemId = Guid.NewGuid();

        // Act
        var response =
            await Client.DeleteAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}/{todoItemId}",
                TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }

    private async Task<Guid> CreateTodoListAsync()
    {
        var request = new CreateTodoListApiDto { Name = "Name" };

        var response = await Client.PostAsJsonAsync(TodoListsPathBase, request);

        var todoListResponse = await response.Content.ReadFromJsonAsync<TodoListApiDto>();

        return todoListResponse!.Id;
    }

    private async Task<Guid> CreateTodoListForOtherUserAsync()
    {
        var request = new CreateTodoListApiDto { Name = "Name" };
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{TodoListsPathBase}");
        httpRequestMessage.Content = JsonContent.Create(request);
        httpRequestMessage.Headers.Authorization =
            new AuthenticationHeaderValue(TestAuthHandler.SchemeName, TestAuthHandler.OtherUserToken);

        var response = await Client.SendAsync(httpRequestMessage);

        var todoListResponse = await response.Content.ReadFromJsonAsync<TodoListApiDto>();

        return todoListResponse!.Id;
    }

    private async Task<Guid> CreateTodoItemAsync(Guid todoListId)
    {
        var request = new CreateTodoItemApiDto { Title = "Title" };

        var response =
            await Client.PostAsJsonAsync($"{TodoListsPathBase}/{todoListId}/{TodoItemsPathSegment}", request);

        var todoItemResponse = await response.Content.ReadFromJsonAsync<TodoItemApiDto>();

        return todoItemResponse!.Id;
    }
}