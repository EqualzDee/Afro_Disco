using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A move that can be executed
/// Should be a barebones class that holds move info
/// </summary>
public class Move : IComparable<Move>
{
    public Dictionary<Vector2, string[]> Patterns = new Dictionary<Vector2, string[]>();
    public Color Color { get; private set;}
    public string MoveName { get; private set; }
    public int Range { get; private set; }
    public int PushPower { get; private set; }



    //Instance variables
    public Vector2 origin { get; private set; }
    public Vector2 foundMoveCard { get; private set; }
    public int Priority { get; private set; }

    //Normal constructor
    public Move(string name, Color col, int range, int pushPower)
    {        
        Color = col;
        MoveName = name;
        PushPower = pushPower;
        Range = range;
    }


    //Priority constructor
    public Move(string name, Color col, int priority)
    {        
        Color = col;
        MoveName = name;
        Priority = priority;
    }

    //constructor for copying the object
    public Move(Move m)
    {   
        Patterns = new Dictionary<Vector2, string[]>(m.Patterns);
        Color = new Color(m.Color.r, m.Color.g, m.Color.b);
        MoveName = string.Copy(m.MoveName);

        Priority = m.Priority;
        PushPower = m.PushPower;
        Range = m.Range;
    }

    /// <summary>
    /// Call to give this move an origin and specific pattern
    /// </summary>
    /// <param name="v"></param>
    /// <param name="index"></param>
    public void SetFound(Vector2 Origin, Vector2 cardinality)
    {
        origin = Origin;
        foundMoveCard = cardinality;
    }


    /// <summary>
    /// Add a pattern that defines the move
    /// </summary>
    /// <param name="pat">string array of pattern, see board for matching</param>
    /// <param name="cardinality">the direction the move fires, used for push logic</param>
    public void AddPattern(string[] pat, Vector2 cardinality)
    {
        if (Patterns.ContainsKey(cardinality))
        {
            Debug.Log(MoveName + "has overlapping keys!");
            return;
        }
        Patterns.Add(cardinality, pat);
    }

    /// <summary>
    /// Get the found move from this move instance, returns null if not found
    /// </summary>
    /// <returns></returns>
    public string[] GetFoundMove()
    {
        string[] a = null;
        Patterns.TryGetValue(foundMoveCard,out a);
        return a;
    }

    //Implement the comparable interface so we can sort easily in lists
    public int CompareTo(Move other)
    {
        var v1 = origin;
        var v2 = other.origin;

        if (v1.x < v2.x && v1.y < v2.y)
            return -1;
        if (v1.x == v2.x && v1.y == v2.y)
            return 0;
        if (v1.x > v2.x && v1.y > v2.y)
            return 1;

        return 0;
    }
}
