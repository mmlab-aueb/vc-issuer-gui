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
    public class AuthorizationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorizationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            string clientName = _context.client.Where(q => q.ID == id).Select(a => a.Name).FirstOrDefault();
            ViewData["ClientName"] = clientName;
            var authorizations = await _context.authorization
                .Where(m => m.ClientID == id).Include(q => q.Operation).Include(q => q.Operation.Resource).Include(q => q.Operation.Resource.Endpoint).ToListAsync();
            return View(authorizations);
        }

        public async Task<IActionResult> Add(int id)
        {
            string clientName = _context.client.Where(q => q.ID == id).Select(a => a.Name).FirstOrDefault();
            ViewData["ClientName"] = clientName;
            List<int> authorizations = _context.authorization.Where(q => q.ClientID == id).Select(a => a.OperationID).ToList();
            var endpoints = await _context.operation.OrderBy(q => q.ResourceID).Include(q => q.Resource).Include(q => q.Resource.Endpoint).ToListAsync();
            List<Operation> availableEndpoints = new List<Operation>();
            foreach (var endpoint in endpoints)
            {
                if (authorizations.IndexOf(endpoint.ID) == -1)
                {
                    availableEndpoints.Add(endpoint);
                }
            }
            return View(availableEndpoints);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, List<int> enpoints)
        {

            var client = await _context.client.FirstAsync(b => b.ID == id);
            if (client == null)
            {
                return NotFound();
            }
            foreach (var endpointId in enpoints)
            {
                var endpoint = await _context.operation.FirstOrDefaultAsync(q => q.ID == endpointId);
                if (endpoint != null)
                {
                    var authorization = new Authorization();
                    authorization.ClientID = id;
                    authorization.OperationID = endpointId;
                    _context.Add(authorization);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var authorization = await _context.authorization.FirstOrDefaultAsync(m => m.ID == id);
            if (authorization == null)
            {
                return NotFound();
            }
            _context.authorization.Remove(authorization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = authorization.ClientID });
        }
    }
}
