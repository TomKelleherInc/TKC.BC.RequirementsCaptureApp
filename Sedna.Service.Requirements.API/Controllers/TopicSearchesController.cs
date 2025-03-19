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
    public class TopicSearchesController : BaseController
    {

        public TopicSearchesController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {

        }

        // GET: api/TopicSearches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.TopicSearch>>> GetTopicSearch()
        {
            return Mapper.Map<List<DTO.TopicSearch>>(await DbContext.TopicSearches.ToListAsync());
        }

        // GET: api/TopicSearches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DTO.TopicSearch>> GetTopicSearch(int id)
        {
            var topicSearch = await DbContext.TopicSearches.FindAsync(id);

            if (topicSearch == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.TopicSearch>(topicSearch);
        }

        // PUT: api/TopicSearches/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTopicSearch(int id, DTO.TopicSearch topicSearch)
        {
            if (id != topicSearch.TopicSearchId)
            {
                return BadRequest();
            }

            try
            {
                var efObj = Mapper.Map<Data.TopicSearch>(topicSearch);

                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.TopicSearches.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TopicSearchExists(id))
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

        // POST: api/TopicSearches
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.TopicSearch>> PostTopicSearch(DTO.TopicSearch topicSearch)
        {
            var efObj = Mapper.Map<Data.TopicSearch>(topicSearch);

            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.TopicSearches.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (TopicSearchExists(topicSearch.TopicSearchId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var newDto = Mapper.Map<DTO.TopicSearch>(efObj);
            return CreatedAtAction("PostTopicSearch", new { id = newDto.TopicSearchId }, newDto);
        }

        // DELETE: api/TopicSearches/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DTO.TopicSearch>> DeleteTopicSearch(int id)
        {
            var topicSearch = await DbContext.TopicSearches.FindAsync(id);
            if (topicSearch == null)
            {
                return NotFound();
            }

            DbContext.TopicSearches.Remove(topicSearch);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.TopicSearch>(topicSearch);
        }

        private bool TopicSearchExists(int id)
        {
            return DbContext.TopicSearches.Any(e => e.TopicSearchId == id);
        }
    }
}
