using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Bibliotec_MVC.Data;
using Projeto_Bibliotec_MVC.Models;

namespace Projeto_Bibliotec_MVC;

[Authorize(Roles = "Administrador, Aluno")]
public class LivroController : Controller
{
    private readonly AppDbContext _context;

    public LivroController(AppDbContext context)
    {
        _context = context;
    }

    //* Lista os livros cadastrados no banco de dados.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        //* Quando temos navigation -> retornamos um objeto completo daquela classe
        //* Ex: IdAutorNavigation - retorna o objeto de autor( nome, id, nacionalidade)
        List<Livro> livros = await _context.Livro
            .Include(l => l.IdAutorNavigation)
            .Include(l => l.IdStatusNavigation)
            .Include(l => l.IdCategoria)
            //* OrderBy -> Alinha por padrão crescente.
            .OrderBy(l => l.Titulo)
            .ToListAsync();

        if (User.IsInRole("Administrador"))
        {
            //* Retorna a View de listagem de livros para adm enviando as informações do livro
            return View("ListagemAdministrador", livros);
        }

        //* Retorna a View de listagem de aluno enviando a lista de livros
        return View("ListagemAluno", livros);
    }


}