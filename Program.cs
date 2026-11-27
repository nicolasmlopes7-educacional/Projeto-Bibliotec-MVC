using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projeto_Bibliotec_MVC.Data;
using Projeto_Bibliotec_MVC.Models;

//* Carrega as variáveis de ambiente do arquivo .env
//* a ordem é importante -> precisamos carregar as variáveis de ambiente (env) antes de inicializar as configurações da aplicação.
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexaoPadrao")
    )
); // isso é padrão

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Página para qual o usuário será direcionado caso não esteja autenticado.
        options.LoginPath = "/Login/Index";

        // Página para qual o usuário será direcionado caso não tenha permissão
        options.AccessDeniedPath = "/Login/AcessoNegado";

        // Define o tempo de validade da autenticação
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        // Renova a validade da autenticação conforme a atividade do usuário
        options.SlidingExpiration = true;
    });

// Registra os recursos de autorização.
builder.Services.AddAuthentication();

// Registra o serviço responsável pela geração e validação de hash de senhas.
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// A ordem é importante -> primeiro identifica o usuário, depois vê se ele tem permissão
// primeiro -> UseAuthentication();
// depois -> UseAuthorization();

// Identifica o usuário do cookie de autenticação
app.UseAuthentication();

// Verifica se o usuário possui permissão para acessar o recurso solicitado. 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
