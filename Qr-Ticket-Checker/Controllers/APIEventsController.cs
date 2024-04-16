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

namespace Qr_Ticket_Checker.Controllers
{
    [Route("api/[controller]/[action]")]
    public class APIEventsController : Controller
    {
        private ApplicationDbContext _context;

        public APIEventsController(ApplicationDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var events = _context.Events.Select(i => new {
                i.EventID,
                i.EventName,
                i.TicketNumber,
                i.TemplateIdentifier,
                i.Created,
                i.CreatedBy,
                i.Modify,
                i.ModifyBy
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "EventID" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(events, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Event();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            model.EventID = Guid.NewGuid();
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

            var result = _context.Events.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.EventID });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            var model = await _context.Events.FirstOrDefaultAsync(item => item.EventID == key);
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
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            var model = await _context.Events.FirstOrDefaultAsync(item => item.EventID == key);

            _context.Events.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> PersonsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Persons
                         orderby i.Name
                         select new {
                             Value = i.PersonID,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Event model, IDictionary values) {
            string EVENT_ID = nameof(Event.EventID);
            string EVENT_NAME = nameof(Event.EventName);
            string TICKET_NUMBER = nameof(Event.TicketNumber);
            string TEMPLATE_IDENTIFIER = nameof(Event.TemplateIdentifier);
            string CREATED = nameof(Event.Created);
            string CREATED_BY = nameof(Event.CreatedBy);
            string MODIFY = nameof(Event.Modify);
            string MODIFY_BY = nameof(Event.ModifyBy);

            if(values.Contains(EVENT_ID)) {
                model.EventID = ConvertTo<System.Guid>(values[EVENT_ID]);
            }

            if(values.Contains(EVENT_NAME)) {
                model.EventName = Convert.ToString(values[EVENT_NAME]);
            }

            if(values.Contains(TICKET_NUMBER)) {
                model.TicketNumber = Convert.ToInt32(values[TICKET_NUMBER]);
            }

            if (values.Contains(TEMPLATE_IDENTIFIER))
            {
                model.TemplateIdentifier = Convert.ToString(values[TEMPLATE_IDENTIFIER]);
            }

            if (values.Contains(CREATED)) {
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