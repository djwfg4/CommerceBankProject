using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Diagnostics;
using BudgetingApplication.ViewModels;

namespace BudgetingApplication.Controllers
{
    public class TransactionsController : Controller
    {
        private TransactionsViewModel transactionsViewModel = new TransactionsViewModel();
        private DataContext dbContext = new DataContext();
        private int CLIENT_ID = 1;

        // GET: Transactions
        public ActionResult Index(string sortBy, string searchString, int? month, int? year)
        {
            this.SetSorting(sortBy);

            ViewBag.Filter = searchString;
            DateTime date = this.GetCurrentDate();
 
            List<Transaction> transactions = new List<Transaction>();

            if (month == null)
            {
                ViewBag.Month = date.Month;
                ViewBag.Year = date.Year;
                ViewBag.MonthAndYear = date;

                transactions = this.GetTransactions(date.Month, date.Year);
            }
            else
            {
                Tuple<int?, int?> monthAndYear = this.ChangeMonth(month, year);
                transactions = this.GetTransactions(monthAndYear.Item1, monthAndYear.Item2);
                ViewBag.MonthAndYear = new DateTime(monthAndYear.Item2.Value, monthAndYear.Item1.Value, 1);
            }
            
            List<Account> accounts = this.GetAccounts();

            Client client = this.GetClient();

            List<Category> categories = this.GetCategories();

            if (!String.IsNullOrEmpty(searchString))
            {                
                transactions = this.SearchTransactionDescriptions(transactions, searchString);
            }

            if(sortBy == "description" || sortBy == "description_desc")
            {
                transactions = this.SortTransactionsByDescription(transactions, sortBy);
            }
            else
            {
                transactions = this.SortTransactions(transactions, sortBy);
            }

            TransactionsViewModel viewModel = new TransactionsViewModel
            {
                Transactions = transactions,
                Accounts = accounts,
                Client = client,
                Categories = categories
            };

            Debug.WriteLine("Categories: ");

            return View(viewModel);
        }

        /// <summary>
        /// Sets the sortBy string passed into the Index ActionResult.
        /// </summary>
        /// <param name="sortBy"></param>
        private void SetSorting(string sortBy)
        {
            ViewBag.AmountSort = sortBy == "amount" ? "amount_desc" : "amount";
            ViewBag.DateSort = sortBy == "date" ? "date_desc" : "date";
            ViewBag.DescriptionSort = sortBy == "description" ? "description_desc" : "description";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="month"></param>
        private Tuple<int?, int?> ChangeMonth(int? month, int? year)
        {
            switch (month)
            {
                case 0:
                    month = 12;
                    ViewBag.Month = month;
                    year--;                   
                    ViewBag.Year = year;                   
                    break;
                case 13:
                    month = 1;
                    ViewBag.Month = month;
                    year++;                   
                    ViewBag.Year = year;
                    break;
                default:
                    ViewBag.Month = month;
                    ViewBag.Year = year;                   
                    break;
            }
            return new Tuple<int?, int?>(month, year);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        private List<Transaction> SortTransactionsByDescription(List<Transaction> transactions, string sortBy)
        {
            switch (sortBy)
            {
                case "description":
                    transactions = transactions.OrderBy(trans => trans.Description).ToList();
                    break;
                case "description_desc":
                    transactions = transactions.OrderByDescending(trans => trans.Description).ToList();
                    break;
            }
            return transactions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        private List<Transaction> SortTransactions(List<Transaction> transactions, string sortBy)
        {
            switch (sortBy)
            {
                case "amount":
                    transactions = transactions.OrderBy(trans => trans.TransactionAmount).ToList();
                    break;
                case "amount_desc":
                    transactions = transactions.OrderByDescending(trans => trans.TransactionAmount).ToList();
                    break;
                case "date_desc":
                    transactions = transactions.OrderByDescending(trans => trans.TransactionDate).ToList();
                    break;
                default:
                    transactions = transactions.OrderBy(trans => trans.TransactionDate).ToList();
                    break;
            }
            return transactions.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Client GetClient()
        {
            var client = dbContext.Clients.Single(cl => cl.ClientID == CLIENT_ID);

            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<Transaction> GetTransactions(int? month, int? year)
        {
            var transactions = from transaction in dbContext.Transactions
                                    join account in dbContext.Accounts
                                    on transaction.TransactionAccountNo equals account.AccountNo
                                    where account.ClientID == CLIENT_ID 
                                            && transaction.TransactionDate.Month == month
                                            && transaction.TransactionDate.Year == year
                                    orderby transaction.TransactionDate
                                    select transaction;

            return transactions.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<Account> GetAccounts()
        {
            var accounts = from account in dbContext.Accounts
                                orderby account.AccountNo
                                where account.ClientID == CLIENT_ID
                                select account;

            return accounts.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<Category> GetCategories()
        {
            var categories = from category in dbContext.Categories
                             orderby category.CategoryType
                             where category.ParentCategoryID == null
                             select category;

            return categories.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DateTime GetCurrentDate()
        {
            return System.DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private List<Transaction> SearchTransactionDescriptions(List<Transaction> transactions, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                transactions = transactions.Where(trans => trans.Description.Contains(searchString)).ToList();
            }
            return transactions;
        }
    }
}