using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    static class EventPriorityQueue
    {
        static int indexOfLastNode = -1;
        static List<KeyValuePair<int, Action>> pq = new List<KeyValuePair<int, Action>>();

        public static void Add(int priority, Action action)
        {
            KeyValuePair<int, Action> item = new KeyValuePair<int, Action>(priority, action);
            indexOfLastNode += 1;
            pq.Add(item);
            pq = pq.OrderBy(i => i.Key).ToList();
        }

        public static void Print()
        {
            foreach(KeyValuePair<int, Action> item in pq)
            {
                Console.Write("("+item.Key+", " + item.Value.Name+")");
            }
            Console.WriteLine();
        }

        private static Action Remove()
        {
            Action min = pq[0].Value;
            pq[0] = pq[indexOfLastNode];
            pq.RemoveAt(indexOfLastNode);
            indexOfLastNode--;
            pq = pq.OrderBy(i => i.Key).ToList();
            return min;
        }

        public static void Execute()
        {
            ActionExecutor.ExecuteAction(Remove());
        }
    }
}
