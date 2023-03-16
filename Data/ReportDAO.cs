using CVGS.Controllers;
using CVGS.Models;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace CVGS.Data
{
    public class ReportDAO
    { 
        private string connectionString = @"Data Source = DESKTOP-A3H41C3; Initial Catalog = cvgs; Integrated Security = True; MultipleActiveResultSets=True;Application Name = EntityFramework";
        public List<WishListReport> WishListFetchAll()
        {
            List<WishListReport> results = new List<WishListReport>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "Select Count(Wishlist.GameID), Game.GameName from Wishlist\r\n  join Game on Wishlist.GameID = Game.GameID\r\n  Group by Wishlist.GameID, Game.GameName; ";
                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        WishListReport wish = new WishListReport();
                        wish.occurence = reader.GetInt32(0);
                        wish.gameName = reader.GetString(1);
                        results.Add(wish);
                    }
                }
            }
            return results;
        }
        public List<PopularMember> PopularMember()
        {
            List<PopularMember> results = new List<PopularMember>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = " select sum(o.TotalAmount) as amount , u.UserName from [Order] o" +
                    " join [User] u on o.MemberID = u.UserID group by o.MemberID, " +
                    "u.UserName order by amount desc;";
                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PopularMember wish = new PopularMember();
                        wish.amount = reader.GetDouble(0);
                        wish.name = reader.GetString(1);
                        results.Add(wish);
                    }
                }
            }
            return results;
        }
        public List<SalesReportModel> SalesFetchAll()
        {
            List<SalesReportModel> results = new List<SalesReportModel>();
            double total = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "Select Count(OrderDetail.GameID),Game.GamePrice, Game.GameName, o.OrderDate from [Order] o join " +
                    "OrderDetail on o.OrderID = OrderDetail.OrderID join Game on OrderDetail.GameID = Game.GameID\r\n  " +
                    "Group by OrderDetail.GameID, Game.GamePrice,Game.GameName, o.OrderDate; ";
                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SalesReportModel sales = new SalesReportModel();
                        sales.Purchases = reader.GetInt32(0);
                        sales.GamePrice = reader.GetDouble(1);
                        sales.GameName = reader.GetString(2);
                       sales.OrderDate = reader.GetDateTime(3);
                        sales.TotalAmount = reader.GetInt32(0) * reader.GetDouble(1);
                        sales.Total = total + sales.TotalAmount;
                        results.Add(sales);
                    }
                }
            }
            return results;
        }
        public List<PopularGameReport> PopularGame()
        {

            List<PopularGameReport> results = new List<PopularGameReport>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "  Select Count(OrderDetail.GameID) as coun, Game.GameName, Game.GameOverallRating from OrderDetail\r\n  join Game on OrderDetail.GameID = Game.GameID\r\n  Group by OrderDetail.GameID,Game.GameOverallRating, Game.GameName\r\n  order by  Game.GameOverallRating desc ;";
                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PopularGameReport wish = new PopularGameReport();
                        wish.purchased = reader.GetInt32(0);
                        wish.GameName = reader.GetString(1);
                        wish.GameOverallRating = reader.GetDouble(2);
                       
                        results.Add(wish);
                    }
                }
            }
            return results;
            
        }


    }
}