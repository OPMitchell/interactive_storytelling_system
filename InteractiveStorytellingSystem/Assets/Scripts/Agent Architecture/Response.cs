using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using InteractiveStorytellingSystem;

namespace InteractiveStorytellingSystem
{
    public class Response
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("sender")]
        public string Sender { get; set; }
        [XmlAttribute("dialogid")]
        public string DialogID { get; set; }

        [XmlElement("Action")]
        public Action Action {get; set;}
    }
}
