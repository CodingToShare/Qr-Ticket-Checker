using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qr_Ticket_Checker.Alerts;
using Qr_Ticket_Checker.Data;
using Qr_Ticket_Checker.Models;
using Qr_Ticket_Checker.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using static iTextSharp.text.pdf.AcroFields;

namespace Qr_Ticket_Checker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Event()
        {
            return View();
        }

        [Authorize]
        public IActionResult Phase()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ValidateQR(Guid id)
        {
            var person = _context.Persons.Where(x => x.PersonID == id).FirstOrDefault();
            var even = _context.Events.Where(x => x.EventID == person.EventID).FirstOrDefault();
            var phase = _context.Phases.Where(x => x.PhaseID == person.PhaseID).FirstOrDefault();

            var model = new ValidatePersonViewModel()
            {
                PersonID = person.PersonID,
                EventName = even.EventName,
                PhaseName = phase.PhaseName,
                Name = person.Name,
                LastName = person.LastName,
                DocumentType = GetDisplayName(person.DocumentType),
                DocumentNumber = person.DocumentNumber,
                Email = person.Email,
                Phone = person.Phone,
                IsActive = person.IsActive ? "Ingresado" : "No Ingresado"
            };

            return View(model);
        }

        [Authorize]
        public IActionResult Validate(Guid id)
        {
            var person = _context.Persons.Where(x => x.PersonID == id).FirstOrDefault();

            person.IsActive = true;

            _context.SaveChanges();

            return RedirectToAction("ValidateQR", new { id = id }).WithSuccess("El cambio de estado fue exitoso");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static string GetDisplayName(Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var enumName = Enum.GetName(enumType, enumValue);
            var memberInfo = enumType.GetMember(enumName)[0];
            var attributes = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            var displayAttribute = attributes[0] as DisplayAttribute;

            return displayAttribute?.Name ?? "Unknown";
        }
    }
}
