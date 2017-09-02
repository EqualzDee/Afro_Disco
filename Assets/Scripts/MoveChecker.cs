using System.Collections;
using System.Collections.Generic;
//using NUnit.Framework;
using UnityEngine;

public class MoveChecker
{
    //private string[][,] Moves = new string[1][,];
    private List<string[]> Moves = new List<string[]>();
    private string[] MoveNames;    
    

    //Constructa (man it's been a while since I've seen one of those in unity)
    public MoveChecker()
    {
        //Test crowd surf
        Moves.Add(new string[]
        {
            ".D.",
            "DDD",

        });

    }

    /// <summary>
    /// Highlevel move checker
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public List<Vector2> CheckForMoves(Dictionary<Vector2, Dancer> board, int boardW, int boardH)
    {
        string[] Rows = ToStringArray(board, boardW, boardH);
        List<Vector2> MovesFound = new List<Vector2>();

        for (int i = 0; i < Moves.Count; i++)
        {
            Vector2 returnVec = CheckMove(Rows, Moves[i]);
            if (!(returnVec.x < 0)) //we got a valid move!
            {
                MovesFound.Add(returnVec);
            }
        }
        return MovesFound;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Rows"></param>
    /// <param name="Move"></param>
    /// <returns>Returns the X and Y of a found move</returns>
    private Vector2 CheckMove(string[] Rows, string[] Move)
    {
        int moveHeight = Move.Length;        
        int rowsRight = 0;
        Vector2 moveStart = Vector2.zero - Vector2.one; //treat as negative value

        for (int i = 0; i < Rows.Length; i++) //loop rows
        {

            int moveStartX = Rows[i].IndexOf(Move[rowsRight]); //Check if move in row

            if (moveStartX != -1) //We found a row!
            {
                if (moveStart.x < 0) //if return is null
                {
                    moveStart = new Vector2(moveStartX, i); //stored in XY, not array format
                }

                rowsRight++;
                if (rowsRight == moveHeight) 
                {
                    //If rows found is the same as rows in move get outta here
                    break;
                }
            }
            else //if not found reset everything
            {
                rowsRight = 0;
                moveStart = Vector2.zero - Vector2.one;
            }
        }

        return moveStart;
    }

    private string[] ToStringArray(Dictionary<Vector2, Dancer> board, int boardW, int boardH)
    {
        //Make dancer array
        bool[,] dancers = new bool[boardH,boardW];
        foreach (KeyValuePair<Vector2, Dancer> d in board)
        {
            var pos = d.Key;
            dancers[(int)pos.y,(int)pos.x] = true;
        }

        //Initalize string array with dancers
        string[] stringBoard = new string[boardH];
        for (int i = 0; i < boardH; i++)
        {
            string row = "";
            for (int j = 0; j < boardW; j++)
            {
                if (dancers[i,j])
                    row += "D";
                else
                    row += ".";
            }
            stringBoard[i] = row;
        }
        return stringBoard;
    }
       
}
