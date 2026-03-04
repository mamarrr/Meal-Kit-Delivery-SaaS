using App.Contracts.BLL.Core;
using App.Contracts.BLL.Identity;
using App.Contracts.BLL.Support;
using App.Domain.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.SupportTickets;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
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
            return View(await BuildEditViewModelAsync(new SupportTicket()));
        }

        // POST: SupportTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportTicketEditViewModel viewModel)
        {
            if (viewModel.SupportTicket == null)
            {
                return BadRequest();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Forbid();
            }

            var supportTicket = viewModel.SupportTicket;
            if (ModelState.IsValid)
            {
                supportTicket.CreatedAt = DateTime.UtcNow;
                supportTicket.CreatedByAppUserId = userId.Value;
                supportTicket.UpdatedAt = null;
                await _supportTicketService.AddAsync(supportTicket);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(supportTicket));
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

            return View(await BuildEditViewModelAsync(supportTicket));
        }

        // POST: SupportTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SupportTicketEditViewModel viewModel)
        {
            if (viewModel.SupportTicket == null)
            {
                return BadRequest();
            }

            var supportTicket = viewModel.SupportTicket;
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

                supportTicket.CreatedAt = existing.CreatedAt;
                supportTicket.CreatedByAppUserId = existing.CreatedByAppUserId;
                supportTicket.UpdatedAt = DateTime.UtcNow;
                await _supportTicketService.UpdateAsync(supportTicket);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(supportTicket));
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

        private async Task<SupportTicketEditViewModel> BuildEditViewModelAsync(SupportTicket supportTicket)
        {
            var companies = await _companyService.GetAllAsync();
            var users = await _appUserService.GetAllAsync();
            var statuses = await _supportTicketStatusService.GetAllAsync();

            return new SupportTicketEditViewModel
            {
                SupportTicket = supportTicket,
                CompanyOptions = companies
                    .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(c.Name, c.Id.ToString(), c.Id == supportTicket.CompanyId))
                    .ToList(),
                CreatedByUserOptions = users
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                        $"{u.FirstName} {u.LastName}".Trim(),
                        u.Id.ToString(),
                        u.Id == supportTicket.CreatedByAppUserId))
                    .ToList(),
                AssignedToUserOptions = users
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                        $"{u.FirstName} {u.LastName}".Trim(),
                        u.Id.ToString(),
                        u.Id == supportTicket.AssignedToAppUserId))
                    .ToList(),
                StatusOptions = statuses
                    .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(s.Label, s.Id.ToString(), s.Id == supportTicket.SupportTicketStatusId))
                    .ToList()
            };
        }

        private Guid? GetCurrentUserId()
        {
            var userIdRaw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("user_id")?.Value;

            return Guid.TryParse(userIdRaw, out var userId)
                ? userId
                : null;
        }
    }
}
