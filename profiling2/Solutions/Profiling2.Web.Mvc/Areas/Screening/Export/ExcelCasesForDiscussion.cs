using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Aspose.Cells;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Domain.Scr.PersonRecommendation;

namespace Profiling2.Web.Mvc.Areas.Screening.Export
{
    // Copied from Profiling1's ExcelPwgList class.
    public class ExcelCasesForDiscussion
    {
        public byte[] PdfBytes { get; set; }
        protected IList<RequestPerson> NominatedRequestPersons { get; set; }
        protected IList<ScreeningEntity> ScreeningEntities { get; set; }
        protected string UserScreeningEntityName { get; set; }

        public ExcelCasesForDiscussion(IList<RequestPerson> nominatedRequestPersons, 
            IList<ScreeningEntity> screeningEntities,
            string userScreeningEntityName)
        {
            this.NominatedRequestPersons = nominatedRequestPersons;
            this.ScreeningEntities = screeningEntities.OrderBy(x => x.Id).ToList();
            this.UserScreeningEntityName = userScreeningEntityName;

            Workbook workbook = ConstructWorkbook();
            this.PdfBytes = GetByteArray(workbook);
        }

        private Workbook ConstructWorkbook()
        {
            Workbook workBook = new Workbook();
            Worksheet workSheet = AddWorkSheet(ref workBook);
            workSheet = AddColumnHeadings(workSheet);
            for (int i = 0; i < this.NominatedRequestPersons.Count; i++)
            {
                RequestPerson rp = this.NominatedRequestPersons[i];
                workSheet.Cells[i + 1, 0].PutValue(rp.Person.Name);
                int j = 0;
                for (j = 0; j < this.ScreeningEntities.Count; j++)
                {
                    ScreeningEntity se = this.ScreeningEntities[j];
                    ScreeningRequestPersonEntity srpe = rp.GetMostRecentScreeningRequestPersonEntity(se.ScreeningEntityName);
                    if (srpe != null)
                    {
                        workSheet.Cells[i + 1, j + 1].PutValue(srpe.ScreeningResult.ToString());
                        workSheet.Cells[i + 1, j + 1].SetStyle(GetCellStyleForResult(srpe.ScreeningResult.ToString()));
                    }
                }
                //dsrsg
                ScreeningRequestPersonRecommendation srpr = rp.GetScreeningRequestPersonRecommendation();
                if (srpr != null)
                {
                    workSheet.Cells[i + 1, j + 1].PutValue(srpr.ScreeningResult.ToString());
                    workSheet.Cells[i + 1, j + 1].SetStyle(GetCellStyleForResult(srpr.ScreeningResult.ToString()));
                }
                //the user's commentary and reason; dsrsg commentary if no screening entity
                if (!string.IsNullOrEmpty(this.UserScreeningEntityName))
                {
                    ScreeningRequestPersonEntity srpe = rp.GetMostRecentScreeningRequestPersonEntity(this.UserScreeningEntityName);
                    if (srpe != null)
                    {
                        workSheet.Cells[i + 1, j + 2].PutValue(srpe.Reason);
                        workSheet.Cells[i + 1, j + 3].PutValue(srpe.Commentary);
                    }
                }
                else
                    if (srpr != null)
                        workSheet.Cells[i + 1, j + 3].PutValue(srpr.Commentary);
            }
            workSheet.AutoFitColumns();
            return workBook;
        }

        private Worksheet AddWorkSheet(ref Workbook workBook)
        {
            const string workSheetName = "Cases for Discussion";
            Worksheet workSheet;
            if (workBook.Worksheets.Count > 0)
            {
                workSheet = workBook.Worksheets[0];
                workSheet.Name = workSheetName;
            }
            else
            {
                workSheet = workBook.Worksheets.Add(workSheetName);
            }
            return workSheet;
        }

        private Worksheet AddColumnHeadings(Worksheet workSheet)
        {
            Style style = new Style();
            style.Font.IsBold = true;
            workSheet.Cells[0, 0].PutValue("Individual");
            workSheet.Cells[0, 0].SetStyle(style);
            for (int i = 1; i <= this.ScreeningEntities.Count; i++)
            {
                workSheet.Cells[0, i].PutValue(this.ScreeningEntities[i - 1]);
                workSheet.Cells[0, i].SetStyle(style);
            }
            workSheet.Cells[0, 4].PutValue("O-DSRSG RoL");
            workSheet.Cells[0, 4].SetStyle(style);
            workSheet.Cells[0, 5].PutValue("Reason");
            workSheet.Cells[0, 5].SetStyle(style);
            workSheet.Cells[0, 6].PutValue("Commentary");
            workSheet.Cells[0, 6].SetStyle(style);
            return workSheet;
        }

        private Style GetCellStyleForResult(string result)
        {
            Style style = new Style();
            style.Pattern = BackgroundType.Solid;
            switch (result.ToLower())
            {
                case "red":
                    style.ForegroundColor = Color.Red;
                    style.Font.Color = Color.Black;
                    break;
                case "yellow":
                    style.ForegroundColor = Color.Yellow;
                    style.Font.Color = Color.Black;
                    break;
                case "green":
                    style.ForegroundColor = Color.Green;
                    style.Font.Color = Color.White;
                    break;
            }
            return style;
        }

        public byte[] GetByteArray(Workbook workBook)
        {
            byte[] bytes;
            MemoryStream stream;
            stream = workBook.SaveToStream();
            try
            {
                bytes = stream.ToArray();
            }
            finally
            {
                stream.Dispose();
            }
            return bytes;
        }
    }
}