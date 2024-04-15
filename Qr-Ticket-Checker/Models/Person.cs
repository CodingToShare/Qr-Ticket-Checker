using Qr_Ticket_Checker.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qr_Ticket_Checker.Models
{
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [Display(Name = "Evento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid EventID { get; set; }

        [Display(Name = "Fase")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Guid PhaseID { get; set; }

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

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }

        [Display(Name = "Creado")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        [Display(Name = "Creado por")]
        public string? CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Modificado")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Modify { get; set; }

        [Display(Name = "Modificado por")]
        public string? ModifyBy { get; set; } = string.Empty;

        public ICollection<Event>? Events { get; } = new List<Event>();
        public ICollection<Phase>? Phases { get; } = new List<Phase>();
    }
}
