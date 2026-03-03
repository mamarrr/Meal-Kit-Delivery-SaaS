using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class NutritionalInfo : BaseEntity
{
    public decimal CaloriesKcal { get; set; }
    public decimal ProteinG { get; set; }
    public decimal CarbsG { get; set; }
    public decimal FatG { get; set; }
    public decimal FiberG { get; set; }
    public decimal SodiumMg { get; set; }
    public decimal SugarG { get; set; }
    public decimal SaturatedFatG { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign Keys
    public Guid RecipeId { get; set; }
    
    // Navigation Properties
    public Recipe? Recipe { get; set; }
}
