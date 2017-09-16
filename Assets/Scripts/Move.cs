using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A move that can be executed
/// Should be a barebones class that holds move info
/// </summary>
public class Move
{
    public Dictionary<Vector2, string[]> Patterns = new Dictionary<Vector2, string[]>();
    public Color Color { get; private set;}
    public string MoveName { get; private set; }


    //Instance variables
    public Vector2 origin { get; private set; }
    public Vector2 foundMoveCard { get; private set; }
    public int Priority { get; private set; }

    //Normal constructor
    public Move(string name, Color col)
    {
        //Patterns = new List<string[]>(paterns); //Copy pasta list
        Color = col;
        MoveName = name;
    }


    //Priority constructor
    public Move(string name, Color col, List<string[]> paterns, int priority)
    {
        //Patterns = new List<string[]>(paterns); //Copy pasta list
        Color = col;
        MoveName = name;
        Priority = priority;
    }

    //constructor for copying the object
    public Move(Move m)
    {
        //todo: fix
        //Patterns = new List<string[]>(m.Patterns);
        Color = new Color(m.Color.r, m.Color.g, m.Color.b);
        MoveName = string.Copy(m.MoveName);
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


    public void AddPattern(string[] pat, Vector2 cardinality)
    {
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
}
