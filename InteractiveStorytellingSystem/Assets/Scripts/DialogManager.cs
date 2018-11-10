using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    static class DialogManager
    {
        const string xml = "Dialog.xml";
        [XmlElement("Dialog")]
        public static List<Dialog> Dialog { get; set; }

        static DialogManager()
        {
            Dialog = ConfigReader.ConfigReader.ReadDialogList(xml);
        }
    }

    public struct Dialog
    {
        [XmlAttribute("dialogid")]
        public string DialogID { get; set; }
        [XmlElement("Value")]
        public string Value { get; set; }
    }
}
