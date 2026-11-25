using System;
using System.Collections.Generic;

namespace Projeto_Bibliotec_MVC.Models;

public partial class Curso
{
    public int IdCurso { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public string? Periodo { get; set; }

    public virtual ICollection<Aluno> Aluno { get; set; } = new List<Aluno>();
}
