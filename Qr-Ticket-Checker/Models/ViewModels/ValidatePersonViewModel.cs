using Qr_Ticket_Checker.Enums;
using System.ComponentModel.DataAnnotations;

namespace Qr_Ticket_Checker.Models.ViewModels
{
    public class ValidatePersonViewModel
    {
        public Guid PersonID { get; set; }

        [Display(Name = "Nombre del Evento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string EventName { get; set; } = string.Empty;

        [Display(Name = "Nombre de la Fase")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string PhaseName { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Tipo de Documento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string DocumentType { get; set; } = string.Empty ;

        [Display(Name = "Numero Documento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Correo Electronico")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Ingresado/No Ingresado")]
        public string IsActive { get; set; } = string.Empty;
    }
}
