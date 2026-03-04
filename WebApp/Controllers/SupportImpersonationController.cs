using App.Contracts.BLL.Core;
using App.Contracts.BLL.Identity;
using App.Contracts.BLL.Support;
using App.Domain.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.SupportImpersonation;

namespace WebApp.Controllers
{
    [Authorize(Policy = "SystemAdmin")]
    public class SupportImpersonationController : Controller
    {
        private readonly ISupportImpersonationSessionService _supportImpersonationSessionService;
        private readonly ICompanyService _companyService;
        private readonly IAppUserService _appUserService;

        public SupportImpersonationController(
            ISupportImpersonationSessionService supportImpersonationSessionService,
            ICompanyService companyService,
            IAppUserService appUserService)
        {
            _supportImpersonationSessionService = supportImpersonationSessionService;
            _companyService = companyService;
            _appUserService = appUserService;
        }

        // GET: SupportImpersonation
        public async Task<IActionResult> Index()
        {
            return View(await _supportImpersonationSessionService.GetAllAsync());
        }

        // GET: SupportImpersonation/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportImpersonationSession = await _supportImpersonationSessionService.GetByIdAsync(id.Value);
            if (supportImpersonationSession == null)
            {
                return NotFound();
            }

            return View(supportImpersonationSession);
        }

        // GET: SupportImpersonation/Create
        public async Task<IActionResult> Create()
        {
            return View(await BuildEditViewModelAsync(new SupportImpersonationSession()));
        }

        // POST: SupportImpersonation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportImpersonationEditViewModel viewModel)
        {
            if (viewModel.SupportImpersonationSession == null)
            {
                return BadRequest();
            }

            var supportImpersonationSession = viewModel.SupportImpersonationSession;
            if (ModelState.IsValid)
            {
                supportImpersonationSession.StartedAt = DateTime.UtcNow;
                await _supportImpersonationSessionService.AddAsync(supportImpersonationSession);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(supportImpersonationSession));
        }

        // GET: SupportImpersonation/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportImpersonationSession = await _supportImpersonationSessionService.GetByIdAsync(id.Value);
            if (supportImpersonationSession == null)
            {
                return NotFound();
            }

            return View(await BuildEditViewModelAsync(supportImpersonationSession));
        }

        // POST: SupportImpersonation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SupportImpersonationEditViewModel viewModel)
        {
            if (viewModel.SupportImpersonationSession == null)
            {
                return BadRequest();
            }

            var supportImpersonationSession = viewModel.SupportImpersonationSession;
            if (id != supportImpersonationSession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _supportImpersonationSessionService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                supportImpersonationSession.StartedAt = existing.StartedAt;
                await _supportImpersonationSessionService.UpdateAsync(supportImpersonationSession);
                return RedirectToAction(nameof(Index));
            }

            return View(await BuildEditViewModelAsync(supportImpersonationSession));
        }

        // GET: SupportImpersonation/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportImpersonationSession = await _supportImpersonationSessionService.GetByIdAsync(id.Value);
            if (supportImpersonationSession == null)
            {
                return NotFound();
            }

            return View(supportImpersonationSession);
        }

        // POST: SupportImpersonation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _supportImpersonationSessionService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<SupportImpersonationEditViewModel> BuildEditViewModelAsync(SupportImpersonationSession supportImpersonationSession)
        {
            var companies = await _companyService.GetAllAsync();
            var users = await _appUserService.GetAllAsync();

            return new SupportImpersonationEditViewModel
            {
                SupportImpersonationSession = supportImpersonationSession,
                CompanyOptions = companies
                    .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(c.Name, c.Id.ToString(), c.Id == supportImpersonationSession.CompanyId))
                    .ToList(),
                SupportUserOptions = users
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                        $"{u.FirstName} {u.LastName}".Trim(),
                        u.Id.ToString(),
                        u.Id == supportImpersonationSession.SupportUserId))
                    .ToList(),
                ImpersonatedUserOptions = users
                    .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(
                        $"{u.FirstName} {u.LastName}".Trim(),
                        u.Id.ToString(),
                        u.Id == supportImpersonationSession.ImpersonatedAppUserId))
                    .ToList()
            };
        }
    }
}
