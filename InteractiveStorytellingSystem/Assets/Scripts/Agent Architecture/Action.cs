using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
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

        public bool HasPrecondition()
	    {
            if(Precondition == "")
                return false;
            return true;
	    }
    }
}
