namespace Banque.API.Models
{
    public class Compte
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public decimal Solde { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
