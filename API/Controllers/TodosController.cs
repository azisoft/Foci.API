using API.Business.Mappers;
using API.DataAccess.DataAccess.DB;
using API.DataAccess.Model.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Todos controller implementing CRUD
/// </summary>
[ApiController]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    private readonly AppDbContext _db;

     public TodosController(AppDbContext db) { _db = db; }

    /// <summary>
    /// Get all todos and optionally filter by title
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> GetAll([FromQuery] string? title = null, [FromQuery] bool? isCompleted= null)
    {
        // build quesry
        var query = _db.Todos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(todo => EF.Functions.Like(todo.Title, $"%{title.Trim()}%"));
        }

        if (isCompleted.HasValue)
        {
            query = query.Where(todo => todo.IsCompleted == isCompleted.Value);
        }

        // run query
        var todos = await query
            .AsNoTracking()
            .ToListAsync();

        // return result
        return Ok(todos.Select(todo => todo.ToResponse()));
    }

    /// <summary>
    /// Get a specific todo by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoResponse>> Get(int id)
    {
        var todo = await _db.Todos
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => g.ToResponse())
            .FirstOrDefaultAsync();

        return todo is null ? NotFound() : Ok(todo);
    }

    /// <summary>
    /// Create a new todo.
    /// </summary>
    /// <param name="todo"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create(CreateTodoRequest todo)
    {
        var domain = todo.ToDomain();
        _db.Todos.Add(domain);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Create), new { id = domain.Id }, domain.ToResponse());
    }

    /// <summary>
    /// Full update an existing todo.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="todo"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTodoRequest todo)
    {
        // compare ids for safety
        if (id != todo.Id) return BadRequest();

        // grab todo to update
        var entity = await _db.Todos.FindAsync(id);
        if (entity is null) return NotFound();

        // apply updates to the entity
        entity.ApplyRequest(todo);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception)
        {
            // something went wrong - 500
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Partial update an existing todo.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="patch"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdatePartial(int id, [FromBody] PatchTodoRequest patch)
    {
        if (patch == null) return BadRequest();

        // get todo to patch
        var entity = await _db.Todos.FindAsync(id);
        if (entity is null) return NotFound();

        // apply provided patch values to entity
        entity.ApplyPatch(patch);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }

        return NoContent();
    }

    /// <summary>
    ///  Delete a todo.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Todos.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Todos.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

}