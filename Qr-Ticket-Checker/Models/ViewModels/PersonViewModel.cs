using Qr_Ticket_Checker.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Qr_Ticket_Checker.Models.ViewModels
{
    public class PersonViewModel
    {

        [Display(Name = "Evento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid EventID { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Tipo de Documento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DocumentType DocumentType { get; set; }

        [Display(Name = "Numero Documento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Correo Electronico")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Fase")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string PhaseIdentifiers { get; set; } = string.Empty;
    }
}
