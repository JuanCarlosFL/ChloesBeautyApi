﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChloesBeauty.API.Models;
using ChloesBeauty.API.ViewModels;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ChloesBeauty.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TreatmentController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors

        public TreatmentController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool TreatmentExists(int id)
        {
            return _context.Treatments.Any(e => e.TreatmentId == id);
        }

        #endregion Private Methods

        #region Public Methods

        // DELETE: api/Treatment/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTreatment(int id)
        {
            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }

            treatment.Deleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        // GET: api/Treatment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Treatment>> GetTreatment(int id)
        {
            var treatment = await _context.Treatments.FindAsync(id);

            if (treatment == null)
            {
                return NotFound();
            }

            return treatment;
        }

        // GET: api/Treatment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Treatment>>> GetTreatments()
        {
            return await _context.Treatments.Where(t => !t.Deleted).ToListAsync();
        }

        [HttpGet]
        [Route("GetTreatmentsForAppointment")]
        public async Task<ActionResult<IEnumerable<TreatmentsForAppointmentViewModel>>> GetTreatmentsForAppointment()
        {
            return await _context.Treatments
                .Where(t => !t.Deleted)
                .Select(t => new TreatmentsForAppointmentViewModel { Id = t.TreatmentId, Name = t.Name, Duration = t.Duration, Price = t.Price, Points = t.Points })
                .ToListAsync();
        }

        // POST: api/Treatment To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Treatment>> PostTreatment(Treatment treatment)
        {
            treatment.Deleted = false;
            treatment.ModifiedDate = DateTime.Now;
            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTreatment", new { id = treatment.TreatmentId }, treatment);
        }

        // PUT: api/Treatment/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTreatment(int id, Treatment treatment)
        {
            if (id != treatment.TreatmentId)
            {
                return BadRequest();
            }

            treatment.ModifiedDate = DateTime.Now;
            _context.Entry(treatment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TreatmentExists(id))
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