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
    public class SupportAccessController : Controller
    {
        private readonly ITenantSupportAccessService _tenantSupportAccessService;
        private readonly ICompanyService _companyService;
        private readonly IAppUserService _appUserService;

        public SupportAccessController(
            ITenantSupportAccessService tenantSupportAccessService,
            ICompanyService companyService,
            IAppUserService appUserService)
        {
            _tenantSupportAccessService = tenantSupportAccessService;
            _companyService = companyService;
            _appUserService = appUserService;
        }

        // GET: SupportAccess
        public async Task<IActionResult> Index()
        {
            return View(await _tenantSupportAccessService.GetAllAsync());
        }

        // GET: SupportAccess/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantSupportAccess = await _tenantSupportAccessService.GetByIdAsync(id.Value);
            if (tenantSupportAccess == null)
            {
                return NotFound();
            }

            return View(tenantSupportAccess);
        }

        // GET: SupportAccess/Create
        public async Task<IActionResult> Create()
        {
            await LoadSelectionsAsync();
            return View();
        }

        // POST: SupportAccess/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,SupportUserId,IsReadOnly,Reason,GrantedAt,RevokedAt,GrantedByAppUserId,Id")] TenantSupportAccess tenantSupportAccess)
        {
            if (ModelState.IsValid)
            {
                await _tenantSupportAccessService.AddAsync(tenantSupportAccess);
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectionsAsync(tenantSupportAccess.CompanyId, tenantSupportAccess.GrantedByAppUserId, tenantSupportAccess.SupportUserId);
            return View(tenantSupportAccess);
        }

        // GET: SupportAccess/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantSupportAccess = await _tenantSupportAccessService.GetByIdAsync(id.Value);
            if (tenantSupportAccess == null)
            {
                return NotFound();
            }
            await LoadSelectionsAsync(tenantSupportAccess.CompanyId, tenantSupportAccess.GrantedByAppUserId, tenantSupportAccess.SupportUserId);
            return View(tenantSupportAccess);
        }

        // POST: SupportAccess/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompanyId,SupportUserId,IsReadOnly,Reason,GrantedAt,RevokedAt,GrantedByAppUserId,Id")] TenantSupportAccess tenantSupportAccess)
        {
            if (id != tenantSupportAccess.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _tenantSupportAccessService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                await _tenantSupportAccessService.UpdateAsync(tenantSupportAccess);
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectionsAsync(tenantSupportAccess.CompanyId, tenantSupportAccess.GrantedByAppUserId, tenantSupportAccess.SupportUserId);
            return View(tenantSupportAccess);
        }

        // GET: SupportAccess/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantSupportAccess = await _tenantSupportAccessService.GetByIdAsync(id.Value);
            if (tenantSupportAccess == null)
            {
                return NotFound();
            }

            return View(tenantSupportAccess);
        }

        // POST: SupportAccess/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _tenantSupportAccessService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSelectionsAsync(Guid? companyId = null, Guid? grantedByUserId = null, Guid? supportUserId = null)
        {
            var companies = await _companyService.GetAllAsync();
            var users = await _appUserService.GetAllAsync();

            ViewData["CompanyId"] = new SelectList(companies, "Id", "ContactEmail", companyId);
            ViewData["GrantedByAppUserId"] = new SelectList(users, "Id", "FirstName", grantedByUserId);
            ViewData["SupportUserId"] = new SelectList(users, "Id", "FirstName", supportUserId);
        }
    }
}
