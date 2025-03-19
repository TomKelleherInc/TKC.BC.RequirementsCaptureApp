using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sedna.Service.Requirements.DTO 
{
    public partial class Topic : BaseDto
    {
        public Topic()
        {
            Requirements = new HashSet<Requirement>();
            SubjectTypeTopics = new HashSet<SubjectTypeTopic>();
            TopicContexts = new HashSet<TopicContext>();
            TopicSearches = new HashSet<TopicSearch>();
        }

        public int TopicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreferredPhrasing { get; set; }
        public string ReferenceKey { get; set; }
        public DateTime CreatedTs { get; set; }
        public DateTime UpdatedTs { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Requirement> Requirements { get; set; }
        public virtual ICollection<SubjectTypeTopic> SubjectTypeTopics { get; set; }
        public virtual ICollection<TopicContext> TopicContexts { get; set; }
        public virtual ICollection<TopicSearch> TopicSearches { get; set; }

       
        private ObservableCollection<TopicSearch> _observerableTopicSearches = new ObservableCollection<TopicSearch>();
    }
}
