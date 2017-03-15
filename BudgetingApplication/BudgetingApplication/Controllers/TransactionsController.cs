using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Diagnostics;

namespace BudgetingApplication.Controllers
{
    public class TransactionsController : Controller
    {
        private DataContext dbContext = new DataContext();
        private int ACCOUNT_NUMBER = 6;
        private int CLIENT_ID = 1;

        // GET: Transactions
        public ActionResult Index()
        {
            var query = from transaction in dbContext.Transactions
                            join account in dbContext.Accounts 
                            on transaction.TransactionAccountNo equals account.AccountNo
                            where account.ClientID == CLIENT_ID
                            orderby transaction.TransactionDate
                            select transaction;

            /*SELECT transactions.*
            FROM transactions
            JOIN account
            ON transactions.transactionaccountno = account.accountno
            WHERE account.clientid = 1
            ORDER BY transactions.transactiondate;*/

            List<Transaction> transactionList = new List<Transaction>();
            transactionList = query.ToList();
           
            return View(transactionList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ActionResult ViewSearchResults(string search)
        {
            List<Transaction> transactionList = new List<Transaction>();
            transactionList = this.GetSearchResults(search);

            return View(transactionList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private List<Transaction> GetSearchResults(string search)
        {
            List<Transaction> transactionList = new List<Transaction>();

            transactionList = dbContext.Transactions.
                Where
                (
                    trans => trans.Category.ToString().Contains(search) ||
                    trans.Description.ToString().Contains(search)
                ).
                OrderBy(date => date.TransactionDate).ToList();

            var query = from transaction in dbContext.Transactions
                        join account in dbContext.Accounts
                        on transaction.TransactionAccountNo equals account.AccountNo
                        where account.ClientID == CLIENT_ID
                        where transaction.Category.CategoryType.Contains(search) ||
                              transaction.Description.Contains(search) ||
                              transaction.Account.AccountType.Contains(search)
                        orderby transaction.TransactionDate
                        select transaction;

            return transactionList;
        }
    }
}