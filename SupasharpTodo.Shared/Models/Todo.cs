using Newtonsoft.Json;
using Postgrest.Attributes;

namespace SupasharpTodo.Shared.Models;

[Table("todos")]
public class Todo : Postgrest.Models.BaseModel
{
    [PrimaryKey("id")] public string Id { get; set; }

    [Column("user_id")] public string UserId { get; set; }

    [Column("list_id")] public string ListId { get; set; }

    [Column("title")] public string Title { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("completed_at", nullValueHandling: NullValueHandling.Include)]
    public DateTime? CompletedAt { get; set; } = null;

    [Column("due_at", nullValueHandling: NullValueHandling.Include)]
    public DateTime? DueAt { get; set; } = null;

    [Column("trashed_at", nullValueHandling: NullValueHandling.Include)]
    public DateTime? TrashedAt { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("updated_at")] public DateTime UpdatedAt { get; set; }
}