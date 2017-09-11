using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//using NUnit.Framework;
using UnityEngine;

public class MoveChecker
{
    //private string[][,] Moves = new string[1][,];
    private List<Move> Moves = new List<Move>();
    private string[] MoveNames;    
    

    //Constructa (man it's been a while since I've seen one of those in unity)
    public MoveChecker()
    {
        //MOVES GO HERE
        //. will match anything (ie empty space)
        //D will match any dancer
        //A will match main dancer(todo)


        //Crowd surf
        var cs = new List<string[]>();
        cs.Add(new string[]
        {
            ".D.",
            "DDD",
        });

        cs.Add(new string[]
        {
            "D..",
            "DDD",
            "D..",
        });

        cs.Add(new string[]
       {
            "..D",
            "DDD",
            "..D",
       });

        cs.Add(new string[]
      {
            "DDD",
            ".D.",
      });
        Moves.Add(new Move("Crowd Surf", Color.blue, cs));

        //Conga 3
        var c3 = new List<string[]>();
        c3.Add(new string[]
      {
            "D",
            "D",
            "D"
      });

        c3.Add(new string[]
    {
            "DDD",
    });
        Moves.Add(new Move("Conga Line Lv.3", Color.green, c3));


        //Conga 2
        var c2 = new List<string[]>();
        c2.Add(new string[]
      {
            "D",
            "D",
      });

        c2.Add(new string[]
    {
            "DD",
    });
        Moves.Add(new Move("Conga Line Lv.2", Color.green, c2));

    }

    /// <summary>
    /// Highlevel move checker
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public List<Move> CheckForMoves(Dictionary<Vector2, Dancer> board, int boardW, int boardH)
    {
        string[] Rows = ToStringArray(board, boardW, boardH); //Convert to string array
        List<Move> MovesFound = new List<Move>();

        //Loop through all moves to check them
        for (int i = 0; i < Moves.Count; i++)
        {
            //Iterate each pattern in the move
            for(int j = 0; j < Moves[i].Patterns.Count; j++)
            {
                //find ALL the moves in a row
                var MovesInRow = CheckMove(Rows, Moves[i].Patterns[j]);
                foreach(Vector2 vec in MovesInRow)
                {
                    //now make a copy of each move and set the origin where we found it
                    var returnMove = new Move(Moves[i]); 
                    returnMove.SetFound(vec,j);
                    MovesFound.Add(returnMove);
                }
            }
        }

        return MovesFound;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Rows"></param>
    /// <param name="Move"></param>
    /// <returns>Returns the X and Y of a found moveS</returns>
    private List<Vector2> CheckMove(string[] Rows, string[] Move)
    {
        int moveHeight = Move.Length;
        int moveWidth = Move[0].Length;
        int rowsRight = 0;
        Vector2 moveStart = Vector2.zero - Vector2.one; //Negative if not found

        var FoundMoves = new List<Vector2>();

        for (int i = 0; i < Rows.Length; i++) //loop rows
        {
            if (moveStart.x < 0) //If we don't have a lock on a move
            {
                //Find all potential starts in row with regex
                List<int> PotentialStartsRegex = new List<int>();

                //Match m = Regex.Match(reg, Move[rowsRight]);
                Regex regexObj = new Regex(Move[rowsRight]);
                Match matchObj = regexObj.Match(Rows[i]);
                while (matchObj.Success)
                {
                    var matchIndex = matchObj.Index; //Match overlaps
                    PotentialStartsRegex.Add(matchIndex);
                    matchObj = regexObj.Match(Rows[i], matchIndex + 1);
                    
                }              
              
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
                        
                        if (match && i + 1 < Rows.Length) //if match is in bounds
                        {
                            rowsRight++;
                            if (rowsRight == moveHeight) //We got a move!
                            {
                                FoundMoves.Add(moveStart);
                                rowsRight = 0;
                                moveStart = Vector2.zero - Vector2.one;
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
                }
            }
        }

        return FoundMoves;
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
