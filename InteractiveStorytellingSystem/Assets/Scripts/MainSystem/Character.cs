using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    /// <summary>
    /// Class for holding data on an individual character, including their personality and emotional profile
    /// </summary>

    public class Character
    {
        [XmlAttribute("name")] public string Name { get; set; } //name of the character
        [XmlAttribute("personality")] public string PersonalityPath { get; set; }
        [XmlAttribute("actionlist")] public string ActionListPath { get; set; }
        public EmotionalPersonality Personality { get; set; } //emotional personality of the character
        public List<Action> ActionList { get; set; }

        public void CreatePersonality()
        {
            //Read in emotional data from xml file for this character
            this.Personality = ConfigReader.ConfigReader.ReadEmotionData(PersonalityPath);
        }

        public void CreateActionList()
        {
            this.ActionList = ConfigReader.ConfigReader.ReadActionList(ActionListPath);
        }
    }
}
