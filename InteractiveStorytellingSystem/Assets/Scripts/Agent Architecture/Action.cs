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
        [XmlAttribute("sendereffect")]
        public string SenderEffect { get; set; }
        [XmlAttribute("targeteffect")]
        public string TargetEffect { get; set; }
        [XmlAttribute("parameters")]
        public string Parameters { get; set; }

        public Status Status{ get; set; }


        public Action()
        {
            Status = Status.notSent;
        }

        public Action(Action a)
        {
            Replace(a);
            Status = Status.notSent;
        }

        public bool HasPrecondition()
	    {
            if(Precondition == "")
                return false;
            return true;
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
            this.SenderEffect = newAction.SenderEffect;
            this.TargetEffect = newAction.TargetEffect;
            this.Parameters = newAction.Parameters;
        }

        public bool Compare(Action a)
        {
            if(this.Name == a.Name
            && this.Type == a.Type
            && this.Sender == a.Sender
            && this.Target == a.Target
            && this.DialogID == a.DialogID
            && this.Precondition == a.Precondition
            && this.SenderEffect == a.SenderEffect
            && this.TargetEffect == a.TargetEffect
            && this.Parameters == a.Parameters
            )
                return true;
            return false;
        }
    }
}
