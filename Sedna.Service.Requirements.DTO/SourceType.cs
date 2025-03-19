using System;
using System.Collections.Generic;

namespace Sedna.Service.Requirements.DTO
{
    public partial class SourceType : BaseDto
    {
        public SourceType()
        {
            Sources = new HashSet<Source>();
        }

        public int SourceTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReferenceKey { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Source> Sources { get; set; }
    }
}
