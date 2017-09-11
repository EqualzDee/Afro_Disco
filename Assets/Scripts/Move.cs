using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A move that can be executed
/// Should be a barebones class that holds move info
/// </summary>
public class Move
{
    public List<string[]> Patterns { get; private set; }
    public Color Color { get; private set;}
    public string MoveName { get; private set; }


    //Instance variables
    public Vector2 origin { get; private set; }
    private int foundMoveindex;
    public int Priority { get; private set; }

    //Normal constructor
    public Move(string name, Color col, List<string[]> paterns)
    {
        Patterns = new List<string[]>(paterns); //Copy pasta list
        Color = col;
        MoveName = name;
    }


    //Priority constructor
    public Move(string name, Color col, List<string[]> paterns, int priority)
    {
        Patterns = new List<string[]>(paterns); //Copy pasta list
        Color = col;
        MoveName = name;
        Priority = priority;
    }

    //constructor for copying the object
    public Move(Move m)
    {
        Patterns = new List<string[]>(m.Patterns);
        Color = new Color(m.Color.r, m.Color.g, m.Color.b);
        MoveName = string.Copy(m.MoveName);
    }

    /// <summary>
    /// Call to give this move an origin and specific pattern
    /// </summary>
    /// <param name="v"></param>
    /// <param name="index"></param>
    public void SetFound(Vector2 v, int index)
    {
        origin = v;
        foundMoveindex = index;
    }

    public string[] FoundMove()
    {
        return Patterns[foundMoveindex];
    }
}
