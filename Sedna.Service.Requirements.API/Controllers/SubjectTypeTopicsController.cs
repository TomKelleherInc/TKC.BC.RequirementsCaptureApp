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
    public class SubjectTypeTopicsController : BaseController
    {


        public SubjectTypeTopicsController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }

        // GET: api/SubjectTypeTopics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.SubjectTypeTopic>>> GetSubjectTypeTopic()
        {
            var objs = await DbContext.SubjectTypeTopics.ToListAsync().ConfigureAwait(false);
            return Mapper.Map<List<DTO.SubjectTypeTopic>>(objs);
        }

        // GET: api/SubjectTypeTopics/5
        [HttpGet("{subject_type_topic_id}")]
        public async Task<ActionResult<DTO.SubjectTypeTopic>> GetSubjectTypeTopic(int subject_type_topic_id)
        {
            var subjectTypeTopic = await DbContext.SubjectTypeTopics.FindAsync(subject_type_topic_id);

            if (subjectTypeTopic == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.SubjectTypeTopic>(subjectTypeTopic);
        }

        // GET: api/SubjectTypeTopics/5
        [HttpGet("{subject_type_topic_id}/topics")]
        public async Task<ActionResult<DTO.SubjectTypeTopic>> GetSubjectTypeTopics(int subject_type_topic_id)
        {
            var subjectTypeTopic = await DbContext.SubjectTypeTopics
                .Include(s => s.Topic)
                .ThenInclude(s => s.TopicSearches)
                .Where(s => s.SubjectTypeTopicId == subject_type_topic_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (subjectTypeTopic == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.SubjectTypeTopic>(subjectTypeTopic);
        }

        // PUT: api/SubjectTypeTopics/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubjectTypeTopic(int id, DTO.SubjectTypeTopic subjectTypeTopic)
        {
            if (id != subjectTypeTopic.SubjectTypeTopicId)
            {
                return BadRequest();
            }


            try
            {
                var efObj = Mapper.Map<Data.SubjectTypeTopic>(subjectTypeTopic);
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.SubjectTypeTopics.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SubjectTypeTopicExists(id))
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

        // POST: api/SubjectTypeTopics
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.SubjectTypeTopic>> PostSubjectTypeTopic(DTO.SubjectTypeTopic subjectTypeTopic)
        {
            var efObj = Mapper.Map<Data.SubjectTypeTopic>(subjectTypeTopic);

            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.SubjectTypeTopics.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (SubjectTypeTopicExists(subjectTypeTopic.SubjectTypeTopicId))
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

            var newDto = Mapper.Map<DTO.SubjectTypeTopic>(efObj);
            return CreatedAtAction("PostSubjectTypeTopic", new { id = newDto.SubjectTypeTopicId }, newDto);
        }

        // DELETE: api/SubjectTypeTopics/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DTO.SubjectTypeTopic>> DeleteSubjectTypeTopic(int id)
        {
            var subjectTypeTopic = await DbContext.SubjectTypeTopics.FindAsync(id);
            if (subjectTypeTopic == null)
            {
                return NotFound();
            }

            DbContext.SubjectTypeTopics.Remove(subjectTypeTopic);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.SubjectTypeTopic>(subjectTypeTopic);
        }

        private bool SubjectTypeTopicExists(int id)
        {
            return DbContext.SubjectTypeTopics.Any(e => e.SubjectTypeTopicId == id);
        }
    }
}
