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

        void Start()
        {
            Executing = false;
        }
        
        public IEnumerator ExecuteAction(Action action)
        {
            Executing = true;
            GameObject sender = GameObject.Find(action.Sender);
            GameObject target = GameObject.Find(action.Target);
            if(target != null)
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
                        Testing.PrintMessage(transform.name + " tried to walk to target: " + target.transform.name + " but that object doesn't exist in the scene.");
                    }
                    if(itemExists)
                    yield return new WaitUntil(() => GetComponent<MovementManager>().movementType == MovementManager.MovementType.Idle);
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
                        Testing.PrintMessage(transform.name + " tried to pick up item: " + action.Parameters + " but that item doesn't exist in the scene.");
                    }
                    if(itemExists)
                    {
                        yield return new WaitUntil(() => GetComponent<MovementManager>().movementType == MovementManager.MovementType.Idle);
                        GetComponent<Inventory>().Add(action.Parameters);
                        yield return new WaitForSeconds(1.0f);
                        CompleteAction(action);
                    }
                }
                else if(action.Type == "GiveItem")
                {
                    GetComponent<MovementManager>().WalkToTarget(target.transform);
                    yield return new WaitUntil(() => GetComponent<MovementManager>().movementType == MovementManager.MovementType.Idle);
                    TalkToTarget(sender, target, action, true);
                    yield return new WaitForSeconds(4);
                    GetComponent<Inventory>().Remove(action.Parameters);
                    CompleteAction(action);
                }
                else if(action.Type == "TalkToTarget")
                {
                    GetComponent<MovementManager>().WalkToTarget(target.transform);
                    yield return new WaitUntil(() => GetComponent<MovementManager>().movementType == MovementManager.MovementType.Idle);
                    TalkToTarget(sender, target, action, true);
                }
                else if(action.Type == "FleeFromSender")
                {
                    TalkToTarget(sender, target, action, false);
                    GetComponent<MovementManager>().FleeFromTarget(sender.transform);
                    yield return new WaitUntil(() => GetComponent<MovementManager>().movementType == MovementManager.MovementType.Idle);
                    CompleteAction(action);
                }
                else
                {
                    Debug.Log("Unknown action: " + Testing.GetActionInfo(action));    
                    CancelAction(action);
                }
            }
            else
            {
                Debug.Log("Target is null for action: " + Testing.GetActionInfo(action));
                CancelAction(action);
            }
        }

        private void TalkToTarget(GameObject sender, GameObject target, Action action, bool turn)
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
                    StartCoroutine(Speak(action, textMesh, speech));
                }
            }
        }

        IEnumerator Speak(Action action, TextMesh t, string dialog)
        {
            t.text = dialog;
            yield return new WaitForSeconds(4);
            t.text = "";
            CompleteAction(action);
        }

        private void CancelAction(Action action)
        {
            Debug.Log("Action Failed: " + Testing.GetActionInfo(action));
            action.Status = Status.Failed;
            StopExecuting();
        }

        private void CompleteAction(Action action)
        {
            Debug.Log("Action Successful: "+ Testing.GetActionInfo(action) + "\nSending confirmation to target for appraisal!");
            action.Status = Status.Successful;
            StopExecuting();
        }

        private void StopExecuting()
        {
            Executing = false;
        }


    }
}
