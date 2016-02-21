using FluentMigrator;

namespace Profiling2.Migrations.Seeds
{
    [Migration(201301230939)]
    public class PRF_seeds : Migration
    {
        public override void Down()
        {
            Delete.FromTable("PRF_AdminAuditType").AllRows();
            Delete.FromTable("PRF_ActionTakenType").AllRows();
            Delete.FromTable("PRF_PersonResponsibilityType").AllRows();
            Delete.FromTable("PRF_PersonRelationshipType").AllRows();
            Delete.FromTable("PRF_OrganizationResponsibilityType").AllRows();
            Delete.FromTable("PRF_OrganizationRelationshipType").AllRows();
            Delete.FromTable("PRF_UnitHierarchyType").AllRows();
            Delete.FromTable("PRF_Reliability").AllRows();
            Delete.FromTable("PRF_ProfileStatus").AllRows();
            Delete.FromTable("PRF_AdminSuggestionFeaturePersonResponsibility").AllRows();
            Delete.FromTable("PRF_EventRelationshipType").AllRows();
        }

        public override void Up()
        {
            //Execute.EmbeddedScript("201301230939_PRF_seeds.sql");
            Insert.IntoTable("PRF_AdminAuditType").Row(new { AdminAuditTypeName = "INSERT" });
            Insert.IntoTable("PRF_AdminAuditType").Row(new { AdminAuditTypeName = "UPDATE" });
            Insert.IntoTable("PRF_AdminAuditType").Row(new { AdminAuditTypeName = "DELETE" });

            Insert.IntoTable("PRF_ActionTakenType").Row(new 
            { 
                ActionTakenTypeName = "ordered arrest of alleged perpetrator(s)",
                Notes = "Subject ordered arrest of alleged perpetrator(s). Object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "ordered arrest of",
                Notes = "Subject ordered arrest of object."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "arrested the alleged perpetrator(s)",
                Notes = "Subject arrested the alleged perpetrator(s). Object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "arrested",
                Notes = "Subject arrested object."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "Alleged perpetrator(s) arrested",
                Notes = "Alleged perpetrator(s) arrested. Subject and object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "arrested by",
                Notes = "Subject arrested by object."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "Investigation opened",
                Notes = "Investigation opened. Subject and object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "Alleged perpetrator(s) transferred to military prosecutor's office",
                Notes = "Alleged perpetrator(s) transferred to military prosecutor's office. Subject and object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "transferred to military prosecutor's office",
                Notes = "Subject transferred to military prosecutor''s office. Object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "sentenced",
                Notes = "Subject sentenced. Object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "Perpetrator(s) sentenced",
                Notes = "Perpetrator(s) sentenced. Subject and object not identified."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new
            {
                ActionTakenTypeName = "Other",
                Notes = "An action taken type that is not captured with the standardized list."
            });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "Complaint lodged" });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "released the victim(s)" });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "was arrested" });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "released the alleged perpetrator(s)" });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "interfered in the legal process" });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "was released" });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "was acquitted " });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "Alleged perpetrator(s) acquitted " });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "Victim(s) released" });
            Insert.IntoTable("PRF_ActionTakenType").Row(new { ActionTakenTypeName = "Minor was demobilized" });

            Insert.IntoTable("PRF_PersonResponsibilityType").Row(new { PersonResponsibilityTypeName = "Command" });
            Insert.IntoTable("PRF_PersonResponsibilityType").Row(new { PersonResponsibilityTypeName = "Direct" });

            Insert.IntoTable("PRF_PersonRelationshipType").Row(new 
            { 
                PersonRelationshipTypeName = "has direct family links with", 
                Notes = "ascendents, descendents, brothers, sisters", 
                IsCommutative = 1
            });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is schoolmates with", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is business partner with", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a superior to", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a mentor to", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "fought with", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a friend of", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a political associate of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "has family connections with", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "belongs to the same clan as", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "has ethnic ties with", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a subordinate of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "acted together with", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "could be the same person known as", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is the deputy of", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is the bodyguard of", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new
            {
                PersonRelationshipTypeName = "fought in the same group as",
                Notes = "when no information on hierarchy is available and individuals are not related to specific events",
                IsCommutative = 1
            });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new
            {
                PersonRelationshipTypeName = "belonged to the same group as ",
                Notes = "when no information on hierarchy is available ; individuals are not related to specific events ; no information on their participation to fightings",
                IsCommutative = 1
            });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new
            {
                PersonRelationshipTypeName = "is NOT the same person as",
                Notes = "Two different persons not to be confused",
                IsCommutative = 1
            });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "often works with", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is the same person as", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a potential rival of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "replaced", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is PROBABLY NOT the same person as", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is in conflict with", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "has contacts with", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "has no real authority on", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "provided weapons and ammunition to", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "fought against", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is suspected to be close to", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "was a political associate", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is from the same village as", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "went to the same school as", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "provided financial/material support to", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is close to", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is PROBABLY the same person as", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "looks like same person", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "AKA", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is the young brother of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a colleague of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "alias", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "was a superior to", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "was a subordinate to", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "was replaced by", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "worked with", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "could be the same person as", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a superior", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "swapped command posts with", IsCommutative = 1 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is allegedly enjoying the protection of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "allegedly protects", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "was the deputy of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "aided", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "was the bodyguard of", IsCommutative = 0 });
            Insert.IntoTable("PRF_PersonRelationshipType").Row(new { PersonRelationshipTypeName = "is a subordinate", IsCommutative = 0 });

            Insert.IntoTable("PRF_OrganizationResponsibilityType").Row(new { OrganizationResponsibilityTypeName = "Command", Notes = "There was no known plan to commit the HRV according to the information available." });
            Insert.IntoTable("PRF_OrganizationResponsibilityType").Row(new { OrganizationResponsibilityTypeName = "Operational" });
            Insert.IntoTable("PRF_OrganizationResponsibilityType").Row(new { OrganizationResponsibilityTypeName = "Material Participant" });
            Insert.IntoTable("PRF_OrganizationResponsibilityType").Row(new { OrganizationResponsibilityTypeName = "Main organization involved in the HRV" });
            Insert.IntoTable("PRF_OrganizationResponsibilityType").Row(new { OrganizationResponsibilityTypeName = "Supporting Organization" });

            Insert.IntoTable("PRF_OrganizationRelationshipType").Row(new { OrganizationRelationshipTypeName = "evolved from" });
            Insert.IntoTable("PRF_OrganizationRelationshipType").Row(new { OrganizationRelationshipTypeName = "partnered with" });
            Insert.IntoTable("PRF_OrganizationRelationshipType").Row(new { OrganizationRelationshipTypeName = "is superior to" });
            Insert.IntoTable("PRF_OrganizationRelationshipType").Row(new { OrganizationRelationshipTypeName = "fought against" });
            Insert.IntoTable("PRF_OrganizationRelationshipType").Row(new { OrganizationRelationshipTypeName = "sub group of" });
            Insert.IntoTable("PRF_OrganizationRelationshipType").Row(new { OrganizationRelationshipTypeName = "is a commander" });

            Insert.IntoTable("PRF_UnitHierarchyType").Row(new { UnitHierarchyTypeName = "DSRSG" });
            Insert.IntoTable("PRF_UnitHierarchyType").Row(new { UnitHierarchyTypeName = "Force" });
            Insert.IntoTable("PRF_UnitHierarchyType").Row(new { UnitHierarchyTypeName = "Hierarchy" });
            Insert.IntoTable("PRF_UnitHierarchyType").Row(new { UnitHierarchyTypeName = "Operation" });
            Insert.IntoTable("PRF_UnitHierarchyType").Row(new { UnitHierarchyTypeName = "Unknown" });

            Insert.IntoTable("PRF_Reliability").Row(new { ReliabilityName = "High" });
            Insert.IntoTable("PRF_Reliability").Row(new { ReliabilityName = "Moderate" });
            Insert.IntoTable("PRF_Reliability").Row(new { ReliabilityName = "Low" });

            Insert.IntoTable("PRF_ProfileStatus").Row(new { ProfileStatusName = "Rough outline", Notes = "For profiles with a name, and very rough career and/or human rights information" });
            Insert.IntoTable("PRF_ProfileStatus").Row(new { ProfileStatusName = "In progress", Notes = "For more developed profiles, with a solid amount of career and human rights information." });
            Insert.IntoTable("PRF_ProfileStatus").Row(new { ProfileStatusName = "Complete", Notes = "For profiles which are comprehensive, given the available information, and complete as of the last update." });
            Insert.IntoTable("PRF_ProfileStatus").Row(new { ProfileStatusName = "FARDC 2007 List", Notes = "Imported following the integration of DSRSG's list of 2007 FARDC integrated soldiers of all ranks.  No profiles." });

            // referenced by ID number in Entity class
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person has a relationship with a person bearing responsibility for suggested event.", CurrentWeight = 10, Archive = 1, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person bears responsibility to an event related to suggested event.", CurrentWeight = 8 });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person's last name appears in suggested event's narrative.", CurrentWeight = 7, Notes = "Each word separated by a space is checked individually." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person's first name appears in suggested event's narrative.", CurrentWeight = 2, Notes = "Each word separated by a space is checked individually." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person shares a source with suggested event.", CurrentWeight = 5 });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person's alias appears in suggested event's narrative.", CurrentWeight = 4, Notes = "Each word separated by a space is checked individually." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person has a career in the location or territory of the suggested event.", CurrentWeight = 3, Notes = "Constained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person has a career in an organization bearing responsibility for suggested event.", CurrentWeight = 2, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person bears responsibility for an event occurring in the same location or territory as suggested event.", CurrentWeight = 1 });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"is superior to\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 10, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"is a subordinate of\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 10, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"is the deputy of\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 10, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"provided weapons and ammunition to\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 7, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"fought with\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 7, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"acted together with\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 7, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"is the bodyguard of\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 7, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"fought in same group as\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 3, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person is part of an \"belonged to the same group as\" relationship with a person bearing responsibility for suggested event.", CurrentWeight = 3, Notes = "Constrained by dates." });
            Insert.IntoTable("PRF_AdminSuggestionFeaturePersonResponsibility").Row(new { Feature = "Profiled person has a career a unit bearing responsibility for suggested event.", CurrentWeight = 5, Notes = "Constrained by dates." });

            Insert.IntoTable("PRF_EventRelationshipType").Row(new { EventRelationshipTypeName = "is a reprisal for" });
            Insert.IntoTable("PRF_EventRelationshipType").Row(new { EventRelationshipTypeName = "is in response to" });
            Insert.IntoTable("PRF_EventRelationshipType").Row(new { EventRelationshipTypeName = "occurred simultaneously" });
            Insert.IntoTable("PRF_EventRelationshipType").Row(new { EventRelationshipTypeName = "are related" });
        }
    }
}
