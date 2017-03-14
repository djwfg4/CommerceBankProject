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
        private cs451Entities dbContext = new cs451Entities();
        // GET: BudgetGoals
        public ActionResult Index()
        {

            List<BudgetGoals_VW> BudgetGoalList = new List<BudgetGoals_VW>();

            
             BudgetGoalList = dbContext.BudgetGoals_VW.ToList();

            return View(BudgetGoalList.ToList());
        }
    }
}