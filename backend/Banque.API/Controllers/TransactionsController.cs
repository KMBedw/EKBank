using Banque.API.Data;
using Banque.API.Models;
using Banque.API.DTOs.Banking;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Banque.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly BanqueContext _context;
        private readonly IWebHostEnvironment _env;

        public TransactionsController(BanqueContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Compte)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Date = t.Date,
                    Montant = t.Montant,
                    Type = t.Type,
                    CompteNumero = t.Compte != null ? t.Compte.Numero : "N/A"
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // ✅ GET: api/transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var t = await _context.Transactions
                .Include(x => x.Compte)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (t == null)
                return NotFound();

            var dto = new TransactionDto
            {
                Id = t.Id,
                Date = t.Date,
                Montant = t.Montant,
                Type = t.Type,
                CompteNumero = t.Compte?.Numero ?? "N/A"
            };

            return Ok(dto);
        }

        // ✅ POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> PostTransaction(Transaction transaction)
        {
            var compte = await _context.Comptes.FindAsync(transaction.CompteId);
            if (compte == null)
                return BadRequest("Le compte associé n'existe pas.");

            if (transaction.Type.ToLower() == "credit")
            {
                compte.Solde += transaction.Montant;
            }
            else if (transaction.Type.ToLower() == "debit")
            {
                if (compte.Solde < transaction.Montant)
                    return BadRequest("Solde insuffisant pour effectuer le débit.");
                compte.Solde -= transaction.Montant;
            }
            else
            {
                return BadRequest("Type de transaction invalide. Utilisez 'credit' ou 'debit'.");
            }

            transaction.Date = DateTime.Now;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var dto = new TransactionDto
            {
                Id = transaction.Id,
                Date = transaction.Date,
                Montant = transaction.Montant,
                Type = transaction.Type,
                CompteNumero = compte.Numero
            };

            return CreatedAtAction(nameof(GetTransaction), new { id = dto.Id }, dto);
        }

        // ✅ PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
                return BadRequest();

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ GET: api/transactions/releve/{compteId}
        [HttpGet("releve/{compteId}")]
        public async Task<ActionResult<object>> GetReleveBancaire(int compteId)
        {
            var compte = await _context.Comptes.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == compteId);
            if (compte == null)
                return NotFound("Compte non trouvé.");

            var transactions = await _context.Transactions
                .Where(t => t.CompteId == compteId)
                .OrderBy(t => t.Date)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Date = t.Date,
                    Montant = t.Montant,
                    Type = t.Type,
                    CompteNumero = compte.Numero
                })
                .ToListAsync();

            decimal soldeFinal = compte.Solde;
            decimal soldeInitial = 0;

            foreach (var t in transactions)
            {
                if (t.Type.ToLower() == "credit")
                    soldeInitial -= t.Montant;
                else if (t.Type.ToLower() == "debit")
                    soldeInitial += t.Montant;
            }

            var releve = new
            {
                Compte = new { compte.Id, compte.Numero, Client = compte.Client?.Nom },
                SoldeInitial = soldeInitial,
                SoldeActuel = soldeFinal,
                Transactions = transactions
            };

            return Ok(releve);
        }
        // ✅ GET: api/transactions/releve-pdf/1
        [HttpGet("releve-pdf/{compteId}")]
        public async Task<IActionResult> GetReleveBancairePdf(int compteId)
        {
            var compte = await _context.Comptes
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == compteId);

            if (compte == null)
                return NotFound("Compte non trouvé.");

            var transactions = await _context.Transactions
                .Where(t => t.CompteId == compteId)
                .OrderBy(t => t.Date)
                .ToListAsync();

            // Calculs
            decimal soldeFinal = compte.Solde;
            decimal soldeInitial = 0;
            foreach (var t in transactions)
            {
                if (t.Type?.ToLower() == "credit") soldeInitial -= t.Montant;
                else if (t.Type?.ToLower() == "debit") soldeInitial += t.Montant;
            }

            // Création du PDF en mémoire
            using (var stream = new MemoryStream())
            {
                // Document et writer
                var document = new Document(PageSize.A4, 36, 36, 54, 54); // marges
                var writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // POLICES (utiliser des polices standards)
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(0, 102, 204)); // bleu
                var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(0, 0, 0)); // noir
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(0, 0, 0)); // noir
                var smallGray = FontFactory.GetFont(FontFactory.HELVETICA, 8, new BaseColor(128, 128, 128)); // gris

                // --- ENTÊTE : logo + infos banque -------------------------
                try
                {
                    var logoPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images", "logo.png");
                    if (System.IO.File.Exists(logoPath))
                    {
                        var logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(100f, 60f);
                        logo.Alignment = Element.ALIGN_LEFT;

                        // table header : 2 colonnes (logo | info banque)
                        var headerTbl = new PdfPTable(2) { WidthPercentage = 100 };
                        headerTbl.SetWidths(new float[] { 1.2f, 3.8f });

                        var logoCell = new PdfPCell(logo) { Border = Rectangle.NO_BORDER, VerticalAlignment = Element.ALIGN_MIDDLE };
                        headerTbl.AddCell(logoCell);

                        var bankInfo = new PdfPTable(1);
                        bankInfo.DefaultCell.Border = Rectangle.NO_BORDER;
                        bankInfo.AddCell(new Phrase("BANQUE.EXAMPLE", titleFont));
                        bankInfo.AddCell(new Phrase("Relevé de compte", subtitleFont));
                        bankInfo.AddCell(new Phrase($"Compte : {compte.Numero}", normalFont));
                        bankInfo.AddCell(new Phrase($"Client : {compte.Client?.Nom}", normalFont));

                        var infoCell = new PdfPCell(bankInfo) { Border = Rectangle.NO_BORDER, VerticalAlignment = Element.ALIGN_MIDDLE };
                        headerTbl.AddCell(infoCell);

                        document.Add(headerTbl);
                    }
                    else
                    {
                        // Si pas de logo, on affiche juste le titre
                        document.Add(new Paragraph("BANQUE.EXAMPLE", titleFont));
                        document.Add(new Paragraph("Relevé de compte", subtitleFont));
                        document.Add(new Paragraph($"Compte : {compte.Numero}", normalFont));
                        document.Add(new Paragraph($"Client : {compte.Client?.Nom}", normalFont));
                    }
                }
                catch
                {
                    // En cas d'erreur de lecture du logo, on continue sans bloquer
                    document.Add(new Paragraph("BANQUE.EXAMPLE", titleFont));
                    document.Add(new Paragraph("Relevé de compte", subtitleFont));
                    document.Add(new Paragraph($"Compte : {compte.Numero}", normalFont));
                    document.Add(new Paragraph($"Client : {compte.Client?.Nom}", normalFont));
                }

                document.Add(new Paragraph(" ")); // espace

                // --- Info de synthèse (solde) --------------------------------
                var synthese = new PdfPTable(2) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_LEFT };
                synthese.SetWidths(new float[] { 1f, 1f });
                synthese.AddCell(new PdfPCell(new Phrase("Solde initial", subtitleFont)) { Border = Rectangle.NO_BORDER });
                synthese.AddCell(new PdfPCell(new Phrase(soldeInitial.ToString("N0") + " €", normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });
                synthese.AddCell(new PdfPCell(new Phrase("Solde actuel", subtitleFont)) { Border = Rectangle.NO_BORDER });
                synthese.AddCell(new PdfPCell(new Phrase(soldeFinal.ToString("N0") + " €", normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });
                document.Add(synthese);

                document.Add(new Paragraph(" ")); // espace

                // --- Tableau des transactions ---------------------------------
                var table = new PdfPTable(4) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 2f, 1f, 1f, 1f }); // Date | Type | Montant | Solde (optionnel)

                // header cells styled
                var headerBg = new BaseColor(230, 230, 230);
                PdfPCell H(string text) => new PdfPCell(new Phrase(text, subtitleFont)) { BackgroundColor = headerBg, Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER };

                table.AddCell(H("Date"));
                table.AddCell(H("Type"));
                table.AddCell(H("Montant"));
                table.AddCell(H("Solde après"));

                // calculer le solde au fil des transactions si tu veux afficher solde après chaque opération
                decimal running = soldeInitial;
                foreach (var t in transactions)
                {
                    if (t.Type?.ToLower() == "credit") running += t.Montant;
                    else if (t.Type?.ToLower() == "debit") running -= t.Montant;

                    // cellules
                    PdfPCell C(string s, int align = Element.ALIGN_LEFT) => new PdfPCell(new Phrase(s, normalFont)) { Padding = 6, HorizontalAlignment = align };

                    table.AddCell(C(t.Date.ToString("dd/MM/yyyy HH:mm"), Element.ALIGN_LEFT));
                    table.AddCell(C((t.Type ?? "").ToUpperInvariant(), Element.ALIGN_CENTER));
                    table.AddCell(C(t.Montant.ToString("N0") + " €", Element.ALIGN_RIGHT));
                    table.AddCell(C(running.ToString("N0") + " €", Element.ALIGN_RIGHT));
                }

                document.Add(table);

                // --- Pied de page simple ----
                document.Add(new Paragraph(" "));
                var footerTbl = new PdfPTable(2) { WidthPercentage = 100 };
                footerTbl.SetWidths(new float[] { 1f, 1f });

                footerTbl.AddCell(new PdfPCell(new Phrase($"Généré le : {DateTime.Now:dd/MM/yyyy HH:mm}", smallGray)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });
                footerTbl.AddCell(new PdfPCell(new Phrase($"Banque.EXAMPLE - Contact : contact@banque.example", smallGray)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });

                document.Add(footerTbl);

                // close
                document.Close();
                writer.Close();

                var bytes = stream.ToArray();
                return File(bytes, "application/pdf", $"Releve_Compte_{compte.Numero}.pdf");
            }
        }

    }
}
