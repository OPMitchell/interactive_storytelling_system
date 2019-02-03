using System.Collections;
using System.Collections.Generic;

public class MemoryPool
{
    private List<Node> nodes;

    public MemoryPool()
    {
        nodes = new List<Node>();
    }

    public void AddMemoryPattern(MemoryPattern mp)
    {
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
