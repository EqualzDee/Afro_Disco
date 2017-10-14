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

	public void CrowdSurf(Vector2 pos, Vector2 firingPos, int push, string[] move, Vector2 cardinality)
    {
		Debug.Assert (push > 1); //can't have a push of 1

		var worldpos = new Vector2(firingPos.y,firingPos.x) + pos; //Get firing position the wimp way		
        //board.painter.AddToLayer(2, worldpos, Color.red);
        var d = board.GetDancer(worldpos);
        board.Move(d, worldpos + (cardinality * 2)); //Move out in front 

        //Push 
        for(int i = 0; i < push - 1; i++)
        {
            board.Push(d, cardinality);
        }
    }

    bool inArrayBounds(string[] arr, Vector2 pos)
    {
        return pos.x > 0 && pos.x < arr[0].Length &&
            pos.y > 0 && pos.y < arr.Length;
            
    }

    public void BootyCall(Vector2 pos, string[] move, Vector2 cardinality)
    {
        //Get lead
        Dancer lead = null;
        for(int i = 0; i < Board.BoardW; i++)
        {
            var dcheck = board.GetDancer(pos + (cardinality * i));
            if (dcheck && dcheck.IsLead)
            {
                lead = dcheck;
                //board.painter.AddToLayer(2, dcheck.GetBoardPos(), Color.red);
                break;
            }
        }

        //Find backups
        Dancer Above = null;
        Dancer Below = null;
        Vector2 leadPos = lead.GetBoardPos();
        int j = 1;
        while(Above == null || Below == null)
        {
            if(!Above)
                Above = board.GetDancer(leadPos + (cardinality * j)); 

            if (!Below)
                Below = board.GetDancer(leadPos - (cardinality * j));

            j++;            
        }

        //Movey backups
        board.Move(Above, leadPos + cardinality);
        board.Move(Below, leadPos - cardinality);
    }
}
