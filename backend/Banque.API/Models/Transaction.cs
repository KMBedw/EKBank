namespace Banque.API.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Montant { get; set; }
        public string Type { get; set; } = string.Empty; // Débit ou Crédit
        public int CompteId { get; set; }
        public Compte? Compte { get; set; }
    }
}
