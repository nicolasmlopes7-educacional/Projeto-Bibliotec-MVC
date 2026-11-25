using System;
using System.Collections.Generic;

namespace Projeto_Bibliotec_MVC.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public virtual ICollection<Livro> IdLivro { get; set; } = new List<Livro>();
}
