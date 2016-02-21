using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201304031210)]
    public class PopulateFunctions : Migration
    {
        public override void Down()
        {
            Delete.FromTable("PRF_Function").AllRows();
        }

        public override void Up()
        {
            Execute.Sql(@"
                INSERT INTO PRF_Function (FunctionName, Aliases) VALUES
                ('Adm / Log', 'adminlog log administrationlogistics amdlog logadm'),
                ('Ops / Rens', 'opsint opsint opsren opsintel opsintelligence int intelligence intelligenceofficer ren'), 
                ('Ops', 'operations')
            ");
        }
    }
}
