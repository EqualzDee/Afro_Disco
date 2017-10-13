using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Painter))]
public class Board : MonoBehaviour
{
    public GameObject TilePrefab;
    public Camera cam;              //used for raycasting
    public GameObject DancerPrefab;

    //The Y position the tile is spawned at so that it doesn't collide with the dancers
    public float YDanceOffset = -0.117f;

    public int DancerRange;

    public LayerMask moverLayer;

    public const int BoardW = 7;
    public const int BoardH = 11;
    private int _tileCounter;


    //TILES SHOULD BE THE ONLY PLACE WHERE X AND Y IS REVERSED (i and j)
    private GameObject[,] Tiles = new GameObject[BoardH, BoardW];

    //False is player 1 turn
    //True is player 2 turn
    public bool turn {get; private set;}

    //Player structs
    Player Player1 = new Player();
    Player Player2 = new Player();

    Dictionary<Vector2, Dancer> _Dancers = new Dictionary<Vector2, Dancer>(); //Big papa
    private Dancer _dancerSelected;
    Dictionary<Dancer, List<Vector2>>_validPositions = new Dictionary<Dancer, List<Vector2>>(); //Grid postions

    //flashing tiles
    bool flashThisTurn = true;
    private int RecurseDepth = 0;
    public Color SelectedColor;
    public Color SelectedOriginColor;

    //Objects
    private MoveChecker CheckyBoy = new MoveChecker();
    private Painter painter;
    private BustAMove busta;

    public Text TurnIndicator;

    //Round timer
    private float RoundTime = 60;
    private float roundCountdown = 60;
    public Text RoundTimerText;

    public bool debugMode;

	//Move finder
	[SerializeField]
	private GameObject uiParent;

    //FOR PROTOTYPE
    public Button conga;
    public Button boogaloo;

    public GameObject Afro;
    public GameObject AfroBackup;

    public GameObject Rock;
    public GameObject RockBackup;

    List<Move> moveOriginList = new List<Move>();
    

    void Awake()
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

                t.name = "Tile " + _tileCounter + " (" + i + "x" + j + ")";
                _tileCounter++;
            }
        }

        //Player 1 Dancers
        GenerateDancers(1,5,Player1);
        //Player 2 dancers
        GenerateDancers(9,5,Player2);
        
        //Let player 1 move for first move
        EnableMove(Player1, true);

        painter = GetComponent<Painter>();
        busta = new BustAMove(this);

        BakeMovement(turn);
        //MoveCheck();
    }

	
	// Update is called once per frame
	void Update ()
    {
        //Select Dancer with mouse via ray
        //if (Input.GetKey(KeyCode.Mouse0))
        if(Input.touchCount > 0)
        {
            RaycastHit hit;
			Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);

            if (!_dancerSelected) //no dancer selected
            {
                if (Physics.Raycast(ray, out hit)) //8 is dancer layer
                {
                    _dancerSelected = hit.transform.GetComponentInParent<Dancer>();

                    if (!_dancerSelected || !_dancerSelected.canMove) return;
                    
                    //Select and paint selected tiles
                    _dancerSelected.Select();
                    //FindValidTiles(_dancerSelected);
                    //paint selected layer
                    PaintSelection(_dancerSelected);


                }
            }
            else //Dancer is selected
            {
                //Move that booty
                if (Physics.Raycast(ray, out hit, 999, moverLayer))
                {
                    Vector2 hitBoardPos = new Vector2(hit.transform.position.x, hit.transform.position.z);

                    List<Vector2> poslist;
                    _validPositions.TryGetValue(_dancerSelected, out poslist);
                    Debug.Assert(_validPositions.ContainsKey(_dancerSelected));

                    if (poslist.Contains(hitBoardPos) || debugMode)
                        Move(_dancerSelected, hitBoardPos);                    
                }
            }
        }
        else if(_dancerSelected) //Deselect
        {
            //Checky moves!
            MoveCheck();
            

            _dancerSelected.DeSelect();
            //ResetTileCol();
            _dancerSelected = null;
            painter.ClearLayer(0);
            BakeMovement(turn);
        }

        //Round timer
        roundCountdown -= Time.deltaTime;
        RoundTimerText.text = roundCountdown.ToString("00");
        if (roundCountdown < 0)
        {
            EndTurn();
        }
    }

    /// <summary>
    /// Spawn Dancers for players
    /// </summary>
    /// <param name="yOffset">Y offset position</param>
    /// <param name="p">Player to spawn for</param>
    void GenerateDancers(int yOffset, int players, Player p)
    {
        for (int i = 0; i < players; i++)
        {
            var v = new Vector2(1 + i, yOffset);

            GameObject dancerObj;
            if (i == 2) //hackjob for prototype
            {
                dancerObj = Instantiate(p == Player1 ? Afro : Rock, new Vector3(v.x, 0, v.y), Quaternion.identity);
            }
            else //backup
            {
                dancerObj = Instantiate(p == Player1 ? AfroBackup : RockBackup, new Vector3(v.x, 0, v.y), Quaternion.identity);
            }

            var dancer = dancerObj.GetComponent<Dancer>();

            dancer.Player = p;
            _Dancers.Add(v, dancer);
            dancer.Initialize(v);
            dancerObj.name = "Dancer " + i;

            dancerObj.transform.localRotation = p == Player1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

            if (i == 2)
            {
                dancer.SetLead(true);
                dancerObj.GetComponentInChildren<MeshRenderer>().material.color = p == Player1
                    ? Color.yellow
                    : Color.gray;
            }
        }
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
    /// Doesn't work diagonally! Delta must have a magnitude of 1 max!
    /// </summary>
    /// <param name="v">the change in position (eg strength of move)</param>
    public void Push(Dancer d, Vector2 delta)
    {
        //Debug
        RecurseDepth++;
        if (RecurseDepth > 50)
        {
            Debug.Log("uh oh! Max recurse depth reached!");
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
            d.Stumble();
            Push(pushDancer, delta);
            Move(d, EndPos);
        }
        else //Space is free or we've reached the end of the board
        {
            d.Stumble();
            Move(d, EndPos);
        }

        RecurseDepth = 0;
    }


    /// <summary>
    /// Check dancers for valid moves
    /// </summary>
    void MoveCheck()
    {
        painter.ClearLayer(1); //Clear move layer

        //Only get dancers of the current player
        Dictionary<Vector2, Dancer> playerMaskedDancers = new Dictionary<Vector2, Dancer>();
        foreach (KeyValuePair<Vector2, Dancer> d in _Dancers)
        {
            if (d.Value.Player == (turn ? Player2 : Player1)) //Add masked players to turn if 
                playerMaskedDancers.Add(d.Key,d.Value);
        }

        moveOriginList = CheckyBoy.CheckForMoves(playerMaskedDancers,BoardW,BoardH + 1);

        //Paint valid moves
        moveOriginList.Sort();
        foreach (Move m in moveOriginList)
        {   
            if(debugMode) Debug.Log(m.origin);
            GlowDancer(m);
        }

        //PaintLayer(MovePaint);
    }

    /// <summary>
    /// Indicates that a group of dancers are ready to perform a move
    /// </summary>
    /// <param name="d"></param>
    void GlowDancer(Move move)
    {
        var m = move.GetFoundMove();
        var pos = move.origin;
        for (int i = 0; i < m.Length; i++)
        {
            for (int j = 0; j < m[i].Length; j++)
            {
                if (m[i][j] == 'D' || m[i][j] == 'A')
                {
                    var offset = new Vector2(j, i);
                    painter.AddToLayer(1, pos + offset, move.Color);
                }
            }
        }
    }

    /// <summary>
    /// Called on move end, called via unity events
    /// </summary>
    public void EndTurn()
    {
        //Deslect any dancers
        if (_dancerSelected)
        {
            _dancerSelected.DeSelect();
            _dancerSelected = null;
        }
        painter.ClearLayer(0);

        //Disable previous player moving
        EnableMove(turn ? Player2 : Player1, false);

        //Switch turn
        turn = !turn;

        //Enable current player moving
        EnableMove(turn ? Player2 : Player1, true);

//        TurnIndicator.text = (turn ? "Player 2" : "Player 1") + "'s turn";

        roundCountdown = RoundTime;

        //Reset move matching
        painter.ClearLayer(1);
        moveOriginList.Clear();

        //Move check + find all availible tiles
        MoveCheck();        
        BakeMovement(turn);

        //Make dis better
        conga.interactable = true;
        boogaloo.interactable = true;
    }

    /// <summary>
    /// Enables a player's dancers, also resets the initial position and range
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
                d.Value.StartRoundPos = d.Key;
                d.Value.rangePoints = DancerRange;
            }
        }
    }


    /// <summary>
    /// Outlines the tiles where the player can go
    /// </summary>
    void FindValidTiles(Dancer d)
    {
        //var dX = (int)d.StartRoundPos.x;
        //var dY = (int)d.StartRoundPos.y;

        var dX = (int)d._target.x;
        var dY = (int)d._target.z;

        //Loop through all directions to check
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int range = d.rangePoints;
                var VecCheck = new Vector2(j, i);

                //Decrease range for diags
                if (!(VecCheck.x == 0 || VecCheck.y == 0)) range -= 1;

                CastValidTile(dX, dY, VecCheck, range, d);
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
            Vector2 check = new Vector2(dX, dY) + (v * i);
            if (IsValidPos(check, d))
            {
                //Change tile col
                var t = Tiles[(int)check.y, (int)check.x]; //The valid tile
                Color c = i == 0 ? SelectedOriginColor : SelectedColor; //Set different color if origin
                //painter.AddToLayer(0, check, c);

                //Adderino dancer if can
                List<Vector2> poslist;
                _validPositions.TryGetValue(d, out poslist);

                if (!poslist.Contains(check))
                    poslist.Add(check);
            }
            else if (i > 0) //if newpos is not start position
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
    /// Bakes all possible move spaces 
    /// </summary>
    private void BakeMovement(bool turn)
    {
        _validPositions.Clear();
        foreach(Dancer d in _Dancers.Values.ToList()) //Get all dancers
        {
            if(turn ? d.Player == Player2 : d.Player == Player1) //Get all dancers from player current turn
            {
                //make sure dancer is in dict
                if (!_validPositions.ContainsKey(d))
                    _validPositions.Add(d, new List<Vector2>());
                
                FindValidTiles(d);
            }
        }
    }

    /// <summary>
    /// Paints the dancer's valid moves
    /// </summary>
    /// <param name="d"></param>
    private void PaintSelection(Dancer d)
    {
        if(d.rangePoints > 0)
            painter.AddToLayer(0, d.StartRoundPos, SelectedOriginColor); //Origin colour

        List<Vector2> poslist;
        _validPositions.TryGetValue(d, out poslist);

        foreach(Vector2 v in poslist)
        {
            painter.AddToLayer(0, v, SelectedColor);                
        }
    }

    //Getters for the dancer map
    public Dancer GetDancer(Vector2 p)
    {
        Dancer d;
        _Dancers.TryGetValue(p, out d);
        return d;
    }

    //Get dancer of a certain player
    public Dancer GetDancer(Vector2 p, bool isp2)
    {
        Dancer d;
        _Dancers.TryGetValue(p, out d);
        if (d && d.Player == (isp2 ? Player2 : Player1))        
            return d;
        else
            return null;
    }

    /// <summary>
    /// Remove dancer from board
    /// </summary>
    /// <param name="d">Dancer to remove</param>
    /// <param name="launchVec">How far to launch the dancer using physics</param>
    private void RemoveDancer(Dancer d, Vector2 launchVec)
    {
        var key = GetDancerPos(d);
        d.KnockOut(launchVec);
        _Dancers.Remove(key);
    }

    /// <summary>
    /// Get the position of a dancer with a reverse lookup
    /// </summary>
    private Vector2 GetDancerPos(Dancer d)
    {
        return _Dancers.FirstOrDefault(x => x.Value == d).Key;
    }

    /// <summary>
    /// Draws the bounds of the spawned dancefloor in the editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5F);

        Gizmos.DrawLine(Vector3.zero - new Vector3(0.5f,0,0.5f), new Vector3(BoardW,0, 0) - new Vector3(0.5f, 0, 0.5f));
        Gizmos.DrawLine(Vector3.zero - new Vector3(0.5f, 0, 0.5f), new Vector3(0,0, BoardH) - new Vector3(0.5f, 0, 0.5f));

        Gizmos.DrawLine(new Vector3(0, 0, BoardH) - new Vector3(0.5f, 0, 0.5f), new Vector3(BoardW, 0, BoardH) - new Vector3(0.5f, 0, 0.5f));
        Gizmos.DrawLine(new Vector3(BoardW, 0, BoardH) - new Vector3(0.5f, 0, 0.5f), new Vector3(BoardW, 0, 0) - new Vector3(0.5f, 0, 0.5f));
    }

    /// <summary>
    /// Test conga, executes all congas
    /// </summary>
    public void TestConga()
    {
        for(int i = 0; i < moveOriginList.Count; i++)
		{
		Move m = moveOriginList[i];
            if(m.MoveName.Contains("Conga"))
            {
                busta.Conga(m.origin, m.Range, m.PushPower, m.foundMoveCard);
                conga.interactable = false;
				
				//Create Conga Buttons
				//moveSelectorButton newButton = GameObject.Instantiate(moveSelectorButton);
				
				//newButton.transform.SetParent(null);
				//TODO: Set position
				//newButton.DanceMoveInstance.index = i;
				//newButton.SetActive(true);
            }
        }
    }
	
	public void ApplyConga(DanceMoveInstance indexHolder) {
		Move m = moveOriginList[indexHolder.index];
		busta.Conga(m.origin, m.Range, m.PushPower, m.foundMoveCard);
	}

    public void TestBoogaloo()
    {
        foreach (Move m in moveOriginList)
        {
            if (m.MoveName.Contains("Boogaloo"))
            {
                busta.Boogaloo(m.origin, m.Range, m.PushPower, m.GetFoundMove(), m.foundMoveCard);
                boogaloo.interactable = false;
            }
        }
    }

    public GameObject[,] GetTiles()
    {
        return Tiles;
    }

    public void ResetDancers()
    {
        var buffer = new List<Dancer>(_Dancers.Values);
        foreach (var key in buffer)
        {
            Move(key, key.StartRoundPos);
            key.rangePoints = DancerRange;
        }
        BakeMovement(turn);
    }
}
