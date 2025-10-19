using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Banque.API.Data;
using Banque.API.Models;
using Banque.API.DTOs.Banking;

namespace Banque.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComptesController : ControllerBase
    {
        private readonly BanqueContext _context;

        public ComptesController(BanqueContext context)
        {
            _context = context;
        }

        // ✅ GET: api/comptes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompteDto>>> GetComptes()
        {
            var comptes = await _context.Comptes
                .Include(c => c.Client)
                .Select(c => new CompteDto
                {
                    Id = c.Id,
                    Numero = c.Numero,
                    Solde = c.Solde,
                    ClientNom = c.Client != null ? c.Client.Nom : "Inconnu"
                })
                .ToListAsync();

            return Ok(comptes);
        }

        // ✅ GET: api/comptes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompteDto>> GetCompte(int id)
        {
            var compte = await _context.Comptes
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compte == null)
                return NotFound();

            var dto = new CompteDto
            {
                Id = compte.Id,
                Numero = compte.Numero,
                Solde = compte.Solde,
                ClientNom = compte.Client != null ? compte.Client.Nom : "Inconnu"
            };

            return Ok(dto);
        }

        // ✅ POST: api/comptes
        [HttpPost]
        public async Task<ActionResult<CompteDto>> PostCompte(Compte compte)
        {
            // Vérifie que le client existe
            var client = await _context.Clients.FindAsync(compte.ClientId);
            if (client == null)
                return BadRequest("Le client associé n'existe pas.");

            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();

            // Retourne le DTO créé
            var dto = new CompteDto
            {
                Id = compte.Id,
                Numero = compte.Numero,
                Solde = compte.Solde,
                ClientNom = client.Nom
            };

            return CreatedAtAction(nameof(GetCompte), new { id = compte.Id }, dto);
        }

        // ✅ PUT: api/comptes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompte(int id, Compte compte)
        {
            if (id != compte.Id)
                return BadRequest();

            _context.Entry(compte).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Comptes.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/comptes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompte(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound();

            _context.Comptes.Remove(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
