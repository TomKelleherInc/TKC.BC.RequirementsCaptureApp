using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.API.Data
{
    public partial class TopicSearch
    {
        public int TopicSearchId { get; set; }
        public int TopicId { get; set; }
        public string SearchString { get; set; }
        public bool IsWholeWord { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Topic Topic { get; set; }
    }
}
