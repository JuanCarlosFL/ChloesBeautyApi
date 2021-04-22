﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChloesBeauty.API.Models;
using System;
using ChloesBeauty.API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ChloesBeauty.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PersonController : ControllerBase
    {
        #region Private Fields

        private readonly ChloesBeautyContext _context;

        #endregion Private Fields

        #region Public Constructors

        public PersonController(ChloesBeautyContext context)
        {
            _context = context;
        }

        #endregion Public Constructors

        #region Private Methods

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.PersonId == id);
        }

        #endregion Private Methods

        #region Public Methods

        // DELETE: api/Person/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound(false);
            }

            person.Deleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        // GET: api/Person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // GET: api/Person
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Persons.Where(p => !p.Deleted).ToListAsync();
        }

        // POST: api/Person To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            var personFound = await _context.Persons.Where(p => p.Email == person.Email).FirstOrDefaultAsync();

            if (personFound != null)
                return BadRequest(false);

            person.Deleted = false;
            person.ModifiedDate = DateTime.Now;
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            var user = new User
            {
                UserName = person.Email,
                Password = Functions.Encrypt("Temporal"),
                PersonId = person.PersonId,
                IsActive = true,
                ModifiedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userRole = new UsersRole
            {
                UserId = person.PersonId,
                RoleId = (byte)Enums.Roles.Client,
                ModifiedDate = DateTime.Now
            };

            _context.UsersRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        // PUT: api/Person/5 To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.PersonId)
            {
                return BadRequest();
            }

            person.ModifiedDate = DateTime.Now;
            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
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