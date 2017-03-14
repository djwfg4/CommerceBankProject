using BudgetingApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext dbContext = new DatabaseContext();

        // GET: Home
        public ActionResult Index()
        {
            
            

            return View();
        }

        public ActionResult TechnicalPrototype()
        {



            return View();
        }



        public JsonResult GetCalendarData()
        {
            Dictionary<string, List<string>> transactionDictionary
                = new Dictionary<string, List<string>>();
            /*var transactions = dbContext.Transactions.Where(
                trans => trans.TransactionAccountNo == 2 ||
                trans.TransactionAccountNo == 3).ToArray();
            var query = from cat in dbContext.Categories
                        join trans in dbContext.Transactions on cat.CategoryID equals trans.TransactionCategory
                        select new { CategoryType = cat.CategoryType, TransactionDate = trans.TransactionDate };*/
            bool found;
            /*foreach (var trans in query)
            {
                found = false;
                if (transactionDictionary.Count == 0)
                {
                    transactionDictionary.Add(trans.CategoryType, new List<string>());
                    string date1 = Convert.ToDateTime(trans.TransactionDate).ToString("yyyy-MM-dd");
                    transactionDictionary[trans.CategoryType].Add(date1);
                }
                else
                {
                    foreach (KeyValuePair<string, List<string>> entry in transactionDictionary)
                    {
                        if (entry.Key.Equals(trans.CategoryType))
                        {
                            string date1 = Convert.ToDateTime(trans.TransactionDate).ToString("yyyy-MM-dd");
                            entry.Value.Add(date1);
                            found = true;
                        }
                    }
                    if (found == false)
                    {

                        string date1 = Convert.ToDateTime(trans.TransactionDate).ToString("yyyy-MM-dd");
                        transactionDictionary.Add(trans.CategoryType, new List<string>());
                        transactionDictionary[trans.CategoryType].Add(date1);
                    }
                }
            }*/
            Debug.WriteLine("Categories: " + transactionDictionary.Keys.ToString());
            Debug.WriteLine("Categories: " + transactionDictionary.Values.ToString());
            return new JsonResult { Data = transactionDictionary, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult GetDonutAndBarGraphData()
        {
            List<Transaction> tr = new List<Transaction>();
            Dictionary<string, List<object>> dict = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> dict2 = new Dictionary<string, List<object>>();


            /*
            var query = from cat in dbContext.Categories
                        join trans in dbContext.Transactions on cat.CategoryID equals trans.TransactionCategory
                        select new { CategoryType = cat.CategoryType, TransactionAmount = trans.TransactionAmount};
                        */
            List<object> labels = new List<object>();
            List<object> nums = new List<object>();
            string[] colors = { "rgb(255, 99, 132)",
        "rgb(255, 159, 64)",
        "rgb(255, 205, 86)",
        "rgb(75, 192, 192)",
        "rgb(54, 162, 235)",
        "rgb(153, 102, 255)",
        "rgb(231,233,237)" };

           /* foreach (var trans in query)
            {
                labels.Add(trans.CategoryType);
                nums.Add(trans.TransactionAmount);
            }
            */
           dict["datasets"] = new List<object>();

            dict2["backgroundColor"] = colors.ToList<object>();
            dict2["data"] = nums;

            dict["datasets"].Add(dict2);
            dict["labels"] = labels;

            return new JsonResult { Data = dict, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}