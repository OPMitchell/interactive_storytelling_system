using System.Collections;
using System.Collections.Generic;

public class Connection
{
    public Node partner {get; private set;}
    public int Strength {get; private set;}

    public Connection(Node n)
    {
        partner = n;
        Strength = 0;
    }

    public void IncreaseStrength()
    {
        Strength++;
    }
}
