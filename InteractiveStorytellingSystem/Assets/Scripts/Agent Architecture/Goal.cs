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

        public Stack<Action> Plan {get; private set;}

        public Goal(GoalType type, string parameters)
        {
            Type = type;
            Parameters = parameters;
            Complete = false;
        }

        public void SetPlan(Stack<Action> p)
        {
            Plan = p;
        }
    }
}
