namespace Banque.API.DTOs.Banking
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int NombreComptes { get; set; }
    }
}
