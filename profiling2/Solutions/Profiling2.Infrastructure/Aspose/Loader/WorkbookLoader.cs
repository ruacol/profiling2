using System;
using System.Configuration;
using System.IO;
using Aspose.Cells;

namespace Profiling2.Infrastructure.Aspose.Loader
{
    public class WorkbookLoader : BaseAsposeLoader
    {
        protected Workbook Workbook { get; set; }

        public WorkbookLoader(Stream stream, string password)
            : base(stream)
        {
            try
            {
                LoadOptions loadOptions = new LoadOptions(LoadFormat.Auto);
                loadOptions.Password = password;
                this.Workbook = new Workbook(stream, loadOptions);
                this.Workbook.Settings.Password = password;

                try
                {
                    this.Workbook.Unprotect(password);
                }
                catch (Exception e)
                {
                    this.Exception = e;
                }
                this.IsPasswordCorrect = true;
            }
            catch (Exception e)
            {
                this.Exception = e;
            }
        }

        public override Stream GetUnprotectedStream(Stream destination)
        {
            if (this.IsPasswordCorrect && this.Workbook != null)
            {
                this.Workbook.Save(destination, SaveFormat.Auto);
                return destination;
            }
            return null;
        }

        /// <summary>
        /// Converts this Aspose Worksheet into HTML.  Copied from Profiling1.
        /// </summary>
        /// <returns>Failure text on exception.</returns>
        public override Stream GetHtml(Stream destination)
        {
            if (this.Workbook != null)
            {
                Range sourceRange, destinationRange;
                Worksheet destinationSheet = this.Workbook.Worksheets[0];
                this.UnhideCells(destinationSheet);

                if (this.Workbook.Worksheets.Count > 1)
                {
                    // Merge all worksheets into one (worksheet index = 0)
                    int totalRowCount = destinationSheet.Cells.MaxDisplayRange.RowCount;
                    for (int i = 1; i < this.Workbook.Worksheets.Count; i++)
                    {
                        this.UnhideCells(this.Workbook.Worksheets[i]);

                        sourceRange = this.Workbook.Worksheets[i].Cells.MaxDisplayRange;
                        destinationRange = destinationSheet.Cells.CreateRange(sourceRange.FirstRow + totalRowCount, sourceRange.FirstColumn, sourceRange.RowCount, sourceRange.ColumnCount);
                        destinationRange.Copy(sourceRange);
                        totalRowCount = sourceRange.RowCount + totalRowCount;
                    }
                    this.Workbook.Worksheets.ActiveSheetIndex = 0;
                }

                HtmlSaveOptions options = new HtmlSaveOptions(SaveFormat.Html);
                options.AttachedFilesDirectory = ConfigurationManager.AppSettings["PreviewTempFolder"];
                options.AttachedFilesUrlPrefix = "Images/";
                
                // required, otherwise generated HTML will include HTML resource files which aren't included in the stream, and aren't included in the 'AttachedFiles*'
                // parameters above.  see http://www.aspose.com/community/forums/thread/564003/how-to-save-all-worksheets-to-stream-in-html-format.aspx
                options.ExportActiveWorksheetOnly = true;

                this.Workbook.Save(destination, options);

                return destination;
            }
            throw new LoadSourceException(this.Exception.Message, this.Exception.InnerException);
        }

        // Unhide all cells so they appear in HTML
        protected void UnhideCells(Worksheet w)
        {
            foreach (Row r in w.Cells.Rows)
                if (r.IsHidden)
                    w.Cells.UnhideRow(r.Index, 9);
            foreach (Column c in w.Cells.Columns)
                if (c.IsHidden)
                    w.Cells.UnhideColumn(c.Index, 9);
        }
    }
}
