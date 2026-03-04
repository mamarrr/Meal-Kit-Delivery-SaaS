using App.Domain.Subscription;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.BoxPrices;

public class BoxPriceEditViewModel
{
    public BoxPrice BoxPrice { get; set; } = default!;
    public List<SelectListItem> BoxOptions { get; set; } = new();
}
