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
    public class TopicContextsController : BaseController
    {


        public TopicContextsController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }

        // GET: api/Topics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.TopicContext>>> GetTopic()
        {
            return Mapper.Map<List<DTO.TopicContext>>(await DbContext.Topics.ToListAsync());
        }

        // GET: api/Topics/5
        [HttpGet("{topic_context_id}")]
        public async Task<ActionResult<DTO.TopicContext>> GetTopic(int topic_context_id)
        {
            var topic = await DbContext.Topics.FindAsync(topic_context_id);

            if (topic == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.TopicContext>(topic);
        }

        // PUT: api/Topics/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{topic_context_id}")]
        public async Task<IActionResult> PutTopic(int topic_context_id, [FromBody] DTO.TopicContext topicContext)
        {
            if (topic_context_id != topicContext.TopicContextId)
            {
                return BadRequest();
            }


            try
            {
                var efObj = Mapper.Map<Data.TopicContext>(topicContext);
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.TopicContexts.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TopicContextExists(topic_context_id))
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
        public async Task<ActionResult<DTO.TopicContext>> PostTopicContext([FromBody] DTO.TopicContext topicContext)
        {

            var efObj = Mapper.Map<Data.TopicContext>(topicContext);

            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.TopicContexts.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (TopicContextExists(topicContext.TopicContextId))
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


            var newDto = Mapper.Map<DTO.TopicContext>(efObj);
            return CreatedAtAction("PostTopicContext", new { id = newDto.TopicId }, newDto);
        }

        // DELETE: api/Topics/5
        [HttpDelete("{topic_context_id}")]
        public async Task<ActionResult<DTO.TopicContext>> DeleteTopic(int topic_context_id)
        {
            var topicContext = await DbContext.TopicContexts.FindAsync(topic_context_id);
            if (topicContext == null)
            {
                return NotFound();
            }

            DbContext.TopicContexts.Remove(topicContext);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.TopicContext>(topicContext);
        }

        private bool TopicContextExists(int topic_context_id)
        {
            return DbContext.TopicContexts.Any(e => e.TopicContextId == topic_context_id);
        }
    }
}
