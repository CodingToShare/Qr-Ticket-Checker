using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qr_Ticket_Checker.Models;
using System.Security.Claims;

namespace Qr_Ticket_Checker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Phase> Phases { get; set; }
        public DbSet<Person> Persons { get; set; }
    }
}
