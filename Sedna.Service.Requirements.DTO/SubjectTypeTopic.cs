using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.DTO
{
    public partial class SubjectTypeTopic : BaseDto
    {
        public int SubjectTypeTopicId { get; set; }
        public int SubjectTypeId { get; set; }
        public int TopicId { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual SubjectType SubjectType { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
