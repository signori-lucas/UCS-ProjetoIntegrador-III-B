using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCS_ProjetoIntegrador_III_B.Entities
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = null!;

        public string? Endereco { get; set; }

        public string? Telefone { get; set; }

        public ICollection<Estagio> Estagios { get; set; } = new List<Estagio>();
    }
}