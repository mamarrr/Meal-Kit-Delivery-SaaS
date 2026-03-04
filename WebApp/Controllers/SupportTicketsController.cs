using App.Contracts.BLL.Core;
using App.Contracts.BLL.Identity;
using App.Contracts.BLL.Support;
using App.Domain.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Controllers
{
    [Authorize]
    public class SupportTicketsController : Controller
    {
        private readonly ISupportTicketService _supportTicketService;
        private readonly ISupportTicketStatusService _supportTicketStatusService;
        private readonly ICompanyService _companyService;
        private readonly IAppUserService _appUserService;

        public SupportTicketsController(
            ISupportTicketService supportTicketService,
            ISupportTicketStatusService supportTicketStatusService,
            ICompanyService companyService,
            IAppUserService appUserService)
        {
            _supportTicketService = supportTicketService;
            _supportTicketStatusService = supportTicketStatusService;
            _companyService = companyService;
            _appUserService = appUserService;
        }

        // GET: SupportTickets
        public async Task<IActionResult> Index()
        {
            return View(await _supportTicketService.GetAllAsync());
        }

        // GET: SupportTickets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportTicket = await _supportTicketService.GetByIdAsync(id.Value);
            if (supportTicket == null)
            {
                return NotFound();
            }

            return View(supportTicket);
        }

        // GET: SupportTickets/Create
        public async Task<IActionResult> Create()
        {
            await LoadSelectionsAsync();
            return View();
        }

        // POST: SupportTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Priority,CreatedAt,UpdatedAt,ClosedAt,CompanyId,CreatedByAppUserId,AssignedToAppUserId,SupportTicketStatusId,Id")] SupportTicket supportTicket)
        {
            if (ModelState.IsValid)
            {
                supportTicket.CreatedAt = DateTime.UtcNow;
                await _supportTicketService.AddAsync(supportTicket);
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectionsAsync(supportTicket.CompanyId, supportTicket.CreatedByAppUserId, supportTicket.AssignedToAppUserId, supportTicket.SupportTicketStatusId);
            return View(supportTicket);
        }

        // GET: SupportTickets/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportTicket = await _supportTicketService.GetByIdAsync(id.Value);
            if (supportTicket == null)
            {
                return NotFound();
            }
            await LoadSelectionsAsync(supportTicket.CompanyId, supportTicket.CreatedByAppUserId, supportTicket.AssignedToAppUserId, supportTicket.SupportTicketStatusId);
            return View(supportTicket);
        }

        // POST: SupportTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Title,Description,Priority,CreatedAt,UpdatedAt,ClosedAt,CompanyId,CreatedByAppUserId,AssignedToAppUserId,SupportTicketStatusId,Id")] SupportTicket supportTicket)
        {
            if (id != supportTicket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _supportTicketService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                supportTicket.UpdatedAt = DateTime.UtcNow;
                await _supportTicketService.UpdateAsync(supportTicket);
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectionsAsync(supportTicket.CompanyId, supportTicket.CreatedByAppUserId, supportTicket.AssignedToAppUserId, supportTicket.SupportTicketStatusId);
            return View(supportTicket);
        }

        // GET: SupportTickets/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportTicket = await _supportTicketService.GetByIdAsync(id.Value);
            if (supportTicket == null)
            {
                return NotFound();
            }

            return View(supportTicket);
        }

        // POST: SupportTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _supportTicketService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSelectionsAsync(Guid? companyId = null, Guid? createdByUserId = null, Guid? assignedToUserId = null, Guid? statusId = null)
        {
            var companies = await _companyService.GetAllAsync();
            var users = await _appUserService.GetAllAsync();
            var statuses = await _supportTicketStatusService.GetAllAsync();

            ViewData["AssignedToAppUserId"] = new SelectList(users, "Id", "FirstName", assignedToUserId);
            ViewData["CompanyId"] = new SelectList(companies, "Id", "ContactEmail", companyId);
            ViewData["CreatedByAppUserId"] = new SelectList(users, "Id", "FirstName", createdByUserId);
            ViewData["SupportTicketStatusId"] = new SelectList(statuses, "Id", "Code", statusId);
        }
    }
}
