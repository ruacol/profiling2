using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace Profiling2.Migrations.Migrations
{
    [Migration(201305071640)]
    public class InitAuditTrail : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.Sql(@"
                SET IDENTITY_INSERT dbo.REVINFO ON;
                INSERT INTO REVINFO (REVINFOID, REV, REVTSTMP) VALUES (100, 0, GETDATE());

                INSERT INTO PRF_ActionTaken_AUD (ActionTakenID, REV, REVTYPE, SubjectPersonID, ObjectPersonID, ActionTakenTypeID,
                    EventID, DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Commentary, Archive, Notes)
                    SELECT ActionTakenID, 100, 0, SubjectPersonID, ObjectPersonID, ActionTakenTypeID,
                        EventID, DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Commentary, Archive, Notes
                        FROM PRF_ActionTaken;

                INSERT INTO PRF_Career_AUD (CareerID, REV, REVTYPE, PersonID, OrganizationID, LocationID, RankID, UnitID, RoleID,
                    DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Job, Commentary, Archive, Notes,
                    DayAsOf, MonthAsOf, YearAsOf, IsCurrentCareer, FunctionID, Defected, Acting)
                    SELECT CareerID, 100, 0, PersonID, OrganizationID, LocationID, RankID, UnitID, RoleID,
                        DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Job, Commentary, Archive, Notes,
                        DayAsOf, MonthAsOf, YearAsOf, IsCurrentCareer, FunctionID, Defected, Acting
                        FROM PRF_Career
                        WHERE CareerID NOT IN (SELECT CareerID FROM PRF_Career_AUD);

                INSERT INTO PRF_Organization_AUD (OrganizationID, REV, REVTYPE, OrgShortName, OrgLongName, DayOfStart, MonthOfStart,
                    YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Archive, Notes)
                    SELECT OrganizationID, 100, 0, OrgShortName, OrgLongName, DayOfStart, MonthOfStart,
                        YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd, Archive, Notes
                        FROM PRF_Organization
                        WHERE OrganizationID NOT IN (SELECT OrganizationID FROM PRF_Organization_AUD);

                INSERT INTO PRF_Person_AUD (PersonID, REV, REVTYPE, LastName, FirstName, DayOfBirth, MonthOfBirth, YearOfBirth,
                    BirthVillage, BirthRegionID, ApproximateBirthDate, EthnicityID, Height, Weight, BackgroundInformation,
                    MilitaryIDNumber, Archive, Notes, ProfileStatusID, IsRestrictedProfile)
                    SELECT PersonID, 100, 0, LastName, FirstName, DayOfBirth, MonthOfBirth, YearOfBirth,
                        BirthVillage, BirthRegionID, ApproximateBirthDate, EthnicityID, Height, Weight, BackgroundInformation,
                        MilitaryIDNumber, Archive, Notes, ProfileStatusID, IsRestrictedProfile
                        FROM PRF_Person
                        WHERE PersonID NOT IN (SELECT PersonID FROM PRF_Person_AUD);

                INSERT INTO PRF_PersonAlias_AUD (PersonAliasID, REV, REVTYPE, PersonID, FirstName, LastName, Archive, Notes)
                    SELECT PersonAliasID, 100, 0, PersonID, FirstName, LastName, Archive, Notes
                        FROM PRF_PersonAlias
                        WHERE PersonAliasID NOT IN (SELECT PersonAliasID FROM PRF_PersonAlias_AUD);

                INSERT INTO PRF_PersonPhoto_AUD (PersonPhotoID, REV, REVTYPE, PersonID, PhotoID, Archive, Notes)
                    SELECT PersonPhotoID, 100, 0, PersonID, PhotoID, Archive, Notes
                        FROM PRF_PersonPhoto
                        WHERE PersonPhotoID NOT IN (SELECT PersonPhotoID FROM PRF_PersonPhoto_AUD);

                INSERT INTO PRF_PersonRelationship_AUD (PersonRelationshipID, REV, REVTYPE, SubjectPersonID, ObjectPersonID,
                    PersonRelationshipTypeID, DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd,
                    Archive, Notes)
                    SELECT PersonRelationshipID, 100, 0, SubjectPersonID, ObjectPersonID,
                        PersonRelationshipTypeID, DayOfStart, MonthOfStart, YearOfStart, DayOfEnd, MonthOfEnd, YearOfEnd,
                        Archive, Notes
                        FROM PRF_PersonRelationship
                        WHERE PersonRelationshipID NOT IN (SELECT PersonRelationshipID FROM PRF_PersonRelationship_AUD);

                INSERT INTO PRF_PersonResponsibility_AUD (PersonResponsibilityID, REV, REVTYPE, PersonID, EventID,
                    PersonResponsibilityTypeID, Commentary, Archive, Notes)
                    SELECT PersonResponsibilityID, 100, 0, PersonID, EventID, PersonResponsibilityTypeID, Commentary, Archive, Notes
                        FROM PRF_PersonResponsibility
                        WHERE PersonResponsibilityID NOT IN (SELECT PersonResponsibilityID FROM PRF_PersonResponsibility_AUD);

                INSERT INTO PRF_PersonSource_AUD (PersonSourceID, REV, REVTYPE, PersonID, SourceID, Commentary, ReliabilityID, Archive, Notes)
                    SELECT PersonSourceID, 100, 0, PersonID, SourceID, Commentary, ReliabilityID, Archive, Notes
                        FROM PRF_PersonSource
                        WHERE PersonSourceID NOT IN (SELECT PersonSourceID FROM PRF_PersonSource_AUD);

                INSERT INTO PRF_Unit_AUD (UnitID, REV, REVTYPE, UnitName, BackgroundInformation, Archive, Notes)
                    SELECT UnitID, 100, 0, UnitName, BackgroundInformation, Archive, Notes
                        FROM PRF_Unit
                        WHERE UnitID NOT IN (SELECT UnitID FROM PRF_Unit_AUD);

                INSERT INTO PRF_Violation_AUD (ViolationID, REV, REVTYPE, Name, Description, Keywords, ParentViolationID)
                    SELECT ViolationID, 100, 0, Name, Description, Keywords, ParentViolationID
                        FROM PRF_Violation
                        WHERE ViolationID NOT IN (SELECT ViolationID FROM PRF_Violation_AUD);

                INSERT INTO SCR_Request_AUD (RequestID, REV, REVTYPE, RequestEntityID, RequestTypeID, RequestName, ReferenceNumber,
                    RespondBy, Notes, Archive)
                    SELECT RequestID, 100, 0, RequestEntityID, RequestTypeID, RequestName, ReferenceNumber, RespondBy, Notes, Archive
                        FROM SCR_Request
                        WHERE RequestID NOT IN (SELECT RequestID FROM SCR_Request_AUD);
            ");

            Delete.Column("EventID").FromTable("PRF_PersonRelationship_AUD");
            Delete.Column("Commentary").FromTable("PRF_PersonRelationship_AUD");
        }
    }
}
