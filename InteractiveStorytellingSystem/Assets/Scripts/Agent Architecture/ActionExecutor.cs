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
        private Action currentAction;

        void Start()
        {
            Executing = false;
            currentAction = null;
        }

        public bool ExecuteAction(Action action)
        {
            if(action.Target == "%s")
                action.Target = transform.name;
            currentAction = action;
            Executing = true;
            GameObject sender = GameObject.Find(currentAction.Sender);
            GameObject target = GameObject.Find(currentAction.Target);
            if(target != null)
            {             
                if(action.Type == "WalkToTarget")
                {
                    sender.GetComponent<MovementManager>().WalkToTarget(target.transform);
                }
                else if(action.Type == "FollowTarget")
                {
                    sender.GetComponent<MovementManager>().FollowTarget(target.transform);
                }
                else if(action.Type == "IncreaseStat")
                {
                    Executing = false;
                }
                else if(action.Type == "DecreaseStat")
                {
                    Executing = false;
                }
                else if(action.Type == "TalkToTarget")
                {
                    TextMesh textMesh = sender.transform.Find("DialogBox").GetComponent<TextMesh>();
                    foreach(Dialog d in DialogManager.Dialog)
                    {
                        if (d.DialogID == currentAction.DialogID)
                        {
                            string speech = d.Value;
                            speech =  speech.Replace("%t", currentAction.Target);
                            GetComponent<MovementManager>().TurnToTarget(target.transform);
                            StartCoroutine(Speak(textMesh, speech));
                            break;
                        }
                    }
                }
                else
                {
                    CancelAction();
                    return false;
                }
            }
            else
            {
                CancelAction();
                return false;
            }
            return true;
        }

        IEnumerator Speak(TextMesh t, string dialog)
        {
            t.text = dialog;
            yield return new WaitForSeconds(4);
            t.text = "";
            StopExecuting();
        }

        void Update()
        {
            if(!Executing && currentAction != null)
            {
                currentAction = null;
            }
        }

        private void CancelAction()
        {
            Debug.Log("Cancelled action: (name: " + currentAction.Name + ", sender: " + currentAction.Sender + ", target: " + currentAction.Target + ")");
            currentAction = null;
            StopExecuting();
        }

        public void StopExecuting()
        {
            Executing = false;
        }


    }
}
