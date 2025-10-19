using System.Numerics;

namespace Banque.API.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;

        // Auth
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Relations
        public ICollection<Compte>? Comptes { get; set; } = new List<Compte>();

    }

}
