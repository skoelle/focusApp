// Copyright (c) 2026 Stefan Koelle (https://stefankoelle.de)
// Licensed under the MIT License. See LICENSE file in project root for details.
using FocusApp.Controllers;
using FocusApp.Data;
using FocusApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FocusApp.Tests;

public class FocusTasksControllerTests : IDisposable
{
    private readonly FocusContext _context;
    private readonly FocusTasksController _controller;
    private readonly ILogger<FocusTasksController> _logger;

    public FocusTasksControllerTests()
    {
        var options = new DbContextOptionsBuilder<FocusContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FocusContext(options);
        _logger = new LoggerFactory().CreateLogger<FocusTasksController>();
        _controller = new FocusTasksController(_context, _logger);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyList_WhenNoTasks()
    {
        var result = await _controller.GetTasks();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var tasks = Assert.IsAssignableFrom<IEnumerable<FocusTask>>(okResult.Value);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedAtAction_WithValidData()
    {
        var dto = new CreateTaskDto { Title = "Test Task", Description = "Test Description" };

        var result = await _controller.CreateTask(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var task = Assert.IsType<FocusTask>(createdResult.Value);
        Assert.Equal("Test Task", task.Title);
        Assert.Equal("Test Description", task.Description);
        Assert.Equal(1, task.Order);
    }

    [Fact]
    public async Task CreateTask_ReturnsBadRequest_WhenTitleIsEmpty()
    {
        var dto = new CreateTaskDto { Title = "" };

        var result = await _controller.CreateTask(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateTask_ReturnsBadRequest_WhenTitleExceedsMaxLength()
    {
        var dto = new CreateTaskDto { Title = new string('A', 256) };

        var result = await _controller.CreateTask(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateTask_ReturnsOk_WithValidData()
    {
        var createDto = new CreateTaskDto { Title = "Original" };
        var created = await _controller.CreateTask(createDto);
        var createdTask = (created.Result as CreatedAtActionResult)!.Value as FocusTask;

        var updateDto = new UpdateTaskDto { Title = "Updated" };
        var result = await _controller.UpdateTask(createdTask!.Id, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var task = Assert.IsType<FocusTask>(okResult.Value);
        Assert.Equal("Updated", task.Title);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        var dto = new UpdateTaskDto { Title = "Updated" };

        var result = await _controller.UpdateTask(999, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContent_WhenTaskExists()
    {
        var createDto = new CreateTaskDto { Title = "To Delete" };
        var created = await _controller.CreateTask(createDto);
        var createdTask = (created.Result as CreatedAtActionResult)!.Value as FocusTask;

        var result = await _controller.DeleteTask(createdTask!.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        var result = await _controller.DeleteTask(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ReorderTasks_ReturnsOk_WithValidOrders()
    {
        var task1 = await _controller.CreateTask(new CreateTaskDto { Title = "Task 1" });
        var task2 = await _controller.CreateTask(new CreateTaskDto { Title = "Task 2" });
        var t1 = (task1.Result as CreatedAtActionResult)!.Value as FocusTask;
        var t2 = (task2.Result as CreatedAtActionResult)!.Value as FocusTask;

        var reorderDto = new ReorderDto
        {
            Orders = new List<OrderItem>
            {
                new OrderItem { Id = t1!.Id, Order = 2 },
                new OrderItem { Id = t2!.Id, Order = 1 }
            }
        };

        var result = await _controller.ReorderTasks(reorderDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var tasks = Assert.IsAssignableFrom<IEnumerable<FocusTask>>(okResult.Value);
        Assert.Equal(2, tasks.Count());
    }

    [Fact]
    public async Task ReorderTasks_ReturnsBadRequest_WhenNoOrders()
    {
        var reorderDto = new ReorderDto { Orders = new List<OrderItem>() };

        var result = await _controller.ReorderTasks(reorderDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
