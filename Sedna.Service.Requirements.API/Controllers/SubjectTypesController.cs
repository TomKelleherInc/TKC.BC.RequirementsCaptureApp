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
    public class SubjectTypesController : BaseController
    {

        public SubjectTypesController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }

        // GET: api/SubjectTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.SubjectType>>> GetSubjectType()
        {
            var objs = await DbContext.SubjectTypes
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.SubjectType>>(objs);
        }

        [HttpGet("topics")]
        public async Task<ActionResult<IEnumerable<DTO.SubjectType>>> GetSubjectTypeWithTopics()
        {
            var objs = await DbContext.SubjectTypes
                .Include(s => s.SubjectTypeTopics)
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.SubjectType>>(objs);
        }


        // GET: api/SubjectTypes/5
        [HttpGet("{subject_type_id}")]
        public async Task<ActionResult<DTO.SubjectType>> GetSubjectType(int subject_type_id)
        {
            var subjectType = await DbContext.SubjectTypes
                .Where(s => s.SubjectTypeId == subject_type_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (subjectType == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.SubjectType>(subjectType);
        }

        // GET: api/SubjectTypes/5
        [HttpGet("{subject_type_id}/topicdetails")]
        public async Task<ActionResult<List<DTO.Topic>>> GetTopicsDetails(int subject_type_id)
        {

            var topicIds = await DbContext.SubjectTypeTopics
                .Where(stt => stt.SubjectTypeId == subject_type_id)
                .Select(stt => stt.TopicId)
                .ToListAsync()
                .ConfigureAwait(false);

            var objs = await DbContext.Topics
                .Where(t => topicIds.Contains(t.TopicId))
                .Include(t => t.TopicContexts)
                .Include(t => t.TopicSearches)
                .ToListAsync()
                .ConfigureAwait(false);

            var result = Mapper.Map<List<DTO.Topic>>(objs);
            return result;
        }

        // GET: api/SubjectTypes/5
        [HttpGet("{subject_type_id}/details")]
        public async Task<ActionResult<List<DTO.SubjectType>>> GetDetails(int subject_type_id)
        {
            var subjectTypes = await DbContext.SubjectTypes
                .Where(st => st.SubjectTypeId == subject_type_id)
                .Include(st => st.SubjectTypeTopics)
                .ToListAsync()
                .ConfigureAwait(false);


                 

            return Mapper.Map<List<DTO.SubjectType>>(subjectTypes);
        }

        // PUT: api/SubjectTypes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{subject_type_id}")]
        public async Task<IActionResult> PutSubjectType(int subject_type_id, DTO.SubjectType subjectType)
        {
            if (subject_type_id != subjectType.SubjectTypeId)
            {
                return BadRequest();
            }


            try
            {
                var efObj = Mapper.Map<Data.SubjectType>(subjectType);
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.SubjectTypes.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SubjectTypeExists(subject_type_id))
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

        // POST: api/SubjectTypes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.SubjectType>> PostSubjectType(DTO.SubjectType subjectType)
        {
            var efObj = Mapper.Map<Data.SubjectType>(subjectType);

            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.SubjectTypes.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (SubjectTypeExists(subjectType.SubjectTypeId))
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

            var newDto = Mapper.Map<DTO.SubjectType>(efObj);
            return CreatedAtAction("PostSubjectType", new { id = newDto.SubjectTypeId }, newDto);
        }

        // DELETE: api/SubjectTypes/5
        [HttpDelete("{subject_type_id}")]
        public async Task<ActionResult<DTO.SubjectType>> DeleteSubjectType(int subject_type_id)
        {
            var subjectType = await DbContext.SubjectTypes.FindAsync(subject_type_id);
            if (subjectType == null)
            {
                return NotFound();
            }

            DbContext.SubjectTypes.Remove(subjectType);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.SubjectType>(subjectType);
        }

        private bool SubjectTypeExists(int id)
        {
            return DbContext.SubjectTypes.Any(e => e.SubjectTypeId == id);
        }
    }
}
