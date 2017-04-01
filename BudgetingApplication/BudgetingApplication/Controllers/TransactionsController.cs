﻿using BudgetingApplication.Models;
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
        private DataContext dbContext = new DataContext();
        private int CLIENT_ID = 2;

        /// <summary>
        /// Called on initial page load.
        /// Creates new TransactionsViewModel with Transactions from current month and year.
        /// Returns the new TransactionsViewModel to the Index View.
        /// </summary>
        /// <returns> Returns the new TransactionsViewModel to the Index View. </returns>
        public ActionResult Index()
        {
            DateTime dateTime = System.DateTime.Now;
            TransactionsViewModel model = this.CreateModel(dateTime.Month, dateTime.Year);
            return View(model);
        }

        /// <summary>
        /// Triggered by clicking on the Update button in the Index View.
        /// Creates a new TransactionsViewModel based on the month and year parameters.
        /// Filters the TransactionsViewModel by account number and category type.
        /// Also filters via searching by description.
        /// Returns the new TransactionsViewModel back to the Index View.
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="sortBy"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="account"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public ActionResult Filter(string searchString, int? month, int? year, int? account, string category)
        {
            TransactionsViewModel model = this.CreateModel(month.Value, year.Value);

            if(account != null)
            {
                model.Transactions = this.FilterTransactionsByAccount(model.Transactions, account.Value);
            }
            if (!String.IsNullOrEmpty(category))
            {
                model.Transactions = this.FilterTransactionsByCategory(model.Transactions, category);
                model.BudgetGoals = this.GetBudgetGoals(month.Value, year.Value);
                model.Category = category;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                model.Transactions = this.SearchTransactions(model.Transactions, searchString);
            }
            
            return View("Index", model);
        }

        /// <summary>
        /// Triggered by a user clicking on the Reset button in the Index View.
        /// Creates a new TransactionsViewModel with every Transaction from the Index View's month and year.
        /// Returns the new TransactionsViewModel back to the Index View.
        /// </summary>
        /// <param name="month">Month</param>
        /// <param name="year">Year</param>
        /// <returns> Returns the new TransactionsViewModel to the Index View. </returns>
        public ActionResult Reset(int? month, int? year)
        {
            TransactionsViewModel model = this.CreateModel(month.Value, year.Value);
            return View("Index", model);
        }

        /// <summary>
        /// Triggered by a user clicking on one of the two arrows at the top of the Index View.
        /// Logic for updating month and year is in the Index View.
        /// Creates a new TransactionsViewModel with the updated month and year.
        /// Returns the new TransactionsViewModel back to the Index View.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns> Returns the new TransactionsViewModel to the Index View. </returns>
        public ViewResult SwitchMonth(int? month, int? year)
        {
            TransactionsViewModel model = this.CreateModel(month.Value, year.Value);
            return View("Index", model);
        }

        /// <summary>
        /// Creates a new TransactionsViewModel.
        /// Initializes its DateTime, Month, Year, Accounts, Categories, Client, and Transactions.
        /// Transactions are queried with the month and year parameters.
        /// Returns the new TransactionsViewModel.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns> Returns the new TransactionsViewModel. </returns>
        private TransactionsViewModel CreateModel(int month, int year)
        {
            TransactionsViewModel model = new TransactionsViewModel();
            model.DateTime = new DateTime(year, month, 1);
            model.Month = month;
            model.Year = year;
            model.Accounts = this.GetAccounts();            
            model.Categories = this.GetCategories();
            model.Client = this.GetClient();
            model.Transactions = this.GetTransactions(month, year);
            return model;
        }

        /// <summary>
        /// Gets Budget Goals for the specified Client from 
        /// the database based on the month and year parameters.
        /// Returns the Budget Goals as a List.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<BudgetGoal> GetBudgetGoals(int month, int year)
        {
            var budgetGoals = from goal in dbContext.BudgetGoals
                              where goal.ClientID == CLIENT_ID &&
                                    goal.Month.Month == month &&
                                    goal.Month.Year == year
                              select goal;

            return budgetGoals.ToList();
        }

        /// <summary>
        /// Gets the Client information from the database based on the current user.
        /// Returns the Client entity.
        /// </summary>
        /// <returns></returns>
        private Client GetClient()
        {
            var client = dbContext.Clients.Single(cl => cl.ClientID == CLIENT_ID);
            return client;
        }

        /// <summary>
        /// Gets Transactions from the database which fall in the month and year.
        /// Returns a list of Transactions.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<Transaction> GetTransactions(int month, int year)
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
        /// Gets a list of Accounts from database for a specific Client ID.
        /// Returns the list of Accounts.
        /// </summary>
        /// <returns> Returns the list of Accounts. </returns>
        private List<Account> GetAccounts()
        {
            var accounts = from account in dbContext.Accounts
                           orderby account.AccountNo
                           where account.ClientID == CLIENT_ID
                           select account;

            return accounts.ToList();
        }

        /// <summary>
        /// Filters the transactionList based on a account number.
        /// Returns a new list of Transactions matching the account number.
        /// </summary>
        /// <param name="transactionList"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private List<Transaction> FilterTransactionsByAccount(List<Transaction> transactionList, int account)
        {
            var transactions = from trans in transactionList
                               where trans.Account.AccountNo == account
                               select trans;

            return transactions.ToList();
        }

        /// <summary>
        /// Filters the transactionList based on a category type.
        /// Returns a new list of Transactions matching the category type.
        /// </summary>
        /// <param name="transactionList"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private List<Transaction> FilterTransactionsByCategory(List<Transaction> transactionList, string category)
        {
            var transactions = from trans in transactionList
                               where trans.Category.CategoryType.Equals(category)
                               select trans;

            return transactions.ToList();
        }

        /// <summary>
        /// Searches each Transaction description in the transactionList.
        /// Case of searchString doesn't matter.
        /// Returns a new list comprised of Transactions for which the 
        /// description contains the searchString.
        /// </summary>
        /// <param name="transactionList"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private List<Transaction> SearchTransactions(List<Transaction> transactionList, string searchString)
        {
            var transactions = from trans in transactionList
                               where trans.Description.ToLower().Contains(searchString.ToLower())
                               select trans;

            return transactions.ToList();
        }

        /// <summary>
        /// Gets all Transaction Categories from database.
        /// Used for populating the Category DropDownList in the Index View.
        /// </summary>
        /// <returns> List of all Transaction Categories from database. </returns>
        private List<Category> GetCategories()
        {
            var categories = from category in dbContext.Categories
                             orderby category.CategoryType
                             where category.ParentCategoryID == null
                             select category;

            return categories.ToList();
        }
    }
}