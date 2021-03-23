using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChloesBeauty.API.Models;
using System;
using ChloesBeauty.API.ViewModels;

namespace ChloesBeauty.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors

        public AvailabilityController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool AvailabilityExists(int id)
        {
            return _context.Availabilities.Any(e => e.AvailabilityId == id);
        }

        private IEnumerable<AvailabilitiesViewModel> Availabities(IEnumerable<Availability> availabilities)
        {
            var availabilitiesByDate = new List<AvailabilitiesViewModel>();

            var groupDates = availabilities.GroupBy(a => a.Date.Date);

            foreach (var groupdate in groupDates)
            {
                var availabilitiesViewModel = new AvailabilitiesViewModel
                {
                    Date = groupdate.FirstOrDefault().Date.Date,
                    TimesByDate = groupdate.Select(t => new TimesByDate { Time = t.Date.ToShortTimeString(), Id = t.AvailabilityId })
                };

                availabilitiesByDate.Add(availabilitiesViewModel);
            }
            return availabilitiesByDate.OrderBy(a => a.Date);
        }

        #endregion Private Methods

        #region Public Methods

        // DELETE: api/Availability/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null)
            {
                return NotFound();
            }

            availability.Deleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Availability
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AvailabilitiesViewModel>>> GetAvailabilities()
        {
            var availabilities = await _context.Availabilities
                .Where(a => !a.Deleted && a.Date >= DateTime.Now)
                .ToListAsync();

            var availabilitiesViewModel = Availabities(availabilities);

            return Ok(availabilitiesViewModel);
        }

        // GET: api/Availability/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Availability>> GetAvailability(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);

            if (availability == null)
            {
                return NotFound();
            }

            return availability;
        }

        // POST: api/Availability To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Availability>> PostAvailability(Availability availability)
        {
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAvailability", new { id = availability.AvailabilityId }, availability);
        }

        // PUT: api/Availability/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAvailability(int id, Availability availability)
        {
            if (id != availability.AvailabilityId)
            {
                return BadRequest();
            }

            _context.Entry(availability).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
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