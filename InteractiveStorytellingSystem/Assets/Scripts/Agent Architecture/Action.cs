using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    public enum Status
    {
        notSent = 1,
        Sent = 2,
        Failed = 3,
        Successful = 4
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

        public Status status{ get; set; }


        public Action()
        {
            status = Status.notSent;
        }

        public Action(Action a)
        {
            Replace(a);
            status = Status.notSent;
        }

        public bool HasPrecondition()
	    {
            if(Precondition == "")
                return false;
            return true;
	    }

        public void SetStatus(Status s)
        {
            status = s;
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
            )
                return true;
            return false;
        }
    }
}
