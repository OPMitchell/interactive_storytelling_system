using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MemoryPool
{
    private List<Node> nodes;
    private List<MemoryPattern> memories;
    private const int n = 3;
    private List<Node> hashmap;

    public MemoryPool()
    {
        nodes = new List<Node>();
        memories = new List<MemoryPattern>();
        hashmap = new List<Node>();
    }

    public void AddMemoryPattern(MemoryPattern mp)
    {
        memories.Add(mp);
        foreach(string keyword in mp.Keywords)
        {
            AddNode(keyword);
        }
        foreach (string keyword in mp.Keywords)
        {
            List<string> keywords = new List<string>(mp.Keywords);
            keywords.Remove(keyword);
            AddConnections(keyword, keywords);
        }
    }

    public MemoryPattern RetrieveMemoryPattern(string keyword)
    {
        hashmap.Clear();
        Node k = GetNodeByKeyword(keyword);
        if(k != null)
        {
            SpreadActivation(1, k, 0.5f);
            hashmap = hashmap.OrderByDescending(x => x.Activation).ToList();
            float highestActivation = hashmap[0].Activation;
            List<Node> topN = new List<Node>();
            int count = 1;
            foreach(Node node in hashmap)
            {
                if(node.Activation == highestActivation)
                    topN.Add(node);
                else if(node.Activation < highestActivation && count < n)
                {
                    count++;
                    highestActivation = node.Activation;
                    topN.Add(node);
                }
                else if (node.Activation < highestActivation && count >= n)
                    break;
            }
            return GetMatch(topN, keyword);
        }
        return null;
    }

    private MemoryPattern GetMatch(List<Node> topN, string keyword)
    {
        Dictionary<MemoryPattern, float> scores = new Dictionary<MemoryPattern, float>();
        foreach(MemoryPattern mp in memories)
        {
            if(!mp.Keywords.Contains(keyword))
                continue;
            float score = 0.0f;
            foreach(string keyword1 in mp.Keywords)
            {
                foreach(Node node in topN)
                {
                    if(keyword1 == node.Keyword)
                    {
                        score += node.Activation;
                    }
                }
            }
            scores.Add(mp, score);
        }

        var scoreList = scores.ToList();
        scoreList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
        if(scoreList.Count > 0)
            return scoreList[scoreList.Count-1].Key;
        return null;
    }

    private void SpreadActivation(int rank, Node n, float activationAmount)
    {
        if(rank == 1)
        {
            IncreaseActivation(rank, n, activationAmount);
            hashmap.Add(n);
        }
        rank++;
        foreach(Connection c in n.Connections)
        {
        if(!hashmap.Contains(c.partner))
        {
                IncreaseActivation(rank, c.partner, activationAmount/(float)n.Connections.Count);
                hashmap.Add(c.partner);
            }
        }
        foreach(Connection c in n.Connections)
        {
            if(rank == 2 || !hashmap.Contains(c.partner))
                SpreadActivation(rank, c.partner, activationAmount);
        }
    }

    private void IncreaseActivation(int rank, Node n, float activationAmount)
    {
        if(activationAmount < n.Activation)
            n.Activation = ((float)(rank*n.Activation) + (n.Activation+activationAmount))/(float)(rank+1.0f);  
    }

    private void AddNode(string keyword)
    {
        if(GetNodeByKeyword(keyword) == null)
        {
            nodes.Add(new Node(keyword));
        }
    }

    private void AddConnections(string keyword, List<string> keywords)
    {
        Node a = GetNodeByKeyword(keyword);
        if (a != null)
        {
            foreach(string k in keywords)
            {
                Node b = GetNodeByKeyword(k);
                if(b != null)
                {
                    Connection newConnection = new Connection(b);
                    Connection c = SearchForConnection(a, newConnection);
                    if(c == null)
                        a.Connections.Add(newConnection); 
                    else
                        c.IncreaseStrength();  
                }
            }
        }
    }

    private Node GetNodeByKeyword(string keyword)
    {
        foreach (Node n in nodes)
        {
            if(n.Keyword == keyword)
            {
                return n;
            }
        }
        return null;
    }

    private Connection SearchForConnection(Node n, Connection newConnection)
    {
        foreach (Connection c in n.Connections)
        {
            if(c.partner == newConnection.partner)
            {
                return c;
            }
         }
        return null;
    }

    public List<Node> GetNodes()
    {
        return nodes;
    }
}
