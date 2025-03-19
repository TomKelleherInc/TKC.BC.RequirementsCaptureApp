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
    public class RequirementsController : BaseController
    {


        public RequirementsController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }

        // GET: v1/Requirements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.Requirement>>> GetRequirements()
        {
            var efObjs = await DbContext.Requirements
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Requirement>>(efObjs);
        }

        // GET: v1/Requirements
        [HttpGet("contexts")]
        public async Task<ActionResult<IEnumerable<DTO.Requirement>>> GetRequirementsWithContexts()
        {
            var efObjs = await DbContext.Requirements
                .Include(r => r.RequirementContexts)
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Requirement>>(efObjs);
        }

        // GET: v1/Requirements/5
        [HttpGet("{requirement_id}")]
        public async Task<ActionResult<DTO.Requirement>> GetRequirement(int requirement_id)
        {
            var requirement = await DbContext.Requirements
                .Where(r => r.RequirementId == requirement_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (requirement == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Requirement>(requirement);
        }

        [HttpGet("{requirement_id}/contexts")]
        public async Task<ActionResult<DTO.Requirement>> GetRequirementWithContexts(int requirement_id)
        {
            var requirement = await DbContext.Requirements
                .Include(r => r.RequirementContexts)
                .Where(r => r.RequirementId == requirement_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (requirement == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Requirement>(requirement);
        }


        [HttpGet("bysubjects")]
        public async Task<ActionResult<List<DTO.Requirement>>> GetRequirementForSubjectIds([FromQuery]string subject_ids)
        {
            if(subject_ids == null ) { throw new ArgumentNullException(nameof(subject_ids)); }

            var strings = subject_ids.Split(',');
            int[] ids;

            try
            {
                ids = Array.ConvertAll<string, int>(strings, int.Parse);
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(nameof(subject_ids), "Could not convert ids to integers");                
            }


            var requirements = await DbContext.Requirements
                .Include(r => r.RequirementContexts)
                .Where(r => ids.Contains(r.SubjectId))
                .ToListAsync()
                .ConfigureAwait(false);

            if (requirements == null)
            {
                return NotFound();
            }

            return Mapper.Map<List<DTO.Requirement>>(requirements);
        }

        [HttpGet("bysubjects/external_identifiers")]
        public async Task<ActionResult<List<DTO.Requirement>>> GetBySubjectExternalIdentifiers([FromQuery]string external_identifiers)
        {
            if (external_identifiers == null) { throw new ArgumentNullException(nameof(external_identifiers)); }

            var externalIds = external_identifiers.Split(',');

            DbContext.ChangeTracker.LazyLoadingEnabled = true;

            var requirements = await DbContext.Requirements
                .Include(r => r.RequirementContexts)
                .Include(r => r.Topic)
                .Include(r => r.Source)
                .Include(r => r.Subject).ThenInclude(s => s.SubjectType)
                .Where(r => externalIds.Contains(r.Subject.ExternalIdentifier))
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            if (requirements == null)
            {
                return NotFound();
            }

            return Mapper.Map<List<DTO.Requirement>>(requirements);
        }



        // PUT: v1/Requirements/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{requirement_id}")]
        public async Task<IActionResult> PutRequirement(int requirement_id, [FromBody] DTO.Requirement requirement)
        {
           
            if (requirement_id != requirement.RequirementId)
            {
                return BadRequest();
            }


            try
            {
                requirement.Source = null;
                requirement.Subject = null;
                requirement.Source = null;

                var efObj = Mapper.Map<Data.Requirement>(requirement);
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.Requirements.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RequirementExists(requirement_id))
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

        // POST: v1/Requirements
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.Requirement>> PostRequirement([FromBody] DTO.Requirement requirement)
        {
            var efObj = Mapper.Map<Data.Requirement>(requirement);

            try
            {
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.CreatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;

                DbContext.Requirements.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (RequirementExists(requirement.RequirementId))
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

            var newDto = Mapper.Map<DTO.Requirement>(efObj);

            return CreatedAtAction("PostRequirement", new { id = newDto.RequirementId }, newDto);
        }

        // DELETE: v1/Requirements/5
        [HttpDelete("{requirement_id}")]
        public async Task<ActionResult<DTO.Requirement>> DeleteRequirement(int requirement_id)
        {
            var requirement = await DbContext.Requirements.FindAsync(requirement_id);
            if (requirement == null)
            {
                return NotFound();
            }

            DbContext.Requirements.Remove(requirement);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.Requirement>(requirement);
        }



        /// <summary>
        /// Takes the ID of a requirement, and a collection of context_ids,
        /// and discards the RequirementsContexts that are not in the collection
        /// of ids, and creates the ones that are new.
        /// </summary>
        /// <param name="requirement_id"></param>
        /// <param name="context_ids">The set of context Ids that we want RequirementContext records for</param>
        /// <returns></returns>
        [HttpPost("{requirement_id}/resetcontexts")]
        public async Task<ActionResult<List<DTO.RequirementContext>>> ResetContexts(int requirement_id, [FromBody] List<int> context_ids)
        {
            var requirement = await DbContext.Requirements.FindAsync(requirement_id);
            if (requirement == null)
            {
                return NotFound();
            }

            try
            {
                var currentReqContextIds = await DbContext.RequirementContexts.Where(c => c.RequirementId == requirement_id).ToListAsync();

                var currentContextIds = currentReqContextIds.Select(id => id.ContextId).ToList();

                var addIds = context_ids.Except(currentContextIds);  // in the new set, but not in the current ones
                var deleteIds = currentContextIds.Except(context_ids); // In the current ones, but not the new ones.

                var deletes = await DbContext.RequirementContexts.Where(rc => rc.RequirementId == requirement_id && deleteIds.Contains(rc.ContextId)).ToListAsync();

                DbContext.RequirementContexts.RemoveRange(deletes);

                foreach(int addId in addIds)
                {
                    var newRC = new Data.RequirementContext()
                    {
                        ContextId = addId,
                        RequirementId = requirement_id,
                        CreatedBy = ApiClientUsername,
                        UpdatedBy = ApiClientUsername,
                        CreatedTs = DateTime.UtcNow,
                        UpdatedTs = DateTime.UtcNow
                    };

                    DbContext.RequirementContexts.Add(newRC);
                }

                await DbContext.SaveChangesAsync();

                var allReqContexts = DbContext.RequirementContexts.Where(rc => rc.RequirementId == requirement_id).ToList();

                List<DTO.RequirementContext> result = Mapper.Map<List<DTO.RequirementContext>>(allReqContexts);

                return result;

            }
            catch (Exception ex)
            {

                throw;
            }
    

        }





        private bool RequirementExists(int requirement_id)
        {
            return DbContext.Requirements.Any(e => e.RequirementId == requirement_id);
        }
    }
}
