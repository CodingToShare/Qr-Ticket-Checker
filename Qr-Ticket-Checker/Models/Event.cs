using System.ComponentModel.DataAnnotations;

namespace Qr_Ticket_Checker.Models
{
    public class Event
    {
        [Key]
        public Guid EventID { get; set; }

        [Display(Name = "Nombre del Evento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string EventName { get; set; } = string.Empty;

        [Display(Name = "Numero de Entradas")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int TicketNumber { get; set; }

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

        public Person? Person { get; set; }
    }
}
