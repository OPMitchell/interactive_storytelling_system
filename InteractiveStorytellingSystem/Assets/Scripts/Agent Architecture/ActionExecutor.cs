using System;
using System.Collections.Generic;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

namespace InteractiveStorytellingSystem
{
    public class ActionExecutor : MonoBehaviour
    {
        public bool Executing {get; private set;}
        public Action currentAction {get; private set;}

        void Start()
        {
            Executing = false;
        }
        
        public IEnumerator ExecuteAction(Action action)
        {
            Executing = true;
            currentAction = action;
            GameObject sender = GameObject.Find(action.Sender);
            GameObject target = GameObject.Find(action.Target);
            if(target != null && Executing)
            {       
                if(action.Type == "WalkToTarget")
                {
                    bool itemExists = false;
                    try
                    {
                        GetComponent<MovementManager>().WalkToTarget(target.transform);
                        itemExists = true;
                    }
                    catch(NullReferenceException ex)
                    {
                        CancelAction(action);
                        Testing.WriteToLog(transform.name, "Error whilst executing action: " + Testing.GetActionInfo(action));
                        Testing.WriteToLog(transform.name, "    Tried to walk to target: " + target.transform.name + " but that object doesn't exist in the scene.");
                    }
                    if(itemExists)
                    yield return new WaitUntil(() => Vector3.Distance(transform.position, target.transform.position) <= 2.0f);
                    CompleteAction(action);
                }
                else if(action.Type == "FollowTarget")
                {
                    CancelAction(action);
                }
                else if(action.Type == "IncreaseStat")
                {
                    CompleteAction(action);
                }
                else if(action.Type == "DecreaseStat")
                {
                    CompleteAction(action);
                }
                else if(action.Type == "PickUpItem")
                {
                    bool itemExists = false;
                    try
                    {
                        GetComponent<MovementManager>().WalkToTarget(GameManager.FindGameObject(action.Parameters).transform);
                        itemExists = true;
                    }
                    catch(NullReferenceException ex)
                    {
                        CancelAction(action);
                        Testing.WriteToLog(transform.name, "Error whilst executing action: " + Testing.GetActionInfo(action));
                        Testing.WriteToLog(transform.name, "    Tried to pick up item: " + action.Parameters + " but that item doesn't exist in the scene.");
                    }
                    if(itemExists)
                    {
                        yield return new WaitUntil(() => Vector3.Distance(transform.position, GameManager.FindGameObject(action.Parameters).transform.position) <= 2.0f);
                        GetComponent<Inventory>().Add(action.Parameters);
                        yield return new WaitForSeconds(1.0f);
                        CompleteAction(action);
                    }
                }
                else if(action.Type == "GiveItem")
                {
                    if(GetComponent<Inventory>().Contains(action.Parameters))
                    {
                        GetComponent<MovementManager>().WalkToTarget(target.transform);
                        yield return new WaitUntil(() => Vector3.Distance(transform.position, target.transform.position) <= 2.0f);
                        GetComponent<Inventory>().Remove(action.Parameters);
                        StartCoroutine(TalkToTarget(sender, target, action, true));
                        yield return new WaitForSeconds(4);
                        CompleteAction(action);
                    }
                    else
                        CancelAction(action);
                }
                else if(action.Type == "TalkToTarget")
                {
                    GetComponent<MovementManager>().WalkToTarget(target.transform);
                    yield return new WaitUntil(() => Vector3.Distance(transform.position, target.transform.position) <= 2.0f);
                    StartCoroutine(TalkToTarget(sender, target, action, true));
                    CompleteAction(action);
                }
                else if(action.Type == "FleeFromSender")
                {
                    StartCoroutine(TalkToTarget(sender, target, action, false));
                    GetComponent<MovementManager>().FleeFromTarget(sender.transform);
                    yield return new WaitUntil(() => Vector3.Distance(transform.position, sender.transform.position) > 10.0f);
                    CompleteAction(action);
                }
                else
                {
                    Testing.WriteToLog(transform.name, "Error whilst executing action: " + Testing.GetActionInfo(action));
                    Testing.WriteToLog(transform.name, "    The action was unknown.");  
                    CancelAction(action);
                }
            }
            else
            {
                Testing.WriteToLog(transform.name, "Error whilst executing action: " + Testing.GetActionInfo(action));
                Testing.WriteToLog(transform.name, "    target is null.");
                CancelAction(action);
            }
        }

        private IEnumerator TalkToTarget(GameObject sender, GameObject target, Action action, bool turn)
        {
            TextMesh textMesh = transform.Find("DialogBox").GetComponent<TextMesh>();
            foreach(Dialog d in DialogManager.Dialog)
            {
                if (d.DialogID == action.DialogID)
                {
                    string speech = d.Value;
                    speech =  speech.Replace("%t", action.Target);
                    if(turn)
                        GetComponent<MovementManager>().TurnToTarget(target.transform);
                    textMesh.text = speech;
                    yield return new WaitForSeconds(4);
                    textMesh.text = "";
                }
            }
        }

        private void CancelAction(Action action)
        {
            if(Executing)
            {
                Testing.WriteToLog(transform.name, "Action failed: " + Testing.GetActionInfo(action));
                action.Status = Status.Failed;
                StopExecuting();
            }
        }

        private void CompleteAction(Action action)
        {
            if(Executing)
            {
                Testing.WriteToLog(transform.name, "Action successful: " + Testing.GetActionInfo(action));
                action.Status = Status.Successful;
                StopExecuting();
            }
        }

        public void InterruptAction()
        {
            if(Executing)
            {
                Testing.WriteToLog(transform.name, "Action interrupted: " + Testing.GetActionInfo(currentAction));
                currentAction.Status = Status.Interrupted;
                StopExecuting();
            }
        }

        private void StopExecuting()
        {
            Executing = false;
            currentAction = null;
        }


    }
}
