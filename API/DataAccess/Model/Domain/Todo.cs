namespace API.DataAccess.Model.Domain;
public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? IsCompleted { get; set; }
}