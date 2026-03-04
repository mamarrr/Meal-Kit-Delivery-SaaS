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
            await LoadSelectionsAsync();
            return View();
        }

        // POST: SupportImpersonation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,SupportUserId,ImpersonatedAppUserId,Reason,StartedAt,EndedAt,Id")] SupportImpersonationSession supportImpersonationSession)
        {
            if (ModelState.IsValid)
            {
                supportImpersonationSession.StartedAt = DateTime.UtcNow;
                await _supportImpersonationSessionService.AddAsync(supportImpersonationSession);
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectionsAsync(supportImpersonationSession.CompanyId, supportImpersonationSession.ImpersonatedAppUserId, supportImpersonationSession.SupportUserId);
            return View(supportImpersonationSession);
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
            await LoadSelectionsAsync(supportImpersonationSession.CompanyId, supportImpersonationSession.ImpersonatedAppUserId, supportImpersonationSession.SupportUserId);
            return View(supportImpersonationSession);
        }

        // POST: SupportImpersonation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompanyId,SupportUserId,ImpersonatedAppUserId,Reason,StartedAt,EndedAt,Id")] SupportImpersonationSession supportImpersonationSession)
        {
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

                await _supportImpersonationSessionService.UpdateAsync(supportImpersonationSession);
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectionsAsync(supportImpersonationSession.CompanyId, supportImpersonationSession.ImpersonatedAppUserId, supportImpersonationSession.SupportUserId);
            return View(supportImpersonationSession);
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

        private async Task LoadSelectionsAsync(Guid? companyId = null, Guid? impersonatedAppUserId = null, Guid? supportUserId = null)
        {
            var companies = await _companyService.GetAllAsync();
            var users = await _appUserService.GetAllAsync();

            ViewData["CompanyId"] = new SelectList(companies, "Id", "ContactEmail", companyId);
            ViewData["ImpersonatedAppUserId"] = new SelectList(users, "Id", "FirstName", impersonatedAppUserId);
            ViewData["SupportUserId"] = new SelectList(users, "Id", "FirstName", supportUserId);
        }
    }
}
