using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Qr_Ticket_Checker.Data;
using Qr_Ticket_Checker.Models;
using QRCoder;

namespace Qr_Ticket_Checker.Controllers
{
    [Route("api/[controller]/[action]")]
    public class APIPeopleController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public APIPeopleController(ApplicationDbContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id, DataSourceLoadOptions loadOptions) {
            var persons = _context.Persons.Where(x => x.EventID == id).Select(i => new {
                i.PersonID,
                i.EventID,
                i.PhaseID,
                i.Name,
                i.LastName,
                i.DocumentNumber,
                i.Email,
                i.Phone,
                i.Created,
                i.CreatedBy,
                i.Modify,
                i.ModifyBy
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "PersonID" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(persons, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Person();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Persons.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.PersonID });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.Persons.FirstOrDefaultAsync(item => item.PersonID == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            model.Created = DateTime.UtcNow.AddHours(-5);
            if (User.Identity is not null)
            {
                if (User.Identity.Name is not null)
                {
                    model.CreatedBy = User.Identity.Name;
                }
            }
            model.Modify = DateTime.UtcNow.AddHours(-5);
            if (User.Identity is not null)
            {
                if (User.Identity.Name is not null)
                {
                    model.ModifyBy = User.Identity.Name;
                }
            }

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();

            var url = _configuration.GetValue<string>("EmailServices:UrlSite") + "Home/ValidateQR/" + model.PersonID;

            // Generación del QR
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrCodeAsPng = qrCode.GetGraphic(20);

            // Convertir el QR a Base64
            var qrBase64 = Convert.ToBase64String(qrCodeAsPng);
            var imgTag = $"<img width='200' height='200' src= 'cid:*'/>";

            var token = _configuration.GetValue<string>("EmailServices:ApiKey");
            var from = _configuration.GetValue<string>("EmailServices:Email");
            var subject = $"Skynet - Recuerda Guardar Este Correo Para Entrar al Evento - {_context.Events.Where(x => x.EventID == model.EventID).FirstOrDefault().EventName}";
            var stringContent = @$"{{
                        ""from"": {{
                            ""email"": ""{from}""
                        }},
                        ""to"": [
                            {{
                                ""email"": ""{model.Email}""
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
                        ""template_id"": ""{_context.Events.Where(x => x.EventID == model.EventID).FirstOrDefault().TemplateIdentifier}""
                    }}";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailersend.com/v1/email");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var content = new StringContent(stringContent, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return Ok();
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            var model = await _context.Persons.FirstOrDefaultAsync(item => item.PersonID == key);

            _context.Persons.Remove(model);
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<IActionResult> PhasesLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Phases
                         orderby i.PhaseName
                         select new
                         {
                             Value = i.PhaseID,
                             Text = i.PhaseName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }


        private void PopulateModel(Person model, IDictionary values) {
            string PERSON_ID = nameof(Person.PersonID);
            string EVENT_ID = nameof(Person.EventID);
            string PHASE_ID = nameof(Person.PhaseID);
            string NAME = nameof(Person.Name);
            string LAST_NAME = nameof(Person.LastName);
            string DOCUMENT_NUMBER = nameof(Person.DocumentNumber);
            string EMAIL = nameof(Person.Email);
            string PHONE = nameof(Person.Phone);
            string CREATED = nameof(Person.Created);
            string CREATED_BY = nameof(Person.CreatedBy);
            string MODIFY = nameof(Person.Modify);
            string MODIFY_BY = nameof(Person.ModifyBy);

            if(values.Contains(PERSON_ID)) {
                model.PersonID = ConvertTo<System.Guid>(values[PERSON_ID]);
            }

            if(values.Contains(EVENT_ID)) {
                model.EventID = ConvertTo<System.Guid>(values[EVENT_ID]);
            }

            if(values.Contains(PHASE_ID)) {
                model.PhaseID = ConvertTo<System.Guid>(values[PHASE_ID]);
            }

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }

            if(values.Contains(LAST_NAME)) {
                model.LastName = Convert.ToString(values[LAST_NAME]);
            }

            if(values.Contains(DOCUMENT_NUMBER)) {
                model.DocumentNumber = Convert.ToString(values[DOCUMENT_NUMBER]);
            }

            if(values.Contains(EMAIL)) {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(PHONE)) {
                model.Phone = Convert.ToString(values[PHONE]);
            }

            if(values.Contains(CREATED)) {
                model.Created = Convert.ToDateTime(values[CREATED]);
            }

            if(values.Contains(CREATED_BY)) {
                model.CreatedBy = Convert.ToString(values[CREATED_BY]);
            }

            if(values.Contains(MODIFY)) {
                model.Modify = Convert.ToDateTime(values[MODIFY]);
            }

            if(values.Contains(MODIFY_BY)) {
                model.ModifyBy = Convert.ToString(values[MODIFY_BY]);
            }

        }

        private T ConvertTo<T>(object value) {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            if(converter != null) {
                return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            } else {
                // If necessary, implement a type conversion here
                throw new NotImplementedException();
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}