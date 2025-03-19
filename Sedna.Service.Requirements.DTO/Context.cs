using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.DTO
{
    public partial class Context : BaseDto
    {
        public Context()
        {
            RequirementContexts = new HashSet<RequirementContext>();
            TopicContexts = new HashSet<TopicContext>();
        }

        public int ContextId { get; set; }
        public string Description { get; set; }
        public string ReferenceKey { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsForExternalAudience { get; set; }

        public virtual ICollection<RequirementContext> RequirementContexts { get; set; }
        public virtual ICollection<TopicContext> TopicContexts { get; set; }
    }
}
