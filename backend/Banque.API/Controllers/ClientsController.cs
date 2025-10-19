using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Banque.API.Data;
using Banque.API.Models;
using Banque.API.DTOs.Banking;
using Microsoft.AspNetCore.Authorization;

namespace Banque.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly BanqueContext _context;

        public ClientsController(BanqueContext context)
        {
            _context = context;
        }

        // ✅ GET: api/clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _context.Clients
                .Include(c => c.Comptes)
                .Select(c => new ClientDto
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Email = c.Email,
                    NombreComptes = c.Comptes.Count
                })
                .ToListAsync();

            return Ok(clients);
        }

        // ✅ GET: api/clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Comptes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound();

            var dto = new ClientDto
            {
                Id = client.Id,
                Nom = client.Nom,
                Email = client.Email,
                NombreComptes = client.Comptes.Count
            };

            return Ok(dto);
        }

        // ✅ POST: api/clients
        [HttpPost]
        public async Task<ActionResult<ClientDto>> PostClient(Client client)
        {
            if (await _context.Clients.AnyAsync(c => c.Email == client.Email))
                return BadRequest("Cet email est déjà utilisé.");

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var dto = new ClientDto
            {
                Id = client.Id,
                Nom = client.Nom,
                Email = client.Email,
                NombreComptes = 0
            };

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, dto);
        }

        // ✅ PUT: api/clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
                return BadRequest();

            var existing = await _context.Clients.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Nom = client.Nom;
            existing.Email = client.Email;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ DELETE: api/clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Comptes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound();

            if (client.Comptes.Any())
                return BadRequest("Impossible de supprimer un client ayant des comptes actifs.");

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ GET: api/clients/{id}/comptes
        [Authorize]
        [HttpGet("{id}/comptes")]
        public async Task<ActionResult<IEnumerable<CompteDto>>> GetComptesByClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Comptes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound($"Aucun client avec l'ID {id}.");

            if (!client.Comptes.Any())
                return Ok(new List<CompteDto>());

            var comptesDto = client.Comptes.Select(c => new CompteDto
            {
                Id = c.Id,
                Numero = c.Numero,
                Solde = c.Solde,
                ClientNom = client.Nom
            }).ToList();

            return Ok(comptesDto);
        }

        // ✅ GET: api/clients/{id}/transactions
        [Authorize]
        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByClient(int id)
        {
            // ✅ Charge le client avec ses comptes et transactions
            var client = await _context.Clients
                .Include(c => c.Comptes!)
                    .ThenInclude(compte => compte.Transactions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound($"Aucun client trouvé avec l'ID {id}.");

            // ✅ Vérifie qu’il a bien des comptes
            if (client.Comptes == null || !client.Comptes.Any())
                return Ok(new List<TransactionDto>());

            // ✅ Regroupe toutes les transactions de tous les comptes
            var transactions = client.Comptes
                .SelectMany(compte => compte.Transactions ?? new List<Transaction>())
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Date = t.Date,
                    Montant = t.Montant,
                    Type = t.Type,
                    CompteNumero = client.Comptes
                        .FirstOrDefault(c => c.Id == t.CompteId)?.Numero ?? "Inconnu"
                })
                .OrderByDescending(t => t.Date)
                .ToList();

            return Ok(transactions);
        }

    }
}
