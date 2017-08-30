using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject TilePrefab;    
    public Camera cam;
    public GameObject DancerPrefab;

	//Test Color variables
	public Color tileColor1;
	public Color tileColor2;
	public Color selectedColor;
	public float emissiveStrength;

    //The Y position the tile is spawned at so that it doesn't touch the dancers
    public float YDanceOffset = -0.117f;

    public int DancerRange;

    public LayerMask moverLayer;

    public const int BoardW = 7;
    public const int BoardH = 11;
    private int _tileCounter;
    private bool _oddEven;

    //public Material[] ROYGBIV;

    //TILES SHOULD BE THE ONLY PLACE WHERE X AND Y IS REVERSED
    private GameObject[,] Tiles = new GameObject[BoardH, BoardW];

    //False is player 1 turn
    //True is player 2 turn
    public bool turn = false;
    Player Player1 = new Player();
    Player Player2 = new Player();

    private int RecurseDepth = 0;

    Dictionary<Vector2, Dancer> _Dancers = new Dictionary<Vector2, Dancer>(); //Big papa

    private Dancer _dancerSelected;
    Dictionary<Vector2, GameObject> _validPositions = new Dictionary<Vector2,GameObject>(); //Grid postions

    void Start ()
    {
        //Make board
        for (int i = 0; i < BoardW; i++)
        {
            for (int j = 0; j < BoardH; j++)
            {
                //Mek da tile
                var t = Instantiate(TilePrefab, new Vector3(i, YDanceOffset, j), Quaternion.identity, transform);
                Tiles[j,i] = t;
                t.layer = 9;

				//Controls the color of both tiles + the emissive value
				var material = t.GetComponent<MeshRenderer>().material;	//Don't worry about this line and the other commented bits, I'm working to make them work properly
				if(_tileCounter % 2 == 1) {
					//t.GetComponent<MeshRenderer>().material.color = tileColor1;
					material.SetColor("_EmissionColor", tileColor1 * emissiveStrength);
					material.EnableKeyword("_EMISSION");
				}

				if(_tileCounter % 2 == 0) {
					//t.GetComponent<MeshRenderer>().material.color = tileColor2;
					material.SetColor("_EmissionColor", tileColor2 * emissiveStrength);
					material.EnableKeyword("_EMISSION");
				}
				t.name = "Tile " + _tileCounter + " (" + i + "x" + j + ")";
                _tileCounter++;
            }
        }

        //Player 1 Dancers
        GenerateDancers(1,Player1);
        //Player 2 dancers
        GenerateDancers(9,Player2);

        //Let player 1 move
        EnableMove(Player1, true);


        //like share subscribe
        //AudioMan.OnBeat += ChangeFloorCol;
    }

    void GenerateDancers(int yOffset, Player p)
    {
        for (int i = 0; i < 5; i++)
        {
            var v = new Vector2(1 + i, yOffset);
            var dancerObj = Instantiate(DancerPrefab, new Vector3(v.x, 0, v.y), Quaternion.identity);
            var dancer = dancerObj.GetComponent<Dancer>();

            dancer.Player = p;
            _Dancers.Add(v, dancer);
            dancer.Initialize(v);
            dancerObj.name = "Dancer " + i;

            //PLS REMOVE DIS
            dancerObj.GetComponentInChildren<MeshRenderer>().material.color = p == Player1 ? Color.red : Color.magenta;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Select Dancer with mouse via ray
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (!_dancerSelected) //no dancer selected
            {
                if (Physics.Raycast(ray, out hit)) //8 is dancer layer
                {
                    _dancerSelected = hit.transform.GetComponentInParent<Dancer>();

                    if (!_dancerSelected || !_dancerSelected.canMove) return;
                    
                    _dancerSelected.Select();
                    ShowValidTiles(_dancerSelected);
                }
            }
            else //Dancer is selected
            {
                //Move that booty
                if (Physics.Raycast(ray, out hit, 999, moverLayer))
                {
                    Vector2 hitBoardPos = new Vector2(hit.transform.position.x, hit.transform.position.z);
                    //if (_validPositions.ContainsKey(hitBoardPos))
                    var pushVec = hitBoardPos - GetDancerPos(_dancerSelected);
                        if (pushVec != Vector2.zero && pushVec.magnitude <= 1)
                        Push(_dancerSelected, hitBoardPos - GetDancerPos(_dancerSelected));
                        //Push(_dancerSelected, hitBoardPos);
                    //Move(_dancerSelected, hitBoardPos);

                }
            }
        }
        else if(_dancerSelected) //Deselect
        {
            Debug.Log(MoveChecker.CheckForMoves(_Dancers));

            _dancerSelected.DeSelect();
            ResetTileCol();
            _dancerSelected = null;
            _validPositions.Clear();


        }
    }

    /// <summary>
    /// Outlines the tiles where the player can go
    /// </summary>
    void ShowValidTiles(Dancer d)
    {
        var dX = (int)d.StartRoundPos.x;
        var dY = (int)d.StartRoundPos.y;

        //Loop through all directions to check
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int range = DancerRange;
                var VecCheck = new Vector2(j, i);

                //Decrease range for diags
                if (!(VecCheck.x == 0 || VecCheck.y == 0)) range -= 1;
                
                CastValidTile(dX, dY, VecCheck, range, d);
            }
        }
    }

    void ResetTileCol()
    {
        for (int i = 0; i < BoardW; i++)
        {
            for (int j = 0; j < BoardH; j++)
            {
                Tiles[j, i].GetComponent<MeshRenderer>().material.color = Color.white;

                //Tiles[j, i].GetComponent<MeshRenderer>().material.color = (i % 2) != (j%2)
                //    ? tileColor1
                //    : tileColor2;
            }
        }
    }

    /// <summary>
    /// Steps in the direction of the vector given to see if the tile is free
    /// </summary>
    /// <param name="dX">dancer x</param>
    /// <param name="dY">dancer y</param>
    /// <param name="v">vector to check</param>
    /// <param name="length">distance to check</param>
    /// <param name="d">dancer</param>
    void CastValidTile(int dX, int dY, Vector2 v, int length, Dancer d)
    {        
        for (int i = 0; i < length + 1; i++)
        {
            Vector2 check = new Vector2(dX, dY) + (v*i);            
            if (IsValidPos(check, d))
            {
                //Change tile col
                var t = Tiles[(int) check.y, (int) check.x]; //The valid tile
                t.GetComponent<MeshRenderer>().material.color =  i == 0 ? Color.blue : selectedColor;

                //Add to dict if not already
                if(!_validPositions.ContainsKey(check))
                    _validPositions.Add(check,t);
            }
            else if(i > 0) //if newpos is not start position
            {
                return;
            }
        }
    }

    /// <summary>
    /// Checks for a valid board position 
    /// </summary>
    /// <param name="v">newpos</param>
    /// <param name="d">the dancer asking</param>
    /// <returns></returns>
    bool IsValidPos(Vector2 v, Dancer d)
    {
        if (InBounds(v))
        {
            //Doesn't contain a dancer at that position, or the dancer is me
            var key = new Vector2(v.x, v.y);
            Dancer d2;
            _Dancers.TryGetValue(key, out d2);

            return !_Dancers.ContainsKey(key) || d == d2; 
        }        
        return false;
    }

    bool InBounds(Vector2 v)
    {
        return v.x >= 0 &&
               v.y >= 0 &&
               v.x < BoardW &&
               v.y < BoardH;
    }

    /// <summary>
    /// Moves a dancer to a postion. Will knock the dancer out if moved off the floor
    /// </summary>
    /// <param name="d">dancer to move</param>
    /// <param name="newpos">position to move to</param>
    void Move(Dancer d, Vector2 newpos)
    {
        //Knock out!
        if (!InBounds(newpos))
        {
            RemoveDancer(d, newpos - GetDancerPos(d));
        }
        else //All good
        {
            //Update Dict
            var oldKey = GetDancerPos(d);
            _Dancers.Remove(oldKey);            
            _Dancers.Add(newpos, d);

            //Update world newpos
            var p = new Vector3(newpos.x, 0, newpos.y);
            d.UpdateTarget(p);
        }
    }

    /// <summary>
    /// Forces a dancer to a position, knocks all others out of the way
    /// Doesn't work diagonally!
    /// </summary>
    /// <param name="v">the change in position (eg strength of move)</param>
    public void Push(Dancer d, Vector2 delta)
    {
        //Debug
        RecurseDepth++;
        if (RecurseDepth > 50)
        {
            Debug.Log("Max recurse depth reached!");
            RecurseDepth = 0;
            return;
        }

        //can't move diagnonally
        Debug.Assert(!(delta.x > 0 && delta.y > 0));

        var pos = GetDancerPos(d);
        Vector2 EndPos = pos + delta; //Position dancer will end up at

        Dancer pushDancer;

        if (InBounds(EndPos) && _Dancers.TryGetValue(EndPos, out pushDancer)) //If next space is full
        {
            Push(pushDancer, delta);
            Move(d, EndPos);
        }
        else //Space is free or we've reached the end of the board
        {
            Move(d, EndPos);
        }

        RecurseDepth = 0;
    }


    /// <summary>
    /// Check dancers for valid moves
    /// </summary>
    void MoveCheck()
    {
        
    }

    /// <summary>
    /// Indicates that a group of dancers are ready to perform a move
    /// </summary>
    /// <param name="d"></param>
    void GlowDancers(Dancer d)
    {
        
    }

    /// <summary>
    /// Called on move end, called via unity events
    /// </summary>
    public void EndTurn()
    {
        //Disable previous player moving
        EnableMove(turn ? Player2 : Player1, false);

        //Switch turn
        turn = !turn;

        //Enable current player moving
        EnableMove(turn ? Player2 : Player1, true);


    }

    /// <summary>
    /// Enables a player's dancers, also resets the initial position
    /// </summary>
    /// <param name="p"></param>
    /// <param name="canMOve"></param>
    private void EnableMove(Player p, bool canMOve)
    {
        foreach (KeyValuePair<Vector2, Dancer> d in _Dancers)
        {
            if (d.Value.Player == p)
            {
                d.Value.canMove = canMOve;
                if (!canMOve) //Update player with new move
                {
                    d.Value.StartRoundPos = d.Key;
                }
            }
        }
    }

    /// <summary>
    /// WIP
    /// </summary>
    private void ChangeFloorCol()
    {
        ResetTileCol();

        _tileCounter = 0;
        _oddEven = !_oddEven;
        var mat = _oddEven ? tileColor1 : tileColor2;

        for(int i = 0; i < BoardW; i++)
        {
            for(int j = 0; j < BoardH; j++)
            {
                if (_oddEven)
                {
                    if(_tileCounter % 2 == 0)
                    {
                        Tiles[j, i].GetComponent<MeshRenderer>().material.color = mat;
                    }
                }
                else
                {
                    if (_tileCounter % 2 == 1)
                    {
                        Tiles[j, i].GetComponent<MeshRenderer>().material.color = mat;
                    }
                }
                _tileCounter++;
            }
        }
    }

    public Dancer GetDancer(Vector2 p)
    {
        Dancer d;
        _Dancers.TryGetValue(p, out d);
        return d;
    }

    private void RemoveDancer(Dancer d, Vector2 launchVec)
    {
        var key = GetDancerPos(d);
        d.KnockOut(launchVec);
        _Dancers.Remove(key);
    }

    /// <summary>
    /// Get the position of a dancer with a reverse lookup
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    private Vector2 GetDancerPos(Dancer d)
    {
        return _Dancers.FirstOrDefault(x => x.Value == d).Key;
    }
}
