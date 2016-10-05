namespace SMS.EPA.Site.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportPDFController : Controller
    {
        private readonly IEmployeeEPAService employeeEPAService;

        private float totalWidth = 800;
        private Color defaultColor = new Color(System.Drawing.ColorTranslator.FromHtml("#333"));
        private Color redColor = new Color(System.Drawing.ColorTranslator.FromHtml("#df1501"));
        private Color whiteColor = new Color(System.Drawing.ColorTranslator.FromHtml("#fff"));

        public ExportPDFController()
            : this(null)
        {
        }

        public ExportPDFController(IEmployeeEPAService employeeEPAService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        private Font NormalFont { get; set; }

        private Font BoldFont { get; set; }

        private Color BorderColor { get; set; }

        private Color BackgroundColor { get; set; }

        public void ExportPdf(int epaId)
        {
            EPAModel epaInfo = employeeEPAService.GetEPAInfoById(epaId);
            string fileName = epaInfo.Employee.NickName + "_" + epaInfo.Employee.Code + ".pdf";

            FontFactory.Register(Server.MapPath("~/content/fonts/ARIALUNI.TTF"), "ARIALUNI");
            NormalFont = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, defaultColor);
            BoldFont = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.BOLD, defaultColor);

            Document document = new Document(PageSize.A3, 0, 0, 50f, 50f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            writer.AddViewerPreference(PdfName.HIDEMENUBAR, new PdfBoolean(true));
            document.Open();

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = totalWidth;
            table.DefaultCell.Border = 0;

            PdfPTable performanceResult, performanceDetail;

            PdfPTable title = RenderTitle(epaInfo.Period.Name);
            PdfPTable generalInfo = RenderGeneralInfo(epaInfo.Employee);
            table.AddCell(title);
            table.AddCell(generalInfo);

            if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetTypes.SemiAnnual)
            {
                performanceResult = RenderPerformanceResultForSemiHasMonthly(epaInfo);
                performanceDetail = RenderPerformanceDetailForSemiHasMonthly(epaInfo);
            }
            else
            {
                performanceResult = RenderPerformanceResult(epaInfo);
                performanceDetail = RenderPerformanceDetail(epaInfo);
            }

            table.AddCell(performanceResult);
            table.AddCell(performanceDetail);
            table.SplitLate = false;

            document.Add(table);
            document.Close();

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;" + "filename=" + fileName);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(document);
            Response.End();
        }

        private PdfPTable RenderTitle(string periodName)
        {
            Paragraph p = new Paragraph();

            PdfPTable table = new PdfPTable(1)
            {
                TotalWidth = totalWidth,
                LockedWidth = true
            };
            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 12, Font.BOLD, defaultColor);
            PdfPCell cell = new PdfPCell(new Phrase(Lang.EPATilte + " - " + periodName, font));
            cell.FixedHeight = 30;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));
            cell.Border = 0;
            cell.PaddingLeft = 12f;
            cell.PaddingRight = 12f;
            table.AddCell(cell);

            return table;
        }

        private string GetImagePath(string code)
        {
            string path = System.IO.File.Exists(Server.MapPath("~/avatars/avatar.jpg")) ? Server.MapPath("~/avatars/avatar.jpg") : string.Empty;
            code += ".jpg";

            if (System.IO.File.Exists(Server.MapPath("~/avatars/" + code)))
            {
                path = Server.MapPath("~/avatars/" + code);
            }

            return path;
        }

        private PdfPTable RenderGeneralInfo(EPAEmployeeModel employeeInfo)
        {
            PdfPTable table = new PdfPTable(5)
            {
                TotalWidth = totalWidth,
                LockedWidth = true
            };

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.NORMAL, whiteColor);
            BorderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ddd"));

            PdfPCell cell = new PdfPCell(new Phrase(Lang.GeneralInfo, font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            string imagePath = GetImagePath(employeeInfo.Code);
            Image img = Image.GetInstance(imagePath);
            img.ScaleToFit(80, 80);

            cell = new PdfPCell(img, false)
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 4,
                BorderColor = BorderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 1,
                BorderWidthBottom = 1
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeCode, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.Code, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.NickName, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 1;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FullName, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.FullName, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.JobTitle, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.JobTitleName, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 1;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Department, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.DepartmentName, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Section, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.GroupName, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 1;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.GroupDate, NormalFont));
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.JoinedDate.ToString("dd MMM, yyyy"), NormalFont));
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.BorderColor = BorderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 1;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            return table;
        }

        private PdfPTable RenderPerformanceResult(EPAModel epaInfo)
        {
            int totalCol = 0;
            string disciplineScore = string.Empty,
                totalScore = string.Empty;

            if (!epaInfo.Process.Managements.Any())
            {
                totalCol = 11;
            }
            else
            {
                totalCol = 14;
            }

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.NORMAL, whiteColor);
            BorderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(totalCol)
            {
                TotalWidth = totalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = BorderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.PerformanceResult.ToUpper(), font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = totalCol;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Employee, NormalFont));
            cell.Colspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Evaluator + " (" + epaInfo.Process.AverageScoreByEvaluators + ")", NormalFont));
            cell.Colspan = 3;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Reviewer + " (" + epaInfo.Process.AverageScoreByReviewers + ")", NormalFont));
            cell.Colspan = 3;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management + " (" + epaInfo.Process.AverageScoreByManagements + ")", NormalFont));
                cell.Colspan = 3;
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.DisciplineScore, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FinalScore, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Grade, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.NickName, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.PerformanceScore, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            totalScore = epaInfo.Process.Self.TotalScore.Length > 0 ? epaInfo.Process.Self.TotalScore : "0";
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.SelfEvaluated, NormalFont));
            cell.Rowspan = epaInfo.Rowspan;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(epaInfo.Process.Self.Comment, NormalFont));
            cell.Rowspan = epaInfo.Rowspan;
            cell.BorderColor = BorderColor;
            table.AddCell(cell);

            for (int index = 0; index < epaInfo.Rowspan; index++)
            {
                if (index < epaInfo.Process.Evaluators.Count())
                {
                    cell = new PdfPCell(new Phrase(epaInfo.Process.Evaluators[index].NickName, NormalFont));
                    cell.BorderColor = BorderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    totalScore = epaInfo.Process.Evaluators[index].TotalScore.Length > 0 ? epaInfo.Process.Evaluators[index].TotalScore : "0";
                    cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Evaluator, NormalFont));
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.Process.Evaluators[index].Comment, NormalFont));
                    cell.BorderColor = BorderColor;
                    table.AddCell(cell);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (index < epaInfo.Process.Reviewers.Count())
                {
                    cell = new PdfPCell(new Phrase(epaInfo.Process.Reviewers[index].NickName, NormalFont));
                    cell.BorderColor = BorderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    totalScore = epaInfo.Process.Reviewers[index].TotalScore.Length > 0 ? epaInfo.Process.Reviewers[index].TotalScore : "0";
                    cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Reviewer, NormalFont));
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.Process.Reviewers[index].Comment, NormalFont));
                    cell.BorderColor = BorderColor;
                    table.AddCell(cell);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (epaInfo.Process.Managements.Any())
                {
                    if (index < epaInfo.Process.Managements.Count())
                    {
                        cell = new PdfPCell(new Phrase(epaInfo.Process.Managements[index].NickName, NormalFont));
                        cell.BorderColor = BorderColor;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        totalScore = epaInfo.Process.Managements[index].TotalScore.Length > 0 ? epaInfo.Process.Managements[index].TotalScore : "0";
                        cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Management, NormalFont));
                        cell.BorderColor = BorderColor;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(epaInfo.Process.Managements[index].Comment, NormalFont));
                        cell.BorderColor = BorderColor;
                        table.AddCell(cell);
                    }
                    else
                    {
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                    }
                }

                if (index == 0)
                {
                    disciplineScore = epaInfo.HasDisciplineScore ? epaInfo.DisciplineScore.ToString() + " / " + epaInfo.MaxDisciplineScore : "-";
                    cell = new PdfPCell(new Phrase(disciplineScore, NormalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.FinalScore + " / " + epaInfo.MaxFinalScore, NormalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.Grade, NormalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                }
            }

            return table;
        }

        private PdfPTable RenderPerformanceDetail(EPAModel epaInfo)
        {
            float[] widths = new float[] { 30, 280, 40, 180, 50, 40, 180 };
            PdfPTable detail = new PdfPTable(7)
            {
                TotalWidth = totalWidth,
                LockedWidth = true
            };
            detail.SetWidths(widths);
            detail.DefaultCell.BorderColor = BorderColor;

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.NORMAL, whiteColor);
            BorderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
            {
                PdfPCell cell = new PdfPCell(new Phrase(kpiGroup.Name + " (" + kpiGroup.Weight + " pts)", font));
                cell.Colspan = 7;
                cell.MinimumHeight = 20;
                cell.PaddingLeft = 5;
                cell.PaddingRight = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
                cell.Border = 0;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.SymbolCount, NormalFont));
                cell.Rowspan = 2;
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.KPI, NormalFont));
                cell.Rowspan = 2;
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Employee, NormalFont));
                cell.Colspan = 2;
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Evaluator, NormalFont));
                cell.Colspan = 3;
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Score, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.NickName, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Score, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                int rowspan = epaInfo.Process.Evaluators.Count();
                var score = string.Empty;
                for (int index = 0; index < kpiGroup.KPIs.Count(); index++)
                {
                    cell = new PdfPCell(new Phrase((index + 1).ToString(), NormalFont));
                    cell.Rowspan = rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    detail.AddCell(cell);

                    Phrase phrase = new Phrase();
                    phrase.Add(new Chunk(kpiGroup.KPIs[index].Name, NormalFont));
                    if (kpiGroup.NumberKPIOptional == 0)
                    {
                        phrase.Add(new Chunk(" (" + kpiGroup.KPIs[index].WeightToDisplay + " pts)", BoldFont));
                    }

                    Font font1 = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.NORMAL, redColor);
                    if (kpiGroup.KPIs[index].IsRequiredEvaluate)
                    {
                        phrase.Add(new Chunk(" (*)", font1));
                    }

                    cell = new PdfPCell(phrase);
                    cell.Rowspan = rowspan;
                    cell.BorderColor = BorderColor;
                    detail.AddCell(cell);

                    score = kpiGroup.KPIs[index].EvaluationBySelf.Score.Length > 0 ? kpiGroup.KPIs[index].EvaluationBySelf.ScoreToDisplay : "0";
                    cell = new PdfPCell(new Phrase(score + " pts", NormalFont));
                    cell.Rowspan = rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    detail.AddCell(cell);

                    cell = new PdfPCell(new Phrase(kpiGroup.KPIs[index].EvaluationBySelf.Comment, NormalFont));
                    cell.Rowspan = rowspan;
                    cell.BorderColor = BorderColor;
                    detail.AddCell(cell);

                    foreach (EvaluationModel evaluator in kpiGroup.KPIs[index].EvaluationByEvaluators)
                    {
                        cell = new PdfPCell(new Phrase(evaluator.NickName, NormalFont));
                        cell.BorderColor = BorderColor;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        detail.AddCell(cell);

                        score = evaluator.Score.Length > 0 ? evaluator.ScoreToDisplay : "0";
                        cell = new PdfPCell(new Phrase(score + " pts", NormalFont));
                        cell.BorderColor = BorderColor;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        detail.AddCell(cell);

                        cell = new PdfPCell(new Phrase(evaluator.Comment, NormalFont));
                        cell.BorderColor = BorderColor;
                        detail.AddCell(cell);
                    }
                }
            }

            return detail;
        }

        private PdfPTable RenderPerformanceResultForSemiHasMonthly(EPAModel epaInfo)
        {
            int totalCol = 0;
            string disciplineScore = string.Empty,
                totalScore = string.Empty;

            if (!epaInfo.Process.Managements.Any())
            {
                totalCol = 8;
            }
            else
            {
                totalCol = 11;
            }

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.NORMAL, whiteColor);
            BorderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(totalCol)
            {
                TotalWidth = totalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = BorderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.PerformanceResult.ToUpper(), font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = totalCol;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeScore, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EvaluatorScore, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Reviewer, NormalFont));
            cell.Colspan = 3;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management, NormalFont));
                cell.Colspan = 3;
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.DisciplineScore, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FinalScore, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Grade, NormalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.NickName, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.PerformanceScore, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Comment, NormalFont));
                cell.BackgroundColor = BackgroundColor;
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            totalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted > 0 ? epaInfo.MonthlyEPA.AverageSelfScoreConverted.ToString() : "0";
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.SelfEvaluated, NormalFont));
            cell.Rowspan = epaInfo.Rowspan;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted > 0 ? epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted.ToString() : "0";
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Evaluator, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            for (int index = 0; index < epaInfo.Rowspan; index++)
            {
                if (index < epaInfo.Process.Reviewers.Count())
                {
                    cell = new PdfPCell(new Phrase(epaInfo.Process.Reviewers[index].NickName, NormalFont));
                    cell.BorderColor = BorderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    totalScore = epaInfo.Process.Reviewers[index].TotalScore.Length > 0 ? epaInfo.Process.Reviewers[index].TotalScore : "0";
                    cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Reviewer, NormalFont));
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.Process.Reviewers[index].Comment, NormalFont));
                    cell.BorderColor = BorderColor;
                    table.AddCell(cell);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (epaInfo.Process.Managements.Any())
                {
                    if (index < epaInfo.Process.Managements.Count())
                    {
                        cell = new PdfPCell(new Phrase(epaInfo.Process.Managements[index].NickName, NormalFont));
                        cell.BorderColor = BorderColor;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        totalScore = epaInfo.Process.Managements[index].TotalScore.Length > 0 ? epaInfo.Process.Managements[index].TotalScore : "0";
                        cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Management, NormalFont));
                        cell.BorderColor = BorderColor;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(epaInfo.Process.Managements[index].Comment, NormalFont));
                        cell.BorderColor = BorderColor;
                        table.AddCell(cell);
                    }
                    else
                    {
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                    }
                }

                if (index == 0)
                {
                    disciplineScore = epaInfo.HasDisciplineScore ? epaInfo.DisciplineScore.ToString() + " / " + epaInfo.MaxDisciplineScore : "-";
                    cell = new PdfPCell(new Phrase(disciplineScore, NormalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.FinalScore + " / " + epaInfo.MaxFinalScore, NormalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.Grade, NormalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = BorderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                }
            }

            return table;
        }

        private PdfPTable RenderPerformanceDetailForSemiHasMonthly(EPAModel epaInfo)
        {
            var totalScore = string.Empty;

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.NORMAL, whiteColor);
            BorderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(4)
            {
                TotalWidth = totalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = BorderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.MonthlyEPA, font));
            cell.MinimumHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.SymbolCount, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeScore, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EvaluatorScore, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.ReviewerScore, NormalFont));
            cell.BackgroundColor = BackgroundColor;
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            foreach (EPAModel monthly in epaInfo.MonthlyEPA.MonthlyEPAs)
            {
                cell = new PdfPCell(new Phrase(monthly.Period.Name, NormalFont));
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                totalScore = monthly.Process.Self.TotalScore.Length > 0 ? monthly.Process.Self.TotalScore : "0";
                cell = new PdfPCell(new Phrase(totalScore, NormalFont));
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(monthly.Process.AverageScoreByEvaluators.ToString(), NormalFont));
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(monthly.Process.AverageScoreByReviewers.ToString(), NormalFont));
                cell.BorderColor = BorderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.MonthlyAverage, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageSelfScore.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.SelfEvaluated, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScores.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.Evaluator, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageReviewerScores.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.Reviewer, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.SemiAverage, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.SelfEvaluated, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Evaluator, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageReviewerScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Reviewer, NormalFont));
            cell.BorderColor = BorderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            return table;
        }
    }
}