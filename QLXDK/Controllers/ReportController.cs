using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;
using QLXDK.Models;

namespace QLXDK.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private qlslContext _db = new qlslContext();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExportExcel(Models.Entities.Report model)
        {
            DateTime startDate = model.FromDate.Value.Date;
            DateTime endDatePlusOne = model.ToDate.Value.Date.AddDays(1);
            try
            {
                // Get salan's name
                string[] salanNames = (from vd in _db.VoyageDetails
                                       join sv in _db.SubVoyages on vd.SubVoyageId equals sv.ID
                                       join v in _db.Voyages on sv.VoyageId equals v.ID
                                       join s in _db.Salans on v.SalanId equals s.ID
                                       where vd.Line == model.Line && vd.ApprovedDate >= startDate && vd.ApprovedDate <= endDatePlusOne
                                       && (string.IsNullOrEmpty(model.Vessel) || vd.VesVoyName.Contains(model.Vessel))
                                       group s.Name by new
                                       {
                                           vd.SubVoyageId,
                                           vd.VesVoyName,
                                           vd.PortOfLoad,
                                           vd.PortOfDischarge,
                                           SalanName = s.Name
                                       } into g
                                       orderby g.Key.SubVoyageId
                                       select g.Key.SalanName)
                       .ToArray();

                var result = from details in _db.VoyageDetails
                             where details.Line == model.Line && details.ApprovedDate >= startDate && details.ApprovedDate <= endDatePlusOne
                             && (string.IsNullOrEmpty(model.Vessel) || details.VesVoyName.Contains(model.Vessel))
                             let sizeTypeNew = (details.SizeType != null && details.SizeType.Length >= 3)
                                 ? ((details.SizeType.Substring(0, 1) == "2" && details.SizeType.Substring(2, 1) == "3" && details.IMO == "") ? "cont_lanh_20" :
                                    (details.SizeType.Substring(0, 1) == "4" && details.SizeType.Substring(2, 1) == "3" && details.IMO == "") ? "cont_lanh_40" :
                                    //(details.SizeType.Substring(0, 1) == "9" && details.SizeType.Substring(2, 1) == "3" && details.IMO == "") ? "cont_lanh_45" :
                                    (details.SizeType.Substring(0, 1) == "2" && details.SizeType.Substring(2, 1) == "0" && details.IMO == "") ? "cont_hang_20" :
                                    (details.SizeType.Substring(0, 1) == "4" && details.SizeType.Substring(2, 1) == "0" && details.IMO == "") ? "cont_hang_40" :
                                    (details.SizeType.Substring(0, 1) == "9" && details.SizeType.Substring(2, 1) == "0" && details.IMO == "") ? "cont_hang_45" :
                                    (details.SizeType.Substring(0, 1) == "2" && (details.IMO != null || details.IMO != "")) ? "cont_imo_20" :
                                    (details.SizeType.Substring(0, 1) == "4" && (details.IMO != null || details.IMO != "")) ? "cont_imo_40" :
                                    (details.SizeType.Substring(0, 1) == "2" && details.FullEmpty == "E" && details.IMO == "") ? "cont_rong_20" :
                                    (details.SizeType.Substring(0, 1) == "4" && details.FullEmpty == "E" && details.IMO == "") ? "cont_rong_40" :
                                    (details.SizeType.Substring(0, 1) == "9" && details.FullEmpty == "E" && details.IMO == "") ? "cont_rong_45" :
                                    details.SizeType)
                                 : details.SizeType
                             group details by new
                             {
                                 details.VesVoyName,
                                 details.PortOfLoad,
                                 details.PortOfDischarge,
                                 SizeType_New = sizeTypeNew,
                                 details.SubVoyageId,
                             } into g
                             orderby g.Key.SubVoyageId
                             select new
                             {
                                 ID = (g.Key.PortOfDischarge != null && g.Key.PortOfDischarge.Length >= 5)
                                      ? g.Key.PortOfDischarge.Substring(0, 5)
                                      : g.Key.PortOfDischarge,
                                 g.Key.VesVoyName,
                                 g.Key.PortOfLoad,
                                 g.Key.PortOfDischarge,
                                 g.Key.SizeType_New,
                                 Amount = g.Count()
                             };
                var data = result.ToList();
                if (data.Count <= 0)
                {
                    TempData["Message"] = "Không có dữ liệu";
                    return RedirectToAction("Index");
                }

                var arrObjList = new Dictionary<string, Dictionary<string, int>>();
                foreach (var row in result)
                {
                    string id = row.VesVoyName + "|" + row.PortOfLoad + "|" + row.PortOfDischarge;
                    string sizeType = row.SizeType_New;

                    if (!arrObjList.ContainsKey(id))
                    {
                        arrObjList[id] = new Dictionary<string, int>();
                    }

                    if (!arrObjList[id].ContainsKey(sizeType))
                    {
                        arrObjList[id][sizeType] = row.Amount;
                    }
                    else
                    {
                        //result1[id][sizeType] += row.Amount;
                    }

                }

                // Create a workbook
                var wb = new XLWorkbook();
                wb.Style.Font.FontName = "Times New Roman";
                wb.Style.Font.FontSize = 12;
                // Add Line sheet
                var wl = wb.AddWorksheet(model.Line);
                // Add image
                string relativePath = @"Content\asset\images\logo_excel.png";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                if (System.IO.File.Exists(fullPath))
                {
                    var image = wl.AddPicture(fullPath)
                                  .MoveTo(wl.Cell("A1"))
                                  .Scale(1);
                }
                // Option width of columns
                wl.Column("B").Width = 15;
                wl.Column("C").Width = 30;
                wl.Column("D").Width = 18;
                wl.Column("E").Width = 18;
                wl.Column("G").Width = 18;
                wl.Column("H").Width = 18;
                wl.Columns("U:W").Width = 16;
                // Range1
                var range1 = wl.Range("A1:X1");
                range1.Merge();
                range1.Value = "CÔNG TY CỔ PHẦN GIANG NAM LOGISTICS";
                range1.Style.Font.Bold = true;
                range1.Style.Font.FontSize = 14;
                range1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Range2
                var range2 = wl.Range("A2:X2");
                range2.Merge();
                range2.Value = "ĐỊA CHỈ: 198/B4 Hoàng Văn Thụ, P.9, Q. PN, TP. HCM";
                range2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range2.Style.Font.Bold = true;

                // Range5
                var range5 = wl.Range("A5:X5");
                range5.Merge();
                range5.Value = "BẢNG TỔNG HỢP SẢN LƯỢNG VẬN  CHUYỂN SALAN";
                range5.Style.Font.Bold = true;
                range5.Style.Font.FontSize = 14;
                range5.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Range6
                var range6 = wl.Range("A6:X6");
                range6.Merge();
                string resultText = $"Từ ngày {model.FromDate:dd} tháng {model.FromDate:MM} năm {model.FromDate:yyyy} đến ngày {model.ToDate:dd} tháng {model.ToDate:MM} năm {model.ToDate:yyyy}";
                range6.Value = resultText;
                range6.Style.Font.FontSize = 14;
                range6.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range6.Style.Font.FontColor = XLColor.Red;

                // Range8
                var range8 = wl.Range("A8:X8");
                range8.Merge();
                var firstCell = range8.Cell(1, 1);
                firstCell.Value = "Kính gởi :  "+ model.Line.ToUpper() + " SHIPPING LINES (Vietnam) Company Limited";
                range8.Style.Font.Bold = true;
                range8.Style.Font.Italic = true;
                range8.Style.Font.FontSize = 14;
                range8.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                firstCell.RichText.Substring(0, 8).SetUnderline(XLFontUnderlineValues.Single).SetBold(false);

                // Range9
                var X9Cell = wl.Cell("X9");
                X9Cell.Value = "Số: .../../2026-COS-GNL";
                X9Cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                int startRow = 10;

                // Header
                // SỐ TT
                var cellSoTt = wl.Range(startRow, 1, startRow + 2, 1); // A7:A9
                cellSoTt.Merge().Value = "SỐ TT";

                // NGÀY
                var cellNgay = wl.Range(startRow, 2, startRow + 2, 2); // B7:B9
                cellNgay.Merge().Value = "NGÀY";

                // TÀU
                var cellTau = wl.Range(startRow, 3, startRow + 2, 3); // C7:C9
                cellTau.Merge().Value = "TÀU";

                //  CHUYẾN
                var cellChuyen = wl.Range(startRow, 4, startRow + 2, 4); // D7:D9
                cellChuyen.Merge().Value = "CHUYẾN";

                // SÀ LAN
                var cellSaLan = wl.Range(startRow, 5, startRow + 2, 5); // E7:E9
                cellSaLan.Merge().Value = "SÀ LAN";

                // BKS
                var cellBks = wl.Range(startRow, 6, startRow + 2, 6); // F7:F9
                cellBks.Merge().Value = "BKS";

                // TUYẾN VẬN CHUYỂN
                var cellTuyenVC = wl.Range(startRow, 7, startRow + 1, 8); // G7:H8
                cellTuyenVC.Merge().Value = "TUYẾN VẬN CHUYỂN";

                // CẢNG XẾP (G9), CẢNG DỠ (H9)
                wl.Cell(startRow + 2, 7).Value = "CẢNG XẾP";
                wl.Cell(startRow + 2, 8).Value = "CẢNG DỠ";

                // SỐ LƯỢNG CONTAINER
                var cellSlCont = wl.Range(startRow, 9, startRow, 18); // I7:O7
                cellSlCont.Merge().Value = "SỐ LƯỢNG CONTAINER";

                // HÀNG, LẠNH, IMO/OT
                var cellHang = wl.Range(startRow + 1, 9, startRow + 1, 11); // I8:K8
                cellHang.Merge().Value = "HÀNG";

                var cellRong = wl.Range(startRow + 1, 12, startRow + 1, 14); // L8:M8
                cellRong.Merge().Value = "RỖNG";

                var cellLanh = wl.Range(startRow + 1, 15, startRow + 1, 16); // L8:M8
                cellLanh.Merge().Value = "LẠNH";

                var cellImo = wl.Range(startRow + 1, 17, startRow + 1, 18); // N8:O8
                cellImo.Merge().Value = "IMO/OT";

                // Type
                wl.Cell(startRow + 2, 9).Value = "20";   // Cột I (Hàng)
                wl.Cell(startRow + 2, 10).Value = "40";  // Cột J (Hàng)
                wl.Cell(startRow + 2, 11).Value = "45";  // Cột K (Hàng)

                wl.Cell(startRow + 2, 12).Value = "20";  // Cột L (Lạnh)
                wl.Cell(startRow + 2, 13).Value = "40";  // Cột M (Lạnh)
                wl.Cell(startRow + 2, 14).Value = "45";  // Cột N (IMO/OT)

                wl.Cell(startRow + 2, 15).Value = "20";  // Cột L (Lạnh)
                wl.Cell(startRow + 2, 16).Value = "40";  // Cột O (IMO/OT)

                wl.Cell(startRow + 2, 17).Value = "20";  // Cột L (Lạnh)
                wl.Cell(startRow + 2, 18).Value = "40";  // Cột O (IMO/OT)

                // TỔNG SỐ CONT (Cột số 17)
                var cellTongCont = wl.Range(startRow, 19, startRow + 2, 19); // Q7:Q9
                cellTongCont.Merge().Value = "TỔNG\nSỐ CONT";
                // TỔNG SỐ TEUS
                var cellTongTeu = wl.Range(startRow, 20, startRow + 2, 20); // Q7:Q9
                cellTongTeu.Merge().Value = "TỔNG\nSỐ TEU";

                // ĐƠN GIÁ (USD) (Cột số 18)
                var cellDonGiaUsd = wl.Range(startRow, 21, startRow + 2, 21); // R7:R9
                cellDonGiaUsd.Merge().Value = "ĐƠN GIÁ\n(USD)";

                // THÀNH TIỀN (USD) (Cột số 19)
                var cellThanhTienUsd = wl.Range(startRow, 22, startRow + 2, 22); // S7:S9
                cellThanhTienUsd.Merge().Value = "THÀNH TIỀN\n(USD)";

                // ĐƠN GIÁ (Cột số 20)
                var cellDonGiaVnd = wl.Range(startRow, 23, startRow + 2, 23); // T7:T9
                cellDonGiaVnd.Merge().Value = "ĐƠN GIÁ";

                // THÀNH TIỀN (Cột số 21)
                var cellThanhTienVnd = wl.Range(startRow, 24, startRow + 2, 24); // U7:U9
                cellThanhTienVnd.Merge().Value = "THÀNH TIỀN";

                // Format Header
                var headerRange = wl.Range(startRow, 1, startRow + 2, 24);

                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Alignment.WrapText = true;
                headerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                //
                string today = DateTime.Now.ToString("dd/MM/yyyy");

                int stt = 0;
                var dataList = new List<object[]>();
                foreach (var parentRow in arrObjList)
                {
                    var cols = wl.Range((13 + stt), 1, (13 + stt), 24);
                    cols.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    cols.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    cols.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    cols.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    cols.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cols.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    // Create object
                    object[] info = new object[20];
                    info[4] = salanNames[stt];
                    stt++;
                    string id = parentRow.Key;
                    info[0] = stt;
                    info[1] = model.FromDate.Value.ToString("dd/MM/yyyy");
                    string[] parts = id.Split('|');
                    string vesVoyName = parts[0];
                    string[] parts_2 = vesVoyName.Split('-');
                    string vessel = parts_2[0];
                    string voyage = "";
                    if (parts_2.Length >=2)
                    {
                        voyage = parts_2[1];
                    }
                    //string voyage = parts_2[1];
                    info[2] = vessel;
                    info[3] = voyage;
                    string portOfLoad = parts[1];
                    string portOfDischarge = parts[2];
                    info[6] = portOfLoad;
                    info[7] = portOfDischarge;

                    int sumContainer = 0;
                    int sumTeus = 0;
                    foreach (var childRow in parentRow.Value)
                    {
                        string sizeType = childRow.Key;

                        if (sizeType == "cont_hang_20")
                        {
                            info[8] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value;
                            //sumHang20 += childRow.Value; ;
                        }
                        if (sizeType == "cont_hang_40")
                        {
                            info[9] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value * 2;
                            //sumHang40 += childRow.Value; ;
                        }
                        if (sizeType == "cont_hang_45")
                        {
                            info[10] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value * 2;
                            //sumHang45 += childRow.Value; ;
                        }
                        if (sizeType == "cont_rong_20")
                        {
                            info[11] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value;
                            //sumRong20 += childRow.Value;
                        }
                        if (sizeType == "cont_rong_40")
                        {
                            info[12] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value * 2;
                            //sumRong40 += childRow.Value;
                        }
                        if (sizeType == "cont_rong_45")
                        {
                            info[13] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value * 2;
                            //sumRong45 += childRow.Value;
                        }
                        if (sizeType == "cont_lanh_20")
                        {
                            info[14] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value;
                            //sumLanh20 += childRow.Value;
                        }
                        if (sizeType == "cont_lanh_40")
                        {
                            info[15] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value * 2;
                            //sumLanh40 += childRow.Value;
                        }
                        if (sizeType == "cont_imo_20")
                        {
                            info[16] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value;
                            //sumIMO20 += childRow.Value;
                        }
                        if (sizeType == "cont_imo_40")
                        {
                            info[17] = childRow.Value;
                            sumContainer += childRow.Value;
                            sumTeus += childRow.Value * 2;
                            //sumIMO40 += childRow.Value;
                        }
                    }
                    info[18] = sumContainer;
                    info[19] = sumTeus;
                    dataList.Add(info);
                }

                // Insert list into the sheet
                wl.Cell("A13").InsertData(dataList);
                // Total
                var totalHeader = wl.Range((13 + stt), 1, (13 + stt), 8); // A12:H12
                totalHeader.Merge().Value = "TỔNG CỘNG";
                totalHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                totalHeader.Style.Font.Bold = true;
                totalHeader.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                totalHeader.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                totalHeader.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                totalHeader.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                var range = wl.Range("I" + (13 + stt) + ":" + "X" + (13 + stt));
                range.SetValue("");
                wl.Cell($"I{13 + stt}").FormulaA1 = $"=SUM(I13:I{(13 + (stt - 1))})";
                wl.Cell($"J{13 + stt}").FormulaA1 = $"=SUM(J13:J{(13 + (stt - 1))})";
                wl.Cell($"K{13 + stt}").FormulaA1 = $"=SUM(K13:K{(13 + (stt - 1))})";
                wl.Cell($"L{13 + stt}").FormulaA1 = $"=SUM(L13:L{(13 + (stt - 1))})";
                wl.Cell($"M{13 + stt}").FormulaA1 = $"=SUM(M13:M{(13 + (stt - 1))})";
                wl.Cell($"N{13 + stt}").FormulaA1 = $"=SUM(N13:N{(13 + (stt - 1))})";
                wl.Cell($"O{13 + stt}").FormulaA1 = $"=SUM(O13:O{(13 + (stt - 1))})";
                wl.Cell($"P{13 + stt}").FormulaA1 = $"=SUM(P13:P{(13 + (stt - 1))})";
                wl.Cell($"Q{13 + stt}").FormulaA1 = $"=SUM(Q13:Q{(13 + (stt - 1))})";
                wl.Cell($"R{13 + stt}").FormulaA1 = $"=SUM(R13:R{(13 + (stt - 1))})";
                wl.Cell($"S{13 + stt}").FormulaA1 = $"=SUM(S13:S{(13 + (stt - 1))})";
                wl.Cell($"T{13 + stt}").FormulaA1 = $"=SUM(T13:T{(13 + (stt - 1))})";
                var customCell = wl.Range("I" + (13 + stt) + ":" + "T" + (13 + stt));
                customCell.Style.NumberFormat.Format = "#,##0;(#,##0);\"-\";@";
                customCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                // Total
                range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                range.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                // VAT
                var vatHeader = wl.Range((14 + stt), 19, (14 + stt), 21); // S13:U13 (Cột 19 đến cột 21)
                vatHeader.Merge().Value = "VAT (8%)";
                vatHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Căn giữa chữ
                vatHeader.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                vatHeader.Style.Font.Bold = true;
                var rangeVat = wl.Range("S" + (14 + stt) + ":" + "X" + (14 + stt));
                rangeVat.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                rangeVat.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rangeVat.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                rangeVat.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                // TONG CONG
                var tcHeader = wl.Range((15 + stt), 19, (15 + stt), 21); // S13:U13 (Cột 19 đến cột 21)
                tcHeader.Merge().Value = "TỔNG CỘNG";
                tcHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Căn giữa chữ
                tcHeader.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                tcHeader.Style.Font.Bold = true;
                var rangeTC = wl.Range("S" + (15 + stt) + ":" + "X" + (15 + stt));
                rangeTC.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                rangeTC.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rangeTC.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                rangeTC.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                // Text
                var JoCell = wl.Cell("A" + (15 + stt));
                JoCell.Value = "JoCell";
                JoCell.Style.Font.Bold = true;
                JoCell.Style.Font.FontColor = XLColor.Red;
                JoCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                var TotalCell = wl.Cell("A" + (16 + stt));
                TotalCell.FormulaA1 = "=\"Tổng tiền USD: \" & V25";
                TotalCell.Style.Font.Bold = true;
                TotalCell.Style.Font.FontColor = XLColor.Red;
                TotalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                var RateCell = wl.Cell("A" + (17 + stt));
                RateCell.FormulaA1 = "=\"Tỷ giá: \" & V25";
                RateCell.Style.Font.Bold = true;
                RateCell.Style.Font.FontColor = XLColor.Red;
                RateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                var DvtCell = wl.Cell("A" + (18 + stt));
                DvtCell.Value = "Đơn vị tính: VNĐ";
                DvtCell.Style.Font.Bold = true;
                DvtCell.Style.Font.FontColor = XLColor.Red;
                DvtCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                var PaidCell = wl.Cell("A" + (20 + stt));
                PaidCell.Value = "*Số tiền thanh toán: ";
                PaidCell.Style.Font.Bold = true;
                PaidCell.Style.Font.FontColor = XLColor.Red;
                PaidCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                var lineCell = wl.Range((22 + stt), 2, (22 + stt), 3); // A12:H12
                lineCell.Merge().Value = "COSCO SHIPPING LINES (Vietnam) Company Limited";
                lineCell.Style.Font.Bold = true;

                var companyCell = wl.Range((22 + stt), 19, (22 + stt), 22); // A12:H12
                companyCell.Merge().Value = "CÔNG TY CỔ PHẦN GIANG NAM LOGISTICS";
                companyCell.Style.Font.Bold = true;

                // Rate
                var rateHeader = wl.Range((16 + stt), 19, (16 + stt), 21); // S13:U13 (Cột 19 đến cột 21)
                rateHeader.Merge().Value = "";
                rateHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Căn giữa chữ
                rateHeader.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var rangeRate = wl.Range("S" + (16 + stt) + ":" + "X" + (16 + stt));
                rangeRate.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                rangeRate.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rangeRate.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                rangeRate.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                // Rate text
                var targetCell = wl.Cell("R" + (16 + stt));
                targetCell.Value = "Tỷ giá Vietcombank ngày " + today;
                targetCell.Style.Font.Bold = true;
                targetCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                /*
                 * List sheet 
                 */

                var ws = wb.AddWorksheet("LIST - " + model.Line);
                // Add Header
                ws.Cell("A1").Value = "No.";
                ws.Cell("B1").Value = "Date";
                ws.Cell("C1").Value = "Cont number";
                ws.Cell("D1").Value = "Type";
                ws.Cell("E1").Value = "Vessel";
                ws.Cell("F1").Value = "No.";
                ws.Cell("G1").Value = "Load Port";
                ws.Cell("H1").Value = "Discharge Port";
                ws.Cell("I1").Value = "Full/Empty";
                ws.Cell("J1").Value = "SÀ LAN";
                ws.Cell("K1").Value = "BOOKING";
                ws.Cell("L1").Value = "BILL";
                var listHeader = ws.Range("A1:G1");
                listHeader.Style.Font.Bold = true;
                var containerList = (from vd in _db.VoyageDetails
                                     join sv in _db.SubVoyages on vd.SubVoyageId equals sv.ID
                                     join v in _db.Voyages on sv.VoyageId equals v.ID
                                     join s in _db.Salans on v.SalanId equals s.ID
                                     where vd.Line == model.Line
                                        && vd.ApprovedDate >= startDate
                                        && vd.ApprovedDate <= endDatePlusOne
                                        && (string.IsNullOrEmpty(model.Vessel) || vd.VesVoyName.Contains(model.Vessel))
                                     select new
                                     {
                                         vd.ApprovedDate,
                                         vd.ContainerNo,
                                         SalanName = s.Name,
                                         vd.SizeType,
                                         vd.VesVoyName,
                                         vd.PortOfLoad,
                                         vd.PortOfDischarge,
                                         vd.BookingBillNo,
                                         vd.FullEmpty,
                                         vd.Category
                                     })
                                    .ToList();

                int rowList = 2;
                int no = 0;
                foreach (var item in containerList)
                {
                    no++;
                    ws.Cell(rowList, 1).Value = no;
                    ws.Cell(rowList, 2).Value = item.ApprovedDate.Value.ToString("dd/MM/yyyy");
                    ws.Cell(rowList, 3).Value = item.ContainerNo;
                    ws.Cell(rowList, 4).Value = item.SizeType;
                    string[] parts = (item.VesVoyName).Split('-');
                    string vessel = parts[0];
                    //string voyage = parts[1];
                    string voyage = "";
                    if (parts.Length >= 2)
                    {
                        voyage = parts[1];
                    }
                    ws.Cell(rowList, 5).Value = vessel;
                    ws.Cell(rowList, 6).Value = voyage;
                    ws.Cell(rowList, 7).Value = item.PortOfLoad;
                    ws.Cell(rowList, 8).Value = item.PortOfDischarge;
                    string fullOrEmpty = "";
                    if (item.FullEmpty == "E")
                    {
                        fullOrEmpty = "Empty";
                    }
                    else
                    {
                        fullOrEmpty = "Full";
                    }
                    ws.Cell(rowList, 9).Value = fullOrEmpty;
                    ws.Cell(rowList, 10).Value = item.SalanName;
                    ws.Cell(rowList, 11).Value = item.Category == "E" ? item.BookingBillNo : "";
                    ws.Cell(rowList, 12).Value = item.Category == "I" ? item.BookingBillNo : "";
                    rowList++;
                }
                ws.Columns().AdjustToContents();

                // Save into memory
                var ms = new MemoryStream();
                wb.SaveAs(ms);

                var bytes = ms.ToArray();
                string filename = "Report_" + model.Line + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filename + ".xlsx");
            }
            catch (Exception e)
            {
                TempData["Message"] = "Đã có lỗi xảy ra";
                return RedirectToAction("Index");
            }
        }

    }
    public class ContainerInfo
    {
        
        public string Vessel { get; set; }
        public int Amount { get; set; }
    }

    public class VoyageInfoViewModel
    {
        public string VesVoyName { get; set; }
        public string PortOfLoad { get; set; }
        public string PortOfDischarge { get; set; }
        public string SizeType { get; set; }
        public string ID { get; set; }
    }
}
