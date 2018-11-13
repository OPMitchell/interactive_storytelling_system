using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace InteractiveStorytellingSystem
{
    public class Character : MonoBehaviour
    {
        [SerializeField] public string Name; //name of the character
        [SerializeField] private TextAsset PersonalityFile;
        [SerializeField] private TextAsset ActionListFile;

        public EmotionalPersonality Personality { get; private set; } //emotional personality of the character
        public List<Action> ActionList { get; private set; }
        private EventPriorityQueue receivingQueue = new EventPriorityQueue();

        public void Awake()
        {
            CreatePersonality();
            CreateActionList();
            receivingQueue = new EventPriorityQueue();
        }

        private void CreatePersonality()
        {
            this.Personality = ConfigReader.ConfigReader.ReadEmotionData(PersonalityFile.name + ".xml");
        }

        private void CreateActionList()
        {
            this.ActionList = ConfigReader.ConfigReader.ReadActionList(ActionListFile.name + ".xml");
        }

        public void SendAction(Action action)
        {
            receivingQueue.Add(1, action); //TODO: implement priority system!
        }

        public void Update()
        {
            if(!receivingQueue.IsEmpty())
            {
                Action receivedAction = receivingQueue.Remove();
                //analyse action
                //respond
                if(receivedAction.Name == "Greeting")
                    GameManager.AddActionToEventManager(ActionList[0]);
            }
        }
    }
}
