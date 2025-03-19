using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Sedna.Service.Requirements.API.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Data.Context, DTO.Context>();
            CreateMap<Data.Requirement, DTO.Requirement>();
            CreateMap<Data.Requirement, DTO.Requirement>();
            CreateMap<Data.RequirementContext, DTO.RequirementContext>();
            CreateMap<Data.Source, DTO.Source>();
            CreateMap<Data.SourceType, DTO.SourceType>();
            CreateMap<Data.Subject, DTO.Subject>();
            CreateMap<Data.SubjectType, DTO.SubjectType>();
            CreateMap<Data.SubjectTypeTopic, DTO.SubjectTypeTopic>();
            CreateMap<Data.Topic, DTO.Topic>()
                .AfterMap((s, d) => {
                    foreach (var c in d.TopicSearches)
                        c.Topic = d;
                    });
            CreateMap<Data.TopicContext, DTO.TopicContext>();
            CreateMap<Data.TopicSearch, DTO.TopicSearch>();


            CreateMap<DTO.Context, Data.Context>();
            CreateMap<DTO.Requirement, Data.Requirement>();
            CreateMap<DTO.Requirement, Data.Requirement>();
            CreateMap<DTO.RequirementContext, Data.RequirementContext>();
            CreateMap<DTO.Source, Data.Source>();
            CreateMap<DTO.SourceType, Data.SourceType>();
            CreateMap<DTO.Subject, Data.Subject>();
            CreateMap<DTO.SubjectType, Data.SubjectType>();
            CreateMap<DTO.SubjectTypeTopic, Data.SubjectTypeTopic>();
            CreateMap<DTO.Topic, Data.Topic>()
                .AfterMap((s, d) => {
                    foreach (var c in d.TopicSearches)
                        c.Topic = d;
                });
            CreateMap<DTO.TopicContext, Data.TopicContext>();
            CreateMap<DTO.TopicSearch, Data.TopicSearch>();
        }
    }
}
