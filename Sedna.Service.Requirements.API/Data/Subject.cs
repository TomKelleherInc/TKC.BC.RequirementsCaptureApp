using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.API.Data
{
    public partial class Subject
    {
        public Subject()
        {
            Requirements = new HashSet<Requirement>();
        }

        public int SubjectId { get; set; }
        public string ExternalIdentifier { get; set; }
        public int SubjectTypeId { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual SubjectType SubjectType { get; set; }
        public virtual ICollection<Requirement> Requirements { get; set; }
    }
}
