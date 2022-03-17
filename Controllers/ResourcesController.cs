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
    public class ResourcesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResourcesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int id)
        {
            var endpoint = _context.endpoint.Where(q => q.ID == id).First();
            ViewData["EndpointName"] = endpoint.Name;
            return View(await _context.resource.Where(q=>q.EndpointID==id).ToListAsync());
        }


        public IActionResult Create(int id)
        {
            var endpoint = _context.endpoint.Where(q => q.ID == id).First();
            ViewData["EndpointName"] = endpoint.Name;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Name,URI")] Resource resource)
        {
            
            resource.EndpointID = id;
            if (ModelState.IsValid)
            {
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = id });
            }
            var endpoint = _context.endpoint.Where(q => q.ID == id).First();
            ViewData["EndpointName"] = endpoint.Name;
            return View(resource);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var resource = await _context.resource.Include(e => e.Endpoint).FirstOrDefaultAsync(m => m.ID == id);
            if (resource == null)
            {
                return NotFound();
            }
            return View(resource);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var resource = await _context.resource.Include(e => e.Endpoint).FirstOrDefaultAsync(m => m.ID == id);
            if (await TryUpdateModelAsync<Resource>(
                resource,
                "",
                s => s.Name, s => s.URI))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = resource.EndpointID});
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(resource);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var resource = await _context.resource.Include(e => e.Endpoint).FirstOrDefaultAsync(m => m.ID == id);
            return View(resource);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _context.resource.FirstOrDefaultAsync(s => s.ID == id);

            if (resource == null)
            {
                return NotFound();
            }
            _context.resource.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = resource.EndpointID});
        }

        
    }
}
