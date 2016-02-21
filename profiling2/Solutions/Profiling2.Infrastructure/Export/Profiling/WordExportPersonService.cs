using Aspose.Words;
using Aspose.Words.Lists;
using Profiling2.Domain.Contracts.Export.Profiling;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Infrastructure.Export.Profiling
{
    public class WordExportPersonService : BaseWordExportService, IWordExportPersonService
    {
        /// <summary>
        /// Ported from Profiling1.
        /// </summary>
        public WordExportPersonService() { }

        public byte[] GetExport(Person person, bool includeBackground)
        {
            //creating the document
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            //adding personal information to profile
            builder = AddPersonalInformation(builder, person, includeBackground);

            //adding current position to profile
            builder = InsertSectionTitle(builder, "Current Position");
            builder = AddCareerInformation(builder, person.GetCurrentCareers());

            //adding career information to profile
            builder = InsertSectionTitle(builder, "Career Information");
            builder = AddCareerInformation(builder, person.GetNonCurrentCareers());

            //adding human rights information to profile
            builder = AddHumanRightsInformation(builder, person.PersonResponsibilities);

            //adds header and footer for document
            builder = AddHeaderAndFooter(builder, string.Format("{0:yyyy-MM-dd}", person.ProfileLastModified));

            return this.GetBytes(doc);
        }

        private DocumentBuilder AddPersonalInformation(DocumentBuilder builder, Person person, bool includeBackground)
        {
            //person name
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln(person.Name);

            //aliases
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading5;
            if (person.PersonAliases != null && person.PersonAliases.Any())
            {
                builder.Writeln("(also known as " + string.Join(", ", person.PersonAliases.Select(x => x.Name).ToArray()) + ")");
            }

            //personal information title and content
            builder = InsertSectionTitle(builder, "Personal Information");
            builder.Write("Date of birth (y/m/d):\t");
            builder.Font.Bold = true;
            if (!string.IsNullOrEmpty(person.DateOfBirth))
            {
                builder.Write(person.DateOfBirth);
            }
            builder.InsertBreak(BreakType.LineBreak);
            builder.Font.Bold = false;
            builder.Write("Place of birth:\t\t");
            builder.Font.Bold = true;
            if (!string.IsNullOrEmpty(person.PlaceOfBirth))
            {
                builder.Write(person.PlaceOfBirth);
            }
            builder.InsertBreak(BreakType.LineBreak);
            builder.Font.Bold = false;
            builder.Write("Ethnicity:\t\t");
            builder.Font.Bold = true;
            builder.Write(person.Ethnicity != null ? person.Ethnicity.ToString() : string.Empty);
            builder.InsertBreak(BreakType.LineBreak);
            builder.Font.Bold = false;
            if (!(string.IsNullOrEmpty(person.MilitaryIDNumber)))
            {
                builder.Write("ID Number:\t\t");
                builder.Font.Bold = true;
                builder.Write(person.MilitaryIDNumber);
                builder.Font.Bold = false;
                builder.InsertBreak(BreakType.LineBreak);
            }
            builder.InsertBreak(BreakType.ParagraphBreak);

            //adding photos to profile
            builder = AddPhotos(builder, person);

            //justify document after photos
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

            if (includeBackground)
            {
                //background information title and content
                builder = InsertSectionTitle(builder, "Background Information");
                //builder.Font.Italic = true;
                if (!string.IsNullOrEmpty(person.BackgroundInformation))
                {
                    builder.Write(person.BackgroundInformation);
                }
                //builder.Font.Italic = false;
                builder.InsertBreak(BreakType.ParagraphBreak);
                builder.InsertBreak(BreakType.ParagraphBreak);
            }

            return builder;
        }

        private DocumentBuilder AddPhotos(DocumentBuilder builder, Person person)
        {
            foreach (Photo photo in person.PersonPhotos.Where(x => !x.Archive).Select(x => x.Photo)
                .Where(x => !x.Archive && x.FileData != null))
            {
                builder.InsertImage(photo.FileData);
            }
            builder.InsertBreak(BreakType.ParagraphBreak);
            builder.InsertBreak(BreakType.ParagraphBreak);
            return builder;
        }

        private DocumentBuilder AddCareerInformation(DocumentBuilder builder, IList<Career> careers)
        {
            double originalFontSize;

            builder.ListFormat.ApplyBulletDefault();
            foreach (Career c in careers)
            {
                if (!(string.IsNullOrEmpty(c.GetFullSummary())))
                {
                    builder.Write(c.GetFullSummary());
                }
                builder.Font.Italic = true;
                originalFontSize = builder.Font.Size;
                builder.Font.Size = 10;
                if (!(string.IsNullOrEmpty(c.Commentary)))
                {
                    builder.Write(" " + c.Commentary);
                }
                builder.Font.Size = originalFontSize;
                builder.Font.Italic = false;
                builder.Writeln(" ");
            } //ends for loop
            builder.ListFormat.RemoveNumbers();
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder.InsertBreak(BreakType.ParagraphBreak);

            return builder;
        }

        private DocumentBuilder AddHumanRightsInformation(DocumentBuilder builder, IList<PersonResponsibility> responsibilities)
        {
            builder = InsertSectionTitle(builder, "Available Human Rights Record");
            builder.ListFormat.ApplyBulletDefault();

            // filter responsibilities that we want to display
            IList<PersonResponsibility> validResponsibilities = responsibilities.Where(x => !x.Archive && x.Event != null && x.Event.HasAtLeastOneReliableSource()).ToList();

            // index by eventId (because multiple PersonResponsibilities may be linked to the same Event, but different Violations)
            IDictionary<int, IList<PersonResponsibility>> indexedResponsibilities = new Dictionary<int, IList<PersonResponsibility>>();
            foreach (PersonResponsibility pr in validResponsibilities)
            {
                if (!indexedResponsibilities.ContainsKey(pr.Event.Id))
                    indexedResponsibilities[pr.Event.Id] = new List<PersonResponsibility>();
                indexedResponsibilities[pr.Event.Id].Add(pr);
            }

            // sort  by most recent Event first
            IList<Event> sortedEvents = validResponsibilities
                .Select(x => x.Event)
                .OrderByDescending(x => x.GetStartDateTime())
                .ThenByDescending(x => x.GetEndDateTime())
                .Distinct().ToList();

            foreach (IList<PersonResponsibility> prs in sortedEvents.Select(x => indexedResponsibilities[x.Id]))
            {
                Event e = prs.First().Event;

                //Date and Location
                builder.Font.Bold = true;
                string line = e.GetDateSummary() + " ";
                if (e.Location != null)
                {
                    line += e.Location.LocationName + " ";
                }
                builder.Writeln(line);
                builder.Font.Bold = false;

                //event summary
                builder.ListFormat.ListIndent();
                if (!string.IsNullOrEmpty(e.NarrativeEn))
                {
                    builder.Writeln(e.NarrativeEn.Trim());
                }

                foreach (PersonResponsibility pr in prs)
                {
                    builder.Font.Bold = true;
                    //Event Categories
                    if (pr.Violations != null && pr.Violations.Any())
                    {
                        builder.Write(string.Join(", ", pr.Violations.Select(x => x.Name)) + ".  ");
                    }
                    else if (!string.IsNullOrEmpty(pr.Event.EventName))
                    {
                        //Event Name
                        builder.Write(pr.Event.EventName);
                    }
                    builder.Font.Bold = false;

                    //Person Responsibility Type
                    builder.Write(pr.PersonResponsibilityType.ToString() + " responsibility.");
                    builder.InsertBreak(BreakType.ParagraphBreak);

                    builder.ListFormat.ListIndent();
                    //commentary nested list item
                    if (!string.IsNullOrEmpty(pr.Commentary))
                    {
                        builder.Writeln(pr.Commentary.Trim());
                    }
                    builder.ListFormat.ListOutdent();
                }

                //follow-up actions
                builder = AddHumanRightsInformationActionTaken(builder, e);

                builder.ListFormat.ListOutdent();
            } //ends for loop

            builder.ListFormat.RemoveNumbers();
            return builder;
        }

        private DocumentBuilder AddHumanRightsInformationActionTaken(DocumentBuilder builder, Event e)
        {
            List originalList;
            double originalFontSize, mainListLevelOneNumberPosition, mainListLevelOneTextPosition;

            originalList = builder.ListFormat.List;
            mainListLevelOneNumberPosition = builder.ListFormat.List.ListLevels[1].NumberPosition;
            mainListLevelOneTextPosition = builder.ListFormat.List.ListLevels[1].TextPosition;
            builder.ListFormat.RemoveNumbers();

            builder.ListFormat.ApplyBulletDefault();
            builder.ListFormat.List = builder.Document.Lists.Add(ListTemplate.BulletArrowHead);
            builder.ListFormat.List.ListLevels[0].NumberPosition = mainListLevelOneNumberPosition;
            builder.ListFormat.List.ListLevels[0].TextPosition = mainListLevelOneTextPosition;

            foreach (ActionTaken at in e.ActionTakens.Where(x => !x.Archive))
            {
                string text = string.Empty;

                // date
                if (!string.IsNullOrEmpty(at.GetDateSummary()))
                {
                    text += at.GetDateSummary().Trim() + " ";
                }

                // subject, type, object
                if (!string.IsNullOrEmpty(at.GetSubjectTypeObjectSummary()))
                {
                    text += at.GetSubjectTypeObjectSummary().Trim();
                }
                text = text.Trim();
                if (!text.EndsWith("."))
                {
                    text += ".";
                }
                builder.Write(text);

                //commentary (if not other action taken type)
                builder.Font.Italic = true;
                originalFontSize = builder.Font.Size;
                builder.Font.Size = 10;
                string italic = string.Empty;
                if (!at.IsOtherType() && !string.IsNullOrEmpty(at.Commentary))
                {
                    italic += at.Commentary;
                }
                builder.Write(" " + italic.Trim());
                builder.Font.Size = originalFontSize;
                builder.Font.Italic = false;
                builder.Writeln(" ");
            }
            builder.ListFormat.RemoveNumbers();
            builder.ListFormat.List = originalList;
            return builder;
        }
    }
}
