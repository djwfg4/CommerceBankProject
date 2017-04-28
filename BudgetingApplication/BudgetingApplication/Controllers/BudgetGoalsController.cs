using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetingApplication.Models;
using System.Data.Entity;
using BudgetingApplication.ViewModels;

namespace BudgetingApplication.Controllers
{
    public class BudgetGoalsController : Controller
    {
        private DataContext dbContext = new DataContext();
        private static int CLIENT_ID;

        // GET: BudgetGoals
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                CLIENT_ID = int.Parse(Session["UserID"].ToString());
            }
            updateBudgetGoals();
            List<BudgetGoals_VW> BudgetGoalList = new List<BudgetGoals_VW>();


            BudgetGoalList = dbContext.BudgetGoals_VW.Where(x => x.ClientID == CLIENT_ID).ToList();

            BudgetGoalModelView budgetGoal = new BudgetGoalModelView();
            budgetGoal.budgetView = BudgetGoalList;
            budgetGoal.totalBudgeted = budgetGoal.budgetView.Select(x => x).Where(x => x.GoalCategory != 1).Sum(x => Convert.ToDouble(x.BudgetGoalAmount));
            budgetGoal.totalSpent = budgetGoal.budgetView.Select(x => x).Where(x => x.GoalCategory != 1).Sum(x => Convert.ToDouble(x.TransactionAmount)) * -1;
            return View(budgetGoal);
        }

        public ActionResult InsertBudgetGoal(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                CLIENT_ID = int.Parse(Session["UserID"].ToString());
            }

            BudgetGoalModelView budget = new BudgetGoalModelView();
            budget.category = GetSelectListItems(GetAllCategories());

            if(id == null)
            {
                return View(budget);
            }

            budget.budgetGoal = dbContext.BudgetGoals.Find(id);
            if(budget.budgetGoal == null || budget.budgetGoal.ClientID != CLIENT_ID)
            {
                return RedirectToAction("Index"); 
            }
            return View(budget);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertBudgetGoal(BudgetGoalModelView model)
        {
            //repopulate the dropdownlist
            model.category = GetSelectListItems(GetAllCategories());
            BudgetGoal newBudgetGoal = new BudgetGoal();
            newBudgetGoal = model.budgetGoal;
            //insert for next month
                newBudgetGoal.ClientID = CLIENT_ID;
                newBudgetGoal.BudgetPointValue = 25;
            if (newBudgetGoal.BudgetGoalID == 0) //create new category
            {
                newBudgetGoal.Month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                newBudgetGoal.Status = "A";
                dbContext.BudgetGoals.Add(newBudgetGoal);
            }
            else
            {
                dbContext.Entry(newBudgetGoal).State = EntityState.Modified;
            }
            //ignore validation errors on Month and BudgetGoalId ---temporary fix?
            ModelState.Remove("budgetGoal.Month");
            ModelState.Remove("budgetGoal.BudgetGoalId");
            if (ModelState.IsValid)
            {
                dbContext.SaveChanges();
                return RedirectToAction("index");

            }
            return View(model);
        }

        // Just return a list of categories
        private IEnumerable<Category> GetAllCategories()
        {
            return dbContext.Categories.ToList();
        }

        // This returns categories as a selectlist item set
        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<Category> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();
            // This will result in MVC rendering each item as:
            //     <option value="CategoryID">Category Type</option>
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.CategoryID.ToString(),
                    Text = element.CategoryType
                });
            }
            selectList.OrderBy(x => x.Text);
            return selectList;
        }

        /// <summary>
        /// This method will be used as a scheduled task to update the budget goals
        /// Badges will also be earned for each user, no matter who is currently logged in
        /// </summary>
        private void updateBudgetGoals()
        {
            List<BudgetGoal> budgetGoals = dbContext.BudgetGoals.Where(x => x.Month.Month < DateTime.Now.Month && x.Month.Year <= DateTime.Now.Year && x.GoalCategory != 1 && x.Status =="A").ToList();
            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            List<Transaction> transactions = dbContext.Transactions.Where(x => x.TransactionDate.Month == lastMonth.Month && x.TransactionDate.Year == lastMonth.Year && x.CategoryID != 1).ToList();

            foreach(Client client in dbContext.Clients)
            {
                var account = dbContext.Accounts.Where(x => x.ClientID == client.ClientID).Select(x => x.AccountNo).ToList();
                var total = transactions.Where(x => account.Contains(x.TransactionAccountNo)).Sum(x => x.TransactionAmount);
                var budgeted = budgetGoals.Where(x => x.ClientID == client.ClientID).Sum(x => x.BudgetGoalAmount);

                if(total <= budgeted)
                {
                    BadgesModelView bmv = new BadgesModelView();
                    bmv.addNewBadge(84, client.ClientID); //Give user inital load of app badge if they havent earned it.
                }
            }
            

        }
    }
}