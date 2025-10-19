namespace Banque.API.DTOs.Banking
{
    public class CompteDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public decimal Solde { get; set; }

        // Optionnel : inclure un mini client pour affichage
        public string ClientNom { get; set; } = string.Empty;
    }
}
