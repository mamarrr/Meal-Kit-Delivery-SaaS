namespace WebApp.ViewModels.Shared;

public class AlertViewModel
{
    public string Variant { get; set; } = "info";
    public string Message { get; set; } = default!;
    public string? Title { get; set; }
    public bool Dismissible { get; set; }
}
