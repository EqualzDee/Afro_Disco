using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


/// <summary>
/// Contains methods for 2d pattern searching
/// Also sets up available moves in constructor
/// todo make singleton
/// </summary>
public class MoveChecker
{
    private readonly List<Move> Moves = new List<Move>();


    Dictionary<string, string> RegReplace = new Dictionary<string, string>()
    {
        {"D","\\w"},
        {"-","\\.*"},
        {"_", "\\."}
    };


    public MoveChecker()
    {
        //MOVES GO HERE
        //. will match anything (ie empty space)    
        //_ will match only a blank space    
        //- will match any number of blank spaces
        //D will match any dancer (ie any letter)
        //A will match main dancer

        //Crowd surf
        var cs = new Move("Crowd Surf", Color.blue, 0,3);
        cs.AddPattern(new string[]
        {
            ".D.",
            "DDD",
        }
        ,new Vector2(0, -1)
        );

        cs.AddPattern(new string[]
        {
            "..D",
            "DDD",
            "..D",
        }
        , new Vector2(1,0)
        );

        cs.AddPattern(new string[]
       {
             "DDD",
             ".D.",
       }
       , new Vector2(0, 1)
       );

        cs.AddPattern(new string[]
        {
            "D..",
            "DDD",
            "D..",
        }
        , new Vector2(-1, 0)
        );

        Moves.Add(cs);


        //Conga 3
        var c3 = new Move("Conga Line Lv.3", Color.green * 0.5f, 3, 2,5);
        c3.AddPattern(new string[]
        {
            "DDD"
        }
        , new Vector2(1, 0)
        );

        c3.AddPattern(new string[]
        {
            "D",
            "D",
            "D"
        }
        , new Vector2(0, 1)
        );
        Moves.Add(c3);


        //Conga 2
        var c2 = new Move("Conga Line Lv.2", Color.green,2,2);
        c2.AddPattern(new string[]
        {
            "DD"
        }
        , new Vector2(1, 0)
        );

        c2.AddPattern(new string[]
        {
            "D",
            "D",
        }
        , new Vector2(0, 1)
        );
        Moves.Add(c2);


        //Boogaloo 1 (Type A)
        var b1A = new Move("Boogaloo Lv.1 A", new Color(0.29f, 0, 0.66f),1,3,8);
        b1A.AddPattern(new string[]
            {
                "A.D",
                ".D.",
            }
            , new Vector2(-1, 0)
        );

        b1A.AddPattern(new string[]
            {
                ".D.",
                "D.A",
            }
            , new Vector2(1, 0)
        );

        b1A.AddPattern(new string[]
            {
                ".A",
                "D.",
                ".D"
            }
            , new Vector2(0, -1)
        );

        b1A.AddPattern(new string[]
          {
                "D.",
                ".D",
                "A."
          }
          , new Vector2(0, 1)
      );
        Moves.Add(b1A);

        //Boogaloo 1 (Type B)
        var b1B = new Move("Boogaloo Lv.1 B", new Color(0.29f, 0, 0.66f),1,3,8);
        b1B.AddPattern(new string[]
            {
                "D.A",
                ".D.",
            }
            , new Vector2(1, 0)
        );

        b1B.AddPattern(new string[]
            {
                ".D.",
                "A.D",
            }
            , new Vector2(-1, 0)
        );

        b1B.AddPattern(new string[]
            {
                ".D",
                "D.",
                ".A"
            }
            , new Vector2(0, 1)
        );

        b1B.AddPattern(new string[]
          {
                "A.",
                ".D",
                "D."
          }
          , new Vector2(0, -1)
      );
        Moves.Add(b1B);


        //Boogaloo 2 (Type A)
        var b2A = new Move("Boogaloo Lv.1 A", new Color(0.29f, 0, 0.66f), 2, 3,9);
        b2A.AddPattern(new string[]
            {
                "A.D",
                ".D.",
                ".D."
            }
            , new Vector2(-1, 0)
        );

        b2A.AddPattern(new string[]
            {
                ".D.",
                ".D.",
                "D.A",
            }
            , new Vector2(1, 0)
        );

        b2A.AddPattern(new string[]
            {
                "..A",
                "DD.",
                "..D"
            }
            , new Vector2(0, -1)
        );

        b2A.AddPattern(new string[]
          {
                "D..",
                ".DD",
                "A.."
          }
          , new Vector2(0, 1)
      );
        Moves.Add(b2A);

        //Boogaloo 2 (Type B)
        var b2B = new Move("Boogaloo Lv.1 B", new Color(0.29f, 0, 0.66f), 2, 3,9);
        b2B.AddPattern(new string[]
            {
                "D.A",
                ".D.",
                ".D."
            }
            , new Vector2(1, 0)
        );

        b2B.AddPattern(new string[]
            {
                ".D.",
                ".D.",
                "A.D",
            }
            , new Vector2(-1, 0)
        );

        b2B.AddPattern(new string[]
            {
                "..D",
                "DD.",
                "..A"
            }
            , new Vector2(0, 1)
        );

        b2B.AddPattern(new string[]
          {
                "A..",
                ".DD",
                "D.."
          }
          , new Vector2(0, -1)
      );
        Moves.Add(b2B);

        //Boogaloo 3 (Type A)
        var b3A = new Move("Boogaloo Lv.1 A", new Color(0.29f, 0, 0.66f), 3, 3, 10);
        b3A.AddPattern(new string[]
            {
                "A.D",
                ".D.",
                ".D.",
                ".D."
            }
            , new Vector2(-1, 0)
        );

        b3A.AddPattern(new string[]
            {
                ".D.",
                ".D.",
                ".D.",
                "D.A",
            }
            , new Vector2(1, 0)
        );

        b3A.AddPattern(new string[]
            {
                "...A",
                "DDD.",
                "...D"
            }
            , new Vector2(0, -1)
        );

        b3A.AddPattern(new string[]
          {
                "D...",
                ".DDD",
                "A..."
          }
          , new Vector2(0, 1)
      );
        Moves.Add(b3A);

        //Boogaloo 3 (Type B)
        var b3B = new Move("Boogaloo Lv.1 B", new Color(0.29f, 0, 0.66f), 3, 3, 9);
        b3B.AddPattern(new string[]
            {
                "D.A",
                ".D.",
                ".D.",
                ".D."
            }
            , new Vector2(1, 0)
        );

        b3B.AddPattern(new string[]
            {
                ".D.",
                ".D.",
                ".D.",
                "A.D",
            }
            , new Vector2(-1, 0)
        );

        b3B.AddPattern(new string[]
            {
                "...D",
                "DDD.",
                "...A"
            }
            , new Vector2(0, 1)
        );

        b3B.AddPattern(new string[]
          {
                "A...",
                ".DDD",
                "D..."
          }
          , new Vector2(0, -1)
      );
        Moves.Add(b3B);


        //Booty Call
        var booty = new Move("Booty Call", new Color(0.95f, 0.26f, 0.211f), 0, 0, 1);
        booty.AddPattern(new string[]
            {
                "D-A-D"
            }
            , new Vector2(1, 0)
        );

        booty.AddPattern(new string[]
           {
                "D",
                "-",
                "A",
                "-",
                "D",
           }
           , new Vector2(0, 1)
       );
        Moves.Add(booty);
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
            foreach(KeyValuePair<Vector2,string[]> pat in Moves[i].Patterns)
            {
                //find ALL the moves in a row
                var MovesInRow = CheckMove(Rows, pat.Value);

                foreach (Vector2 vec in MovesInRow)
                {
                    //now make a copy of each move and set the origin where we found it
                    var found = new Move(Moves[i]);
                    found.SetFound(vec, pat.Key);
                    MovesFound.Add(found);
                }
            }
        }

        //Move priority filter
        //Iterate in reverse to avoid out of range when removing
        //if (MovesFound.Count != 0)
        //{
        //    for (int i = MovesFound.Count - 1; i >= 0; i--)
        //    {
        //        for (int j = MovesFound.Count - 1; j >= 0; j--)
        //        {
        //            var m1 = MovesFound[i];
        //            var m2 = MovesFound[j];
        //            if (m1.origin == m2.origin)
        //            {
        //                if (m1.Priority > m2.Priority)
        //                    MovesFound.RemoveAt(i);
        //                else if (m1.Priority < m2.Priority)
        //                    MovesFound.RemoveAt(i);
        //            }
        //        }
        //    }
        //}
        //tHiS wOn'T dO

        //judo-code
        //double looparino moves found
        //find moves of the same type         
        //iterate the pattern like in glow tiles
        //delete any moves of lower priority
        //????
        //profit

        return MovesFound;
    }

    /// <summary>
    /// WHERE THE FUCKING MAGIC HAPPENS
    /// </summary>
    /// <param name="Rows"></param>
    /// <param name="Move"></param>
    /// <returns>Returns the X and Y of a found moveS</returns>
    private List<Vector2> CheckMove(string[] Rows, string[] Move)
    {
        //Move restriction here won't work for n blank spaces :/
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

                //string reg = Move[rowsRight].Replace("D", "\\w"); //replace D with match any word character
                string reg = replaceRegex(Move[rowsRight], RegReplace);
                Regex regexObj = new Regex(reg);
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
                        //string reg2 = Move[rowsRight].Replace("D", "\\w");
                        string reg2 = replaceRegex(Move[rowsRight], RegReplace);
                        Match m2 = Regex.Match(substring, reg2);
                        var match = m2.Success; 
                        
                        if (match && rowsRight + i + 1 < Rows.Length) //if match is in bounds
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
        //Initalize string array with dancers
        string[] stringBoard = new string[boardH];
        for (int i = 0; i < boardH; i++)
        {
            string row = "";
            for (int j = 0; j < boardW; j++)
            {
                Dancer d = null;
                board.TryGetValue(new Vector2(j, i), out d); //get dancer at pos

                if(!d)
                    row += ".";
                else if (d.IsLead)
                    row += "A";
                else if (d)
                    row += "D";
                
            }
            stringBoard[i] = row;
        }
        return stringBoard;
    }

    private string replaceRegex(string s, Dictionary<string,string> d)
    {
        string returnString = s;
        foreach (KeyValuePair<string, string> entry in d)
        {
            returnString =  returnString.Replace(entry.Key, entry.Value);
        }
        return returnString;
    }
}
