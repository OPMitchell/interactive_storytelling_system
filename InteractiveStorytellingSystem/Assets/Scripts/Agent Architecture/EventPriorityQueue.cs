using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveStorytellingSystem;
using InteractiveStorytellingSystem.ConfigReader;

public abstract class EventPriorityQueue : MonoBehaviour 
{
	protected PriorityQueue queue = new PriorityQueue();

    public void QueueAction(Action action)
    {
        queue.Add(action.Priority, action); //TODO: implement priority system!
    }

    void Update()
    {
        CheckQueue();
    }

    public bool ContainsActionType(string actionType)
    {
        return queue.ContainsActionType(actionType);
    }

    public abstract void CheckQueue();
}
