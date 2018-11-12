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
        [SerializeField] private TextAsset ActionListFile;
        private EventManager eventManager;

        public EmotionalPersonality Personality { get; private set; } //emotional personality of the character
        public List<Action> ActionList { get; private set; }
        private EventPriorityQueue ReceivingQueue = new EventPriorityQueue();

        public void SendEventManagerReference(EventManager eventManagerReference)
        {
            eventManager = eventManagerReference;
        }

        public void CreatePersonality()
        {
            this.Personality = ConfigReader.ConfigReader.ReadEmotionData(PersonalityFile.name + ".xml");
        }

        public void CreateActionList()
        {
            this.ActionList = ConfigReader.ConfigReader.ReadActionList(ActionListFile.name + ".xml");
        }

        public void SendAction(Action action)
        {
            ReceivingQueue.Add(1, action); //TODO: implement priority system!
        }

        private void SendActionToEventManager(Action action)
        {
            eventManager.AddAction(action);
        }

        public void Update()
        {
            if(!ReceivingQueue.IsEmpty())
            {
                Action receivedAction = ReceivingQueue.Remove();
                Debug.Log(receivedAction.Name);
                //analyse action
                //respond
                if(receivedAction.Name == "Greeting")
                    SendActionToEventManager(ActionList[0]);
            }
        }
    }
}
