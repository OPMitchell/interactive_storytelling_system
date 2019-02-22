using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using InteractiveStorytellingSystem.ConfigReader;

public class ActionQueue : EventPriorityQueue
{
    Coroutine coroutine;

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
        if(!queue.IsEmpty() && GetComponent<ActionExecutor>().Executing && queue.Peek().Priority == 0 && GetComponent<ActionExecutor>().currentAction.Priority != 0)
        {
            Action save = GetComponent<ActionExecutor>().currentAction;
            GetComponent<ActionExecutor>().InterruptAction();
            queue.Add(save.Priority, save);
        }
        if(!queue.IsEmpty() && !GetComponent<ActionExecutor>().Executing)
        {
            Action action = queue.Remove();
            if(!GameManager.FindGameObject(action.Target).GetComponent<ActionExecutor>().Executing)
                coroutine = StartCoroutine(Execute(action));
            else
                queue.Add(action.Priority, action);
        }
        
    }

    private IEnumerator Execute(Action action)
    {
        string actionInfo = Testing.GetActionInfo(action);
        Testing.WriteToLog(transform.name, "Action started: " + Testing.GetActionInfo(action));
        StartCoroutine(GetComponent<ActionExecutor>().ExecuteAction(action));
        yield return new WaitUntil(() => !GetComponent<ActionExecutor>().Executing);
        if(action.Status == Status.Successful)
        {
            Transform target = GameManager.FindGameObject(action.Target).transform;
            if(target.GetComponent<ReceivingQueue>() != null && target.tag == "Character")
                target.GetComponent<ReceivingQueue>().QueueAction(action);
            GetComponent<ReceivingQueue>().QueueAction(action);
        }
    }

}
