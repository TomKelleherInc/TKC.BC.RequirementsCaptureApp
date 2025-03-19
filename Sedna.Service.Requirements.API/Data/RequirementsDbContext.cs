using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sedna.Service.Requirements.API.Data
{
    public partial class RequirementsDbContext : DbContext
    {
        public RequirementsDbContext()
        {
        }

        public RequirementsDbContext(DbContextOptions<RequirementsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Context> Contexts { get; set; }
        public virtual DbSet<Requirement> Requirements { get; set; }
        public virtual DbSet<RequirementContext> RequirementContexts { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<SourceType> SourceTypes { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<SubjectType> SubjectTypes { get; set; }
        public virtual DbSet<SubjectTypeTopic> SubjectTypeTopics { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<TopicContext> TopicContexts { get; set; }
        public virtual DbSet<TopicSearch> TopicSearches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Database=Requirements;Username=tomkelleher;Password=tk)local(postgres;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Context>(entity =>
            {
                entity.ToTable("context");

                entity.HasComment(@"The Brighton Cromwell workflow steps and contexts 
in which requirements might be relevant or impact 
activity/decisions/etc.  For example, ""PreAward.Rfq.Creation""
and ""PostAward.PurchaseOrder.Creation.""");

                entity.HasIndex(e => e.ContextId)
                    .HasName("context_context_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ReferenceKey)
                    .HasName("context_reference_tag_uindex")
                    .IsUnique();

                entity.Property(e => e.ContextId)
                    .HasColumnName("context_id")
                    .HasDefaultValueSql("nextval('context_context_seq'::regclass)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(250)
                    .HasComment(@"A description of the topic to better 
explain how it will impact our work
processes.");

                entity.Property(e => e.IsForExternalAudience)
                    .HasColumnName("is_for_external_audience")
                    .HasComment(@"Indicaes that this usage-context will include
non-Brighton Cromwell readers, such as vendors
and customers.  Therefore, the author of the
note or requirement must use appropriate phrasing
and thought in their phrasing.");

                entity.Property(e => e.ReferenceKey)
                    .IsRequired()
                    .HasColumnName("reference_key")
                    .HasMaxLength(75)
                    .HasComment(@"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Description field. ");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");
            });

            modelBuilder.Entity<Requirement>(entity =>
            {
                entity.ToTable("requirement");

                entity.HasComment(@"The specifics of a given requirement, including 
what it pertains to (a given Opportunity or Line), 
the source it came from (e.g., a DIBBS solicitation 
PDF), what the original source of the requirement said
and what re-phrasing we want to use instead for clarity.	");

                entity.HasIndex(e => e.RequirementId)
                    .HasName("requirement_requirement_id_index");

                entity.Property(e => e.RequirementId).HasColumnName("requirement_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment(@"The network username of the person who
created the record. ");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())")
                    .HasComment(@"Timestamp that this record was creatd. This
is provided by the server on creation. ");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasComment(@"Indicates whether this record should be
used, or if it has been ""retired.""  Recurring
Notes especially may be retired over time.
(Not to be confused with ""is_approved."") ");

                entity.Property(e => e.IsApproved)
                    .HasColumnName("is_approved")
                    .HasComment(@"For some (especially Recurring Notes) 
a qualified person must review the requirement
and flag it as approved before it can be used.");

                entity.Property(e => e.PreferredPhrasing)
                    .HasColumnName("preferred_phrasing")
                    .HasComment(@"The phrasing that we prefer to use in our
workflow process, as opposed (for instance)
from the boilerplate text provided in DIBBS
PDF files. ");

                entity.Property(e => e.ReviewDt)
                    .HasColumnName("review_dt")
                    .HasColumnType("date")
                    .HasComment(@"The planned (future) date for reviewing
this record for validity.  Used mainly with
Recurring Notes, where a future change of
circumstance is anticipated. ");

                entity.Property(e => e.SourceId)
                    .HasColumnName("source_id")
                    .HasComment(@"The ID of the Source table record that
this record was captured from. ");

                entity.Property(e => e.SourceText)
                    .HasColumnName("source_text")
                    .HasComment(@"The original text (if any) that this
record's ""preferred phrasing"" was derived
from.  This is more important for requirements
pulled from source PDFs than for Recurring Notes
which are culled from real-life experience. ");

                entity.Property(e => e.SourceTextLocation)
                    .HasColumnName("source_text_location")
                    .HasComment(@"The location in the source document (if any)
where the source_text was located.  For
example, ""page:17"" of the source PDF.  Used
to provide traceability back to the
original text. ");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject_id")
                    .HasComment(@"The external ID of the thing that this requirement pertains to.
e.g., the opportunity_id, or opportunity_line_id, etc.
Note that this database does not keep a list of these,
nor know what such entities are.  The calling service
needs to know what it's looking for based on its own
data and organization. ");

                entity.Property(e => e.TopicId)
                    .HasColumnName("topic_id")
                    .HasComment(@"The ID of the Topic record that this
requirement is associated with. ");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment(@"The network username of the person
who last updated this record.");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())")
                    .HasComment(@"Timestamp that this record was last updated. This
is provided by the server on creation. ");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.Requirements)
                    .HasForeignKey(d => d.SourceId)
                    .HasConstraintName("requirement_source_source_id_fk");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Requirements)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("requirement_subject_subject_id_fk");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Requirements)
                    .HasForeignKey(d => d.TopicId)
                    .HasConstraintName("requirement_topic_topic_id_fk");
            });

            modelBuilder.Entity<RequirementContext>(entity =>
            {
                entity.ToTable("requirement_context");

                entity.HasComment(@"List of the circumstances or contexts in which 
a particular requirement is meaningful or relevant.  
For instance, certain packaging requirements can
affect pricing, so ""PreAward.Bid.Creation"" would be a
relevant context, as would ""Warehouse.Packaging.Alert.""");

                entity.HasIndex(e => new { e.ContextId, e.RequirementId })
                    .HasName("requirement_context_context_id_requirement_id_uindex")
                    .IsUnique();

                entity.Property(e => e.RequirementContextId).HasColumnName("requirement_context_id");

                entity.Property(e => e.ContextId).HasColumnName("context_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.RequirementId).HasColumnName("requirement_id");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.HasOne(d => d.Context)
                    .WithMany(p => p.RequirementContexts)
                    .HasForeignKey(d => d.ContextId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("requirement_context_context_context_fk");

                entity.HasOne(d => d.Requirement)
                    .WithMany(p => p.RequirementContexts)
                    .HasForeignKey(d => d.RequirementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("requirement_context_requirement_requirement_id_fk");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.ToTable("source");

                entity.HasComment(@"The source of the requirement.  This can be 
a specific solicitation PDF, a Word document,
an Excel file, or a person.");

                entity.HasIndex(e => e.ExternalIdentifier)
                    .HasName("source_external_identifier_uindex")
                    .IsUnique();

                entity.Property(e => e.SourceId).HasColumnName("source_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.ExternalIdentifier)
                    .IsRequired()
                    .HasColumnName("external_identifier");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.SourceTypeId).HasColumnName("source_type_id");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.HasOne(d => d.SourceType)
                    .WithMany(p => p.Sources)
                    .HasForeignKey(d => d.SourceTypeId)
                    .HasConstraintName("source_source_type_source_type_id_fk");
            });

            modelBuilder.Entity<SourceType>(entity =>
            {
                entity.ToTable("source_type");

                entity.HasComment("Different source types, like PDF, URL, a Sedna User, or other.");

                entity.HasIndex(e => e.ReferenceKey)
                    .HasName("source_type_reference_key_uindex")
                    .IsUnique();

                entity.Property(e => e.SourceTypeId).HasColumnName("source_type_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.ReferenceKey)
                    .IsRequired()
                    .HasColumnName("reference_key")
                    .HasComment(@"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Name field. ");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subject");

                entity.HasComment(@"The ""subject"" of the requirement; what it 
pertains to.  The Opportunity, the 
Opportunity Line, the Award or Award 
Line, etc.");

                entity.HasIndex(e => e.ExternalIdentifier)
                    .HasName("subject_external_identifier_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.SubjectId)
                    .HasName("subject_subject_id_uindex")
                    .IsUnique();

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject_id")
                    .HasComment("The autoincremented id for the record");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasDefaultValueSql("''::text")
                    .HasComment("A human-readable description, for UI display.  For example \"NSN ##### (Oil Filter)\"");

                entity.Property(e => e.ExternalIdentifier)
                    .IsRequired()
                    .HasColumnName("external_identifier")
                    .HasComment("The external id that the calling routine knows this subject by. For an Opportunity, likely the \"opportunity_id\"; for an Opportunity Line, the \"opportunity_line_id.\"");

                entity.Property(e => e.SubjectTypeId)
                    .HasColumnName("subject_type_id")
                    .HasComment("The type of entity this is; an Opportunity, an Opportunity Line, etc.");

                entity.Property(e => e.Tag)
                    .HasColumnName("tag")
                    .HasColumnType("json")
                    .HasComment("An optional field that permits descriptions of the entity, in key-value pairs.");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.HasOne(d => d.SubjectType)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.SubjectTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subject_entity_type_entity_type_id_fk");
            });

            modelBuilder.Entity<SubjectType>(entity =>
            {
                entity.ToTable("subject_type");

                entity.HasComment(@"Things we can attach requirements to: 
Opportunities, Opp Lines, Awards, Award 
Lines, Vendors, Customers, Parts, 
Vendor-Parts, etc.");

                entity.Property(e => e.SubjectTypeId).HasColumnName("subject_type_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.ReferenceKey)
                    .HasColumnName("reference_key")
                    .HasComment(@"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Name field. ");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");
            });

            modelBuilder.Entity<SubjectTypeTopic>(entity =>
            {
                entity.ToTable("subject_type_topic");

                entity.HasComment(@"A cross reference of the topics that are 
pertinent to each entity type.  For instance, 
an Opportunity can have requirements around 
""DeliveryDate"" while an OpportunityLine 
can have them about ""ShelfLife.""");

                entity.HasIndex(e => e.SubjectTypeTopicId)
                    .HasName("subject_type_topic_subject_topic_id_uindex")
                    .IsUnique();

                entity.Property(e => e.SubjectTypeTopicId)
                    .HasColumnName("subject_type_topic_id")
                    .HasDefaultValueSql("nextval('entity_concept_entity_concept_id_seq'::regclass)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.SubjectTypeId).HasColumnName("subject_type_id");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.HasOne(d => d.SubjectType)
                    .WithMany(p => p.SubjectTypeTopics)
                    .HasForeignKey(d => d.SubjectTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subject_topic_subject_subject_id_fk");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.SubjectTypeTopics)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("subject_topic_topic_id_fk");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("topic");

                entity.HasComment(@"The overarching concept the requirement is 
in service to; e.g., ""Shelf Life,"" 
""Mercury"", ""Hazmat""");

                entity.HasIndex(e => e.Name)
                    .HasName("topic_name_uindex")
                    .IsUnique();

                entity.Property(e => e.TopicId)
                    .HasColumnName("topic_id")
                    .HasDefaultValueSql("nextval('concept_concept_id_seq'::regclass)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.PreferredPhrasing)
                    .HasColumnName("preferred_phrasing")
                    .HasComment("The desired paraphrasing/layman's terminloogy for this concept (e.g., \"Vendor will not use Child Labor\")");

                entity.Property(e => e.ReferenceKey)
                    .IsRequired()
                    .HasColumnName("reference_key")
                    .HasComment(@"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Name field. ");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");
            });

            modelBuilder.Entity<TopicContext>(entity =>
            {
                entity.ToTable("topic_context");

                entity.HasComment(@"Cross-reference of the contexts that are 
typically impacted by requirements associated 
with each topic.  These provide a templated 
pattern of default contexts when adding new 
requirements for that topic.");

                entity.HasIndex(e => e.TopicContextId)
                    .HasName("topic_context_topic_context_id_uindex")
                    .IsUnique();

                entity.Property(e => e.TopicContextId).HasColumnName("topic_context_id");

                entity.Property(e => e.ContextId).HasColumnName("context_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Context)
                    .WithMany(p => p.TopicContexts)
                    .HasForeignKey(d => d.ContextId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("topic_context_context_context_fk");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.TopicContexts)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("topic_context_topic_topic_id_fk");
            });

            modelBuilder.Entity<TopicSearch>(entity =>
            {
                entity.ToTable("topic_search");

                entity.HasComment(@"The various search strings used to detect 
a requirement in a source PDF/webpage/etc. 
(e.g., ""Shelf life"" vs. ""shelflife"" vs. 
""shelf-life"")");

                entity.Property(e => e.TopicSearchId).HasColumnName("topic_search_id");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("created_by")
                    .HasComment("The network username of the person who created the record.");

                entity.Property(e => e.CreatedTs)
                    .HasColumnName("created_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.Property(e => e.IsWholeWord)
                    .HasColumnName("is_whole_word")
                    .HasComment(@"If true, this string will be searched as a whole word. 
Best for short terms or acronyms that might appear inside
other words, such as First Article Test abbreviated
to ""FAT"" might appear inside ""fatuous"" or ""infatuated""
or ""indefatiguable.""  Though if those words start to
appear in DIBBS solicitations, someone at DLA is dearly
in need of a vacation or counseling or both.");

                entity.Property(e => e.SearchString)
                    .IsRequired()
                    .HasColumnName("search_string");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasColumnName("updated_by")
                    .HasComment("The network username of the person who last updated this record");

                entity.Property(e => e.UpdatedTs)
                    .HasColumnName("updated_ts")
                    .HasDefaultValueSql("timezone('utc'::text, now())");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.TopicSearches)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("topic_search_topic_topic_id_fk");
            });

            modelBuilder.HasSequence("requirement_context_requirement_context_id_seq");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
