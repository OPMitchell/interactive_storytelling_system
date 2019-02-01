using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using InteractiveStorytellingSystem;

namespace InteractiveStorytellingSystem
{
    public class PhysicalResponse
    {
        [XmlAttribute("parameters")]
        public string Parameters { get; set; }

        [XmlElement("Action")]
        public Action Action {get; set;}
    }
}
