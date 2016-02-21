using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201507271236)]
    public class AddSeveralPermissions : Migration
    {
        public override void Down()
        {
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanChangeActionsTaken" });
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanLinkEvents" });
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanLinkEventsAndSources" });
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanViewAndSearchOrganizations" });
            Delete.FromTable("PRF_AdminPermission").Row(new { Name = "CanChangeOrganizations" });
        }

        public override void Up()
        {
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanChangeActionsTaken" });
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanLinkEvents" });
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanLinkEventsAndSources" });
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanViewAndSearchOrganizations" });
            Insert.IntoTable("PRF_AdminPermission").Row(new { Name = "CanChangeOrganizations" });
        }
    }
}
