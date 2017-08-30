using System.Collections;
using System.Collections.Generic;
//using NUnit.Framework;
using UnityEngine;

public class MoveChecker
{
    private static bool[][,] Shapes;

     ////Test1
     //   {
     //       {false, true},
     //       {true, true}
     //   },

     //   //Test2
     //   {
     //       {true, true},
     //       {true, false}
     //   },

     //   //Test3
     //   {
     //       {true, true},
     //       {false, true}
     //   },
     //     //Test3
     //   {
     //       {true, false},
     //       {true, true}
     //   },

    static List<int[,]> arrayList = new List<int[,]>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    

    public static bool CheckForMoves(Dictionary<Vector2, Dancer> board)
    {
        return true;
        //SHITTY HACK
        //Shapes[0] = new bool[,]
        //{
        //    {
        //        {false, true},
        //        {true, true}
        //    }
        //};

            //Find the highest values in the array
            //int maxX = 0;
            //int maxY = 0;
            //foreach (KeyValuePair<Vector2, Dancer> pos in board)
            //{
            //    if (pos.Key.x > maxX) maxX = (int) pos.Key.x;
            //    if (pos.Key.y > maxY) maxY = (int) pos.Key.y;
            //}

        //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

            //Create a board of max size
            bool[,] boolBoard = new bool[11, 7];

        //Put in new values
        foreach (KeyValuePair<Vector2, Dancer> pos in board)
        {
            var vec = pos.Key;
            boolBoard[(int)pos.Key.y, (int)pos.Key.x] = true;
        }

        //Test Shapes
        for (int i = 0; i < Shapes.Length; i++)
        {
            var a = Shapes[i];
            //if (doesShapeExistInWorld(, boolBoard))
            {
                
            }
        }

        return true;

    }


    //Kind of eh 
    //https://gamedev.stackexchange.com/questions/114142/how-can-i-find-a-shape-on-2d-array
    public static bool doesShapeExistInWorld(bool[,] shape, bool[,] world)
    {
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);
        int worldWidth = world.GetLength(0);
        int worldHeight = world.GetLength(1);
        if (shapeWidth <= worldWidth && shapeHeight <= worldHeight)
        {
            //Enumerate through each possible origin position for shape
            for (int startY = 0; startY < worldHeight - shapeHeight; startY++)
            {
                for (int startX = 0; startX < worldWidth - shapeWidth; startX++)
                {
                    //At each origin, test all points in shape against world
                    bool shapeExists = true;
                    for (int shapeY = 0; shapeY < shapeHeight; shapeY++)
                    {
                        for (int shapeX = 0; shapeX < shapeWidth; shapeX++)
                        {
                            shapeExists = !shape[shapeX, shapeY] ||
                                          (shape[shapeX, shapeY] && world[startX + shapeX, startY + shapeY]);
                            if (!shapeExists)
                            {
                                break;
                            }
                        }
                        if (!shapeExists)
                        {
                            break;
                        }
                    }
                    if (shapeExists)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
