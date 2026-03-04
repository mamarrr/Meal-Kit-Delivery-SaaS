namespace WebApp.ViewModels.Shared;

public class PageHeaderViewModel
{
    public string ContextLabel { get; set; } = "Workflow";
    public string Title { get; set; } = default!;
    public string? Subtitle { get; set; }
    public string? StatusText { get; set; }
}
