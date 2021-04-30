using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChloesBeauty.API.Models;
using System;
using ChloesBeauty.API.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ChloesBeauty.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AvailabilityController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors
        //Inyectamos en el constructor la dependecia del context (la conexión con la bbdd)
        public AvailabilityController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool AvailabilityExists(int id)
        {
            //Devolvemos true si existe alguna disponibilidad con ese id, false en caso contrario
            return _context.Availabilities.Any(e => e.AvailabilityId == id);
        }

        private static IEnumerable<AvailabilitiesViewModel> Availabities(IEnumerable<Availability> availabilities)
        {
            // Instanciamos una lista de disponibilidades, utilizo un viewModel para devolver solo lo que necesito
            var availabilitiesByDate = new List<AvailabilitiesViewModel>();
            // Agrupo las disponibilidades por fecha
            var groupDates = availabilities.GroupBy(a => a.Date.Date);
            // Recorro cada fecha
            foreach (var groupdate in groupDates)
            {
                // Y creo una lista separando la fecha de la hora, así sabré para cada fecha cuantas horas hay disponibles
                var availabilitiesViewModel = new AvailabilitiesViewModel
                {
                    Date = groupdate.FirstOrDefault().Date.Date,
                    TimesByDate = groupdate.Select(t => new TimesByDate { Time = t.Date.ToShortTimeString(), Id = t.AvailabilityId })
                };
                // Las voy añadiendo a una lista
                availabilitiesByDate.Add(availabilitiesViewModel);
            }
            // Y la devuelvo ordenada por fecha
            return availabilitiesByDate.OrderBy(a => a.Date);
        }

        #endregion Private Methods

        #region Public Methods

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            //Buscamos los datos de la disponibilidad con ese id
            var availability = await _context.Availabilities.FindAsync(id);
            //Si es null devolvemos un 404
            if (availability == null)
            {
                return NotFound();
            }
            //Si se ha encontrado ponemos el campo deleted a 1 para hacer un borrado lógico y guardamos los datos
            availability.Deleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AvailabilitiesViewModel>>> GetAvailabilities()
        {
            //Devolvemos todas las disponibilidades que no estén borradas y que la fecha sea igual o mayor a la actual
            var availabilities = await _context.Availabilities
                .Where(a => !a.Deleted && a.Date >= DateTime.Now)
                .ToListAsync();
            // Le pasamos la lista de disponibilidades al método para que las formatee tal como necesitamos devolverlas
            var availabilitiesViewModel = Availabities(availabilities);

            return Ok(availabilitiesViewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Availability>> GetAvailability(int id)
        {
            // Buscamos una disponibilidad en la bbdd con el id
            var availability = await _context.Availabilities.FindAsync(id);
            // Si es null devolvemos un 404
            if (availability == null)
            {
                return NotFound();
            }
            // Si se ha encontrado devolvemos los datos de la disponibilidad
            return availability;
        }

        [HttpPost]
        public async Task<ActionResult<Availability>> PostAvailability(Availability availability)
        {
            try
            {
                // Instanciamos una nueva disponibilidad
                var newAvailability = new Availability
                {
                    // A la hora de la fecha le añadimos dos horas porque la bbdd no coincide con el GMT
                    Date = availability.Date.AddHours(2),
                    ModifiedDate = DateTime.Now
                };
                // Buscamos si ya existe una cita para esa fecha y hora
                var availabilityExist = await _context.Availabilities.Where(a => a.Date == newAvailability.Date).FirstOrDefaultAsync();
                // Si existe devolvemos un 400
                if (availabilityExist != null)
                    return BadRequest(false);
                // Si no existe guardamos los datos
                _context.Availabilities.Add(newAvailability);
                await _context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception)
            {
                return BadRequest(false);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAvailability(int id, Availability availability)
        {
            //Si el id no corresponde con el id de la disponibilidad devolvemos un 400
            if (id != availability.AvailabilityId)
            {
                return BadRequest();
            }

            availability.ModifiedDate = DateTime.Now;
            // Ponemos el flag del estado a modificado
            _context.Entry(availability).State = EntityState.Modified;

            try
            {
                //Guardamos los datos para que se actualice
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Si ha habido algún problema comprobamos si la cita existe para manejar el error
                if (!AvailabilityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        #endregion Public Methods
    }
}