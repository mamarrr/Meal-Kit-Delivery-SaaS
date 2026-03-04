using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Core;
using App.Contracts.DAL.Delivery;
using App.Contracts.DAL.Menu;
using App.Contracts.DAL.Subscription;
using App.DAL.EF;
using DeliveryEntity = App.Domain.Delivery.Delivery;

namespace App.BLL.Delivery;

public class DeliveryService : BaseTenantService<DeliveryEntity, IDeliveryRepository>, IDeliveryService
{
    private readonly ICustomerRepository? _customerRepository;
    private readonly IWeeklyMenuRepository? _weeklyMenuRepository;
    private readonly IDeliveryZoneRepository? _deliveryZoneRepository;
    private readonly IBoxRepository? _boxRepository;
    private readonly IMealSelectionRepository? _mealSelectionRepository;
    private readonly IMealSubscriptionRepository? _mealSubscriptionRepository;
    private readonly AppDbContext? _context;

    public DeliveryService(IDeliveryRepository repository) : base(repository)
    {
    }

    public DeliveryService(
        IDeliveryRepository repository,
        ICustomerRepository customerRepository,
        IWeeklyMenuRepository weeklyMenuRepository,
        IDeliveryZoneRepository deliveryZoneRepository,
        IBoxRepository boxRepository,
        IMealSelectionRepository mealSelectionRepository,
        IMealSubscriptionRepository mealSubscriptionRepository,
        AppDbContext context) : base(repository)
    {
        _customerRepository = customerRepository;
        _weeklyMenuRepository = weeklyMenuRepository;
        _deliveryZoneRepository = deliveryZoneRepository;
        _boxRepository = boxRepository;
        _mealSelectionRepository = mealSelectionRepository;
        _mealSubscriptionRepository = mealSubscriptionRepository;
        _context = context;
    }

    protected override async Task<ICollection<DeliveryEntity>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<DeliveryEntity>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await Repository.GetAllByCustomerIdAsync(customerId, companyId);
    }

    public override async Task<DeliveryEntity> AddAsync(DeliveryEntity entity, Guid companyId)
    {
        await ValidateTenantReferencesAsync(entity, companyId);
        return await base.AddAsync(entity, companyId);
    }

    public override async Task<DeliveryEntity> UpdateAsync(DeliveryEntity entity, Guid companyId)
    {
        await ValidateTenantReferencesAsync(entity, companyId);
        return await base.UpdateAsync(entity, companyId);
    }

    private async Task ValidateTenantReferencesAsync(DeliveryEntity entity, Guid companyId)
    {
        if (_customerRepository == null
            || _weeklyMenuRepository == null
            || _deliveryZoneRepository == null
            || _boxRepository == null
            || _mealSelectionRepository == null
            || _mealSubscriptionRepository == null
            || _context == null)
        {
            return;
        }

        var customer = await _customerRepository.GetByIdAsync(entity.CustomerId);
        var weeklyMenu = await _weeklyMenuRepository.GetByIdAsync(entity.WeeklyMenuId);
        var deliveryZone = await _deliveryZoneRepository.GetByIdAsync(entity.DeliveryZoneId);
        var box = await _boxRepository.GetByIdAsync(entity.BoxId);
        var mealSelection = await _mealSelectionRepository.GetByIdAsync(entity.MealSelectionId, companyId);
        var mealSubscription = await _mealSubscriptionRepository.GetByIdAsync(entity.MealSubscriptionId);
        var deliveryWindow = await _context.DeliveryWindows.FindAsync(entity.DeliveryWindowId);

        var outOfScope = customer == null || customer.CompanyId != companyId
                         || weeklyMenu == null || weeklyMenu.CompanyId != companyId
                         || deliveryZone == null || deliveryZone.CompanyId != companyId
                         || box == null || box.CompanyId != companyId
                         || mealSelection == null
                         || mealSubscription == null || mealSubscription.CompanyId != companyId
                         || deliveryWindow == null || deliveryWindow.DeliveryZoneId != entity.DeliveryZoneId;

        if (outOfScope)
        {
            throw new KeyNotFoundException("Delivery references are outside company scope.");
        }
    }
}

