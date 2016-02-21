using System;
using System.IO;
using Aspose.Pdf.Generator;
using Profiling2.Domain.Scr;

namespace Profiling2.Infrastructure.Export.Screening
{
    /// <summary>
    /// Ported from Profiling1.
    /// 
    /// TODO rare problem with word wrap not working properly in table cells when columns set (PROF2-344).
    /// </summary>
    public abstract class BasePdfExportRequest
    {
        public byte[] PdfBytes { get; set; }

        /// <summary>
        /// Sets the PDF document information such as authors, etc.
        /// </summary>
        /// <returns>Pdf document object</returns>
        protected Pdf SetDocumentInformation()
        {
            Pdf pdf = new Pdf();
            pdf.Author = "MONUSCO";
            pdf.Creator = "MONUSCO";
            pdf.Keywords = "MONUSCO Conditionality Policy";
            pdf.Subject = "MONUSCO Conditionality Policy";
            pdf.Title = "Request for Screening";
            pdf.DestinationType = DestinationType.FitPage;
            pdf.PageSetup.Margin.Top = 72;
            pdf.PageSetup.Margin.Right = 72;
            pdf.PageSetup.Margin.Bottom = 72;
            pdf.PageSetup.Margin.Left = 72;
            return pdf;
        }

        /// <summary>
        /// Sets the header and footer for a section
        /// </summary>
        /// <param name="section">Pdf section for which to set header and footer</param>
        /// <returns>Pdf section object</returns>
        protected Section SetHeaderFooter(Request request, Section section)
        {
            Text text;

            //adding header
            HeaderFooter header = new HeaderFooter();
            header.Margin.Top = 36;
            header.TextInfo.Alignment = AlignmentType.Center;
            header.TextInfo.Color = new Color("Red");
            header.TextInfo.FontSize = 10;
            section.OddHeader = header;
            section.EvenHeader = header;
            header.Paragraphs.Add(new Text(header, "MONUSCO CONFIDENTIAL"));
            header.Paragraphs.Add(new Text(header, "SHARING WITH THIRD PARTIES SHOULD BE AUTHORISED BY MONUSCO"));

            //adding footer
            HeaderFooter footer = new HeaderFooter();
            footer.Margin.Top = 0;
            footer.Margin.Bottom = 0;
            section.OddFooter = footer;
            section.EvenFooter = footer;
            text = new Text(footer, "Request for Screening #" + request.ReferenceNumber + " exported on " + DateTime.Now.ToString("dd MMMM yyyy h:mm:ss tt"));

            text.TextInfo.Alignment = AlignmentType.Center;
            text.TextInfo.FontSize = 10;
            footer.Paragraphs.Add(text);
            text = new Text(footer, "$p of $P");
            text.TextInfo.Alignment = AlignmentType.Right;
            text.TextInfo.FontSize = 8;
            footer.Paragraphs.Add(text);
            return section;
        }

        /// <summary>
        /// Retrieves a byte array from the Pdf object
        /// </summary>
        /// <param name="pdf">Pdf object from which to retrieve byte array</param>
        /// <returns>Byte array of pdf object</returns>
        protected byte[] GetByteArray(Pdf pdf)
        {
            byte[] bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                pdf.Save(stream);
                bytes = stream.ToArray();
                stream.Dispose();
            }
            return bytes;
        }

        /// <summary>
        /// Sets the title
        /// </summary>
        /// <param name="section">Pdf section for which to set the title</param>
        /// <returns>Pdf section object</returns>
        protected Section SetTitle(Request request, ScreeningEntity screeningEntity, Section section)
        {
            string s = "Request for Screening #" + request.ReferenceNumber;
            if (screeningEntity != null)
                s += ", Response From " + screeningEntity.ToString();
            Text text = new Text(section, s);
            text.TextInfo.FontSize = 18;
            text.TextInfo.IsUnderline = true;
            text.TextInfo.Alignment = AlignmentType.Left;
            text.Margin.Top = 24;
            text.Margin.Bottom = 24;
            section.Paragraphs.Add(text);
            return section;
        }

        /// <summary>
        /// Sets a row in the request details
        /// </summary>
        /// <param name="row">The row that has already been added to the request details table</param>
        /// <param name="label">The label of this request details row</param>
        /// <param name="value">The value for this request details row</param>
        /// <returns>Pdf row object</returns>
        protected Row SetRequestDetailsRow(Row row, string label, string value)
        {
            Cell labelCell, valueCell;
            TextInfo labelTextInfo = new TextInfo();
            TextInfo valueTextInfo = new TextInfo();

            row.VerticalAlignment = VerticalAlignmentType.Top;

            //text customization for labels in request details
            labelTextInfo.FontSize = 12;
            labelTextInfo.IsTrueTypeFontBold = false;
            labelTextInfo.Alignment = AlignmentType.Right;

            //text customization for values in request details
            valueTextInfo.FontSize = 12;
            valueTextInfo.IsTrueTypeFontBold = true;
            valueTextInfo.Alignment = AlignmentType.Left;

            //adding cell holding label
            labelCell = row.Cells.Add(label, labelTextInfo);
            labelCell.IsNoBorder = true;
            labelCell.Padding.Top = 9;
            labelCell.Padding.Right = 9;
            labelCell.Padding.Bottom = 9;
            labelCell.Padding.Left = 9;

            //adding cell holding value
            valueCell = row.Cells.Add(value, valueTextInfo);
            valueCell.IsNoBorder = true;
            valueCell.IsWordWrapped = true;
            valueCell.Padding.Top = 9;
            valueCell.Padding.Right = 9;
            valueCell.Padding.Bottom = 9;
            valueCell.Padding.Left = 9;

            return row;
        }
    }
}
