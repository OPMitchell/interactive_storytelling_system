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

    private bool executing;

	void Start()
	{
        CreatePersonality();
        CreateResponseList();
	    receivingQueue = new EventPriorityQueue();
        actionQueue = new EventPriorityQueue();
        executing = false;
        //[TESTING] 
        if(transform.name == "Rachel")
            QueueAction(GetComponent<ActionDirectory>()
            .GetActionByIndex(1));
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

    void Update()
    {
        CheckReceivingQueue();
        CheckActionQueue();
    }

    private void CheckActionQueue()
    {
        if(!actionQueue.IsEmpty() && !executing)
        {
            Action action = actionQueue.Remove();
            executing = true;
            StartCoroutine(Execute(action));
        }
    }

    private void CheckReceivingQueue()
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
    }

    private IEnumerator Execute(Action action)
    {
        string actionInfo = GameManager.GetActionInfo(action);
        Debug.Log(actionInfo + " has been started");
        StartCoroutine(GetComponent<ActionExecutor>().ExecuteAction(action));
        yield return new WaitUntil(() => !GetComponent<ActionExecutor>().Executing);
        if(action.status == Status.Successful && action.Target != "Player")
        {
            SendActionToCharacter(action);
        }
        executing = false;
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
