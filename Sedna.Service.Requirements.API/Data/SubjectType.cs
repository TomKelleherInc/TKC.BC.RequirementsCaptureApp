using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.API.Data
{
    public partial class SubjectType
    {
        public SubjectType()
        {
            SubjectTypeTopics = new HashSet<SubjectTypeTopic>();
            Subjects = new HashSet<Subject>();
        }

        public int SubjectTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReferenceKey { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<SubjectTypeTopic> SubjectTypeTopics { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
