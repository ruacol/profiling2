using Aspose.Words;
using Aspose.Words.Saving;
using System.Drawing;
using System.IO;

namespace Profiling2.Infrastructure.Export.Profiling
{
    public abstract class BaseWordExportService
    {
        protected byte[] GetBytes(Document doc)
        {
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                DocSaveOptions opts = new DocSaveOptions(SaveFormat.Doc);
                doc.Save(ms, opts);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        protected DocumentBuilder InsertSectionTitle(DocumentBuilder builder, string title)
        {
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading3;
            builder.Font.Bold = true;
            builder.Writeln(title);
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder.Font.Bold = false;
            builder.InsertBreak(BreakType.ParagraphBreak);
            return builder;
        }

        protected DocumentBuilder AddHeaderAndFooter(DocumentBuilder builder, string lastModifiedDate)
        {
            //start at beginning of the document
            builder.MoveToDocumentStart();

            PageSetup pageSetup = builder.CurrentSection.PageSetup;

            //save key current document settings
            ParagraphAlignment previousAlignment = builder.ParagraphFormat.Alignment;
            Color previousFontColor = builder.Font.Color;
            double previousFontSize = builder.Font.Size;
            //caculating a table width for the footer by subtracting the margins (will be used to create two columns to align text left and right)
            double tableWidth = pageSetup.PageWidth - pageSetup.LeftMargin - pageSetup.RightMargin;

            pageSetup.DifferentFirstPageHeaderFooter = false;
            //1 inch = 72 points, distance below specified in points
            pageSetup.HeaderDistance = 36;
            pageSetup.FooterDistance = 36;

            //adds header
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.Font.Color = Color.Red;
            builder.Font.Size = 10;
            builder.Writeln("CONFIDENTIAL");

            //adds footer
            //builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            //builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            //builder.Font.Color = Color.Red;
            //builder.Font.Size = 10;
            //builder.Writeln("PROFILING PROJECT DOCUMENT");

            //adds page numbering and last modified date
            builder.StartTable();
            //last modified date
            builder.InsertCell();
            builder.CellFormat.Width = tableWidth / 3;
            builder.Font.Color = Color.Black;
            builder.CurrentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            builder.Writeln("Last Modified");
            builder.Writeln(lastModifiedDate);

            //page numbering
            builder.InsertCell();
            builder.CellFormat.Width = tableWidth * 2 / 3;
            builder.Font.Color = Color.Black;
            builder.InsertField("PAGE");
            builder.Write(" of ");
            builder.InsertField("NUMPAGES");
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            builder.EndRow();
            builder.EndTable();

            //go back to the beginning of the document
            builder.MoveToDocumentStart();

            //restore key current document settings
            builder.ParagraphFormat.Alignment = previousAlignment;
            builder.Font.Color = previousFontColor;
            builder.Font.Size = previousFontSize;

            return builder;
        }
    }
}
