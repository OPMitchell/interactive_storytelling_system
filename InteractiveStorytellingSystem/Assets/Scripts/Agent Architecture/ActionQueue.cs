using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using InteractiveStorytellingSystem.ConfigReader;

public class ActionQueue : MonoBehaviour 
{
    [SerializeField] private TextAsset PersonalityFile;
    [SerializeField] private TextAsset ResponseListFile;

    public EmotionalPersonality Personality { get; private set; } //emotional personality of the character
    public List<Response> ResponseList {get; private set;}
	private EventPriorityQueue receivingQueue = new EventPriorityQueue();
    private EventPriorityQueue actionQueue = new EventPriorityQueue();

	void Start()
	{
        CreatePersonality();
        CreateResponseList();
	    receivingQueue = new EventPriorityQueue();
        actionQueue = new EventPriorityQueue();
        if(transform.name == "Rachel")
            QueueAction(GetComponent<ActionDirectory>()
            .GetActionByIndex(1));
        StartCoroutine(MonitorReceivingQueue());
	}

    public void QueueAction(Action action)
    {
        actionQueue.Add(1, action); //TODO: implement priority system!
    }

	private void QueueReceivedAction(Action action)
    {
        receivingQueue.Add(1, action); //TODO: implement priority system!
    }

	public void SendActionToCharacter(Action action)
	{
		GameManager.FindCharacter(action.Target)
			.GetComponent<ActionQueue>()
			.QueueReceivedAction(action);
	}

 	//put this code in CAP instead
    public void Update()
    {
        if(!actionQueue.IsEmpty())
        {
            Action action = actionQueue.Remove();
            StartCoroutine(Execute(action));
        }
    }

    private IEnumerator MonitorReceivingQueue()
    {
        while(true)
        {
            //Respond to incoming actions from other characters
            if(!receivingQueue.IsEmpty())
            {
                Action receivedAction = receivingQueue.Remove();
                Debug.Log(receivedAction.Target + " received action: " + receivedAction.Name + " from sender: " + receivedAction.Sender);
                //analyse action
                //respond
                foreach (Response r in ResponseList)
                {
                    if(r.Name == receivedAction.Name && r.Sender == receivedAction.Sender && r.DialogID == receivedAction.DialogID)
                    {
                        Action response = r.Action;
                        if (response.Target == "*")
                            response.Target = receivedAction.Sender;
                        QueueAction(response);              
                    }
                }
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    private IEnumerator Execute(Action action)
    {
        string actionInfo = GameManager.GetActionInfo(action);
        Debug.Log(actionInfo + " has been started");
        if(GetComponent<ActionExecutor>().ExecuteAction(action))
        {
            Debug.Log(transform.name + " is waiting until " + actionInfo + " is complete...");
            yield return new WaitUntil(() => !GetComponent<ActionExecutor>().Executing);
            if(action.Target != "Player")
            {
                Debug.Log(actionInfo + " is complete! Sending confirmation to target for appraisal!");
                SendActionToCharacter(action);
            }
        }
    }
    
	private void CreatePersonality()
    {
        this.Personality = ConfigReader.ReadEmotionData(PersonalityFile.name + ".xml");
    }

    private void CreateResponseList()
    {
        this.ResponseList = ConfigReader.ReadResponseList(ResponseListFile.name + ".xml");
    }


}
