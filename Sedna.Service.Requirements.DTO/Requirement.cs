using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.DTO
{
    public partial class Requirement : BaseDto
    {
        public Requirement()
        {
            RequirementContexts = new HashSet<RequirementContext>();
        }

        public int RequirementId { get; set; }
        public int? SourceId { get; set; }
        public string SourceText { get; set; }
        public string SourceTextLocation { get; set; }
        public string PreferredPhrasing { get; set; }
        public int? TopicId { get; set; }
        public int SubjectId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ReviewDt { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsApproved { get; set; }

        public virtual Source Source { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual ICollection<RequirementContext> RequirementContexts { get; set; }
    }
}
