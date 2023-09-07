using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Postgrest.Attributes;

namespace SupasharpTodo.Shared.Models;

[Table("todos")]
public class Todo : Postgrest.Models.BaseModel, INotifyPropertyChanged
{
    private string _title;
    private string? _description;
    private string _listId;
    private DateTime? _completedAt = null;
    private DateTime? _dueAt = null;
    private DateTime? _trashedAt;
    private DateTime? _displayAt;
    [PrimaryKey("id")] public string Id { get; set; }

    [Column("user_id")] public string UserId { get; set; }

    [Column("list_id")]
    public string ListId
    {
        get => _listId;
        set => SetField(ref _listId, value);
    }

    [Column("title")]
    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    [Column("description")]
    public string? Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    [Column("completed_at", nullValueHandling: NullValueHandling.Include)]
    public DateTime? CompletedAt
    {
        get => _completedAt;
        set => SetField(ref _completedAt, value);
    }

    [Column("due_at", nullValueHandling: NullValueHandling.Include)]
    public DateTime? DueAt
    {
        get => _dueAt;
        set => SetField(ref _dueAt, value);
    }

    [Column("display_at")]
    public DateTime? DisplayAt
    {
        get => _displayAt;
        set => SetField(ref _displayAt, value);
    }

    [Column("trashed_at", nullValueHandling: NullValueHandling.Include)]
    public DateTime? TrashedAt
    {
        get => _trashedAt;
        set => SetField(ref _trashedAt, value);
    }

    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("updated_at")] public DateTime UpdatedAt { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}