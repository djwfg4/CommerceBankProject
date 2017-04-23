using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetingApplication.ViewModels
{
    public class BadgesModelView
    {
        //an iterator of the badges needs to be here
        public int badgeCount;
        public IEnumerable<String> badgeNames { get; set; }
    }
}