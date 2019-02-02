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
        [SerializeField] private TextAsset ResponseListFile;
        [SerializeField] private TextAsset PhysicalResponseListFile;

        public EmotionalPersonality Personality { get; private set; } //emotional personality of the character
        public List<Action> ActionList { get; private set; }
        public List<Response> ResponseList {get; private set;}
        public List<PhysicalResponse> PhysicalResponseList {get; private set;}
        private EventPriorityQueue receivingQueue = new EventPriorityQueue();

        private MemoryPool mp = new MemoryPool();

        void Start()
        {
            mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Vacation", "Beach", "Dad"}, MemoryType.social, 1.0f));
            mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Walk", "Forest", "Dad"}, MemoryType.social, 1.0f));
            mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Vacation", "Beach", "Mum"}, MemoryType.social, 1.0f));

            List<Node> nodes = mp.GetNodes();
            List<Connection> connections = mp.GetConnections();
            foreach(Node n in nodes)
            {
                print("Node: " + n.Keyword + ", Activation = " + n.Activation);
            }
            foreach(Connection c in connections)
            {
                print("Connection: nodeA = " + c.A.Keyword + ", nodeB = " + c.B.Keyword + ", strength = " + c.Strength);
            }
        }

        public void Awake()
        {
            CreatePersonality();
            CreateActionList();
            CreateResponseList();
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

        private void CreateResponseList()
        {
            this.ResponseList = ConfigReader.ConfigReader.ReadResponseList(ResponseListFile.name + ".xml");
        }

        private void CreatePhysicalResponseList()
        {
            this.PhysicalResponseList = ConfigReader.ConfigReader.ReadPhysicalResponseList(PhysicalResponseListFile.name + ".xml");
        }

        public void SendAction(Action action)
        {
            receivingQueue.Add(1, action); //TODO: implement priority system!
        }

        public void Update()
        {
            //Respond to incoming actions from other characters
            if(!receivingQueue.IsEmpty())
            {
                Action receivedAction = receivingQueue.Remove();
                //analyse action
                //respond
                foreach (Response r in ResponseList)
                {
                    if(r.Name == receivedAction.Name && r.Sender == receivedAction.Sender && r.DialogID == receivedAction.DialogID)
                    {
                        Action response = r.Action;
                        if (response.Target == "*")
                            response.Target = receivedAction.Sender;
                        GameManager.AddActionToEventManager(response);
                    }
                }
            }
        }


    }
}
