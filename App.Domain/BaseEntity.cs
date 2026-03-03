using Base.Contracts.Domain;

namespace App.Domain;

public abstract class BaseEntity: IBaseEntity
{
    public Guid Id { get; set; } =  Guid.NewGuid();
}