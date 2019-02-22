using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;

namespace InteractiveStorytellingSystem
{
    public enum Status
    {
        notSent = 1,
        Sent = 2,
        Failed = 3,
        Successful = 4,
        Interrupted = 5,
    };

    public class Action
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("sender")]
        public string Sender { get; set; }
        [XmlAttribute("target")]
        public string Target { get; set; }
        [XmlAttribute("dialogid")]
        public string DialogID { get; set; }
        [XmlAttribute("precondition")]
        public string Precondition { get; set; }
        [XmlAttribute("effect")]
        public string Effect { get; set; }
        [XmlAttribute("parameters")]
        public string Parameters { get; set; }
        [XmlAttribute("priority")]
        public int Priority { get; set; }
        [XmlAttribute("keywords")]
        public string Keywords { get; set; }
        public Status Status{ get; set; }

        public Action()
        {
            Status = Status.notSent;
        }

        public Action(string name, string type, string sender, string target, string dialogid, string precondition, string effect, string parameters, int priority, string keywords)
        {
            Name = name;
            Type = type;
            Sender = sender;
            Target = target;
            DialogID = dialogid;
            Precondition = precondition;
            Effect = effect;
            Parameters = parameters;
            Priority = priority;
            Keywords = keywords;
            Status = Status.notSent;
        }

        public Action(Action a)
        {
            Replace(a);
            Status = Status.notSent;
        }

        public string[] GetKeywords()
        {
            string[] keywords = ((string)Keywords.Clone()).Split(',');
            for(int i = 0; i < keywords.Length; i++)
            {
                switch(keywords[i])
                {
                    case "*":
                        keywords[i] = Sender;
                        break;
                    case "%tgt":
                        keywords[i] = Target;
                        break;
                    case "%val":
                        keywords[i] = Parameters;
                        break;
                }
            }
            return keywords;
        }

        public bool HasPrecondition()
	    {
            return (Precondition != "");
	    }

        public bool IsPreconditionSatisfied()
        {
            bool result = false;
            string[] split = GameManager.SplitParameterString(Precondition);
            string statName = split[0];
            string operation = split[1];
            string value = split[2];
            switch(statName)
            {
                case "hunger": 
                    result = IsFloatPreconditionSatisfied(operation, value, GameManager.GetStat<float>(Target, statName));
                break;
                case "anger":
                    result = IsFloatPreconditionSatisfied(operation, value, GameManager.GetStat<float>(Target, statName));
                break;
                case "happiness":
                    result = IsFloatPreconditionSatisfied(operation, value, GameManager.GetStat<float>(Target, statName));
                    break;
                case "inventory":
                    result = GameManager.FindGameObject(Target).GetComponent<Inventory>().Contains(value);
                break;
                case "location":
                {
                    try
                    {
                        result = GameManager.FindGameObject(Target).GetComponent<MovementManager>().CheckIfAtLocation(GameManager.FindGameObject(value).transform);
                    }
                    catch(Exception ex)
                    {
                        Testing.PrintMessage(ex.ToString());
                        result = false;
                    }
                }
                break;
            }
            return result;
        }

        private bool IsFloatPreconditionSatisfied(string operation, string value, float actualValue)
        {
            float Value = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            if(operation == "lessthan")
            {
                return(actualValue < Value);
            }
            else if (operation == "greaterthan")
            {
                return(actualValue > Value);
            }
            return false;
        }

        public void SetStatus(Status s)
        {
            Status = s;
        }

        public void Replace(Action newAction)
        {
            this.Name = newAction.Name;
            this.Type = newAction.Type;
            this.Sender = newAction.Sender;
            this.Target = newAction.Target;
            this.DialogID = newAction.DialogID;
            this.Precondition = newAction.Precondition;
            this.Effect = newAction.Effect;
            this.Parameters = newAction.Parameters;
            this.Priority = newAction.Priority;
            this.Keywords = newAction.Keywords;
        }

        public bool Compare(Action a)
        {
            if(this.Name == a.Name
            && this.Type == a.Type
            && this.Sender == a.Sender
            && this.Target == a.Target
            && this.DialogID == a.DialogID
            && this.Precondition == a.Precondition
            && this.Effect == a.Effect
            && this.Parameters == a.Parameters
            && this.Priority == a.Priority
            && this.Keywords == a.Keywords
            )
                return true;
            return false;
        }
    }
}
