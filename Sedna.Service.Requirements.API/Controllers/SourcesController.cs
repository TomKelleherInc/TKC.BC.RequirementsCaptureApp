using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sedna.Service.Requirements.API;
using Sedna.Service.Requirements.API.Data;

namespace Sedna.Service.Requirements.API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class SourcesController : BaseController
    {
        public SourcesController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            
        }

        // GET: v1/Sources
        [HttpGet]
        [Produces(typeof(IEnumerable<DTO.Source>))]
        public async Task<ActionResult<IEnumerable<DTO.Source>>> GetSource()
        {
            var sources = await DbContext.Sources
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Source>>(sources);
        }

        // GET: v1/Sources
        [HttpGet("/requirements")]
        [Produces(typeof(IEnumerable<DTO.Source>))]
        public async Task<ActionResult<IEnumerable<DTO.Source>>> GetSourceWithRequirements()
        {
            var sources = await DbContext.Sources
                .Include(s => s.Requirements)
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Source>>(sources);
        }

        // GET: v1/Sources/5
        [HttpGet("{source_id}")]
        [Produces(typeof(DTO.Source))]
        public async Task<ActionResult<DTO.Source>> GetSource(int source_id)
        {
            var source = await DbContext.Sources
                .Where(s => s.SourceId == source_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (source == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Source>(source);
        }

        // GET: v1/Sources/5
        [HttpGet("{source_id}/requirements")]
        [Produces(typeof(DTO.Source))]
        public async Task<ActionResult<DTO.Source>> GetSourceWithRequirements(int source_id)
        {
            var source = await DbContext.Sources
                .Include(s => s.Requirements)
                .Where(s => s.SourceId == source_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (source == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Source>(source);
        }

        // GET: v1/Sources/5
        //[HttpGet("{external_identifier}/requirements")]
        //public async Task<ActionResult<IEnumerable<DTO.Requirement>>> GetSourceRequirements(string external_identifier)
        //{
        //    if (string.IsNullOrWhiteSpace(external_identifier)) { throw new ArgumentNullException("No value for external_identifier"); }

        //    external_identifier = external_identifier.ToLower().Trim();

        //    var source = await DbContext.Source
        //        .Where(s => s.ExternalIdentifier.ToLower().Trim() == external_identifier)
        //        .FirstOrDefaultAsync()
        //        .ConfigureAwait(false);

        //    if(source == null) { return NotFound(); }

        //    var reqs = await DbContext.Requirement
        //        .Where(r => r.SourceId == source.SourceId)
        //        .Include(r => r.Subject)
        //        .Include(r => r.RequirementContext)
        //        .Include(r => r.Topic)
        //            .ThenInclude(t => t.TopicSearch)
        //        .AsNoTracking()
        //        .ToListAsync()
        //        .ConfigureAwait(false);

        //    return Mapper.Map<List<DTO.Requirement>>(reqs);
        //}

        [HttpGet("external_identifier/{external_identifier}")]
        public async Task<ActionResult<DTO.Source>> GetSourceByExternalIdentifier(string external_identifier)
        {
            if(string.IsNullOrWhiteSpace(external_identifier)) { throw new ArgumentNullException("No value for external_identifier"); }

            external_identifier = external_identifier.ToLower().Trim();

            var source = await DbContext.Sources.Where(s => s.ExternalIdentifier.ToLower().Trim() == external_identifier).FirstOrDefaultAsync();

            if (source == null)
            {
                return NotFound(source);
            }

            return Mapper.Map<DTO.Source>(source);
        }

        // PUT: v1/Sources/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{source_id}")]
        public async Task<IActionResult> PutSource(int source_id, [FromBody] DTO.Source source)
        {
            if (source_id != source.SourceId)
            {
                return BadRequest();
            }


            try
            {
                Data.Source efObj = Mapper.Map<Data.Source>(source);
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.Sources.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SourceExists(source_id))
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

        // POST: v1/Sources
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.Source>> PostSource([FromBody] DTO.Source source)
        {
            Data.Source efObj = Mapper.Map<Data.Source>(source);

            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.Sources.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (SourceExists(efObj.SourceId))
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

            var newDto = Mapper.Map<DTO.Source>(efObj);

            return CreatedAtAction("PostSource", new { id = newDto.SourceId }, newDto);
        }

        // DELETE: v1/Sources/5
        [HttpDelete("{source_id}")]
        public async Task<ActionResult<DTO.Source>> DeleteSource(int source_id)
        {
            var source = await DbContext.Sources.FindAsync(source_id);
            if (source == null)
            {
                return NotFound();
            }


            DbContext.Sources.Remove(source);
            await DbContext.SaveChangesAsync();

            DTO.Source src = Mapper.Map<DTO.Source>(source);

            return src;
        }

        private bool SourceExists(int source_id)
        {
            return DbContext.Sources.Any(e => e.SourceId == source_id);
        }
    }
}
