using Aspose.Words;
using Profiling2.Domain.Contracts.Export.Profiling;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Responsibility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Infrastructure.Export.Profiling
{
    public class WordExportEventService : BaseWordExportService, IWordExportEventService
    {
        /// <summary>
        /// Ported from Profiling1.
        /// </summary>
        public WordExportEventService() { }

        public byte[] GetExport(Event ev, DateTime lastModifiedDate)
        {
            //creating the document
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            //adding event information to profile
            builder = AddEventInformation(builder, ev);

            //adding organization responsibilities to profile
            builder = AddOrganizationResponsibilities(builder, ev.OrganizationResponsibilities);

            //adding person responsibilities to profile
            builder = AddPersonResponsibilities(builder, ev.PersonResponsibilities);

            //adding actions taken to profile
            builder = AddActionsTaken(builder, ev.ActionTakens);

            //adds header and footer for document
            builder = AddHeaderAndFooter(builder, string.Format("{0:yyyy-MM-dd}", lastModifiedDate));

            return this.GetBytes(doc);
        }

        private DocumentBuilder AddEventInformation(DocumentBuilder builder, Event ev)
        {
            //event name
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln(ev.Headline);

            //tags
            //builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading5;
            //builder.Writeln(GetLineofTags());

            //event categories
            if (ev.Violations != null && ev.Violations.Count > 0)
            {
                builder = InsertSectionTitle(builder, "Event Categories");
                builder.ListFormat.ApplyBulletDefault();
                foreach (Violation v in ev.Violations)
                {
                    builder.Write(v.Name);
                    builder.InsertBreak(BreakType.ParagraphBreak);
                }
                builder.ListFormat.RemoveNumbers();
            }

            //event information title and content
            builder = InsertSectionTitle(builder, "Event Information");
            builder.Write("Date of start (y/m/d):\t");
            builder.Font.Bold = true;
            builder.Write(ev.GetStartDateString());
            builder.InsertBreak(BreakType.LineBreak);
            builder.Font.Bold = false;
            builder.Write("Date of end (y/m/d):\t");
            builder.Font.Bold = true;
            builder.Write(ev.GetEndDateString());
            builder.InsertBreak(BreakType.LineBreak);
            builder.Font.Bold = false;
            builder.Write("Location:\t\t");
            builder.Font.Bold = true;
            builder.Write(ev.Location.ToString());
            builder.InsertBreak(BreakType.LineBreak);
            builder.Font.Bold = false;
            builder.Write("Notes:\t\t\t");
            builder.Font.Italic = true;
            if (!string.IsNullOrEmpty(ev.Notes))
            {
                builder.Write(ev.Notes);
            }
            builder.InsertBreak(BreakType.LineBreak);
            builder.Font.Italic = false;
            builder.InsertBreak(BreakType.ParagraphBreak);

            //justify document
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

            //English narrative
            if (!string.IsNullOrEmpty(ev.NarrativeEn))
            {
                builder = InsertSectionTitle(builder, "Narrative (English)");
                builder.Write(ev.NarrativeEn);
                builder.InsertBreak(BreakType.ParagraphBreak);
                builder.InsertBreak(BreakType.ParagraphBreak);
            }

            //French narrative
            if (!string.IsNullOrEmpty(ev.NarrativeFr))
            {
                builder = InsertSectionTitle(builder, "Narrative (French)");
                builder.Write(ev.NarrativeFr);
                builder.InsertBreak(BreakType.ParagraphBreak);
                builder.InsertBreak(BreakType.ParagraphBreak);
            }

            return builder;
        }

        private DocumentBuilder AddOrganizationResponsibilities(DocumentBuilder builder, IList<OrganizationResponsibility> ors)
        {
            //adding title
            builder = InsertSectionTitle(builder, "Organization Responsibilities");
            builder.ListFormat.ApplyBulletDefault();
            foreach (OrganizationResponsibility or in ors.Where(x => !x.Archive))
            {
                //name of organization
                builder.Font.Bold = true;
                if (or.Organization != null)
                {
                    builder.Write(or.Organization.ToString() + ". ");
                }
                builder.Font.Bold = false;

                //unit within organization
                if (or.Unit != null)
                {
                    builder.Write(or.Unit.ToString() + ". ");
                }

                //responsibility type
                if (or.OrganizationResponsibilityType != null)
                {
                    builder.Write(or.OrganizationResponsibilityType.ToString());
                }
                builder.InsertBreak(BreakType.ParagraphBreak);

                //responsibility commentary as a nested list item
                if (!string.IsNullOrEmpty(or.Commentary))
                {
                    builder.ListFormat.ListIndent();
                    builder.Writeln(or.Commentary.Trim());
                    builder.ListFormat.ListOutdent();
                }
            } //ends for loop

            builder.ListFormat.RemoveNumbers();
            return builder;
        }

        private DocumentBuilder AddPersonResponsibilities(DocumentBuilder builder, IList<PersonResponsibility> prs)
        {
            //adding title
            builder = InsertSectionTitle(builder, "Person Responsibilities");
            builder.ListFormat.ApplyBulletDefault();
            foreach (PersonResponsibility pr in prs.Where(x => !x.Archive))
            {
                //name of person
                builder.Font.Bold = true;
                if (pr.Person != null && !string.IsNullOrEmpty(pr.Person.Name))
                {
                    builder.Write(pr.Person.Name + ". ");
                }
                builder.Font.Bold = false;

                //responsibility type
                if (pr.PersonResponsibilityType != null)
                {
                    builder.Write(pr.PersonResponsibilityType.ToString());
                }
                builder.InsertBreak(BreakType.ParagraphBreak);

                //responsibility commentary as a nested list item
                if (!string.IsNullOrEmpty(pr.Commentary))
                {
                    builder.ListFormat.ListIndent();
                    builder.Writeln(pr.Commentary.Trim());
                    builder.ListFormat.ListOutdent();
                }
            } //ends for loop

            builder.ListFormat.RemoveNumbers();
            return builder;
        }

        private DocumentBuilder AddActionsTaken(DocumentBuilder builder, IList<ActionTaken> ats)
        {
            string listItemText;

            //adding title
            builder = InsertSectionTitle(builder, "Actions Taken");
            builder.ListFormat.ApplyBulletDefault();
            foreach (ActionTaken at in ats.Where(x => !x.Archive))
            {
                listItemText = string.Empty;

                //date
                if (!string.IsNullOrEmpty(at.GetDateSummary()))
                {
                    listItemText += at.GetDateSummary().Trim() + " ";
                }

                // subject, type, object
                if (!string.IsNullOrEmpty(at.GetSubjectTypeObjectSummary()))
                {
                    listItemText += at.GetSubjectTypeObjectSummary().Trim();
                }
                listItemText += listItemText.EndsWith(".") ? string.Empty : ".";
                builder.Write(listItemText);
                builder.InsertBreak(BreakType.ParagraphBreak);

                //not other action taken so add commentary
                if (!at.IsOtherType() && !string.IsNullOrEmpty(at.Commentary))
                {
                    builder.ListFormat.ListIndent();
                    builder.Writeln(at.Commentary.Trim());
                    builder.ListFormat.ListOutdent();
                }
            } //ends for loop

            builder.ListFormat.RemoveNumbers();
            return builder;
        }
    }
}
