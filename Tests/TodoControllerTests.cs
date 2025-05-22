using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Controllers;
using TodoApp.Data;
using TodoApp.Models;
using Xunit;

namespace TodoApp.Tests
{
    public class TodoControllerTests : IDisposable
    {
        private readonly TodoContext _context;
        private readonly TodoController _controller;
        private readonly Mock<ILogger<TodoController>> _loggerMock;

        public TodoControllerTests()
        {
            // 使用内存数据库进行测试
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TodoContext(options);
            _loggerMock = new Mock<ILogger<TodoController>>();
            _controller = new TodoController(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task GetTodos_ReturnsEmptyList_WhenNoTodosExist()
        {
            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var todos = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Empty(todos);
        }

        [Fact]
        public async Task GetTodos_ReturnsTodos_WhenTodosExist()
        {
            // Arrange
            var todo1 = new TodoItem { Title = "Test Todo 1", Description = "Description 1" };
            var todo2 = new TodoItem { Title = "Test Todo 2", Description = "Description 2" };
            
            _context.TodoItems.AddRange(todo1, todo2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var todos = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Equal(2, todos.Count());
        }

        [Fact]
        public async Task GetTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Act
            var result = await _controller.GetTodo(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTodo_ReturnsTodo_WhenTodoExists()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo", Description = "Test Description" };
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetTodo(todo.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodo = Assert.IsType<TodoItem>(okResult.Value);
            Assert.Equal(todo.Id, returnedTodo.Id);
            Assert.Equal(todo.Title, returnedTodo.Title);
        }

        [Fact]
        public async Task CreateTodo_ReturnsCreatedResult_WithValidData()
        {
            // Arrange
            var request = new CreateTodoRequest
            {
                Title = "New Todo",
                Description = "New Description"
            };

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var todo = Assert.IsType<TodoItem>(createdResult.Value);
            Assert.Equal(request.Title, todo.Title);
            Assert.Equal(request.Description, todo.Description);
            Assert.False(todo.IsCompleted);
        }

        [Fact]
        public async Task CreateTodo_ReturnsBadRequest_WithEmptyTitle()
        {
            // Arrange
            var request = new CreateTodoRequest
            {
                Title = "",
                Description = "Description"
            };
            
            _controller.ModelState.AddModelError("Title", "标题不能为空");

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            var request = new UpdateTodoRequest { Title = "Updated Title" };

            // Act
            var result = await _controller.UpdateTodo(999, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateTodo_UpdatesTodo_WhenTodoExists()
        {
            // Arrange
            var todo = new TodoItem { Title = "Original Title", Description = "Original Description" };
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            var request = new UpdateTodoRequest
            {
                Title = "Updated Title",
                Description = "Updated Description",
                IsCompleted = true
            };

            // Act
            var result = await _controller.UpdateTodo(todo.Id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedTodo = Assert.IsType<TodoItem>(okResult.Value);
            Assert.Equal(request.Title, updatedTodo.Title);
            Assert.Equal(request.Description, updatedTodo.Description);
            Assert.True(updatedTodo.IsCompleted);
        }

        [Fact]
        public async Task DeleteTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteTodo(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTodo_DeletesTodo_WhenTodoExists()
        {
            // Arrange
            var todo = new TodoItem { Title = "Todo to Delete", Description = "Description" };
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteTodo(todo.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.TodoItems.FindAsync(todo.Id));
        }

        [Fact]
        public async Task ToggleTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Act
            var result = await _controller.ToggleTodo(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ToggleTodo_TogglesCompletion_WhenTodoExists()
        {
            // Arrange
            var todo = new TodoItem { Title = "Toggle Todo", IsCompleted = false };
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ToggleTodo(todo.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var toggledTodo = Assert.IsType<TodoItem>(okResult.Value);
            Assert.True(toggledTodo.IsCompleted);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}