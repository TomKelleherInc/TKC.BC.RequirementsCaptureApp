using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Sedna.Service.Requirements.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "requirement_context_requirement_context_id_seq");

            migrationBuilder.CreateTable(
                name: "context",
                columns: table => new
                {
                    context_id = table.Column<int>(nullable: false, defaultValueSql: "nextval('context_context_seq'::regclass)"),
                    tag = table.Column<string>(nullable: false),
                    description = table.Column<string>(maxLength: 250, nullable: false),
                    reference_key = table.Column<string>(maxLength: 75, nullable: false, comment: @"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Description field. ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_context", x => x.context_id);
                },
                comment: @"The Brighton Cromwell workflow steps and contexts 
in which requirements might be relevant or impact 
activity/decisions/etc.  For example, ""PreAward.Rfq.Creation""
and ""PostAward.PurchaseOrder.Creation.""");

            migrationBuilder.CreateTable(
                name: "source_type",
                columns: table => new
                {
                    source_type_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(nullable: true),
                    reference_key = table.Column<string>(nullable: false, comment: @"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Name field. ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_source_type", x => x.source_type_id);
                },
                comment: "Different source types, like PDF, URL, a Sedna User, or other.");

            migrationBuilder.CreateTable(
                name: "subject_type",
                columns: table => new
                {
                    subject_type_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(nullable: true),
                    reference_key = table.Column<string>(nullable: true, comment: @"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Name field. ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subject_type", x => x.subject_type_id);
                },
                comment: @"Things we can attach requirements to: 
Opportunities, Opp Lines, Awards, Award 
Lines, Vendors, Customers, Parts, 
Vendor-Parts, etc.");

            migrationBuilder.CreateTable(
                name: "topic",
                columns: table => new
                {
                    topic_id = table.Column<int>(nullable: false, defaultValueSql: "nextval('concept_concept_id_seq'::regclass)"),
                    name = table.Column<string>(type: "character varying", nullable: false),
                    description = table.Column<string>(nullable: true),
                    preferred_phrasing = table.Column<string>(nullable: true, comment: "The desired paraphrasing/layman's terminloogy for this concept (e.g., \"Vendor will not use Child Labor\")"),
                    reference_key = table.Column<string>(nullable: false, comment: @"A human-readable unique key for each record
to simplify coding.  More understandable
than the ID but less change-able than the
Name field. ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topic", x => x.topic_id);
                },
                comment: @"The overarching concept the requirement is 
in service to; e.g., ""Shelf Life,"" 
""Mercury"", ""Hazmat""");

            migrationBuilder.CreateTable(
                name: "source",
                columns: table => new
                {
                    source_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    source_type_id = table.Column<int>(nullable: true),
                    external_identifier = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_source", x => x.source_id);
                    table.ForeignKey(
                        name: "source_source_type_source_type_id_fk",
                        column: x => x.source_type_id,
                        principalTable: "source_type",
                        principalColumn: "source_type_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: @"The source of the requirement.  This can be 
a specific solicitation PDF, a Word document,
an Excel file, or a person.");

            migrationBuilder.CreateTable(
                name: "subject",
                columns: table => new
                {
                    subject_id = table.Column<int>(nullable: false, comment: "The autoincremented id for the record")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    external_identifier = table.Column<string>(nullable: false, comment: "The external id that the calling routine knows this subject by. For an Opportunity, likely the \"opportunity_id\"; for an Opportunity Line, the \"opportunity_line_id.\""),
                    subject_type_id = table.Column<int>(nullable: false, comment: "The type of entity this is; an Opportunity, an Opportunity Line, etc."),
                    tag = table.Column<string>(type: "json", nullable: true, comment: "An optional field that permits descriptions of the entity, in key-value pairs."),
                    description = table.Column<string>(nullable: false, defaultValueSql: "''::text", comment: "A human-readable description, for UI display.  For example \"NSN ##### (Oil Filter)\"")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subject", x => x.subject_id);
                    table.ForeignKey(
                        name: "subject_entity_type_entity_type_id_fk",
                        column: x => x.subject_type_id,
                        principalTable: "subject_type",
                        principalColumn: "subject_type_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: @"The ""subject"" of the requirement; what it 
pertains to.  The Opportunity, the 
Opportunity Line, the Award or Award 
Line, etc.");

            migrationBuilder.CreateTable(
                name: "subject_type_topic",
                columns: table => new
                {
                    subject_type_topic_id = table.Column<int>(nullable: false, defaultValueSql: "nextval('entity_concept_entity_concept_id_seq'::regclass)"),
                    subject_type_id = table.Column<int>(nullable: false),
                    topic_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subject_type_topic", x => x.subject_type_topic_id);
                    table.ForeignKey(
                        name: "subject_topic_subject_subject_id_fk",
                        column: x => x.subject_type_id,
                        principalTable: "subject_type",
                        principalColumn: "subject_type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "subject_topic_topic_id_fk",
                        column: x => x.topic_id,
                        principalTable: "topic",
                        principalColumn: "topic_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: @"A cross reference of the topics that are 
pertinent to each entity type.  For instance, 
an Opportunity can have requirements around 
""DeliveryDate"" while an OpportunityLine 
can have them about ""ShelfLife.""");

            migrationBuilder.CreateTable(
                name: "topic_context",
                columns: table => new
                {
                    topic_context_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    topic_id = table.Column<int>(nullable: false),
                    context_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topic_context", x => x.topic_context_id);
                    table.ForeignKey(
                        name: "topic_context_context_context_fk",
                        column: x => x.context_id,
                        principalTable: "context",
                        principalColumn: "context_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "topic_context_topic_topic_id_fk",
                        column: x => x.topic_id,
                        principalTable: "topic",
                        principalColumn: "topic_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: @"Cross-reference of the contexts that are 
typically impacted by requirements associated 
with each topic.  These provide a templated 
pattern of default contexts when adding new 
requirements for that topic.");

            migrationBuilder.CreateTable(
                name: "topic_search",
                columns: table => new
                {
                    topic_search_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    topic_id = table.Column<int>(nullable: false),
                    search_string = table.Column<string>(nullable: false),
                    is_whole_word = table.Column<bool>(nullable: false, comment: @"If true, this string will be searched as a whole word. 
Best for short terms or acronyms that might appear inside
other words, such as First Article Test abbreviated
to ""FAT"" might appear inside ""fatuous"" or ""infatuated""
or ""indefatiguable.""  Though if those words start to
appear in DIBBS solicitations, someone at DLA is dearly
in need of a vacation or counseling or both.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topic_search", x => x.topic_search_id);
                    table.ForeignKey(
                        name: "topic_search_topic_topic_id_fk",
                        column: x => x.topic_id,
                        principalTable: "topic",
                        principalColumn: "topic_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: @"The various search strings used to detect 
a requirement in a source PDF/webpage/etc. 
(e.g., ""Shelf life"" vs. ""shelflife"" vs. 
""shelf-life"")");

            migrationBuilder.CreateTable(
                name: "requirement",
                columns: table => new
                {
                    requirement_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source_id = table.Column<int>(nullable: true),
                    source_text = table.Column<string>(nullable: true),
                    source_text_location = table.Column<string>(nullable: true),
                    preferred_phrasing = table.Column<string>(nullable: true),
                    topic_id = table.Column<int>(nullable: true),
                    subject_id = table.Column<int>(nullable: false, comment: @"The external ID of the thing that this requirement pertains to.  e.g., the opportunity_id, or opportunity_line_id, etc.  
Note that this database does not keep a list of these, nor know what such entities are.
The calling service needs to know what it's looking for based on its own data and organization."),
                    is_active = table.Column<bool>(nullable: false),
                    review_dt = table.Column<DateTime>(type: "date", nullable: true),
                    created_ts = table.Column<DateTime>(type: "timestamp(3) without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_ts = table.Column<DateTime>(type: "timestamp(3) without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_user_id = table.Column<int>(nullable: false, defaultValueSql: "529"),
                    updated_user_id = table.Column<int>(nullable: false, defaultValueSql: "529")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement", x => x.requirement_id);
                    table.ForeignKey(
                        name: "requirement_source_source_id_fk",
                        column: x => x.source_id,
                        principalTable: "source",
                        principalColumn: "source_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "requirement_subject_subject_id_fk",
                        column: x => x.subject_id,
                        principalTable: "subject",
                        principalColumn: "subject_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "requirement_topic_topic_id_fk",
                        column: x => x.topic_id,
                        principalTable: "topic",
                        principalColumn: "topic_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: @"The specifics of a given requirement, including 
what it pertains to (a given Opportunity or Line), 
the source it came from (e.g., a DIBBS solicitation 
PDF), what the original source of the requirement said
and what re-phrasing we want to use instead for clarity.	");

            migrationBuilder.CreateTable(
                name: "requirement_context",
                columns: table => new
                {
                    requirement_context_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    requirement_id = table.Column<int>(nullable: false),
                    context_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement_context", x => x.requirement_context_id);
                    table.ForeignKey(
                        name: "requirement_context_context_context_fk",
                        column: x => x.context_id,
                        principalTable: "context",
                        principalColumn: "context_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "requirement_context_requirement_requirement_id_fk",
                        column: x => x.requirement_id,
                        principalTable: "requirement",
                        principalColumn: "requirement_id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: @"List of the circumstances or contexts in which 
a particular requirement is meaningful or relevant.  
For instance, certain packaging requirements can
affect pricing, so ""PreAward.Bid.Creation"" would be a
relevant context, as would ""Warehouse.Packaging.Alert.""");

            migrationBuilder.CreateIndex(
                name: "context_context_uindex",
                table: "context",
                column: "context_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "context_reference_tag_uindex",
                table: "context",
                column: "reference_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "requirement_requirement_id_index",
                table: "requirement",
                column: "requirement_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_source_id",
                table: "requirement",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_subject_id",
                table: "requirement",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_topic_id",
                table: "requirement",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_context_requirement_id",
                table: "requirement_context",
                column: "requirement_id");

            migrationBuilder.CreateIndex(
                name: "requirement_context_context_id_requirement_id_uindex",
                table: "requirement_context",
                columns: new[] { "context_id", "requirement_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "source_external_identifier_uindex",
                table: "source",
                column: "external_identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_source_source_type_id",
                table: "source",
                column: "source_type_id");

            migrationBuilder.CreateIndex(
                name: "source_type_reference_key_uindex",
                table: "source_type",
                column: "reference_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "subject_external_identifier_uindex",
                table: "subject",
                column: "external_identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "subject_subject_id_uindex",
                table: "subject",
                column: "subject_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subject_subject_type_id",
                table: "subject",
                column: "subject_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_subject_type_topic_subject_type_id",
                table: "subject_type_topic",
                column: "subject_type_id");

            migrationBuilder.CreateIndex(
                name: "subject_type_topic_subject_topic_id_uindex",
                table: "subject_type_topic",
                column: "subject_type_topic_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subject_type_topic_topic_id",
                table: "subject_type_topic",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "topic_name_uindex",
                table: "topic",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_topic_context_context_id",
                table: "topic_context",
                column: "context_id");

            migrationBuilder.CreateIndex(
                name: "topic_context_topic_context_id_uindex",
                table: "topic_context",
                column: "topic_context_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_topic_context_topic_id",
                table: "topic_context",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "IX_topic_search_topic_id",
                table: "topic_search",
                column: "topic_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "requirement_context");

            migrationBuilder.DropTable(
                name: "subject_type_topic");

            migrationBuilder.DropTable(
                name: "topic_context");

            migrationBuilder.DropTable(
                name: "topic_search");

            migrationBuilder.DropTable(
                name: "requirement");

            migrationBuilder.DropTable(
                name: "context");

            migrationBuilder.DropTable(
                name: "source");

            migrationBuilder.DropTable(
                name: "subject");

            migrationBuilder.DropTable(
                name: "topic");

            migrationBuilder.DropTable(
                name: "source_type");

            migrationBuilder.DropTable(
                name: "subject_type");

            migrationBuilder.DropSequence(
                name: "requirement_context_requirement_context_id_seq");
        }
    }
}
