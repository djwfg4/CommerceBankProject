using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetingApplication.Models;

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
           
            return View(BudgetGoalList.ToList());
        }

        public ActionResult InsertBudgetGoal()
        {
            ViewBag.GoalCategory = dbContext.Categories.Select(x => new SelectListItem()
            {
                Text = x.CategoryType,
                Value = x.CategoryID.ToString()
            }).OrderBy(v => v.Text);

            return View();
        }
        [HttpPost]
        public ActionResult InsertBudgetGoal(BudgetGoal id)
        {
            //BudgetGoal Add_Brand = new BudgetGoal();
            //dbContext.BudgetGoals.Add(Add_Brand);
            //dbContext.SaveChanges();
            return RedirectToAction("index");
        }
    }
}