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
    public class SourceTypesController : BaseController
    {

        public SourceTypesController(RequirementsDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            
        }

        // GET: v1/SourceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO.SourceType>>> GetSourceType()
        {
            return Mapper.Map<List<DTO.SourceType>>(await DbContext.SourceTypes.ToListAsync());
        }

        // GET: v1/SourceTypes/5
        [HttpGet("{source_type_id}")]
        public async Task<ActionResult<DTO.SourceType>> GetSourceType(int source_type_id)
        {
            var sourceType = await DbContext.SourceTypes.FindAsync(source_type_id);

            if (sourceType == null)
            {
                return NotFound();
            }

            return Mapper.Map<DTO.SourceType>(sourceType);
        }

        // PUT: v1/SourceTypes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{source_type_id}")]
        public async Task<IActionResult> PutSourceType(int source_type_id, [FromBody] DTO.SourceType sourceType)
        {
            if (source_type_id != sourceType.SourceTypeId)
            {
                return BadRequest();
            }

            

            try
            {
                var efObj = Mapper.Map<Data.SourceType>(sourceType);
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.SourceTypes.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Modified;

                // don't change the created-info in a standard PUT.
                DbContext.Entry(efObj).Property("CreatedBy").IsModified = false;
                DbContext.Entry(efObj).Property("CreatedTs").IsModified = false;
                 
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SourceTypeExists(source_type_id))
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

        // POST: v1/SourceTypes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DTO.SourceType>> PostSourceType([FromBody] DTO.SourceType sourceType)
        {
            var efObj = Mapper.Map<Data.SourceType>(sourceType);


            try
            {
                efObj.CreatedBy = ApiClientUsername;
                efObj.CreatedTs = DateTime.UtcNow;
                efObj.UpdatedBy = ApiClientUsername;
                efObj.UpdatedTs = DateTime.UtcNow;

                DbContext.SourceTypes.Attach(efObj);
                DbContext.Entry(efObj).State = EntityState.Added;

                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (SourceTypeExists(sourceType.SourceTypeId))
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

            var newDto = Mapper.Map<DTO.SourceType>(efObj);

            return CreatedAtAction("PostSourceType", new { id = newDto.SourceTypeId }, newDto);
        }

        // DELETE: v1/SourceTypes/5
        [HttpDelete("{source_type_id}")]
        public async Task<ActionResult<DTO.SourceType>> DeleteSourceType(int source_type_id)
        {
            var sourceType = await DbContext.SourceTypes.FindAsync(source_type_id);
            if (sourceType == null)
            {
                return NotFound();
            }

            DbContext.SourceTypes.Remove(sourceType);
            await DbContext.SaveChangesAsync();

            return Mapper.Map<DTO.SourceType>(sourceType);
        }

        private bool SourceTypeExists(int source_type_id)
        {
            return DbContext.SourceTypes.Any(e => e.SourceTypeId == source_type_id);
        }
    }
}
