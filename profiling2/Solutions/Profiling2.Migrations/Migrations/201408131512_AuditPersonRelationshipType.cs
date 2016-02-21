using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201408131512)]
    public class AuditPersonRelationshipType : Migration
    {
        public override void Down()
        {
            Delete.Table("PRF_PersonRelationshipType_AUD");
        }

        public override void Up()
        {
            Create.Table("PRF_PersonRelationshipType_AUD")
                .WithColumn("PersonRelationshipTypeID").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32()
                .WithColumn("REVTYPE").AsInt16()
                .WithColumn("PersonRelationshipTypeName").AsString(255).Nullable()
                .WithColumn("Archive").AsBoolean().Nullable()
                .WithColumn("Notes").AsString(int.MaxValue).Nullable()
                .WithColumn("IsCommutative").AsBoolean().Nullable();
            Create.PrimaryKey().OnTable("PRF_PersonRelationshipType_AUD").Columns(new string[] { "PersonRelationshipTypeID", "REV" });

            Execute.Sql(@"
                IF EXISTS (SELECT * FROM REVINFO WHERE REVINFOID = 100)
                BEGIN
                    INSERT INTO PRF_PersonRelationshipType_AUD (PersonRelationshipTypeID, REV, REVTYPE, PersonRelationshipTypeName, Archive, Notes, IsCommutative)
                        SELECT PersonRelationshipTypeID, 100, 0, PersonRelationshipTypeName, Archive, Notes, IsCommutative
                        FROM PRF_PersonRelationshipType;

                    INSERT INTO REVINFO (REVTSTMP, REV, UserName) VALUES (GETDATE(), 0, 'GEN-01328');

                    UPDATE PRF_PersonRelationshipType_AUD SET REV = 
                    (
                        SELECT TOP 1 REVINFOID FROM REVINFO ORDER BY REVINFOID DESC
                    )
                    WHERE REV = 100;
                END
            ");
        }
    }
}
