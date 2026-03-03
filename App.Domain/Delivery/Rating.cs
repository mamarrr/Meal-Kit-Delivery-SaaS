using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class Rating : BaseEntity
{
    public int Score { get; set; }
    public DateTime RatedAt { get; set; }
    public string Notes { get; set; } = default!;
    
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign Keys
    public Guid CustomerId { get; set; }
    public Guid RecipeId { get; set; }
    
    // Navigation Properties
    public Customer? Customer { get; set; }
    public Recipe? Recipe { get; set; }
}
