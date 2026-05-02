using JobTrackingSystem.Areas.Identity.Data;
using JobTrackingSystem.Data;
using JobTrackingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobTrackingSystem.Controllers
{
    [Authorize]
    public class JobApplicationController : Controller
    {
        private readonly JobTrackingSystemContext _context;
        public JobApplicationController(JobTrackingSystemContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            ViewBag.Companies = _context.Companies.ToList();
            return View();
        }
      
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var data = _context.JobApplications
                .Include(j => j.Company)
                .Where(j => j.UserId == userId)   // 🔥 THIS IS THE KEY
                .ToList();

            return View(data);
        }

        [HttpPost]
        public IActionResult Create(JobApplication model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;

            if (!ModelState.IsValid)
            {
                ViewBag.Companies = _context.Companies.ToList();
                return View(model);
            }

            _context.JobApplications.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Create");
        }
 

public IActionResult Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var job = _context.JobApplications
            .FirstOrDefault(j => j.Id == id && j.UserId == userId);

        if (job == null)
            return NotFound(); // 🔥 critical

        ViewBag.Companies = _context.Companies.ToList();

        return View(job);
    }
        //[HttpPost]
        //    public IActionResult Edit(JobApplication model)
        //    {
        //        var existing = _context.JobApplications.Find(model.Id);

        //        if (existing == null)
        //            return NotFound();

        //        // ✅ Update only allowed fields
        //        existing.CompanyId = model.CompanyId;
        //        existing.Role = model.Role;
        //        existing.Status = model.Status;
        //        existing.DateApplied = model.DateApplied;
        //        existing.Notes = model.Notes;

        //        // ❗ DO NOT TOUCH UserId

        //        _context.SaveChanges();

        //        return RedirectToAction("Index");
        //    }
        [HttpPost]
        public IActionResult Edit(JobApplication model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var job = _context.JobApplications
                .FirstOrDefault(j => j.Id == model.Id && j.UserId == userId);

            if (job == null)
                return NotFound(); // 🔥 blocks tampering

            if (!ModelState.IsValid)
            {
                ViewBag.Companies = _context.Companies.ToList();
                return View(model);
            }

            job.CompanyId = model.CompanyId;
            job.Role = model.Role;
            job.Status = model.Status;
            job.DateApplied = model.DateApplied;
            job.Notes = model.Notes;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var job = _context.JobApplications
                .FirstOrDefault(j => j.Id == id && j.UserId == userId);

            if (job == null)
                return NotFound();

            return View(job);
            }
            //[HttpPost, ActionName("Delete")]
            //public IActionResult DeleteConfirmed(int id)
            //{
            //    var job = _context.JobApplications.Find(id);

            //    if (job != null)
            //    {
            //        _context.JobApplications.Remove(job);
            //        _context.SaveChanges();
            //    }

            //    return RedirectToAction("Index");
            //}
            [HttpPost, ActionName("Delete")]
            public IActionResult DeleteConfirmed(int id)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var job = _context.JobApplications
                    .FirstOrDefault(j => j.Id == id && j.UserId == userId);

                if (job == null)
                    return NotFound();

                _context.JobApplications.Remove(job);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
        }
}
