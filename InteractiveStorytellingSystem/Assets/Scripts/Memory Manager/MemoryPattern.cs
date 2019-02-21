using System.Collections;
using System.Collections.Generic;

public class MemoryPattern
{
    public int ID {get; private set;}
    public string[] Keywords {get; private set;}
    public string Type {get; private set;}
    public float Weight {get; private set;}
    public string Description {get; private set;}

    public MemoryPattern(int id, string[] keywords, string type, float weight, string description)
    {
        ID = id;
        Keywords = keywords;
        Type = type;
        Weight = weight;
        Description = description;
    }
}
