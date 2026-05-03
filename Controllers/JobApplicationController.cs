//using JobTrackingSystem.Areas.Identity.Data;
//using JobTrackingSystem.Data;
//using JobTrackingSystem.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace JobTrackingSystem.Controllers
//{
//    [Authorize]
//    public class JobApplicationController : Controller
//    {
//        private readonly JobTrackingSystemContext _context;
//        public JobApplicationController(JobTrackingSystemContext context)
//        {
//            _context = context;
//        }

//        public IActionResult Create()
//        {
//            ViewBag.Companies = _context.Companies.ToList();
//            return View();
//        }

//        public IActionResult Index()
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            var data = _context.JobApplications
//                .Include(j => j.Company)
//                .Where(j => j.UserId == userId)   // 🔥 THIS IS THE KEY
//                .ToList();

//            return View(data);
//        }

//        [HttpPost]
//        public IActionResult Create(JobApplication model)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            model.UserId = userId;

//            if (!ModelState.IsValid)
//            {
using JobTrackingSystem.Data;
using JobTrackingSystem.Models;
using JobTrackingSystem.Services;
using JobTrackingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class JobApplicationController : Controller
{
    private readonly IJobApplicationService _service;
    private readonly JobTrackingSystemContext _context;

    public JobApplicationController(
        IJobApplicationService service,
        JobTrackingSystemContext context)
    {
        _service = service;
        _context = context;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // INDEX
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var data = await _service.GetForUserAsync(userId);
        return View(data);
    }

    // CREATE (GET)
    public IActionResult Create()
    {
        var vm = new JobApplicationCreateVm
        {
            Companies = _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
        };

        return View(vm);
    }

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobApplicationCreateVm vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Companies = _context.Companies.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
            return View(vm);
        }

        var entity = new JobApplication
        {
            UserId = GetUserId(),
            CompanyId = vm.CompanyId,
            Role = vm.Role,
            Status = vm.Status,
            DateApplied = vm.DateApplied,
            Notes = vm.Notes
        };

        await _service.CreateAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var entity = await _service.GetByIdForUserAsync(id, userId);
        if (entity == null) return NotFound(); // also enforces row-level security

        var vm = new JobApplicationEditVm
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            Role = entity.Role,
            Status = entity.Status,
            DateApplied = entity.DateApplied,
            Notes = entity.Notes,

            Companies = await _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(JobApplicationEditVm model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!ModelState.IsValid)
        {
            // Refill dropdown or your view breaks
            model.Companies = await _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            return View(model);
        }

        // Fetch entity WITH user scope (security)
        var entity = await _service.GetByIdForUserAsync(model.Id, userId);
        if (entity == null) return NotFound();

        // Map VM → Entity (only allowed fields)
        entity.CompanyId = model.CompanyId;
        entity.Role = model.Role;
        entity.Status = model.Status;
        entity.DateApplied = model.DateApplied;
        entity.Notes = model.Notes;

        await _service.UpdateAsync(entity);

        return RedirectToAction(nameof(Index)); // don’t loop back to Edit
    }
}
//                ViewBag.Companies = _context.Companies.ToList();
//                return View(model);
//            }

//            _context.JobApplications.Add(model);
//            _context.SaveChanges();

//            return RedirectToAction("Create");
//        }


//public IActionResult Edit(int id)
//    {
//        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//        var job = _context.JobApplications
//            .FirstOrDefault(j => j.Id == id && j.UserId == userId);

//        if (job == null)
//            return NotFound(); // 🔥 critical

//        ViewBag.Companies = _context.Companies.ToList();

//        return View(job);
//    }
//        //[HttpPost]
//        //    public IActionResult Edit(JobApplication model)
//        //    {
//        //        var existing = _context.JobApplications.Find(model.Id);

//        //        if (existing == null)
//        //            return NotFound();

//        //        // ✅ Update only allowed fields
//        //        existing.CompanyId = model.CompanyId;
//        //        existing.Role = model.Role;
//        //        existing.Status = model.Status;
//        //        existing.DateApplied = model.DateApplied;
//        //        existing.Notes = model.Notes;

//        //        // ❗ DO NOT TOUCH UserId

//        //        _context.SaveChanges();

//        //        return RedirectToAction("Index");
//        //    }
//        [HttpPost]
//        public IActionResult Edit(JobApplication model)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            var job = _context.JobApplications
//                .FirstOrDefault(j => j.Id == model.Id && j.UserId == userId);

//            if (job == null)
//                return NotFound(); // 🔥 blocks tampering

//            if (!ModelState.IsValid)
//            {
//                ViewBag.Companies = _context.Companies.ToList();
//                return View(model);
//            }

//            job.CompanyId = model.CompanyId;
//            job.Role = model.Role;
//            job.Status = model.Status;
//            job.DateApplied = model.DateApplied;
//            job.Notes = model.Notes;

//            _context.SaveChanges();

//            return RedirectToAction("Index");
//        }
//        public IActionResult Delete(int id)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            var job = _context.JobApplications
//                .FirstOrDefault(j => j.Id == id && j.UserId == userId);

//            if (job == null)
//                return NotFound();

//            return View(job);
//            }
//            //[HttpPost, ActionName("Delete")]
//            //public IActionResult DeleteConfirmed(int id)
//            //{
//            //    var job = _context.JobApplications.Find(id);

//            //    if (job != null)
//            //    {
//            //        _context.JobApplications.Remove(job);
//            //        _context.SaveChanges();
//            //    }

//            //    return RedirectToAction("Index");
//            //}
//            [HttpPost, ActionName("Delete")]
//            public IActionResult DeleteConfirmed(int id)
//            {
//                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//                var job = _context.JobApplications
//                    .FirstOrDefault(j => j.Id == id && j.UserId == userId);

//                if (job == null)
//                    return NotFound();

//                _context.JobApplications.Remove(job);
//                _context.SaveChanges();

//                return RedirectToAction("Index");
//            }
//        }
//}
