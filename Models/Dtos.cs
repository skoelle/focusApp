namespace FocusApp.Models;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
}

public class ReorderDto
{
    public List<OrderItem> Orders { get; set; } = new();
}

public class OrderItem
{
    public int Id { get; set; }
    public int Order { get; set; }
}
