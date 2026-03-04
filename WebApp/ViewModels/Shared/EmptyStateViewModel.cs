namespace WebApp.ViewModels.Shared;

public class EmptyStateViewModel
{
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? ActionText { get; set; }
    public string? ActionUrl { get; set; }
}
