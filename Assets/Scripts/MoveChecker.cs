using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        //MOVES GO HERE
        //. will match anything (ie empty space)
        //D will match any dancer
        //A will match main dancer(todo)

        //Crowd surf
        Moves.Add(new string[]
        {
            ".D.",
            "DDD",
        });

        Moves.Add(new string[]
        {
            "D..",
            "DDD",
            "D..",
        });

        Moves.Add(new string[]
       {
            "..D",
            "DDD",
            "..D",
       });

        Moves.Add(new string[]
      {
            "DDD",
            ".D.",
      });

        //Conga 2
        Moves.Add(new string[]
      {
            "D",
            "D",
      });

        Moves.Add(new string[]
    {
            "DD",
    });

        //Conga 3
        Moves.Add(new string[]
      {
            "D",
            "D",
            "D"
      });

        Moves.Add(new string[]
    {
            "DDD",
    });
    }

    /// <summary>
    /// Highlevel move checker
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public Dictionary<Vector2, string[]> CheckForMoves(Dictionary<Vector2, Dancer> board, int boardW, int boardH)
    {
        string[] Rows = ToStringArray(board, boardW, boardH); //Convert to string array
        Dictionary<Vector2, string[]> MovesFound = new Dictionary<Vector2, string[]>();

        //Loop through all moves to check them
        for (int i = 0; i < Moves.Count; i++)
        {
            Vector2 returnVec = CheckMove(Rows, Moves[i]);    
            if (!(returnVec.x < 0)) //we got a valid move!
            {
                if(!MovesFound.ContainsKey(returnVec)) //possibly missing moves
                    MovesFound.Add(returnVec,Moves[i]);
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
        Vector2 moveStart = Vector2.zero - Vector2.one; //Negative if not found

        bool moveFound = false;

        for (int i = 0; i < Rows.Length; i++) //loop rows
        {
            if (moveStart.x < 0) //If we don't have a lock on a move
            {
                //find all moveStarts in current row
                List<int> PotentialStarts = new List<int>();

                //Find all potential starts in row with regex
                List<int> PotentialStartsRegex = new List<int>();
                //string reg = ConvertMoveToRegex(Rows[i]);
                string reg = Rows[i];
                Match m = Regex.Match(reg, Move[rowsRight]);
                while (m.Success)
                {                    
                    PotentialStartsRegex.Add(m.Index);
                    m = m.NextMatch();
                }

                //Find all potential starts in a row, old method
                //Actually this won't work because it's looking for an exact match
                //int offset = 0;
                //while (true)
                //{
                //    int startX = Rows[i].IndexOf(Move[rowsRight], offset);
                //    if (startX != -1)
                //    {
                //        PotentialStarts.Add(startX); //Check if start of move is in row
                //        offset = startX + 1;
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}

                //debug - compare loops
                //Debug.Assert(PotentialStarts.Count == PotentialStartsRegex.Count);

                //Now check out those potential starts to see if they're the move
                foreach (int start in PotentialStartsRegex)
                {
                    moveStart = new Vector2(start, i); //stored in XY, not array format

                    while (true) //WE'VE GOT A LOCK
                    {
                        //Now only check for move in a substring
                        //Also iterate up the list
                        //Also this wastes one loop cycle here but eh
                        var substring = Rows[i + rowsRight].Substring((int) moveStart.x, moveWidth);
                        Match m2 = Regex.Match(substring, Move[rowsRight]);
                        var match = m2.Success; 
                        //var match = substring.Contains(Move[rowsRight]); //old match
                        if (match)
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

    private string ConvertMoveToRegex(string s)
    {
        string SReturn = s;
        int lineCount = 0;
        while (lineCount < s.Length)
        {
            int index = s.IndexOf(".", lineCount);
            if (index != -1)
            {
                SReturn = SReturn.Insert(index, "\\");
                lineCount = index;
            }
        }
        return SReturn;
    }
       
}
