using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move execution and logic happens here
/// This class is so tightly coupled to Board it's like they're basically conjoined twins
/// </summary>
public class BustAMove
{

    private Board board;
    public BustAMove(Board b)
    {
        board = b;
    }

    //Moves here
    public void Conga(Vector2 pos, int range, int push, Vector2 cardinality)
    {
        //Get just the tips
        Vector2 top;
        top = (cardinality * (range - 1)) + pos;    //Assumes range = length of line

        Dancer dtop = null; //null if not found
        Dancer dBot = null;

        //find targets in range (only one since that's all we need for push to work)
        for (int i = 1; i < range + 1; i++)
        {
            if (!dtop) //top                
                dtop = board.GetDancer(top + (cardinality * i), !board.turn); //Look for enemies

            if (!dBot) //bot
                dBot = board.GetDancer(pos - (cardinality * i), !board.turn);
        }

        //Push
        for (int i = 0; i < push; i++)
        {
            if (dtop)
                board.Push(dtop, cardinality);
            if (dBot)
                board.Push(dBot, -cardinality);
        }
    }


    public void Boogaloo(Vector2 pos, int range, int push, string[] move, Vector2 cardinality)
    {
        //First we need to find the firing point
        //To do this we subtract the cardinality from the position of A
        //So first get the pos of A
        Vector2 APos = Vector2.zero;
        for (int i = 0; i < move.Length; i++)
            for (int j = 0; j < move[i].Length; j++)
            {
                if (move[i][j] == 'A')
                {
                    APos = new Vector2(j, i);
                }
            }

        var localFire = APos - cardinality; //now find where we fire from 
        Vector2 firingPoint = localFire + pos; //Convert to board space

        //Find the direction the move is facing since cardinality is the push direction
        //To do this, get the 2d cross product (perpendicular vector)
        Vector2 facing = new Vector2(cardinality.y, -cardinality.x);

        //This might be wrong depending on the move, but we can tell it's valid
        //by checking if a dancer is below the firing pos
        if (board.GetDancer(firingPoint + facing, board.turn))
            facing = -facing; //if invalid flip direction

        //Now find a dancer in range (Starting at the furtherest range and going in)
        Dancer d = null;
        for (int i = range; i > 0; i--)
        {
            Vector2 searchPos = firingPoint + (facing * i);
            d = board.GetDancer(searchPos, !board.turn);
            if (d) break; //if we found dancer
        }

        //Now pushy pushy
        if (d)
        {
            for (int i = 0; i < push; i++)
            {
                board.Push(d, cardinality);
            }
        }
    }
}
