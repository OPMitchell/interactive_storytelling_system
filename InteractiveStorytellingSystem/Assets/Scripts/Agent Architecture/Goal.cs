using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using InteractiveStorytellingSystem;

namespace InteractiveStorytellingSystem
{
    public enum GoalType
    {
        Pursuit = 1,
        Interest = 2
    };

    public class Goal
    {
        public GoalType Type { get; private set; }
        public string Parameters {get; private set;}
        public bool Complete {get; set;}

        public LinkedList<Action> Plan {get; private set;}
        public List<Action> FailedActions { get; private set; }
        public int TimesFailed { get; set; }

        public Goal(GoalType type, string parameters)
        {
            FailedActions = new List<Action>();
            Type = type;
            Parameters = parameters;
            Complete = false;
        }

        public void SetPlan(LinkedList<Action> p)
        {
            Plan = p;
        }

        public void AddFailedAction(Action action)
        {
            Action newA = new Action();
            newA.Replace(action);

            bool exists = false;
            foreach(Action a in FailedActions)
            {
                if(a.Compare(newA))
                {
                    exists = true;
                    break;
                }
            }
            if(!exists)
            {
                FailedActions.Add(newA);
            }

        }
    }
}
