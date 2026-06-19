using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCS_ProjetoIntegrador_III_B.Entities
{
    public class Aluno
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = null!;

        public string? Matricula { get; set; }

        [Required]
        public string CPF { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        public string? Endereco { get; set; }

        // Sexo: enum
        public Sexo? Sexo { get; set; }

        public ICollection<Estagio> Estagios { get; set; } = new List<Estagio>();
    }
}