﻿using System;
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
                    GetComponent<MovementManager>().WalkToTarget(target.transform);
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
                    GetComponent<MovementManager>().WalkToTarget(GameManager.FindGameObject(action.Parameters).transform);
                    yield return new WaitUntil(() => GetComponent<MovementManager>().movementType == MovementManager.MovementType.Idle);
                    GetComponent<Inventory>().Add(action.Parameters);
                    yield return new WaitForSeconds(1.0f);
                    CompleteAction(action);
                }
                else if(action.Type == "GiveItem")
                {
                    GetComponent<MovementManager>().WalkToTarget(target.transform);
                    yield return new WaitUntil(() => GetComponent<MovementManager>().movementType == MovementManager.MovementType.Idle);
                    GetComponent<Inventory>().Remove(action.Parameters);
                    CompleteAction(action);
                }
                else if(action.Type == "TalkToTarget")
                {
                    TalkToTarget(sender, target, action);
                }
                else
                {
                    Debug.Log("Unknown action: " + Testing.GetActionInfo(action));    
                    CancelAction(action);
                }

                if(action.DialogID != "")
                {
                    TalkToTarget(sender, target, action);
                }
            }
            else
            {
                Debug.Log("Target is null for action: " + Testing.GetActionInfo(action));
                CancelAction(action);
            }
        }

        private void TalkToTarget(GameObject sender, GameObject target, Action action)
        {
            TextMesh textMesh = sender.transform.Find("DialogBox").GetComponent<TextMesh>();
            foreach(Dialog d in DialogManager.Dialog)
            {
                if (d.DialogID == action.DialogID)
                {
                    string speech = d.Value;
                    speech =  speech.Replace("%t", action.Target);
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
