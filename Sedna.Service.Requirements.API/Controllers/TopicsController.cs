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
    public class TopicsController : BaseController
    {


        public TopicsController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }

        // GET: api/Topics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.Topic>>> GetTopic()
        {
            var objs = await DbContext.Topics
                .ToListAsync()
                .ConfigureAwait(false);

            return Mapper.Map<List<DTO.Topic>>(objs);
        }

        //// GET: api/Topics
        //[HttpGet("contextandsearch")]
        //public async Task<ActionResult<IEnumerable<DTO.Topic>>> GetTopicWithContextAndSearch()
        //{
        //    var objs = await DbContext.Topic
        //        .Include(t => t.TopicContext)
        //        .Include(t => t.TopicSearch)
        //        .ToListAsync()
        //        .ConfigureAwait(false);

        //    return Mapper.Map<List<DTO.Topic>>(objs);
        //}

             



        // GET: api/Topics/5
        [HttpGet("{topic_id}")]
        public async Task<ActionResult<DTO.Topic>> GetTopic(int topic_id)
        {
            var topic = await DbContext.Topics
                .Where(t => t.TopicId == topic_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (topic == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Topic>(topic);
        }

        // GET: api/Topics/5
        [HttpGet("{topic_id}/contextandsearch")]
        public async Task<ActionResult<DTO.Topic>> GetTopicWithContextAndSearch(int topic_id)
        {
            var topic = await DbContext.Topics
                .Include(t => t.TopicContexts)
                .Include(t => t.SubjectTypeTopics)
                .Include(t => t.TopicSearches)
                .Where(t => t.TopicId == topic_id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (topic == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.Topic>(topic);
        }

        // PUT: api/Topics/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{topic_id}")]
        public async Task<IActionResult> PutTopic(int topic_id, [FromBody] DTO.Topic topic)
        {
            if (topic_id != topic.TopicId)
            {
                return BadRequest();
            }


            try
            {
                var efObj = Mapper.Map<Data.Topic>(topic);

                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.Topics.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TopicExists(topic_id))
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

        // POST: api/Topics
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.Topic>> PostTopic([FromBody] DTO.Topic topic)
        {
            var efObj = Mapper.Map<Data.Topic>(topic);

            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.Topics.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                if (TopicExists(topic.TopicId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var newDto = Mapper.Map<DTO.Topic>(efObj);
            return CreatedAtAction("PostTopic", new { id = newDto.TopicId }, newDto);
        }

        // DELETE: api/Topics/5
        [HttpDelete("{topic_id}")]
        public async Task<ActionResult<DTO.Topic>> DeleteTopic(int topic_id)
        {
            var topic = await DbContext.Topics.FindAsync(topic_id);
            if (topic == null)
            {
                return NotFound();
            }

            DbContext.Topics.Remove(topic);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.Topic>(topic);
        }

        private bool TopicExists(int topic_id)
        {
            return DbContext.Topics.Any(e => e.TopicId == topic_id);
        }
    }
}
