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

        // GET: BudgetGoals
        public ActionResult Index()
        {

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
           
            BudgetGoalModelView budget = new BudgetGoalModelView();
            budget.category = GetSelectListItems(GetAllCategories());

            if(id == null)
            {
                return View(budget);
            }
            budget.budgetGoal = dbContext.BudgetGoals.Find(id);
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

    }
}