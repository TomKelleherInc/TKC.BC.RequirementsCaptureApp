using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sedna.Service.Requirements.API.Data;

namespace Sedna.Service.Requirements.API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class SubjectsController : BaseController
    {

        public SubjectsController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.Subject>>> GetSubject()
        {

            var objs = await DbContext.Subjects
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Subject>>( objs );
        }

        // GET: api/Subjects/5
        [HttpGet("{subject_id}")]
        public async Task<ActionResult<DTO.Subject>> GetSubject(int subject_id)
        {
            var subject = await DbContext.Subjects.FindAsync(subject_id);

            if (subject == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Subject>(subject);
        }


        [HttpGet("{subject_id}/requirements")]
        public async Task<ActionResult<DTO.Subject>> GetSubjectWithRequirements(int subject_id)
        {
            var subject = await DbContext.Subjects
                .Where(s => s.SubjectId == subject_id)
                .Include(s => s.Requirements)
                    .ThenInclude(r => r.RequirementContexts)
                .Include(s => s.SubjectType)
                .FirstOrDefaultAsync()
                .ConfigureAwait(true);

            return Mapper.Map<DTO.Subject>(subject);
        }


        [HttpGet("external_identifier/{external_identifier}/requirements")]
        public async Task<ActionResult<DTO.Subject>> GetByExternalIdentifierWithRequirements(string external_identifier)
        {
            external_identifier = external_identifier.ToLower().Trim();

            var subject = await DbContext.Subjects
                .Where(s => s.ExternalIdentifier.ToLower().Trim() == external_identifier)
                .Include(s => s.Requirements)
                    .ThenInclude(r => r.RequirementContexts)
                .Include(s => s.SubjectType)
                .FirstOrDefaultAsync()
                .ConfigureAwait(true);

            if(subject == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Subject>(subject);
        }



        // PUT: api/Subjects/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{subject_id}")]
        public async Task<IActionResult> PutSubject(int subject_id, [FromBody] DTO.Subject subject)
        {
            if (subject_id != subject.SubjectId)
            {
                return BadRequest();
            }


            try
            {
                var efObj = Mapper.Map<Data.Subject>(subject);
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.Subjects.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SubjectExists(subject_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Subjects
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.Subject>> PostSubject([FromBody] DTO.Subject subject)
        {
            var efObj = Mapper.Map<Data.Subject>(subject);


            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.Subjects.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (SubjectExists(efObj.SubjectId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw;
            }


            var newDto = Mapper.Map<DTO.Subject>(efObj);
            return CreatedAtAction("PostSubject", new { id = newDto.SubjectId }, newDto);
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{subject_id}")]
        public async Task<ActionResult<DTO.Subject>> DeleteSubject(int subject_id)
        {
            var subject = await DbContext.Subjects.FindAsync(subject_id);
            if (subject == null)
            {
                return NotFound();
            }

            DbContext.Subjects.Remove(subject);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.Subject>(subject);
        }

        private bool SubjectExists(int subject_id)
        {
            return DbContext.Subjects.Any(e => e.SubjectId == subject_id);
        }
    }
}
