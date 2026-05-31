using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCS_ProjetoIntegrador_III_B.Entities
{
    public class Estagio
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = null!;

        public string? Descricao { get; set; }

        // Empresa
        public int EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }

        // Orientador
        public int OrientadorId { get; set; }
        public Orientador? Orientador { get; set; }

        // Alunos (many-to-many)
        public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
    }
}