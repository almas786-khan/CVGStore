using CVGS.Data;
using CVGS.Models;
using NUnit.Framework.Internal.Execution;
using OfficeOpenXml;
using Org.BouncyCastle.Utilities.Date;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;



using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace CVGS.Controllers
{
    public class ReportController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MemberReport()
        {
            List<MemberDetailsView> list = new List<MemberDetailsView>();
            var query = from member in _context.Users
                        //join address in _context.Addresses
                        //on member.UserID equals address.MemberID
                        where member.UserType != "Admin"
                        select new MemberDetailsView
                        {
                            UserID = member.UserID,
                            UserName = member.UserName,
                            UserEmail = member.UserEmail,
                            UserFirstName = member.UserFirstName,
                            UserLastName = member.UserLastName,
                            Gender = member.Gender,
                            Birthdate = member.Birthdate
                        };
            list = query.ToList();
            return View(list);
        }
        public ActionResult PopularReport()
        {
            List<PopularGameReport> list = new List<PopularGameReport>();
            ReportDAO report = new ReportDAO();
            list = report.PopularGame();
            return View(list);
        }
        public ActionResult WishListReport()
        {
            List<WishListReport> list = new List<WishListReport>();
            ReportDAO report = new ReportDAO();
            list = report.WishListFetchAll();
            return View(list);
        }

        public ActionResult PopularMember()
        {
            List<PopularMember> list = new List<PopularMember>();
            ReportDAO report = new ReportDAO();
            list = report.PopularMember();
            return View(list);
        }
        public ActionResult SalesReport()
        { 
            List<SalesReportModel> salesReportModels = new List<SalesReportModel>();
            double s=0;
            ReportDAO report = new ReportDAO();
            salesReportModels = report.SalesFetchAll();
            for (int i = 0; i < salesReportModels.Count; i++)
            {
                s = salesReportModels[i].Total + s;
            }
            TempData["Total"] = s;
            ViewBag.Total = TempData["Total"];
            return View(salesReportModels);
        }
        public ActionResult GameReport()
        {
            return View(_context.Games.ToList());
           
        }
        public void ExportToExcelForGame()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            List<Games> gamelist = new List<Games>();

            var query = from game in _context.Games
                       
                        select new Games
                        {
                            GameID = game.GameID,
                            GameName = game.GameName,
                            GameDescription = game.GameDescription,
                            GamePrice = game.GamePrice,
                            GameOverallRating = game.GameOverallRating,
                           
                        };

            gamelist = query.ToList();
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
          
            ws.Cells["A1"].Value = "Game";
            ws.Cells["B1"].Value = "Report";

            ws.Cells["A2"].Value = "List of all games with detail";
            ws.Cells["B2"].Value = "Pink Games have less than 3 rating";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            
            ws.Cells["B6"].Value = "Game Name";
            ws.Cells["C6"].Value = "Game Description";
            ws.Cells["D6"].Value = "Game Price";
            ws.Cells["E6"].Value = "Game Overall Rating";
           

            int rowStart = 7;
            foreach (var item in gamelist)
            {
                //if game overall rating is less than 3, the row color is pink.
                if (item.GameOverallRating < 3)
                {
                    ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));

                }
                
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.GameName;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.GameDescription;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.GamePrice;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.GameOverallRating;
               
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
        public void ExportToExcelForSales()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            List<SalesReportModel> list = new List<SalesReportModel>();
            ReportDAO report = new ReportDAO();
            double s = 0;
            list = report.SalesFetchAll();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
            ws.Cells["A1"].Value = "Report";
            ws.Cells["B1"].Value = "Sales Details";

            ws.Cells["A2"].Value = "";
            ws.Cells["B2"].Value = "";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H mm tt}", DateTimeOffset.Now);

            ws.Cells["B6:F6"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells["B6:F6"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("yellow")));

            ws.Cells["B6"].Value = "Order Date";
            ws.Cells["C6"].Value = "Game Name";
            ws.Cells["D6"].Value = "Game Price";
            ws.Cells["E6"].Value = "Purchases";
            ws.Cells["F6"].Value = "Total Amount";

            int rowStart = 7;

            foreach (var item in list)
            {
           
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.OrderDate.ToString("MM/dd/yyyy");
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.GameName;
                ws.Cells[string.Format("D{0}", rowStart)].Value = "$"+ item.GamePrice;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Purchases;
                ws.Cells[string.Format("F{0}", rowStart)].Value = "$"+item.TotalAmount;

                rowStart++;
            }
            int rown = 7 + list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                s = list[i].Total + s;
            }
            ws.Cells[$"E{rown}"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[$"E{rown}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));
            ws.Cells[$"F{rown}"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[$"F{rown}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));
            ws.Cells[$"E{rown}"].Value = "Total Sale";
            ws.Cells[$"F{rown}"].Value = "$"+s;

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();



        }
        public void ExportToExcelForWishList()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            List<WishListReport> list = new List<WishListReport>();
            ReportDAO report = new ReportDAO();
            list = report.WishListFetchAll();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Report";
            ws.Cells["B1"].Value = "WishList Game Details";

            ws.Cells["A2"].Value = "";
            ws.Cells["B2"].Value = "";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H mm tt}", DateTimeOffset.Now);


            ws.Cells["B6"].Value = "In People Wishlist";
            ws.Cells["C6"].Value = "Game Name";


            int rowStart = 7;

            foreach (var item in list)
            {

                ws.Cells[string.Format("B{0}", rowStart)].Value = item.occurence;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.gameName;
                rowStart++;
            }
            int rown = 8 + list.Count;
            // ws.Cells["E",rown].Value = TempData["Total"];

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }
        public void ExportToExcelForPopularM()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            List<PopularMember> list = new List<PopularMember>();
            ReportDAO report = new ReportDAO();
            list = report.PopularMember();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Report";
            ws.Cells["B1"].Value = "Top Buyers Details";

            ws.Cells["A2"].Value = "";
            ws.Cells["B2"].Value = "";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H mm tt}", DateTimeOffset.Now);


            ws.Cells["B6"].Value = "Amount Spentt";
            ws.Cells["C6"].Value = "Member User Name";


            int rowStart = 7;

            foreach (var item in list)
            {

                ws.Cells[string.Format("B{0}", rowStart)].Value = item.amount;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.name;
                rowStart++;
            }
            int rown = 8 + list.Count;
          

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }
        public void ExportToExcelForPopular()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            List<PopularGameReport> list = new List<PopularGameReport>();
            ReportDAO report = new ReportDAO();
            list = report.PopularGame();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
            ws.Cells["A1"].Value = "Report";
            ws.Cells["B1"].Value = "Popular Game Details";

            ws.Cells["A2"].Value = "";
            ws.Cells["B2"].Value = "";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H mm tt}", DateTimeOffset.Now);


            ws.Cells["B6"].Value = "Purchases";
            ws.Cells["C6"].Value = "Game Name";
            ws.Cells["D6"].Value = "Overall Rating";
           

            int rowStart = 7;

            foreach (var item in list)
            {

                ws.Cells[string.Format("B{0}", rowStart)].Value = item.purchased;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.GameName;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.GameOverallRating;
                rowStart++;
            }
            int rown = 8 + list.Count;
            // ws.Cells["E",rown].Value = TempData["Total"];

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }
        public void ExportToExcelMember()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            List<MemberDetailsView> list = new List<MemberDetailsView>();
            var query = from member in _context.Users
                        where member.UserType != "Admin"
                        select new MemberDetailsView
                        {
                            UserID = member.UserID,
                            UserName = member.UserName,
                            UserEmail = member.UserEmail,
                            UserFirstName = member.UserFirstName,
                            UserLastName = member.UserLastName,
                            Gender = member.Gender,
                            Birthdate = member.Birthdate
                        };
            list = query.ToList();
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
            ws.Cells["A1"].Value = "Report";
            ws.Cells["B1"].Value = "Member Details";

            ws.Cells["A2"].Value = "";
            ws.Cells["B2"].Value = "";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H mm tt}", DateTimeOffset.Now);

          
            ws.Cells["B6"].Value = "UserName";
            ws.Cells["C6"].Value = "Email";
            ws.Cells["D6"].Value = "FirstName";
            ws.Cells["E6"].Value = "LastName";
            ws.Cells["E6"].Value = "Gender";
            ws.Cells["F6"].Value = "Birthdate";

            int rowStart = 7;

            foreach (var item in list)
            {
                DateTime date = DateTime.Now;
                DateTime birth = (DateTime)item.Birthdate;
                if ((date.Year - birth.Year) < 30 )
                {
                    ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));

                }

                ws.Cells[string.Format("B{0}", rowStart)].Value = item.UserName;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.UserEmail;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.UserFirstName;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.UserLastName;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.Gender;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Birthdate;


                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();



        }
       
    }
}