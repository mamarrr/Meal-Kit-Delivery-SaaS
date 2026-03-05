using App.Contracts.BLL.Menu;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.Menu;

public class MenusSchedulingIndexViewModel
{
    public Guid CompanyId { get; set; }
    public string CompanySlug { get; set; } = string.Empty;

    public DateTime WeekStartDate { get; set; }

    public WeeklyMenuRuleConfigDto RuleConfig { get; set; } = new();

    public WeeklyMenuRuleConfigUpdateDto RuleConfigForm { get; set; } = new();
    public WeeklyMenuAssignmentCreateDto AssignmentForm { get; set; } = new();

    public List<SelectListItem> CategoryOptions { get; set; } = new();
    public List<SelectListItem> RecipeOptions { get; set; } = new();

    public List<WeeklyMenuAssignmentDto> Assignments { get; set; } = new();
    public WeeklyMenuSimulationResultDto? Simulation { get; set; }
}
