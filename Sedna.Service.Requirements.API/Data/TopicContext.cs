using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.API.Data
{
    public partial class TopicContext
    {
        public int TopicContextId { get; set; }
        public int TopicId { get; set; }
        public int ContextId { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Context Context { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
