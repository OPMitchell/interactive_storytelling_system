using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace InteractiveStorytellingSystem
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private string Name; //name of the character
        [SerializeField] private TextAsset PersonalityFile;
        [SerializeField] private object ActionListFile;
        public EmotionalPersonality Personality { get; private set; } //emotional personality of the character
        public List<Action> ActionList { get; private set; }

        public void Start()
        {
            CreatePersonality();
            CreateActionList();
        }

        public void CreatePersonality()
        {
            this.Personality = ConfigReader.ConfigReader.ReadEmotionData(PersonalityFile.name + ".xml");
        }

        public void CreateActionList()
        {
            //this.ActionList = ConfigReader.ConfigReader.ReadActionList(ActionListPath);
        }
    }
}
