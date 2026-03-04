using App.Contracts.BLL.Subscription;
using App.Domain.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.ViewModels.Boxes;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CompanyManager")]
    public class BoxesController : Controller
    {
        private readonly IBoxService _boxService;

        public BoxesController(IBoxService boxService)
        {
            _boxService = boxService;
        }

        // GET: Boxes
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            return View(await _boxService.GetAllByCompanyIdAsync(companyId.Value));
        }

        // GET: Boxes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var box = await _boxService.GetByIdAsync(id.Value, companyId.Value);
            if (box == null)
            {
                return NotFound();
            }

            return View(box);
        }

        // GET: Boxes/Create
        public IActionResult Create()
        {
            return View(new BoxEditViewModel { Box = new Box { IsActive = true } });
        }

        // POST: Boxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoxEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            var userId = GetCurrentUserId();
            if (companyId == null || userId == null)
            {
                return Forbid();
            }

            if (viewModel.Box == null)
            {
                return BadRequest();
            }

            var box = viewModel.Box;
            if (ModelState.IsValid)
            {
                box.CreatedAt = DateTime.UtcNow;
                box.UpdatedAt = null;
                box.DeletedAt = null;
                box.CreatedByAppUserId = userId.Value;
                box.CompanyId = companyId.Value;

                await _boxService.AddAsync(box, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Boxes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var box = await _boxService.GetByIdAsync(id.Value, companyId.Value);
            if (box == null)
            {
                return NotFound();
            }

            return View(new BoxEditViewModel { Box = box });
        }

        // POST: Boxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BoxEditViewModel viewModel)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            if (viewModel.Box == null)
            {
                return BadRequest();
            }

            var box = viewModel.Box;
            if (id != box.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _boxService.GetByIdAsync(id, companyId.Value);
                if (existing == null)
                {
                    return NotFound();
                }

                box.CompanyId = companyId.Value;
                box.CreatedByAppUserId = existing.CreatedByAppUserId;
                box.CreatedAt = existing.CreatedAt;
                box.DeletedAt = existing.DeletedAt;
                box.UpdatedAt = DateTime.UtcNow;

                await _boxService.UpdateAsync(box, companyId.Value);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Boxes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var box = await _boxService.GetByIdAsync(id.Value, companyId.Value);
            if (box == null)
            {
                return NotFound();
            }

            return View(box);
        }

        // POST: Boxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            await _boxService.RemoveAsync(id, companyId.Value);
            return RedirectToAction(nameof(Index));
        }

        private Guid? GetCurrentCompanyId()
        {
            var companyIdRaw = User.FindFirst("company_id")?.Value
                               ?? User.FindFirst("tenant_id")?.Value
                               ?? User.FindFirst("companyId")?.Value;

            return Guid.TryParse(companyIdRaw, out var companyId)
                ? companyId
                : null;
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
