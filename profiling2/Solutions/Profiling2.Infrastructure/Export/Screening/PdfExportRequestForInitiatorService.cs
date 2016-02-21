using Aspose.Pdf.Generator;
using Profiling2.Domain.Contracts.Export.Screening;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonFinalDecision;
using Profiling2.Domain.Scr.Proposed;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Infrastructure.Export.Screening
{
    public class PdfExportRequestForInitiatorService : BasePdfExportRequest, IPdfExportRequestForInitiatorService
    {
        protected readonly IRequestPersonTasks requestPersonTasks;

        public PdfExportRequestForInitiatorService(IRequestPersonTasks requestPersonTasks)
        {
            this.requestPersonTasks = requestPersonTasks;
        }

        /// <summary>
        /// Ported from Profiling1.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="sortByRank"></param>
        /// <returns></returns>
        public byte[] GetExport(Request request, bool sortByRank)
        {
            //sets the document information such as authors, etc.
            Pdf pdf = SetDocumentInformation();

            //landscape format
            pdf.IsLandscape = true;

            //creates the section that holds document content
            Section section = pdf.Sections.Add();

            //sets the header and footer for document
            section = SetHeaderFooter(request, section);

            //sets the title for document
            section = SetTitle(request, null, section);

            //sets the request details
            section = SetRequestDetails(request, section);

            //sets the persons attached to the request
            section = SetPersonsAttached(request, sortByRank, section);

            //creates the pdf in a byte array
            return GetByteArray(pdf);
        }

        /// <summary>
        /// Sets the request details
        /// </summary>
        /// <param name="section">Pdf section for which to set the request details</param>
        /// <returns>Pdf section object</returns>
        private Section SetRequestDetails(Request request, Section section)
        {
            Row row;
            Table requestDetails = new Table(section);

            requestDetails.ColumnWidths = "68 600";

            //request name
            row = requestDetails.Rows.Add();
            row = SetRequestDetailsRow(row, "Name:", request.RequestName);

            //request type
            row = requestDetails.Rows.Add();
            row = SetRequestDetailsRow(row, "Type:", request.RequestType.ToString());

            //request description
            row = requestDetails.Rows.Add();
            row = SetRequestDetailsRow(row, "Description:", request.Notes);

            row = requestDetails.Rows.Add();
            string completionDate = request.CurrentStatus != null && request.CurrentStatus.RequestStatusName == RequestStatus.NAME_COMPLETED
                ? string.Format("{0:yyyy-MM-dd}", request.CurrentStatusDate)
                : string.Empty;
            row = SetRequestDetailsRow(row, "Completion Date:", completionDate);

            section.Paragraphs.Add(requestDetails);
            return section;
        }

        /// <summary>
        /// Sets the persons attached header row
        /// </summary>
        /// <param name="row">The header row that has already been added to the persons attached table</param>
        /// <returns>Pdf row object</returns>
        private Row SetPersonAttachedHeaderRow(Row row)
        {
            Cell cell;
            TextInfo textInfo = new TextInfo();

            row.VerticalAlignment = VerticalAlignmentType.Top;
            row.IsBroken = false;

            //text customization for labels in request details
            textInfo.FontSize = 12;
            textInfo.IsTrueTypeFontBold = true;
            textInfo.Alignment = AlignmentType.Center;

            cell = row.Cells.Add();

            //Career Column
            cell = row.Cells.Add("Function", textInfo);

            cell = row.Cells.Add("Rank", textInfo);

            //Name Column
            cell = row.Cells.Add("Name", textInfo);

            //Military ID Column
            cell = row.Cells.Add("ID Number", textInfo);

            //Support status Column
            cell = row.Cells.Add("Support Status", textInfo);

            return row;
        }

        /// <summary>
        /// Sets a row in the person attached
        /// </summary>
        /// <param name="row">The row that has already been added to the persons attached table</param>
        /// <param name="person">The person whose details should be added to the row</param>
        /// <returns>Pdf row object</returns>
        private Row SetPersonAttachedRow(Row row, RequestPerson rp, ScreeningRequestPersonFinalDecision srpfd, int rowNum)
        {
            Text text;
            Cell cell;
            TextInfo textInfo = new TextInfo();

            row.VerticalAlignment = VerticalAlignmentType.Top;
            row.IsBroken = false;

            //text customization for labels in request details
            textInfo.FontSize = 12;
            textInfo.IsTrueTypeFontBold = false;
            textInfo.Alignment = AlignmentType.Center;

            cell = row.Cells.Add(rowNum.ToString(), textInfo);

            //Career Column
            IList<Career> careers = this.requestPersonTasks.GetHistoricalCurrentCareers(rp, false);
            string functions = string.Empty;
            string rank = string.Empty;
            if (careers != null && careers.Any())
            {
                IEnumerable<string> list = careers.Select(x => x.FunctionUnitSummary);
                functions = string.Join("; ", list.Where(x => !string.IsNullOrEmpty(x)));
                if (careers.First().Rank != null)
                    rank = careers.First().Rank.ToString();
            }
            cell = row.Cells.Add(functions, textInfo);
            cell = row.Cells.Add(rank, textInfo);

            //Name Column
            //personName = (person.IsProposed) ? person.PersonName + " (proposed)" : person.PersonName;
            cell = row.Cells.Add(rp.Person.Name.Trim(), textInfo);

            //Military ID Column
            cell = row.Cells.Add(rp.Person.MilitaryIDNumber, textInfo);

            //Support Status Column
            string status = string.Empty;
            if (srpfd != null)
                status = srpfd.ScreeningSupportStatus.ToString();
            cell = row.Cells.Add();
            text = new Text(status);
            //if (!System.String.IsNullOrEmpty(person.ScreeningSupportStatus))
            //{
            //Support Recommended
            //if (person.ScreeningSupportStatus.Equals("Support Recommended", StringComparison.OrdinalIgnoreCase))
            //{
            //text.TextInfo.BackgroundColor = new Color("Green");
            //text.TextInfo.Color = new Color("White");
            //}
            //Support Not Recommended
            //else if (person.ScreeningSupportStatus.Equals("Support Not Recommended", StringComparison.OrdinalIgnoreCase))
            //{
            //text.TextInfo.BackgroundColor = new Color("Red");
            //text.TextInfo.Color = new Color("White");
            //}
            //Monitored Support
            //else
            //{

            //}
            //}
            text.TextInfo.Alignment = AlignmentType.Center;
            //text.TextInfo.LineSpacing = 6;
            cell.Paragraphs.Add(text);
            return row;
        }

        /// <summary>
        /// Sets the persons attached
        /// </summary>
        /// <param name="section">Pdf section to which perons attached should be added</param>
        /// <param name="requestId">Id of request</param>
        /// <returns>Pdf section object</returns>
        private Section SetPersonsAttached(Request request, bool sortByRank, Section section)
        {
            Row row;
            Table attachedPersons = new Table(section);
            attachedPersons.ColumnWidths = "15 200 100 200 100 100";
            attachedPersons.Margin.Top = 18;
            attachedPersons.DefaultCellBorder = new BorderInfo((int)BorderSide.Bottom, new Color("Black"));
            attachedPersons.DefaultCellPadding.Top = 9;
            attachedPersons.DefaultCellPadding.Right = 9;
            attachedPersons.DefaultCellPadding.Bottom = 9;
            attachedPersons.DefaultCellPadding.Left = 9;

            if (request.ProposedPersons.Where(x => !x.Archive).Any()
                || request.Persons.Where(x => !x.Archive).Any())
            {
                row = attachedPersons.Rows.Add();
                row = SetPersonAttachedHeaderRow(row);

                if (request.ProposedPersons.Where(x => !x.Archive).Any())
                {
                    foreach (RequestProposedPerson rpp in request.ProposedPersons.Where(x => !x.Archive))
                    {
                        row = attachedPersons.Rows.Add();
                        row = SetPersonAttachedRow(row, new RequestPerson()
                        {
                            Person = new Person()
                            {
                                FirstName = rpp.PersonName + " (proposed)",
                                MilitaryIDNumber = rpp.MilitaryIDNumber
                            }
                        }, null, attachedPersons.Rows.Count - 1);
                    }
                }

                if (request.Persons.Where(x => !x.Archive).Any())
                {
                    foreach (RequestPerson rp in request.Persons.Where(x => !x.Archive).OrderBy(x => sortByRank ? x.Person.CurrentRankSortOrder : x.Id))
                    {
                        row = attachedPersons.Rows.Add();
                        row = SetPersonAttachedRow(row, rp, rp.GetScreeningRequestPersonFinalDecision(), attachedPersons.Rows.Count - 1);
                    }
                }
            }

            section.Paragraphs.Add(attachedPersons);
            return section;
        }

    }
}
