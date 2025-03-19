using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.API.Data
{
    public partial class Source
    {
        public Source()
        {
            Requirements = new HashSet<Requirement>();
        }

        public int SourceId { get; set; }
        public string Name { get; set; }
        public int? SourceTypeId { get; set; }
        public string ExternalIdentifier { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual SourceType SourceType { get; set; }
        public virtual ICollection<Requirement> Requirements { get; set; }
    }
}
