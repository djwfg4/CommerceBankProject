﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BudgetingApplication.Controllers
{
    public class SavingsGoalController : Controller
    {
        // GET: SavingsGoal
        public ActionResult Index()
        {
            return View();
        }
    }
}