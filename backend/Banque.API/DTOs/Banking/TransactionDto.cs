namespace Banque.API.DTOs.Banking
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Montant { get; set; }
        public string Type { get; set; } = string.Empty; // "Crédit" ou "Débit"
        public string CompteNumero { get; set; } = string.Empty;
    }
}
