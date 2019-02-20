using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    public class PriorityQueue
    {
        private int indexOfLastNode = -1;
        private List<KeyValuePair<int, Action>> pq = new List<KeyValuePair<int, Action>>();

        public void Add(int priority, Action action)
        {
            KeyValuePair<int, Action> item = new KeyValuePair<int, Action>(priority, action);
            indexOfLastNode += 1;
            pq.Add(item);
            pq = pq.OrderBy(i => i.Key).ToList();
        }

        public Action Peek()
        {
            return pq[0].Value;
        }

        public Action Remove()
        {
            Action min = pq[0].Value;
            pq[0] = pq[indexOfLastNode];
            pq.RemoveAt(indexOfLastNode);
            indexOfLastNode--;
            pq = pq.OrderBy(i => i.Key).ToList();
            return min;
        }

        public List<KeyValuePair<int, Action>> GetQueue()
        {
            return pq;
        }

        public bool IsEmpty()
        {
            if(pq.Count == 0)
                return true;
            return false;
        }
    }
}
