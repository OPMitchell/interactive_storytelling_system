using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using InteractiveStorytellingSystem.ConfigReader;

public class ActionQueue : EventPriorityQueue
{
    void Start()
    {
        //[TESTING]]
        /*
        if(transform.name == "Rachel")
        {
            QueueAction(GetComponent<ActionDirectory>().GetActionByIndex(1));
        */
    }

    public PriorityQueue GetQueue()
    {
        return queue;
    }

    public override void CheckQueue()
    {
        if(!queue.IsEmpty() && !GetComponent<ActionExecutor>().Executing)
        {
            Testing.PrintMessage(Testing.GetActionQueue(transform));
            Action action = queue.Remove();
            StartCoroutine(Execute(action));
        }
    }

    private IEnumerator Execute(Action action)
    {
        string actionInfo = Testing.GetActionInfo(action);
        Debug.Log(actionInfo + " has been started");
        StartCoroutine(GetComponent<ActionExecutor>().ExecuteAction(action));
        yield return new WaitUntil(() => !GetComponent<ActionExecutor>().Executing);
        if(action.Status == Status.Successful)
        {
            Transform target = GameManager.FindGameObject(action.Target).transform;
            if(target.GetComponent<ReceivingQueue>() != null)
                target.GetComponent<ReceivingQueue>().QueueAction(action);
        }
    }


}
