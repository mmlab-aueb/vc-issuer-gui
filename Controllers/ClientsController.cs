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

        /*
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }
        */
  
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,ClientId,ClientSecret")] Client client)
        {
            if (id != client.ID)
            {
                return NotFound();
            }
            try
            {
                _context.Update(client);
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

        public async Task<IActionResult> Authorizations(int? id)
        {
            /*
             * TODO check what happens if a user puts a random id
             */
            if (id == null)
            {
                return NotFound();
            }
            var authorizations = await _context.authorization
                .Where(m => m.ClientID == id).Include(q => q.Operation).Include(q => q.Operation.Resource).Include(q => q.Operation.Resource.Endpoint).ToListAsync();
            return View(authorizations);
        }

        public async Task<IActionResult> AuthorizationsAdd(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<int> authorizations = _context.authorization.Where(q => q.ClientID == id).Select(a => a.OperationID).ToList();
            var endpoints = await _context.operation.OrderBy(q=>q.ResourceID).Include(q=>q.Resource).Include(q => q.Resource.Endpoint).ToListAsync();
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
        public async Task<IActionResult> AuthorizationsAdd(int id, List<int> enpoints)
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
            return RedirectToAction(nameof(Authorizations), new { id = id });
        }

        public async Task<IActionResult> AuthorizationsDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var authorization = await _context.authorization.FirstOrDefaultAsync(m => m.ID == id);
            if (authorization == null)
            {
                return NotFound();
            }
            _context.authorization.Remove(authorization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Authorizations), new { id = authorization.ClientID });
        }

        public IActionResult RedirectURIAdd(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View();
        }

       

        public IActionResult Secrets(int? id)
        {
            return View();
        }

        public async Task<IActionResult> SecretsConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = await _context.client.FirstOrDefaultAsync(m => m.ID == id);
            if (client == null)
            {
                return NotFound();
            }
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[18];
            rng.GetBytes(randomBytes);
            client.ClientId = Base64UrlEncoder.Encode(randomBytes);
            rng.GetBytes(randomBytes);
            client.ClientSecret = Base64UrlEncoder.Encode(randomBytes);
            _context.Update(client);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool ClientExists(int id)
        {
            return _context.client.Any(e => e.ID == id);
        }
    }
}
