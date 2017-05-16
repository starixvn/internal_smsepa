namespace SMS.EPA.Site.Controllers
{
    using Core.Enumerations;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Model;
    using Resources;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [SuppressMessage(
        "StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
    public class ExportPdfController : Controller
    {
        private const int FontSize = 8;
        private const float TotalWidth = 590;
        private readonly Color _defaultColor = new Color(System.Drawing.ColorTranslator.FromHtml("#333"));
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly ICommonService _commonService;
        private readonly Color _redColor = new Color(System.Drawing.ColorTranslator.FromHtml("#df1501"));
        private readonly Color _whiteColor = new Color(System.Drawing.ColorTranslator.FromHtml("#fff"));
        private Font _boldFont;
        private Color _borderColor;
        private Font _defaultFont;
        private Font _normalFont;
        private Font _redFont;

        public ExportPdfController()
            : this(null, null)
        {
        }

        public ExportPdfController(IEmployeeEPAService employeeEPAService, ICommonService commonService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _commonService = commonService ?? new CommonService();

            SetDefaultStyles();
        }

        public void ExportPdf(int epaId)
        {
            EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(epaId);
            string fileName = $"{epaInfo.Employee.Code}_{epaInfo.Employee.NickName}.pdf";

            FontFactory.Register(Server.MapPath("~/content/fonts/ARIALUNI.TTF"), "ARIALUNI");
            _normalFont = FontFactory.GetFont(
                "ARIALUNI",
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED,
                FontSize,
                Font.NORMAL,
                _defaultColor);
            _redFont = FontFactory.GetFont(
                "ARIALUNI",
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED,
                FontSize,
                Font.NORMAL,
                _redColor);
            _boldFont = FontFactory.GetFont(
                "ARIALUNI",
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED,
                FontSize,
                Font.BOLD,
                _defaultColor);

            Document document = new Document(PageSize.A4, 0, 0, 30f, 30f);
            PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();

            PdfPTable table = new PdfPTable(1) { TotalWidth = TotalWidth };
            table.DefaultCell.Border = 0;

            PdfPTable performanceResult;
            PdfPTable performanceDetail;

            PdfPTable title = RenderTitle(epaInfo.Period.Name);
            PdfPTable generalInfo = RenderGeneralInfo(epaInfo.Employee);
            PdfPTable summaryComment = RenderSummaryComment(epaInfo);

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
                PdfPTable recommendation;
                if (epaInfo.IsProbation || epaInfo.IsPromotion)
                {
                    recommendation = RenderRecommendationForProbationAndPromotion(epaInfo);
                    table.AddCell(recommendation);
                }
                else
                {
                    if (epaInfo.IsSemiAnnual)
                    {
                        recommendation = RenderRecommendationForSemi(epaInfo);
                        table.AddCell(recommendation);
                    }
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

        private static PdfPTable CreateNewDefaultPdfPTable(float[] widths, Color borderColor)
        {
            PdfPTable table = new PdfPTable(2)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };

            table.SetWidths(widths);
            table.DefaultCell.BorderColor = borderColor;

            return table;
        }

        private string GetAvatar(string code)
        {
            string path = System.IO.File.Exists(Server.MapPath("~/avatars/avatar.jpg"))
                ? Server.MapPath("~/avatars/avatar.jpg")
                : string.Empty;

            var codeWithExtension = code + ".jpg";

            if (System.IO.File.Exists(Server.MapPath("~/avatars/" + codeWithExtension)))
            {
                path = Server.MapPath("~/avatars/" + codeWithExtension);
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

        private string GetRadioButton(bool value)
        {
            string path = Server.MapPath("~/content/site/images/radiobutton_no.png");
            if (value)
            {
                path = Server.MapPath("~/content/site/images/radiobutton_yes.png");
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

            var border = new Color(System.Drawing.ColorTranslator.FromHtml("#ddd"));

            PdfPCell cell = new PdfPCell(new Phrase(Lang.GeneralInfo, _defaultFont))
            {
                FixedHeight = 20,
                PaddingLeft = 5,
                PaddingRight = 5,
                Colspan = 5,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                Border = 0
            };
            table.AddCell(cell);

            string imagePath = GetAvatar(employeeInfo.Code);
            Image img = Image.GetInstance(imagePath);
            img.ScaleToFit(60, 60);

            cell = new PdfPCell(img, false)
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 4,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 1,
                BorderWidthBottom = 1
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeCode, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.Code, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.NickName, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 1,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FullName, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.FullName, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.JobTitle, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.JobTitleName, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 1,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Department, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.DepartmentName, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Section, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.GroupName, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 1,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.GroupDate, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_TOP,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(employeeInfo.JoinedDate.ToString("dd MMM, yyyy"), _normalFont))
            {
                Colspan = 3,
                VerticalAlignment = Element.ALIGN_TOP,
                BorderColor = border,
                BorderWidthTop = 0,
                BorderWidthRight = 1,
                BorderWidthLeft = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 5
            };
            table.AddCell(cell);

            return table;
        }

        private PdfPTable RenderPerformanceDetail(EPAModel epaInfo)
        {
            float[] widths = { 20, 190, 40, 120, 60, 40, 120 };
            PdfPTable detail = new PdfPTable(7)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            detail.SetWidths(widths);
            detail.DefaultCell.BorderColor = _borderColor;

            var backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            foreach (EPAKpiGroupModel kpiGroup in epaInfo.KPIGroups)
            {
                PdfPCell cell =
                    new PdfPCell(new Phrase(kpiGroup.Name + " (" + kpiGroup.Weight + " pts)", _defaultFont))
                    {
                        Colspan = 7,
                        MinimumHeight = 20,
                        PaddingLeft = 5,
                        PaddingRight = 5,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                        Border = 0
                    };
                detail.AddCell(cell);

                AddCenterAlignedCell(detail, Lang.SymbolCount, backgroundColor, rowspan: 2);

                AddCenterAlignedCell(detail, Lang.KPI, backgroundColor, rowspan: 2);

                AddCenterAlignedCell(detail, Lang.Employee, backgroundColor, 2);

                AddCenterAlignedCell(detail, Lang.Evaluator, backgroundColor, 3);

                AddCenterAlignedCell(detail, Lang.Score, backgroundColor);

                AddCenterAlignedCell(detail, Lang.Comment, backgroundColor);

                AddCenterAlignedCell(detail, Lang.NickName, backgroundColor);

                AddCenterAlignedCell(detail, Lang.Score, backgroundColor);

                AddCenterAlignedCell(detail, Lang.Comment, backgroundColor);

                int rowspan = epaInfo.Process.Evaluators.Count;
                for (int index = 0; index < kpiGroup.KPIs.Count; index++)
                {
                    AddCenterAlignedCell(detail, (index + 1).ToString(), rowspan: rowspan);

                    Phrase phrase = new Phrase { new Chunk(kpiGroup.KPIs[index].Name, _normalFont) };
                    if (kpiGroup.NumberKPIOptional == 0)
                    {
                        phrase.Add(new Chunk(" (" + kpiGroup.KPIs[index].WeightToDisplay + " pts)", _boldFont));
                    }

                    Font font1 = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 9, Font.NORMAL, _redColor);
                    if (kpiGroup.KPIs[index].IsRequiredEvaluate)
                    {
                        phrase.Add(new Chunk(" (*)", font1));
                    }

                    cell = new PdfPCell(phrase)
                    {
                        Rowspan = rowspan,
                        BorderColor = _borderColor
                    };
                    detail.AddCell(cell);

                    EvaluationModel self = kpiGroup.KPIs[index].EvaluationBySelf;
                    var score = epaInfo.Process.Self.IsEndProcess
                        ? (string.IsNullOrEmpty(self.Score) ? Lang.NotAvailable : self.ScoreToDisplay + " pts")
                        : Lang.Pending;
                    cell = new PdfPCell(new Phrase(score, _normalFont))
                    {
                        Rowspan = rowspan,
                        BorderColor = _borderColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    detail.AddCell(cell);

                    cell = new PdfPCell(new Phrase(
                        epaInfo.Process.Self.IsEndProcess
                            ? self.Comment
                            : Lang.Pending,
                        _normalFont))
                    {
                        Rowspan = rowspan,
                        BorderColor = _borderColor
                    };

                    detail.AddCell(cell);

                    foreach (EvaluationModel evaluator in kpiGroup.KPIs[index].EvaluationByEvaluators)
                    {
                        cell = new PdfPCell(new Phrase(evaluator.NickName, _normalFont))
                        {
                            BorderColor = _borderColor,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        detail.AddCell(cell);

                        score =
                            epaInfo.Process.Evaluators.First().IsEndProcess
                                ? (string.IsNullOrEmpty(evaluator.Score)
                                    ? Lang.NotAvailable
                                    : evaluator.ScoreToDisplay + " pts")
                                : Lang.Pending;
                        cell = new PdfPCell(new Phrase(score, _normalFont))
                        {
                            BorderColor = _borderColor,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        detail.AddCell(cell);

                        cell = new PdfPCell(new Phrase(
                            epaInfo.Process.Evaluators.First().IsEndProcess ? evaluator.Comment : Lang.Pending,
                            _normalFont))
                        {
                            BorderColor = _borderColor
                        };

                        detail.AddCell(cell);
                    }
                }
            }

            return detail;
        }

        private PdfPTable RenderPerformanceDetailForSemiHasMonthly(EPAModel epaInfo)
        {
            string totalScore;

            var backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(4)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = _borderColor;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.MonthlyEPA, _defaultFont))
            {
                MinimumHeight = 20,
                PaddingLeft = 5,
                PaddingRight = 5,
                Colspan = 4,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                Border = 0
            };
            table.AddCell(cell);

            AddCenterAlignedCell(table, Lang.SymbolCount, backgroundColor);

            AddCenterAlignedCell(table, Lang.EmployeeScore, backgroundColor);

            AddCenterAlignedCell(table, Lang.EvaluatorScore, backgroundColor);

            AddCenterAlignedCell(table, Lang.ReviewerScore, backgroundColor);

            foreach (EPAModel monthly in epaInfo.MonthlyEPA.MonthlyEPAs)
            {
                cell = new PdfPCell(new Phrase(monthly.Period.Name, _normalFont))
                {
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                totalScore = monthly.Process.Self.TotalScore.Length > 0 ? monthly.Process.Self.TotalScore : "0";
                cell = new PdfPCell(new Phrase(totalScore, _normalFont))
                {
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(monthly.Process.AverageScoreByEvaluators.ToString(), _normalFont))
                {
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(monthly.Process.AverageScoreByReviewers.ToString(), _normalFont))
                {
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.MonthlyAverage, _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageSelfScore.ToString();
            cell = new PdfPCell(new Phrase(
                totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.SelfEvaluated,
                _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScores.ToString();
            cell = new PdfPCell(new Phrase(
                totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.Evaluator,
                _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageReviewerScores.ToString();
            cell = new PdfPCell(new Phrase(
                totalScore + " / " + epaInfo.MonthlyEPA.MonthlyEPAs.First().ScoreRatingType.Reviewer, _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.SemiAverage, _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(
                $"{totalScore} / {epaInfo.ScoreRatingType.SelfEvaluated}",
                _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Evaluator, _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageReviewerScoreConverted.ToString();
            cell = new PdfPCell(new Phrase(totalScore + " / " + epaInfo.ScoreRatingType.Reviewer, _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            return table;
        }

        private PdfPTable RenderPerformanceResult(EPAModel epaInfo)
        {
            var totalCol = !epaInfo.Process.Managements.Any() ? 8 : 10;

            var backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(totalCol)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = _borderColor;

            // Header rows
            PdfPCell cell = new PdfPCell(new Phrase(Lang.PerformanceResult.ToUpperInvariant(), _defaultFont))
            {
                FixedHeight = 20,
                PaddingLeft = 5,
                PaddingRight = 5,
                Colspan = totalCol,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                Border = 0
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Employee, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Evaluator, _normalFont))
            {
                Colspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Reviewer, _normalFont))
            {
                Colspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management, _normalFont))
                {
                    Colspan = 2,
                    BackgroundColor = backgroundColor,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.DisciplineScore, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FinalScore, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Grade, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont))
            {
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont))
            {
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont))
            {
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont))
            {
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont))
                {
                    BackgroundColor = backgroundColor,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont))
                {
                    BackgroundColor = backgroundColor,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);
            }

            EPAProcessModel self = epaInfo.Process.Self;
            AddCellToTableFromEPAProcessModel(epaInfo, table, self);

            for (int index = 0; index < epaInfo.Rowspan; index++)
            {
                if (index < epaInfo.Process.Evaluators.Count)
                {
                    cell = new PdfPCell(new Phrase(
                        epaInfo.Process.Evaluators[index].EvaluatorInfo.NickName,
                        _normalFont))
                    {
                        BorderColor = _borderColor,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);

                    EPAProcessModel evaluator = epaInfo.Process.Evaluators[index];
                    AddCellToTableFromEPAProcessModel(epaInfo, table, evaluator);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (index < epaInfo.Process.Reviewers.Count)
                {
                    EPAProcessModel reviewer = epaInfo.Process.Reviewers[index];
                    cell = new PdfPCell(new Phrase(reviewer.EvaluatorInfo.NickName, _normalFont))
                    {
                        BorderColor = _borderColor,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);
                    AddCellToTableFromEPAProcessModel(epaInfo, table, reviewer);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (epaInfo.Process.Managements.Any())
                {
                    if (index < epaInfo.Process.Managements.Count)
                    {
                        EPAProcessModel management = epaInfo.Process.Managements[index];
                        cell = new PdfPCell(new Phrase(management.EvaluatorInfo.NickName, _normalFont))
                        {
                            BorderColor = _borderColor,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        table.AddCell(cell);
                        AddCellToTableFromEPAProcessModel(epaInfo, table, management);
                    }
                    else
                    {
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                    }
                }

                if (index != 0)
                {
                    continue;
                }

                var disciplineScore = epaInfo.HasDisciplineScore
                    ? epaInfo.DisciplineScore.ToString(CultureInfo.InvariantCulture) + " / " +
                      epaInfo.MaxDisciplineScore
                    : "-";
                cell = new PdfPCell(new Phrase(disciplineScore, _normalFont))
                {
                    Rowspan = epaInfo.Rowspan,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                Font selectedFont = epaInfo.IsClosed ? _normalFont : _redFont;

                var finalScoreString = epaInfo.FinalScoreToExport;

                cell = new PdfPCell
                {
                    Rowspan = epaInfo.Rowspan,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Phrase = new Phrase(finalScoreString, selectedFont)
                };
                table.AddCell(cell);

                var standardGrades = _commonService.GetStandardGrade().ToList();

                float finalScore;
                float.TryParse(finalScoreString, out finalScore);

                float? performanceScore = null;
                if (finalScore != 0)
                {
                    performanceScore = finalScore - epaInfo.DisciplineScore;
                }

                var tempGrade = _employeeEPAService.ConvertScoreToGrade(
                    performanceScore,
                    epaInfo.DisciplineScore,
                    epaInfo.HasDisciplineScore,
                    standardGrades);

                cell = new PdfPCell
                {
                    Rowspan = epaInfo.Rowspan,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Phrase = new Phrase(tempGrade, selectedFont)
                };
                table.AddCell(cell);
            }

            return table;
        }

        private void AddCellToTableFromEPAProcessModel(EPAModel epaInfo, PdfPTable table, EPAProcessModel process)
        {
            var score = GetScoreFromEPAProcessModel(epaInfo, process);
            var cell = CreateCellFromEPAProcessModel(epaInfo, process, score);
            table.AddCell(cell);
        }

        private PdfPCell CreateCellFromEPAProcessModel(EPAModel epaInfo, EPAProcessModel process, string totalScore)
        {
            var selectedFont = process.StatusId == (int)ProcessStatusType.InProgress ? _redFont : _normalFont;

            var cell = new PdfPCell
            {
                Rowspan = epaInfo.Rowspan,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Phrase = new Phrase(totalScore, selectedFont)
            };

            return cell;
        }

        private static string GetScoreFromEPAProcessModel(EPAModel epaInfo, EPAProcessModel process)
        {
            float maxScore;
            switch (process.ProcessId)
            {
                case (int)ProcessType.Self:
                    maxScore = epaInfo.ScoreRatingType.SelfEvaluated;
                    break;

                case (int)ProcessType.Evaluator:
                    maxScore = epaInfo.ScoreRatingType.Evaluator;
                    break;

                case (int)ProcessType.Reviewer:
                    maxScore = epaInfo.ScoreRatingType.Reviewer;
                    break;

                case (int)ProcessType.Management:
                    maxScore = epaInfo.ScoreRatingType.Management;
                    break;

                default:
                    throw new ArgumentException("type not in case");
            }

            return string.IsNullOrEmpty(process.TotalScore)
                ? Lang.NotAvailable
                : process.TotalScore + " / " + maxScore;
        }

        private PdfPTable RenderPerformanceResultForSemiHasMonthly(EPAModel epaInfo)
        {
            var totalCol = !epaInfo.Process.Managements.Any() ? 7 : 9;

            var backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = new PdfPTable(totalCol)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            table.DefaultCell.BorderColor = _borderColor;

            PdfPCell cell =
                new PdfPCell(new Phrase(Lang.PerformanceResult.ToUpperInvariant(), _defaultFont))
                {
                    FixedHeight = 20,
                    PaddingLeft = 5,
                    PaddingRight = 5,
                    Colspan = totalCol,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                    Border = 0
                };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EmployeeScore, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.EvaluatorScore, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Reviewer, _normalFont))
            {
                Colspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management, _normalFont))
                {
                    Colspan = 2,
                    BackgroundColor = backgroundColor,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);
            }

            cell = new PdfPCell(new Phrase(Lang.DisciplineScore, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FinalScore, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Grade, _normalFont))
            {
                Rowspan = 2,
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont))
            {
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont))
            {
                BackgroundColor = backgroundColor,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            table.AddCell(cell);

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.NickName, _normalFont))
                {
                    BackgroundColor = backgroundColor,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.PerformanceScore, _normalFont))
                {
                    BackgroundColor = backgroundColor,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

                table.AddCell(cell);
            }

            var totalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted + " / " +
                             epaInfo.ScoreRatingType.SelfEvaluated;
            cell = new PdfPCell(new Phrase(totalScore, _normalFont))
            {
                Rowspan = epaInfo.Rowspan,
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            table.AddCell(cell);

            totalScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted + " / " +
                         epaInfo.ScoreRatingType.Evaluator;
            cell = new PdfPCell(new Phrase(totalScore, _normalFont))
            {
                BorderColor = _borderColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            table.AddCell(cell);

            for (int index = 0; index < epaInfo.Rowspan; index++)
            {
                if (index < epaInfo.Process.Reviewers.Count)
                {
                    EPAProcessModel reviewer = epaInfo.Process.Reviewers[index];
                    cell = new PdfPCell(new Phrase(reviewer.EvaluatorInfo.NickName, _normalFont))
                    {
                        BorderColor = _borderColor,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);
                    AddCellToTableFromEPAProcessModel(epaInfo, table, reviewer);
                }
                else
                {
                    table.AddCell(string.Empty);
                    table.AddCell(string.Empty);
                }

                if (epaInfo.Process.Managements.Any())
                {
                    if (index < epaInfo.Process.Managements.Count)
                    {
                        EPAProcessModel management = epaInfo.Process.Managements[index];
                        cell = new PdfPCell(new Phrase(management.EvaluatorInfo.NickName, _normalFont))
                        {
                            BorderColor = _borderColor,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        };
                        table.AddCell(cell);
                        AddCellToTableFromEPAProcessModel(epaInfo, table, management);
                    }
                    else
                    {
                        table.AddCell(string.Empty);
                        table.AddCell(string.Empty);
                    }
                }

                if (index == 0)
                {
                    var disciplineScore = epaInfo.HasDisciplineScore
                        ? epaInfo.DisciplineScore + " / " + epaInfo.MaxDisciplineScore
                        : "-";
                    cell = new PdfPCell(new Phrase(disciplineScore, _normalFont))
                    {
                        Rowspan = epaInfo.Rowspan,
                        BorderColor = _borderColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);

                    Font selectedFont = epaInfo.IsClosed ? _normalFont : _redFont;

                    var finalScoreString = epaInfo.FinalScoreToExport;

                    cell = new PdfPCell(new Phrase(finalScoreString, selectedFont))
                    {
                        Rowspan = epaInfo.Rowspan,
                        BorderColor = _borderColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);

                    var standardGrades = _commonService.GetStandardGrade().ToList();

                    float finalScore;
                    float.TryParse(finalScoreString, out finalScore);

                    float? performanceScore = null;
                    if (finalScore != 0)
                    {
                        performanceScore = finalScore - epaInfo.DisciplineScore;
                    }

                    var tempGrade = _employeeEPAService.ConvertScoreToGrade(
                        performanceScore,
                        epaInfo.DisciplineScore,
                        epaInfo.HasDisciplineScore,
                        standardGrades);

                    cell = new PdfPCell(new Phrase(tempGrade, selectedFont))
                    {
                        Rowspan = epaInfo.Rowspan,
                        BorderColor = _borderColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);
                }
            }

            return table;
        }

        private PdfPTable RenderRecommendationForProbationAndPromotion(EPAModel epaInfo)
        {
            PdfPTable table = CreateNewDefaultPdfPTable(new float[] { 10, 580 }, _borderColor);
            table.DefaultCell.Border = 0;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.Recommendation, _defaultFont))
            {
                FixedHeight = 20,
                PaddingLeft = 5,
                PaddingRight = 5,
                Colspan = 2,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                Border = 0
            };
            table.AddCell(cell);

            string imagePath = GetRadioButton(epaInfo.Recommendation.PassedProbation);
            Image img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false)
            {
                PaddingTop = 3,
                PaddingLeft = 2,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.PassProbation, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            imagePath = GetRadioButton(epaInfo.Recommendation.ExtendedProbation);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false)
            {
                PaddingTop = 3,
                PaddingLeft = 2,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.ExtendProbation, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            imagePath = GetRadioButton(epaInfo.Recommendation.FailedProbation);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false)
            {
                PaddingTop = 3,
                PaddingLeft = 2,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.FailProbation, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthLeft = 0
            };
            table.AddCell(cell);

            return table;
        }

        private PdfPTable RenderRecommendationForSemi(EPAModel epaInfo)
        {
            PdfPTable table = CreateNewDefaultPdfPTable(new float[] { 10, 580 }, _borderColor);
            table.DefaultCell.Border = 0;

            PdfPCell cell = new PdfPCell(new Phrase(Lang.Recommendation, _defaultFont))
            {
                FixedHeight = 20,
                PaddingLeft = 5,
                PaddingRight = 5,
                Colspan = 2,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                Border = 0
            };
            table.AddCell(cell);

            string imagePath = GetCheckbox(epaInfo.Recommendation.Promoted);
            Image img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false)
            {
                PaddingTop = 3,
                PaddingLeft = 2,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.Promotion, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            imagePath = GetCheckbox(epaInfo.Recommendation.PayIncreased);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false)
            {
                PaddingTop = 3,
                PaddingLeft = 2,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Lang.AnnualIncrement, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0
            };
            table.AddCell(cell);

            imagePath = GetCheckbox(epaInfo.Recommendation.JobGradeIncreased);
            img = Image.GetInstance(imagePath);
            img.ScaleToFit(8, 8);
            cell = new PdfPCell(img, false)
            {
                PaddingTop = 3,
                PaddingLeft = 2,
                BorderColor = _borderColor,
                BorderWidthTop = 0,
                BorderWidthRight = 0
            };
            table.AddCell(cell);

            string jobGrade = epaInfo.Recommendation.JobGrade != null
                ? Lang.JobGrade + ": " + epaInfo.Recommendation.JobGrade.Name
                : Lang.JobGrade;
            cell = new PdfPCell(new Phrase(jobGrade, _normalFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BorderColor = _borderColor
            };
            cell.BorderColor = _borderColor;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            table.AddCell(cell);

            return table;
        }

        private PdfPTable RenderSummaryComment(EPAModel epaInfo)
        {
            var backgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4"));

            PdfPTable table = CreateNewDefaultPdfPTable(new float[] { 140, 450 }, _borderColor);

            PdfPCell cell = new PdfPCell(new Phrase(Lang.SummaryComment, _defaultFont))
            {
                FixedHeight = 20,
                PaddingLeft = 5,
                PaddingRight = 5,
                Colspan = 2,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#8b8f9e")),
                Border = 0
            };
            table.AddCell(cell);

            AddCenterAlignedCell(table, Lang.FullName, backgroundColor);

            AddCenterAlignedCell(table, Lang.Comment, backgroundColor);

            if (!epaInfo.IsSemiHasMonthly)
            {
                EPAProcessModel self = epaInfo.Process.Self;
                cell = new PdfPCell(new Phrase(
                    $"{self.EvaluatorInfo.FullName} ({self.EvaluatorInfo.NickName})",
                    _normalFont))
                {
                    BorderColor = _borderColor,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                Font selectedFont = self.IsEndProcess ? _normalFont : _redFont;

                cell = new PdfPCell(new Phrase(self.Comment, selectedFont))
                {
                    BorderColor = _borderColor
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Lang.Evaluator, _boldFont))
                {
                    Colspan = 2,
                    BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ececec")),
                    BorderColor = _borderColor,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                foreach (EPAProcessModel evaluator in epaInfo.Process.Evaluators)
                {
                    cell = new PdfPCell(new Phrase(
                        evaluator.EvaluatorInfo.FullName + " (" + evaluator.EvaluatorInfo.NickName + ")",
                        _normalFont))
                    {
                        BorderColor = _borderColor,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);

                    selectedFont = evaluator.IsEndProcess ? _normalFont : _redFont;

                    cell = new PdfPCell(new Phrase(evaluator.Comment, selectedFont))
                    {
                        BorderColor = _borderColor
                    };

                    table.AddCell(cell);
                }
            }

            cell = new PdfPCell(new Phrase(Lang.Reviewer, _boldFont))
            {
                Colspan = 2,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ececec")),
                BorderColor = _borderColor,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            table.AddCell(cell);

            foreach (EPAProcessModel reviewer in epaInfo.Process.Reviewers)
            {
                cell = new PdfPCell(new Phrase(
                    reviewer.EvaluatorInfo.FullName + " (" + reviewer.EvaluatorInfo.NickName + ")", _normalFont))
                {
                    BorderColor = _borderColor,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                Font selectedFont = reviewer.IsEndProcess ? _normalFont : _redFont;

                cell = new PdfPCell(new Phrase(reviewer.Comment, selectedFont))
                {
                    BorderColor = _borderColor
                };

                table.AddCell(cell);
            }

            if (epaInfo.Process.Managements.Any())
            {
                cell = new PdfPCell(new Phrase(Lang.Management, _boldFont))
                {
                    Colspan = 2,
                    BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ececec")),
                    BorderColor = _borderColor,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                table.AddCell(cell);

                foreach (EPAProcessModel management in epaInfo.Process.Managements)
                {
                    cell = new PdfPCell(new Phrase(
                        management.EvaluatorInfo.FullName + " (" + management.EvaluatorInfo.NickName + ")",
                        _normalFont))
                    {
                        BorderColor = _borderColor,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };
                    table.AddCell(cell);

                    Font selectedFont = management.IsEndProcess ? _normalFont : _redFont;

                    cell = new PdfPCell(new Phrase(management.Comment, selectedFont))
                    {
                        BorderColor = _borderColor
                    };
                    table.AddCell(cell);
                }
            }

            return table;
        }

        private void AddCenterAlignedCell(
            PdfPTable table,
            string content,
            Color backgroundColor = null,
            int colspan = 1,
            int rowspan = 1)
        {
            var cell = backgroundColor != null
                ? new PdfPCell(new Phrase(content, _normalFont))
                {
                    Rowspan = rowspan,
                    Colspan = colspan,
                    BackgroundColor = backgroundColor,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                }
                : new PdfPCell(new Phrase(content, _normalFont))
                {
                    Rowspan = rowspan,
                    Colspan = colspan,
                    BorderColor = _borderColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

            table.AddCell(cell);
        }

        private PdfPTable RenderTitle(string periodName)
        {
            PdfPTable table = new PdfPTable(1)
            {
                TotalWidth = TotalWidth,
                LockedWidth = true
            };
            Font font = FontFactory.GetFont("ARIALUNI", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, 10, Font.BOLD, _defaultColor);
            PdfPCell cell = new PdfPCell(new Phrase(Lang.EPATilte + " - " + periodName, font))
            {
                FixedHeight = 30,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#ebe4d4")),
                Border = 0,
                PaddingLeft = 12f,
                PaddingRight = 12f
            };
            table.AddCell(cell);

            return table;
        }

        private void SetDefaultStyles()
        {
            _borderColor = new Color(System.Drawing.ColorTranslator.FromHtml("#cec5c2"));
            _defaultFont = FontFactory.GetFont(
                "ARIALUNI",
                BaseFont.IDENTITY_H,
                BaseFont.NOT_EMBEDDED,
                9,
                Font.NORMAL,
                _whiteColor);
        }
    }
}