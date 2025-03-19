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
    public class ContextsController : BaseController
    { 

        public ContextsController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        { 

        }

        // GET: api/Contexts
        [HttpGet]
        [Produces(typeof(List<DTO.Context>))]
        public async Task<ActionResult<IEnumerable<DTO.Context>>> GetContext()
        {
            var obj = await DbContext.Contexts
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Context>>(obj);
        }

        [HttpGet("topiccontexts")]
        [Produces(typeof(List<DTO.Context>))]
        public async Task<ActionResult<IEnumerable<DTO.Context>>> GetContextWithTopics()
        {
            var obj = await DbContext.Contexts
                .Include(c => c.TopicContexts)
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Context>>(obj);
        }

        // GET: api/Contexts/5
        [HttpGet("{context_id}")]
        [Produces(typeof(DTO.Context))]
        public async Task<ActionResult<DTO.Context>> GetContext(int context_id)
        {
            var context = await DbContext.Contexts
                .Include(c => c.TopicContexts)
                .Where(c => c.ContextId == context_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (context == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Context>(context);
        }


        [HttpGet("{context_id}/topiccontexts")]
        [Produces(typeof(DTO.Context))]
        public async Task<ActionResult<DTO.Context>> GetContextWithContexts(int context_id)
        {
            var context = await DbContext.Contexts
                .Include(c => c.TopicContexts)
                .Where(c => c.ContextId == context_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (context == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Context>(context);
        }

        // PUT: api/Contexts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{context_id}")]
        public async Task<IActionResult> PutContext(int context_id, [FromBody] DTO.Context context)
        {
            if (context_id != context.ContextId)
            {
                return BadRequest();
            }


            try
            {
                Data.Context efObj = Mapper.Map<Data.Context>(context);
                efObj.UpdatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;

                DbContext.Contexts.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change these on an update/PUT operation
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ContextExists(context_id))
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

        // POST: api/Contexts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.Context>> PostContext([FromBody] DTO.Context context)
        {
            var efObj = Mapper.Map<Data.Context>(context);

            try
            {
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedTs = DateTime.UtcNow;
                efObj.CreatedBy = ApiClientUsername;
                efObj.UpdatedBy = ApiClientUsername;

                DbContext.Contexts.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            var newDTO = Mapper.Map<DTO.Context>(efObj);

            return CreatedAtAction("PostContext", new { id = newDTO.ContextId }, newDTO);
        }

        // DELETE: api/Contexts/5
        [HttpDelete("{context_id}")]
        public async Task<ActionResult<DTO.Context>> DeleteContext(int context_id)
        {
            var context = await DbContext.Contexts.FindAsync(context_id);
            if (context == null)
            {
                return NotFound();
            }

            DbContext.Contexts.Remove(context);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.Context>(context);
        }

        private bool ContextExists(int context_id)
        {
            return DbContext.Contexts.Any(e => e.ContextId == context_id);
        }
    }
}
