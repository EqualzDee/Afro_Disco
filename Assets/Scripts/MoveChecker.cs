using System;
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
        //Crowd surf
        //Rot 1 permutations
        Moves.Add(new string[]
        {
            ".D.",
            "DDD",
        });
        
        Moves.Add(new string[]
        {
            "DD.",
            "DDD",
        });

        Moves.Add(new string[]
        {
            "DDD",
            "DDD",
        });

        Moves.Add(new string[]
        {
            ".DD",
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
        string[] Rows = ToStringArray(board, boardW, boardH); //Convert to string array
        List<Vector2> MovesFound = new List<Vector2>();
        for (int i = 0; i < Moves.Count; i++)
        {
            Vector2 returnVec = CheckMove(Rows, Moves[i]);    //Loop through all moves to check them
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
        int moveWidth = Move[0].Length;
        int rowsRight = 0;
        Vector2 moveStart = Vector2.zero - Vector2.one; //treat as negative value

        bool moveFound = false;

        for (int i = 0; i < Rows.Length; i++) //loop rows
        {
            if (moveStart.x < 0) //If we don't have a lock on a move
            {
                //find all moveStarts in current row
                List<int> PotentialStarts = new List<int>();
                int counter = 0;
                int offset = 0;
                while (true)
                {
                    int startX = Rows[i].IndexOf(Move[rowsRight], offset);
                    if (startX != -1)
                    {
                        PotentialStarts.Add(startX); //Check if start of move is in row
                        offset = startX + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                //Now check out those potential starts to see if they're the move
                foreach (int start in PotentialStarts)
                {
                    moveStart = new Vector2(start, i); //stored in XY, not array format

                    while (true) //WE'VE GOT A LOCK
                    {
                        //Now only check for move in a substring
                        //Also iterate up the list
                        //Also this wastes one loop cycle here but eh
                        var substring = Rows[i + rowsRight].Substring((int) moveStart.x, moveWidth);
                        if (substring.Contains(Move[rowsRight]))
                        {
                            rowsRight++;
                            if (rowsRight == moveHeight) //we got the move!
                            {
                                moveFound = true;
                                break;
                            }
                        }
                        else //Lock failed! No move here
                        {
                            rowsRight = 0;
                            moveStart = Vector2.zero - Vector2.one;
                            break;
                        }
                    }
                    if (moveFound) break;
                }
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
