using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Authorization_Server.Data;
using Authorization_Server.Models;
using System.Security.Claims;

namespace Authorization_Server.Controllers
{
    public class OperationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperationsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int id)
        {
            var resource = _context.resource.Include(e => e.Endpoint).Where(q => q.ID == id).First();
            ViewData["EndpointID"] = resource.EndpointID;
            ViewData["EndpointName"] = resource.Endpoint.Name;
            ViewData["ResourceName"] = resource.Name;
            return View(await _context.operation.Where(q=>q.ResourceID==id).ToListAsync());
        }


        public IActionResult Create(int id)
        {
            var resource = _context.resource.Include(e => e.Endpoint).Where(q => q.ID == id).First();
            ViewData["EndpointID"] = resource.EndpointID;
            ViewData["EndpointName"] = resource.Endpoint.Name;
            ViewData["ResourceName"] = resource.Name;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Name,URI")] Operation operation)
        {
            operation.ResourceID = id;
            if (ModelState.IsValid)
            {
                _context.Add(operation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = id });
            }
            return View(operation);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var operation = await _context.operation.Include(e => e.Resource).Include(e => e.Resource.Endpoint).FirstOrDefaultAsync(m => m.ID == id);
            if (operation == null)
            {
                return NotFound();
            }
            return View(operation);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var operation = await _context.operation.Include(e => e.Resource).Include(e => e.Resource.Endpoint).FirstOrDefaultAsync(m => m.ID == id);
            if(await TryUpdateModelAsync<Operation>(
                operation,
                "",
                s => s.Name, s => s.URI))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = operation.ResourceID });
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            return View(operation);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var operation = await _context.operation.Include(e => e.Resource).Include(e => e.Resource.Endpoint).FirstOrDefaultAsync(m => m.ID == id);
            return View(operation);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var operation = await _context.operation.FirstOrDefaultAsync(s => s.ID == id);

            if (operation == null)
            {
                return NotFound();
            }
            _context.operation.Remove(operation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = operation.ResourceID });
        }

        
    }
}
