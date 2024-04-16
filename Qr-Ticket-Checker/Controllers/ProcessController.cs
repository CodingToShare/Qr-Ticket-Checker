using Microsoft.AspNetCore.Mvc;
using Qr_Ticket_Checker.Data;
using Qr_Ticket_Checker.Models;
using Qr_Ticket_Checker.Models.ViewModels;
using QRCoder;

namespace Qr_Ticket_Checker.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProcessController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProcessController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventCount = _context.Persons.Where(x => x.EventID == model.EventID).Count();
            var eve = _context.Events.First(x => x.EventID == model.EventID);
            var phases = model.PhaseIdentifiers.Split(',');


            if (eventCount + phases.Count() <= eve.TicketNumber)
            {
                foreach (var phase in phases)
                {
                    var person = new Person()
                    {
                        PersonID = Guid.NewGuid(),
                        EventID = model.EventID,
                        PhaseID = Guid.Parse(phase),
                        Name = model.Name,
                        LastName = model.LastName,
                        DocumentType = model.DocumentType,
                        DocumentNumber = model.DocumentNumber,
                        Email = model.Email,
                        Phone = model.Phone,
                        IsActive = false,
                        Created = DateTime.UtcNow.AddHours(-5),
                        CreatedBy = User.Identity.Name,
                        Modify = DateTime.UtcNow.AddHours(-5),
                        ModifyBy = User.Identity.Name
                    };

                    _context.Persons.Add(person);


                    var url = _configuration.GetValue<string>("EmailServices:UrlSite") + "Home/ValidateQR/" + person.PersonID;

                    // Generación del QR
                    using var qrGenerator = new QRCodeGenerator();
                    using var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                    using var qrCode = new PngByteQRCode(qrData);
                    var qrCodeAsPng = qrCode.GetGraphic(20);

                    // Convertir el QR a Base64
                    var qrBase64 = Convert.ToBase64String(qrCodeAsPng);
                    //var imgTag = $"<img width='200' height='200' src= 'cid:*'/>";
                    var imgTag = $"";

                    var token = _configuration.GetValue<string>("EmailServices:ApiKey");
                    var from = _configuration.GetValue<string>("EmailServices:Email");
                    var subject = $"Skynet - Recuerda Guardar Este Correo Para Entrar al Evento - {eve.EventName}";

                    var stringContent = @$"{{
                        ""from"": {{
                            ""email"": ""{from}""
                        }},
                        ""to"": [
                            {{
                                ""email"": ""{person.Email}""
                            }}
                        ],
                        ""subject"": ""{subject}"",
                        ""attachments"": [
                            {{
                                ""content"": ""{qrBase64}"",
                                ""disposition"": ""inline"",
                                ""filename"": ""QREvent"",
                                ""id"": ""*""
                            }}
                        ],
                        ""template_id"": ""{eve.TemplateIdentifier}""
                    }}";

                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailersend.com/v1/email");
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Headers.Add("Authorization", $"Bearer {token}");
                    var content = new StringContent(stringContent, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                }

                _context.SaveChanges();
            }
            else
            {
                return Ok(Json(false));
            }

            return Ok(Json(true));
        }
    }
}
