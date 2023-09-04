using Postgrest.Attributes;

namespace SupasharpTodo.Shared.Models;

[Table("lists")]
public class Lists
{
    [PrimaryKey("id")]
    public string Id { get; set; }
    
    [Column("parent_list_id")]
    public string ParentListId { get; set; }
    
    [Column("user_id")]
    public string UserId { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("updated_at")] public DateTime UpdatedAt { get; set; }

}