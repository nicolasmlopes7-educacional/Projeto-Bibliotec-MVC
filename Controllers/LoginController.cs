using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Bibliotec_MVC.Data;
using Projeto_Bibliotec_MVC.Models;
using Projeto_Bibliotec_MVC.ViewModels;

namespace Projeto_Bibliotec_MVC.Controllers
{
    public class LoginController : Controller
    {
        // RECEBENDO DEPENDÊNCIAS (registradas na program)
        // permite consultar e manipular os dados do banco 
        private readonly AppDbContext _context;
        // permite gerar e validar o hash das senhas
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public LoginController(AppDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }



        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        // Recebe os dados enviados pelo formulário  
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            //* Verifica se os dados recebidos atendem as regras de validação
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //* O Async serve para utilizar coisas externas da aplicação sem alterar a ordem de rodagem do código (utilizado sempre quando se tratar de banco de dados)
            Usuario usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == model.Email && u.Excluido == false); //* Esse código busca um usuário pelo email informado e verifica se ele não está excluído
            //* Se não encontrar nenhum usuário ou se estiver excluído -> retorna null


            if (usuario == null)
            {
                ModelState.AddModelError("", "E-mail ou senha inválidos.");
                return View(model);
            }

            //* Verifica se a senha infromada corresponde ao hash armazenado.
            //* o método recebe 3 parâmetros
            var resultadoSenha = _passwordHasher.VerifyHashedPassword(
                usuario,    //* objeto que representa o usuário encontrado no banco.
                usuario.Senha,  //* hash da senha armazenada no banco de dados
                model.Senha     //* senha digitada pelo usuário no formulário de login.
            );

            //* verifica se a senha está incorreta
            if (resultadoSenha == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "E-mail ou senha inválidos.");
                return View(model);
            }

            //* verifica se a senha está correta, mas o hash precisa ser atualizado
            if (resultadoSenha == PasswordVerificationResult.Success)
            {
                //* gera um novo hash da senha utilizando as configurações de segurança atuais.
                usuario.Senha = _passwordHasher.HashPassword(usuario, model.Senha);

                //* salva o novo hash no banco de dados
                await _context.SaveChangesAsync();
            }

            //* verifica se o usuário está cadastrado como administrador
            //* verifica dentro da tabela de adm se o usuário possuí registro
            //* retorna true se existir pelo menos um registro que atende a condição informada.
            bool administrador = await _context.Administrador.AnyAsync(a => a.IdAdministrador == usuario.IdUsuario);

            bool aluno = await _context.Aluno.AnyAsync(a => a.IdAluno == usuario.IdUsuario);

            string? tipoUsuario = administrador ? "Administrador" : aluno ? "Aluno" : null;

            //* cria uma lista de claims com as informações do usuário autenticado
            //* claims -> são informações do usuário que permitem identificar e controlar o acesso.
            var claims = new List<Claim>
            {
                //* Armazena o identificador(id) do usuário
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),

                //* Armazena o nome do usuário
                new Claim(ClaimTypes.Name, usuario.Nome),

                //* Armazena o email do usuário
                new Claim(ClaimTypes.Email, usuario.Email),

                //* Armazena o tipo de usuário para controle de acesso
                new Claim(ClaimTypes.Role, tipoUsuario)
            };

            //* Cria a identidade do usuário utilizando as Claims
            //* ClaimsIdentity -> como se fosse uma carteirinha de identificação do usuário dentro do sistema.
            //* cookies -> informa que estamos utilizando o tipo de authenticação por cookies.
            var identidade = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            //* representar o usuário autenticado
            //* classe do asp.net para representar usuário
            var principal = new ClaimsPrincipal(identidade);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            //* Remove os cookies de autenticação
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            //* Redireciona o usuário para a tela de login.
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}