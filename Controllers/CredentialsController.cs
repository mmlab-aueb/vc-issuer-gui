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
    public class CredentialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CredentialsController(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.credential.Where(q=>q.isRevoked == false).OrderByDescending(q=>q.iat).ToListAsync());
        }
  
        

        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var entry = await _context.credential.FirstOrDefaultAsync(s => s.jti == id);

            if (entry == null)
            {
                return NotFound();
            }
            entry.isRevoked = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
