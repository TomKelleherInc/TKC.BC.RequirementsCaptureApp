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
    public class RequirementContextsController : BaseController
    {

        public RequirementContextsController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }



        // GET: v1/RequirementContexts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.RequirementContext>>> GetRequirementContext()
        {
            var objs = await DbContext.RequirementContexts.ToListAsync();
            return Mapper.Map<List<DTO.RequirementContext>>(objs);
        }

        // GET: v1/RequirementContexts/5
        [HttpGet("{requirementDbContext_id}")]
        public async Task<ActionResult<DTO.RequirementContext>> GetRequirementContext(int requirementDbContext_id)
        {
            var requirementContext = await DbContext.RequirementContexts.FindAsync(requirementDbContext_id);

            if (requirementContext == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.RequirementContext>(requirementContext);
        }

        // PUT: v1/RequirementContexts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{requirementContext_id}")]
        public async Task<IActionResult> PutRequirementContext(int requirementContext_id, [FromBody] DTO.RequirementContext requirementContext)
        {
            if (requirementContext_id != requirementContext.RequirementContextId)
            {
                return BadRequest();
            }


            try
            {
                var efObj = Mapper.Map<Data.RequirementContext>(requirementContext);
                efObj.UpdatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;

                DbContext.RequirementContexts.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change these on an update/PUT operation
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RequirementContextExists(requirementContext_id))
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




        // POST: v1/RequirementContexts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.RequirementContext>> PostRequirementContext([FromBody] DTO.RequirementContext requirementContext)
        {

            var efObj = Mapper.Map<Data.RequirementContext>(requirementContext);

            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.RequirementContexts.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

            var newDto = Mapper.Map<DTO.RequirementContext>(efObj);

            return CreatedAtAction("PostRequirementContext", new { id = newDto.RequirementContextId }, newDto);
        }

        // DELETE: v1/RequirementContexts/5
        [HttpDelete("{requirementDbContext_id}")]
        public async Task<ActionResult<DTO.RequirementContext>> DeleteRequirementContext(int requirementDbContext_id)
        {
            var requirementContext = await DbContext.RequirementContexts.FindAsync(requirementDbContext_id);
            if (requirementContext == null)
            {
                return NotFound();
            }

            DbContext.RequirementContexts.Remove(requirementContext);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.RequirementContext>(requirementContext);
        }

        private bool RequirementContextExists(int requirementDbContext_id)
        {
            return DbContext.RequirementContexts.Any(e => e.RequirementContextId == requirementDbContext_id);
        }
    }
}
