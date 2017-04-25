using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetingApplication.Models;
using System.Data.Entity;

namespace BudgetingApplication.Controllers
{
    public class BudgetGoalsController : Controller
    {
        private DataContext dbContext = new DataContext();
        private static int CLIENT_ID = 1;

        /// <summary>
        /// Get the view for BudgetGoals 
        /// </summary>
        // GET: BudgetGoals
        public ActionResult Index()
        {

            List<BudgetGoals_VW> BudgetGoalList = new List<BudgetGoals_VW>();

            //use the view that displays all transaction for current month
            BudgetGoalList = dbContext.BudgetGoals_VW.Where(x => x.ClientID == CLIENT_ID).ToList();

            BudgetGoalModelView budgetGoal = new BudgetGoalModelView();
            budgetGoal.budgetView = BudgetGoalList;
            budgetGoal.totalBudgeted = budgetGoal.budgetView.Select(x => x).Where(x => x.GoalCategory != 1).Sum(x => Convert.ToDouble(x.BudgetGoalAmount));
            budgetGoal.totalSpent = budgetGoal.budgetView.Select(x => x).Where(x => x.GoalCategory != 1).Sum(x => Convert.ToDouble(x.TransactionAmount)) * -1;
            return View(budgetGoal);
        }

        /// <summary>
        /// Used when user wants to create a new budget category or edit a current one. 
        /// If the id of budget is not client's or a real one, then it returns back to the budget page
        /// </summary>
        /// <param name="id">The id of the budget category</param>
        /// <returns>empty budget for inserting or view of budget to edit.</returns>
        public ActionResult InsertBudgetGoal(int? id)
        {
           
            BudgetGoalModelView budget = new BudgetGoalModelView();
            budget.category = GetSelectListItems(GetAllCategories());

            //null ID means we are inserting a new category
            if(id == null)
            {
                return View(budget);
            }

            //let's look for it
            budget.budgetGoal = dbContext.BudgetGoals.Find(id);
            if(budget.budgetGoal == null || budget.budgetGoal.ClientID != CLIENT_ID)
            {
                //we couldn't find the budget category to edit or it is not the client's, redirect
                return RedirectToAction("Index"); 
            }
            return View(budget);
        }

        /// <summary>
        /// The action for the post when user submits the changes. It can tell between inserting new or updating one.
        /// </summary>
        /// <param name="model">This is passed in by form</param>
        /// <returns>returns us back to the budget page</returns>
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
            else //we're just updating a current budget
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

    }
}