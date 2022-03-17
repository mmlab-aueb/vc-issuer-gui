using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Authorization_Server.Data;
using Authorization_Server.Models;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Authorization_Server
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.client.ToListAsync());
        }
  
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Client client)
        {
            if (ModelState.IsValid)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] randomBytes = new byte[18];
                rng.GetBytes(randomBytes);
                client.ClientId = Base64UrlEncoder.Encode(randomBytes);
                rng.GetBytes(randomBytes);
                client.ClientSecret = Base64UrlEncoder.Encode(randomBytes);
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = await _context.client.FirstAsync(b=>b.ID == id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = await _context.client.FirstOrDefaultAsync(m => m.ID == id);
            if (await TryUpdateModelAsync<Client>(
                client,
                "",
                s => s.Name, s => s.ClientId, s => s.ClientSecret))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(client);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.client.FirstOrDefaultAsync(s => s.ID == id);

            if (entry == null)
            {
                return NotFound();
            }
            _context.client.Remove(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
