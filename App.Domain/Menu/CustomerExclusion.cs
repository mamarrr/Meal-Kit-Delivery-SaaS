using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class CustomerExclusion : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid IngredientId { get; set; }
    public Guid CustomerId { get; set; }
    
    // Navigation Properties
    public Ingredient? Ingredient { get; set; }
    public Customer? Customer { get; set; }
}
