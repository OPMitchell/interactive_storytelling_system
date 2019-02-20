using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using InteractiveStorytellingSystem.ConfigReader;

public class ReceivingQueue : EventPriorityQueue
{
    [SerializeField] private TextAsset ResponseListFile;
    public List<Response> ResponseList {get; private set;}

    private void CreateResponseList()
    {
        this.ResponseList = ConfigReader.ReadResponseList(ResponseListFile.name + ".xml");
    }

    void Start()
    {
        CreateResponseList();
    }

    public override void CheckQueue()
    {
        //Respond to incoming actions from other characters
        if(!queue.IsEmpty())
        {
            Action receivedAction = queue.Remove();
            Debug.Log(transform.name + " received action: " + receivedAction.Name + " from sender: " + receivedAction.Sender);
            //analyse action
            //respond
            foreach (Response r in ResponseList)
            {
                if(r.Name == receivedAction.Name && (r.Sender == receivedAction.Sender || r.Sender == "*") && r.DialogID == receivedAction.DialogID)
                {
                    Action response = r.Action;
                    if (response.Sender == "*")
                        response.Sender = receivedAction.Sender;
                    if (response.Target == "*")
                        response.Target = receivedAction.Sender;
                    GetComponent<ActionQueue>().QueueAction(response);        
                }
            }
        }
    }

}
