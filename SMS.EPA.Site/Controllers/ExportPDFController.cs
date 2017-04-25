namespace SMS.EPA.Site.Controllers
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Model;
    using Resources;
    using Services.Implement;
    using Services.Interfaces;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportPdfController : Controller
    {
        private const float TotalWidth = 590;
        private const int FontSize = 8;

        private readonly Color _defaultColor = new Color(System.Drawing.ColorTranslator.FromHtml("#333"));
        private readonly Color _redColor = new Color(System.Drawing.ColorTranslator.FromHtml("#df1501"));
        private readonly Color _whiteColor = new Color(System.Drawing.ColorTranslator.FromHtml("#fff"));
        private readonly IEmployeeEPAService _employeeEPAService;

        private Font _normalFont;
        private Font _boldFont;
        private Color _borderColor;
        private Color _backgroundColor;

        public ExportPdfController()
            : this(null)
        {
        }

        public ExportPdfController(IEmployeeEPAService employeeEPAService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public void ExportPdf(int epaId)
        {
            EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(epaId);
            string fileName = string.Format("{0}_{1}.pdf", epaInfo.Employee.Code, epaInfo.Employee.NickName);

            FontFactory.Register(Server.MapPath("~/content/fonts/ARIALUNI.TTF"), "ARIALUNI");
            _normalFont = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, FontSize, Font.NORMAL, _defaultColor);
            _boldFont = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, FontSize, Font.BOLD, _defaultColor);

            Document document = new Document(PageSize.A4, 0, 0, 30f, 30f);
            PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = TotalWidth;
            table.DefaultCell.Border = 0;

            PdfPTable performanceResult, performanceDetail, recommendation;

            PdfPTable title = RenderTitle(epaInfo.Period.Name),
                generalInfo = RenderGeneralInfo(epaInfo.Employee),
                summaryComment = RenderSummaryComment(epaInfo);

            if (epaInfo.IsSemiHasMonthly)
            {
                performanceResult = RenderPerformanceResultForSemiHasMonthly(epaInfo);
                performanceDetail = RenderPerformanceDetailForSemiHasMonthly(epaInfo);
            }
            else
            {
                performanceResult = RenderPerformanceResult(epaInfo);
                performanceDetail = RenderPerformanceDetail(epaInfo);
            }

            table.AddCell(title);
            table.AddCell(generalInfo);
            table.AddCell(performanceResult);

            if (epaInfo.Recommendation != null)
            {
                if (epaInfo.IsProbation || epaInfo.IsPromotion)
                {
                    recommendation = RenderRecommendationForProbationAndPromotion(epaInfo);
                    table.AddCell(recommendation);
                }
                else if (epaInfo.IsSemiAnnual)
                {
                    recommendation = RenderRecommendationForSemi(epaInfo);
                    table.AddCell(recommendation);
                }
            }

            document.Add(table);
            document.Add(performanceDetail);
            document.Add(summaryComment);
            document.Close();

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;" + "filename=" + fileName);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(document);
            Response.End();
        }

        private PdfPTable RenderTitle(string periodName)
        {
            PdfPTable table = new PdfPTable(1)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.BOLD, _defaultColor);
            PdfPCell cell = new PdfPCell(new Phrase(Lang.EPATilte + " - " + periodName, font));
            cell.FixedHeight = 30;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));
            cell.Border = 0;
            cell.PaddingLeft = 12f;
            cell.PaddingRight = 12f;
            table.AddCell(cell);

            return table;
        }

        private string GetAvatar(string code)
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
                TotalWidth = TotalWidth,
                LockedWidth = true
            };

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ddd"));

            PdfPCell cell = new PdfPCell(new Phrase(Lang.GeneralInfo, font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            string imagePath = GetAvatar(employeeInfo.Code);
            Image img = Image.GetInstance(imagePath);
            img.ScaleToFit(60, 60);

            cell = new PdfPCell(img, false)
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 4,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 1,
                BorderWidthBottom = 1
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeCode, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.Code, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.NickName, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 1;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FullName, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.FullName, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.JobTitle, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.JobTitleName, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 1;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Department, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.DepartmentName, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Section, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.GroupName, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 1;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.GroupDate, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 1;
            cell.PaddingBottom = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.JoinedDate.ToString("dd MMM, yyyy"), _normalFont));
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.BorderColor = _borderColor;
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
            var totalCol = !epaInfo.Process.Managements.Any() ? 8 : 10;

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            _backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(totalCol)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = _borderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.PerformanceResult.ToUpper(), font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = totalCol;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Employee, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Evaluator, _normalFont));
            cell.Colspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Reviewer, _normalFont));
            cell.Colspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management, _normalFont));
                cell.Colspan = 2;
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.DisciplineScore, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FinalScore, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Grade, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            EPAProcessModel self = epaInfo.Process.Self;
            var totalScore = self.IsEndProcess
                ? (string.IsNullOrEmpty(self.TotalScore) ? Lang.NotAvailable : self.TotalScore + " / " + epaInfo.ScoreRatingType.SelfEvaluated)
                : Lang.Pending;
            cell = new PdfPCell(new Phrase(totalScore, _normalFont));
            cell.Rowspan = epaInfo.Rowspan;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            for (int index = 0; index < epaInfo.Rowspan; index++)
            {
                if (index < epaInfo.Process.Evaluators.Count())
                {
                    EPAProcessModel evaluator = epaInfo.Process.Evaluators[index];
                    cell = new PdfPCell(new Phrase(epaInfo.Process.Evaluators[index].EvaluatorInfo.NickName, _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    totalScore =
                            evaluator.IsEndProcess
                            ? (string.IsNullOrEmpty(evaluator.TotalScore) ? Lang.NotAvailable : evaluator.TotalScore + " / " + epaInfo.ScoreRatingType.Evaluator)
                            : Lang.Pending;
                    cell = new PdfPCell(new Phrase(totalScore, _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (index < epaInfo.Process.Reviewers.Count())
                {
                    EPAProcessModel reviewer = epaInfo.Process.Reviewers[index];
                    cell = new PdfPCell(new Phrase(reviewer.EvaluatorInfo.NickName, _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    totalScore =
                            reviewer.IsEndProcess
                            ? (string.IsNullOrEmpty(reviewer.TotalScore) ? Lang.NotAvailable : reviewer.TotalScore + " / " + epaInfo.ScoreRatingType.Reviewer)
                            : Lang.Pending;
                    cell = new PdfPCell(new Phrase(totalScore, _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (epaInfo.Process.Managements.Any())
                {
                    if (index < epaInfo.Process.Managements.Count())
                    {
                        EPAProcessModel management = epaInfo.Process.Managements[index];
                        cell = new PdfPCell(new Phrase(management.EvaluatorInfo.NickName, _normalFont));
                        cell.BorderColor = _borderColor;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        totalScore =
                            management.IsEndProcess
                            ? (string.IsNullOrEmpty(management.TotalScore) ? Lang.NotAvailable : management.TotalScore + " / " + epaInfo.ScoreRatingType.Management)
                            : Lang.Pending;
                        cell = new PdfPCell(new Phrase(totalScore, _normalFont));
                        cell.BorderColor = _borderColor;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);
                    }
                    else
                    {
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                    }
                }

                if (index == 0)
                {
                    var disciplineScore = epaInfo.HasDisciplineScore ? epaInfo.DisciplineScore.ToString(CultureInfo.InvariantCulture) + " / " + epaInfo.MaxDisciplineScore : "-";
                    cell = new PdfPCell(new Phrase(disciplineScore, _normalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    string finalScore =
                        epaInfo.Period.IsEndPeriod
                        ? (string.IsNullOrEmpty(epaInfo.FinalScore) ? Lang.NotAvailable : epaInfo.FinalScore + " / " + epaInfo.MaxFinalScore)
                        : Lang.Pending;
                    cell = new PdfPCell(new Phrase(finalScore, _normalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    string grade = epaInfo.Period.IsEndPeriod ? (string.IsNullOrEmpty(epaInfo.Grade) ? Lang.NotAvailable : epaInfo.Grade) : Lang.Pending;
                    cell = new PdfPCell(new Phrase(grade, _normalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                }
            }

            return table;
        }

        private PdfPTable RenderPerformanceDetail(EPAModel epaInfo)
        {
            float[] widths = new float[] { 20, 190, 40, 120, 60, 40, 120 };
            PdfPTable detail = new PdfPTable(7)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            detail.SetWidths(widths);
            detail.DefaultCell.BorderColor = _borderColor;

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            _backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            foreach (EPAKpiGroupModel kpiGroup in epaInfo.KPIGroups)
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

                cell = new PdfPCell(new Phrase(Lang.SymbolCount, _normalFont));
                cell.Rowspan = 2;
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.KPI, _normalFont));
                cell.Rowspan = 2;
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Employee, _normalFont));
                cell.Colspan = 2;
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Evaluator, _normalFont));
                cell.Colspan = 3;
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Score, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Comment, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Score, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Comment, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                detail.AddCell(cell);

                int rowspan = epaInfo.Process.Evaluators.Count();
                for (int index = 0; index < kpiGroup.KPIs.Count(); index++)
                {
                    cell = new PdfPCell(new Phrase((index + 1).ToString(), _normalFont));
                    cell.Rowspan = rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    detail.AddCell(cell);

                    Phrase phrase = new Phrase();
                    phrase.Add(new Chunk(kpiGroup.KPIs[index].Name, _normalFont));
                    if (kpiGroup.NumberKPIOptional == 0)
                    {
                        phrase.Add(new Chunk(" (" + kpiGroup.KPIs[index].WeightToDisplay + " pts)", _boldFont));
                    }

                    Font font1 = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _redColor);
                    if (kpiGroup.KPIs[index].IsRequiredEvaluate)
                    {
                        phrase.Add(new Chunk(" (*)", font1));
                    }

                    cell = new PdfPCell(phrase);
                    cell.Rowspan = rowspan;
                    cell.BorderColor = _borderColor;
                    detail.AddCell(cell);

                    EvaluationModel self = kpiGroup.KPIs[index].EvaluationBySelf;
                    var score = epaInfo.Process.Self.IsEndProcess
                        ? (string.IsNullOrEmpty(self.Score) ? Lang.NotAvailable : self.ScoreToDisplay + " pts")
                        : Lang.Pending;
                    cell = new PdfPCell(new Phrase(score, _normalFont));
                    cell.Rowspan = rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    detail.AddCell(cell);

                    cell = new PdfPCell(new Phrase(epaInfo.Process.Self.IsEndProcess ? self.Comment : Lang.Pending, _normalFont));
                    cell.Rowspan = rowspan;
                    cell.BorderColor = _borderColor;
                    detail.AddCell(cell);

                    foreach (EvaluationModel evaluator in kpiGroup.KPIs[index].EvaluationByEvaluators)
                    {
                        cell = new PdfPCell(new Phrase(evaluator.NickName, _normalFont));
                        cell.BorderColor = _borderColor;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        detail.AddCell(cell);

                        score =
                            epaInfo.Process.Evaluators.First().IsEndProcess
                            ? (string.IsNullOrEmpty(evaluator.Score) ? Lang.NotAvailable : evaluator.ScoreToDisplay + " pts")
                            : Lang.Pending;
                        cell = new PdfPCell(new Phrase(score, _normalFont));
                        cell.BorderColor = _borderColor;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        detail.AddCell(cell);

                        cell = new PdfPCell(new Phrase(epaInfo.Process.Evaluators.First().IsEndProcess ? evaluator.Comment : Lang.Pending, _normalFont));
                        cell.BorderColor = _borderColor;
                        detail.AddCell(cell);
                    }
                }
            }

            return detail;
        }

        private PdfPTable RenderPerformanceResultForSemiHasMonthly(EPAModel epaInfo)
        {
            int totalCol;
            string totalScore;

            if (!epaInfo.Process.Managements.Any())
            {
                totalCol = 7;
            }
            else
            {
                totalCol = 9;
            }

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            _backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(totalCol)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = _borderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.PerformanceResult.ToUpper(), font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = totalCol;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeScore, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EvaluatorScore, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Reviewer, _normalFont));
            cell.Colspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management, _normalFont));
                cell.Colspan = 2;
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.DisciplineScore, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FinalScore, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Grade, _normalFont));
            cell.Rowspan = 2;
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont));
                cell.BackgroundColor = _backgroundColor;
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            totalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted.ToString() + " / " + epaInfo.ScoreRatingType.SelfEvaluated;
            cell = new PdfPCell(new Phrase(totalScore, _normalFont));
            cell.Rowspan = epaInfo.Rowspan;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted.ToString() + " / " + epaInfo.ScoreRatingType.Evaluator;
            cell = new PdfPCell(new Phrase(totalScore, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            for (int index = 0; index < epaInfo.Rowspan; index++)
            {
                if (index < epaInfo.Process.Reviewers.Count())
                {
                    EPAProcessModel reviewer = epaInfo.Process.Reviewers[index];
                    cell = new PdfPCell(new Phrase(reviewer.EvaluatorInfo.NickName, _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    totalScore =
                        reviewer.IsEndProcess
                        ? (string.IsNullOrEmpty(reviewer.TotalScore) ? Lang.NotAvailable : reviewer.TotalScore + " / " + epaInfo.ScoreRatingType.Reviewer)
                        : Lang.Pending;
                    cell = new PdfPCell(new Phrase(totalScore, _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (epaInfo.Process.Managements.Any())
                {
                    if (index < epaInfo.Process.Managements.Count())
                    {
                        EPAProcessModel management = epaInfo.Process.Managements[index];
                        cell = new PdfPCell(new Phrase(management.EvaluatorInfo.NickName, _normalFont));
                        cell.BorderColor = _borderColor;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        totalScore =
                            management.IsEndProcess
                            ? (string.IsNullOrEmpty(management.TotalScore) ? Lang.NotAvailable : management.TotalScore + " / " + epaInfo.ScoreRatingType.Management)
                            : Lang.Pending;
                        cell = new PdfPCell(new Phrase(totalScore, _normalFont));
                        cell.BorderColor = _borderColor;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);
                    }
                    else
                    {
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                    }
                }

                if (index == 0)
                {
                    var disciplineScore = epaInfo.HasDisciplineScore ? epaInfo.DisciplineScore + " / " + epaInfo.MaxDisciplineScore : "-";
                    cell = new PdfPCell(new Phrase(disciplineScore, _normalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    string finalScore =
                        epaInfo.Period.IsEndPeriod
                        ? (string.IsNullOrEmpty(epaInfo.FinalScore) ? Lang.NotAvailable : epaInfo.FinalScore + " / " + epaInfo.MaxFinalScore)
                        : Lang.Pending;
                    cell = new PdfPCell(new Phrase(finalScore, _normalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    string grade = epaInfo.Period.IsEndPeriod ? (string.IsNullOrEmpty(epaInfo.Grade) ? Lang.NotAvailable : epaInfo.Grade) : Lang.Pending;
                    cell = new PdfPCell(new Phrase(grade, _normalFont));
                    cell.Rowspan = epaInfo.Rowspan;
                    cell.BorderColor = _borderColor;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);
                }
            }

            return table;
        }

        private PdfPTable RenderPerformanceDetailForSemiHasMonthly(EPAModel epaInfo)
        {
            string totalScore;

            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            _backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(4)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = _borderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.MonthlyEPA, font));
            cell.MinimumHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.SymbolCount, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeScore, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EvaluatorScore, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.ReviewerScore, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            foreach (EPAModel monthly in epaInfo.MonthlyEPA.MonthlyEPAs)
            {
                cell = new PdfPCell(new Phrase(monthly.Period.Name, _normalFont));
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                totalScore = monthly.Process.Self.TotalScore.Length > 0 ? monthly.Process.Self.TotalScore : "0";
                cell = new PdfPCell(new Phrase(totalScore, _normalFont));
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(monthly.Process.AverageScoreByEvaluators.ToString(), _normalFont));
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(monthly.Process.AverageScoreByReviewers.ToString(), _normalFont));
                cell.BorderColor = _borderColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.MonthlyAverage, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageSelfScore.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.SelfEvaluated, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScores.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.Evaluator, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageReviewerScores.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.Reviewer, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.SemiAverage, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.SelfEvaluated, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Evaluator, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageReviewerScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Reviewer, _normalFont));
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            return table;
        }

        private PdfPTable RenderSummaryComment(EPAModel epaInfo)
        {
            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            _backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            float[] widths = new float[] { 140, 450 };
            PdfPTable table = new PdfPTable(2)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.SetWidths(widths);
            table.DefaultCell.BorderColor = _borderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.SummaryComment, font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FullName, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Comment, _normalFont));
            cell.BackgroundColor = _backgroundColor;
            cell.BorderColor = _borderColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            if (!epaInfo.IsSemiHasMonthly)
            {
                EPAProcessModel self = epaInfo.Process.Self;
                cell = new PdfPCell(new Phrase(self.EvaluatorInfo.FullName + " (" + self.EvaluatorInfo.NickName + ")", _normalFont));
                cell.BorderColor = _borderColor;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(self.IsEndProcess ? self.Comment : Lang.Pending, _normalFont));
                cell.BorderColor = _borderColor;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Evaluator, _boldFont));
                cell.Colspan = 2;
                cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ececec"));
                cell.BorderColor = _borderColor;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                foreach (EPAProcessModel evaluator in epaInfo.Process.Evaluators)
                {
                    cell = new PdfPCell(new Phrase(evaluator.EvaluatorInfo.FullName + " (" + evaluator.EvaluatorInfo.NickName + ")", _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(evaluator.IsEndProcess ? evaluator.Comment : Lang.Pending, _normalFont));
                    cell.BorderColor = _borderColor;
                    table.AddCell(cell);
                }
            }

            cell = new PdfPCell(new Phrase(Lang.Reviewer, _boldFont));
            cell.Colspan = 2;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ececec"));
            cell.BorderColor = _borderColor;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            foreach (EPAProcessModel reviewer in epaInfo.Process.Reviewers)
            {
                cell = new PdfPCell(new Phrase(reviewer.EvaluatorInfo.FullName + " (" + reviewer.EvaluatorInfo.NickName + ")", _normalFont));
                cell.BorderColor = _borderColor;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(reviewer.IsEndProcess ? reviewer.Comment : Lang.Pending, _normalFont));
                cell.BorderColor = _borderColor;
                table.AddCell(cell);
            }

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management, _boldFont));
                cell.Colspan = 2;
                cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ececec"));
                cell.BorderColor = _borderColor;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                foreach (EPAProcessModel management in epaInfo.Process.Managements)
                {
                    cell = new PdfPCell(new Phrase(management.EvaluatorInfo.FullName + " (" + management.EvaluatorInfo.NickName + ")", _normalFont));
                    cell.BorderColor = _borderColor;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(management.IsEndProcess ? management.Comment : Lang.Pending, _normalFont));
                    cell.BorderColor = _borderColor;
                    table.AddCell(cell);
                }
            }

            return table;
        }

        private PdfPTable RenderRecommendationForSemi(EPAModel epaInfo)
        {
            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            float[] widths = new float[] { 10, 580 };

            PdfPTable table = new PdfPTable(2)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.SetWidths(widths);
            table.DefaultCell.Border = 0;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.Recommendation, font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            string imagePath = GetCheckbox(epaInfo.Recommendation.Promoted);
            Image img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false);
            cell.PaddingTop = 3;
            cell.PaddingLeft = 2;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Promotion, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            imagePath = GetCheckbox(epaInfo.Recommendation.PayIncreased);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false);
            cell.PaddingTop = 3;
            cell.PaddingLeft = 2;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.AnnualIncrement, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            imagePath = GetCheckbox(epaInfo.Recommendation.JobGradeIncreased);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false);
            cell.PaddingTop = 3;
            cell.PaddingLeft = 2;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            table.AddCell(cell);

            string jobGrade = epaInfo.Recommendation.JobGrade != null ? Lang.JobGrade + ": " + epaInfo.Recommendation.JobGrade.Name : Lang.JobGrade;
            cell = new PdfPCell(new Phrase(jobGrade, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            table.AddCell(cell);

            return table;
        }

        private PdfPTable RenderRecommendationForProbationAndPromotion(EPAModel epaInfo)
        {
            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _whiteColor);
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            float[] widths = new float[] { 10, 580 };

            PdfPTable table = new PdfPTable(2)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.SetWidths(widths);
            table.DefaultCell.Border = 0;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.Recommendation, font));
            cell.FixedHeight = 20;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e"));
            cell.Border = 0;
            table.AddCell(cell);

            string imagePath = GetRadioButton(epaInfo.Recommendation.PassedProbation);
            Image img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false);
            cell.PaddingTop = 3;
            cell.PaddingLeft = 2;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PassProbation, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            imagePath = GetRadioButton(epaInfo.Recommendation.ExtendedProbation);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false);
            cell.PaddingTop = 3;
            cell.PaddingLeft = 2;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.ExtendProbation, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);

            imagePath = GetRadioButton(epaInfo.Recommendation.FailedProbation);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false);
            cell.PaddingTop = 3;
            cell.PaddingLeft = 2;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FailProbation, _normalFont));
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            table.AddCell(cell);

            return table;
        }

        private string GetRadioButton(bool value)
        {
            string path = Server.MapPath("~/content/site/images/radiobutton_no.png");
            if (value)
            {
                path = Server.MapPath("~/content/site/images/radiobutton_yes.png");
            }

            return path;
        }

        private string GetCheckbox(bool value)
        {
            string path = Server.MapPath("~/content/site/images/checkbox_no.png");
            if (value)
            {
                path = Server.MapPath("~/content/site/images/checkbox_yes.png");
            }

            return path;
        }
    }
}