using System;
using System.Collections.Generic;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

namespace InteractiveStorytellingSystem
{
    public class ActionExecutor : MonoBehaviour
    {
        private bool executing;
        private Action currentAction;

        void Start()
        {
            executing = false;
            currentAction = null;
        }

        public void ExecuteAction(Action action)
        {
            currentAction = action;
            executing = true;
            GameObject sender = GameObject.Find(currentAction.Sender);
            GameObject target = GameObject.Find(currentAction.Target);
            if(target != null)
            {             
                if(action.Name == "WalkToTarget")
                {
                    Debug.Log(currentAction.Sender + " executing action: " + currentAction.Name + " on target: " + currentAction.Target);
                    sender.GetComponent<MovementManager>().WalkToTarget(target.transform);
                }
                else if(action.Name == "FollowTarget")
                {
                    Debug.Log(currentAction.Sender + " executing action: " + currentAction.Name + " on target: " + currentAction.Target);
                    sender.GetComponent<MovementManager>().FollowTarget(target.transform);
                }
                else if(action.Name == "TalkToTarget")
                {
                    Debug.Log(currentAction.Sender + " executing action: " + currentAction.Name + " on target: " + currentAction.Target);
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
                }
            }
            else
            {
                CancelAction();
            }
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
            if(!executing && currentAction != null)
            {
                if(currentAction.Target != "Player")
			        GameManager.FindCharacter(currentAction.Target).SendAction(currentAction); //Send action to target for memory storage and response
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
            executing = false;
        }


    }
}
