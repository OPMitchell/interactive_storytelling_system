using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using InteractiveStorytellingSystem;
using System.Linq;

namespace InteractiveStorytellingSystem
{
    public enum GoalType
    {
        Pursuit = 1,
        Interest = 2
    };

    public class Goal
    {
        [XmlAttribute("type")]
        public GoalType Type { get; private set; }
        [XmlAttribute("target")]
        public string Target { get; private set; }
        [XmlAttribute("parameters")]
        public string Parameters {get; private set;}
        public bool Complete {get; set;}

        [XmlIgnore]
        public Plan Plan {get; private set;}
        public List<Action> FailedActions { get; private set; }
        public int TimesFailed { get; set; }

        public Goal()
        {
            FailedActions = new List<Action>();
            Complete = false;
        }

        public Goal(GoalType type, string target, string parameters)
        {
            FailedActions = new List<Action>();
            Type = type;
            Target = target;
            Parameters = parameters;
            Complete = false;
        }

        public void SetPlan(Plan p)
        {
            Plan = p;
        }

        public void AddFailedAction(Action action)
        {
            if(!FailedActions.Any(x => x.Compare(action)))
            {
                FailedActions.Add(new Action(action));
            }
        }
    }
}
