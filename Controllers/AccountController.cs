using Microsoft.AspNetCore.Mvc;
using SeuProjeto.Data;
using SeuProjeto.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Net;
using System.Net.Mail;

namespace SeuProjeto.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            // Exibe mensagens do TempData se existirem
            if (TempData["Mensagem"] != null)
            {
                ViewBag.Mensagem = TempData["Mensagem"];
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            // Consulta usuário no banco
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (usuario != null)
            {
                // Salva dados na sessão
                HttpContext.Session.SetString("Nome", usuario.Nome);
                HttpContext.Session.SetString("Foto", usuario.Foto ?? "");

                // ✅ Redireciona para o outro projeto (CadClientes)
                return Redirect("https://localhost:5002");
            }

            // ❌ Login inválido
            ViewBag.Mensagem = TempData["Mensagem"];
            ViewBag.Erro = "Login e Senha inválidos!";
            return View();
        }

        public IActionResult Index()
        {
            // Verifica se o usuário está logado
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Nome")))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Nome = HttpContext.Session.GetString("Nome");
            ViewBag.Foto = HttpContext.Session.GetString("Foto");

            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Usuario usuario, IFormFile FotoUpload)
        {
            if (FotoUpload != null)
            {
                var filePath = $"wwwroot/fotos/{FotoUpload.FileName}";
                using var stream = System.IO.File.Create(filePath);
                FotoUpload.CopyTo(stream);
                usuario.Foto = "/fotos/" + FotoUpload.FileName;
            }

            _db.Usuarios.Add(usuario);
            _db.SaveChanges();

            TempData["Mensagem"] = "Usuário criado com sucesso! Faça login para continuar.";
            return RedirectToAction("Login");
        }

        public IActionResult EsqueceuSenha() => View();

        [HttpPost]
        public IActionResult EsqueceuSenha(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Mensagem = "Por favor, informe um e-mail válido.";
                return View();
            }

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario != null)
            {
                try
                {
                    EnviarEmail(usuario.Email, "Recuperação de Senha", $"Sua senha é: {usuario.Senha}");
                    ViewBag.Mensagem = "Email enviado com sucesso!";
                }
                catch (Exception ex)
                {
                    ViewBag.Mensagem = $"Erro ao enviar o e-mail: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Mensagem = "Email não encontrado.";
            }

            return View();
        }

        public void EnviarEmail(string para, string assunto, string corpo)
        {
            var remetente = "webradiofabriciocast@gmail.com";
            var smtpUsuario = "1c4b1e6703015e";
            var smtpSenha = "718a768cd51e06";

            using var mensagem = new MailMessage
            {
                From = new MailAddress(remetente),
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = false
            };

            mensagem.To.Add(para);

            using var smtp = new SmtpClient("smtp.mailtrap.io", 587)
            {
                Credentials = new NetworkCredential(smtpUsuario, smtpSenha),
                EnableSsl = true
            };

            try
            {
                smtp.Send(mensagem);
                Console.WriteLine("E-mail enviado com sucesso (via Mailtrap).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}