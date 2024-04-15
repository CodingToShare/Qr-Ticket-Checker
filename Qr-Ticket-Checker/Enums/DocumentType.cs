using System.ComponentModel.DataAnnotations;

namespace Qr_Ticket_Checker.Enums
{
    public enum DocumentType
    {

        [Display(Name = "Cedula")]
        CC = 10,

        [Display(Name = "Pasaporte")]
        Pasport = 20,

        [Display(Name = "Cedula Extranjera")]
        CCE = 30
    }
}
