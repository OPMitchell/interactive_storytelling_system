using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace InteractiveStorytellingSystem
{
    static class ActionExecutor
    {
        public static void ExecuteAction(Action action)
        {
            Debug.Log(action.Sender + ": ");
            foreach(Dialog d in DialogManager.Dialog)
            {
                if (d.DialogID == action.DialogID)
                {
                    string speech = d.Value;
                    speech =  speech.Replace("%t", action.Target);
                    Debug.Log(speech);
                }

            }
        }
    }
}
