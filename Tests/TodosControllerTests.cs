using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.DataAccess.DataAccess.DB;
using API.DataAccess.Model.Domain;
using API.DataAccess.Model.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests
{
    public class TodosControllerTests : IDisposable
    {
        private readonly AppDbContext _db;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new AppDbContext(opts);
            Seed();
            _controller = new TodosController(_db);
        }

        private void Seed()
        {
            var todo1 = new Todo
            {
                Id = 1,
                Title = "Add DB",
                Description = "Set up the database",
                IsCompleted = false,
                DueDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow
            };
            var todo2 = new Todo
            {
                Id = 2,
                Title = "Create API",
                Description = "Create .NET REST API",
                IsCompleted = false,
                DueDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow
            };
            var todo3 = new Todo
            {
                Id = 3,
                Title = "Create APP",
                Description = "Create Angular SPA to access the API",
                IsCompleted = false,
                DueDate = DateTime.UtcNow.AddDays(3),
                CreatedAt = DateTime.UtcNow
            };

            _db.Todos.Add(todo1);
            _db.Todos.Add(todo2);
            _db.Todos.Add(todo3);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        [Fact]
        public async Task GetAll()
        {
            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<TodoResponse>>(ok.Value);
            Assert.Equal(3, list.Count());
        }

        [Fact]
        public async Task GetById()
        {
            var result = await _controller.Get(1);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var todo = Assert.IsType<TodoResponse>(ok.Value);
            Assert.Equal(1, todo.Id);
        }

        [Fact]
        public async Task Create()
        {
            var req = new CreateTodoRequest
            {
                Title = "New Todo",
                Description = "New Todo Description",
                IsCompleted = false,
                DueDate = DateTime.UtcNow.AddDays(4),
            };

            var result = await _controller.Create(req);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TodosController.Create), created.ActionName);

            // verify saved
            var saved = _db.Todos.FirstOrDefault(g => g.Title == "New Todo");
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task Update()
        {
            var update = new UpdateTodoRequest
            {
                Id = 1,
                Title = "Updated",
                Description = "Updated Description",
                IsCompleted = true,
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            var result = await _controller.Update(1, update);
            Assert.IsType<NoContentResult>(result);

            var saved = _db.Todos.Find(1);
            Assert.Equal("Updated", saved.Title);
        }

        [Fact]
        public async Task Delete()
        {
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
            Assert.Null(_db.Todos.Find(1));
        }

        [Fact]
        public async Task PartialUpdateTitles()
        {
            var patch = new PatchTodoRequest
            {
                Title = "Patched Title"
            };

            var result = await _controller.UpdatePartial(2, patch);
            Assert.IsType<NoContentResult>(result);

            var saved = _db.Todos.Find(2);
            Assert.Equal("Patched Title", saved.Title);
            // ensure other fields untouched
            Assert.Equal("Create .NET REST API", saved.Description);
        }
    }
}
