using System.Collections;
using System.Collections.Generic;

public enum MemoryType
{
    combat,
    social
};

public class MemoryPattern
{
    public int ID {get; private set;}
    public string[] Keywords {get; private set;}
    public MemoryType Type {get; private set;}
    public float Weight {get; private set;}

    public MemoryPattern(int id, string[] keywords, MemoryType type, float weight)
    {
        ID = id;
        Keywords = keywords;
        Type = type;
        Weight = weight;
    }
}
