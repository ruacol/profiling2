using Aspose.Pdf.Generator;
using Profiling2.Domain.Contracts.Export.Screening;
using Profiling2.Domain.Contracts.Services;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using System;
using System.Linq;

namespace Profiling2.Infrastructure.Export.Screening
{
    public class PdfExportRequestForConditionalityParticipantService : BasePdfExportRequest, IPdfExportRequestForConditionalityParticipantService
    {
        public PdfExportRequestForConditionalityParticipantService() { }

        /// <summary>
        /// Ported from Profiling1.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="screeningEntity"></param>
        /// <param name="sortByRank"></param>
        /// <returns></returns>
        public byte[] GetExport(Request request, ScreeningEntity screeningEntity, bool sortByRank)
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
            section = SetTitle(request, screeningEntity, section);

            //sets the request details
            section = SetRequestDetails(request, section);

            //sets the persons attached to the request
            section = SetPersonsAttached(request, screeningEntity, sortByRank, section);

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

            requestDetails.ColumnWidths = "68 700";

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
            row = SetRequestDetailsRow(row, "Individuals:", string.Empty);

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

            //Name Column
            cell = row.Cells.Add("Name", textInfo);

            //Military ID Column
            cell = row.Cells.Add("ID Number", textInfo);

            //Color
            cell = row.Cells.Add("Color", textInfo);

            //Reason
            cell = row.Cells.Add("Reason", textInfo);

            //Commentary
            cell = row.Cells.Add("Commentary", textInfo);

            return row;
        }

        /// <summary>
        /// Sets a row in the person attached
        /// </summary>
        /// <param name="row">The row that has already been added to the persons attached table</param>
        /// <param name="person">The person whose details should be added to the row</param>
        /// <returns>Pdf row object</returns>
        private Row SetPersonAttachedRow(ScreeningEntity screeningEntity, Row row, RequestPerson requestPerson)
        {
            string personName;
            Text text;
            Cell cell;
            TextInfo textInfo = new TextInfo();

            row.VerticalAlignment = VerticalAlignmentType.Top;
            row.IsBroken = false;

            //text customization for labels in request details
            textInfo.FontSize = 12;
            textInfo.IsTrueTypeFontBold = false;
            textInfo.Alignment = AlignmentType.Center;

            //Name Column
            personName = requestPerson.Person.Name;
            cell = row.Cells.Add(personName.Trim(), textInfo);

            //Military ID Column
            cell = row.Cells.Add(requestPerson.Person.MilitaryIDNumber, textInfo);

            //Color
            ScreeningRequestPersonEntity srpe = requestPerson.GetMostRecentScreeningRequestPersonEntity(screeningEntity.ScreeningEntityName);
            string color = string.Empty;
            if (srpe != null)
                color = srpe.ScreeningResult.ToString();
            cell = row.Cells.Add();
            text = new Text(color);
            if (!string.IsNullOrEmpty(color))
            {
                //Green
                if (color.Equals(ScreeningResult.GREEN, StringComparison.OrdinalIgnoreCase))
                {
                    text.TextInfo.BackgroundColor = new Color("Green");
                    text.TextInfo.Color = new Color("White");
                }
                //Yellow
                else if (color.Equals(ScreeningResult.YELLOW, StringComparison.OrdinalIgnoreCase))
                {
                    text.TextInfo.BackgroundColor = new Color("Yellow");
                    text.TextInfo.Color = new Color("Black");
                }
                //Red
                else if (color.Equals(ScreeningResult.RED, StringComparison.OrdinalIgnoreCase))
                {
                    text.TextInfo.BackgroundColor = new Color("Red");
                    text.TextInfo.Color = new Color("White");
                }
            }
            text.TextInfo.Alignment = AlignmentType.Center;
            text.TextInfo.LineSpacing = 6;
            cell.Paragraphs.Add(text);

            //Reason
            cell = row.Cells.Add(srpe != null ? srpe.Reason : string.Empty, textInfo);

            //Commentary
            cell = row.Cells.Add(srpe != null ? srpe.Commentary : string.Empty, textInfo);

            return row;
        }

        /// <summary>
        /// Sets the persons attached
        /// </summary>
        /// <param name="section">Pdf section to which perons attached should be added</param>
        /// <param name="requestId">Id of request</param>
        /// <returns>Pdf section object</returns>
        private Section SetPersonsAttached(Request request, ScreeningEntity screeningEntity, bool sortByRank, Section section)
        {
            Row row;
            Table attachedPersons = new Table(section);
            attachedPersons.ColumnWidths = "100 80 60 230 230";
            attachedPersons.Margin.Top = 18;
            attachedPersons.DefaultCellBorder = new BorderInfo((int)BorderSide.Bottom, new Color("Black"));
            attachedPersons.DefaultCellPadding.Top = 9;
            attachedPersons.DefaultCellPadding.Right = 9;
            attachedPersons.DefaultCellPadding.Bottom = 9;
            attachedPersons.DefaultCellPadding.Left = 9;

            if (request.Persons.Where(x => !x.Archive).Any())
            {
                row = attachedPersons.Rows.Add();
                row = SetPersonAttachedHeaderRow(row);
                foreach (RequestPerson requestPerson in request.Persons.Where(x => !x.Archive).OrderBy(x => sortByRank ? x.Person.CurrentRankSortOrder : x.Id))
                {
                    row = attachedPersons.Rows.Add();
                    row = SetPersonAttachedRow(screeningEntity, row, requestPerson);
                }
            }

            section.Paragraphs.Add(attachedPersons);
            return section;
        }
    }
}
