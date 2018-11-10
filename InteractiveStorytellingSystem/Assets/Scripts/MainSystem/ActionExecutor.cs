using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    static class ActionExecutor
    {
        public static void ExecuteAction(Action action)
        {
            Console.Write(action.Sender + ": ");
            foreach(Dialog d in DialogManager.Dialog)
            {
                if (d.DialogID == action.DialogID)
                {
                    string speech = d.Value;
                    speech =  speech.Replace("%t", action.Target);
                    Console.WriteLine(speech);
                }

            }
        }
    }
}
