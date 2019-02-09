using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace InteractiveStorytellingSystem
{
    public class PlanList
    {
        private List<Plan> plans = new List<Plan>();

        public void Add(Plan plan)
        {
            if(!plans.Any(x => x.isEqual(plan)))
                plans.Add(plan);
        }

        public void Remove(Plan plan)
        {
            plans.Remove(plans.Single(x => x == plan));
        }

        public Plan GetBestPlan()
        {
            return plans.OrderByDescending(x => x.Score).FirstOrDefault();
        }

        public int Count()
        {
            return plans.Count;
        }
    }
}
