using Microsoft.AspNetCore.Mvc;
using Projeto_Bibliotec_MVC.ViewModels;

namespace Projeto_Bibliotec_MVC.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        // Recebe os dados enviados pelo formulário  
        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            // Verifica se os dados recebidos atendem as regras de validação
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // mensagem temporária para testar o recebimento dos dados.
            ModelState.AddModelError("", "Autenticação ainda não implementada");

            return View(model);
        }
    }
}