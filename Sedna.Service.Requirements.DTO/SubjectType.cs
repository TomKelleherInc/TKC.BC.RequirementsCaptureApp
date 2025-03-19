using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.DTO
{
    public partial class SubjectType : BaseDto
    {
        public SubjectType()
        {
            Subjects = new HashSet<Subject>();
            SubjectTypeTopics = new HashSet<SubjectTypeTopic>();
        }

        public int SubjectTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReferenceKey { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<SubjectTypeTopic> SubjectTypeTopics { get; set; }
    }
}
