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
    public LayerMask DancerLayer;

    public const int BoardW = 7;
    public const int BoardH = 11;
    private int _tileCounter;


    //TILES SHOULD BE THE ONLY PLACE WHERE X AND Y IS REVERSED (i and j)
    private GameObject[,] Tiles = new GameObject[BoardH, BoardW];

    //False is player 1 turn
    //True is player 2 turn
    public bool turn {get; private set;}

    //Player structs
    public Player Player1 = new Player();
    public Player Player2 = new Player();

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
    public Painter painter;
    private BustAMove busta;
    public UIMan UI;

    public Text TurnIndicator;

    //Round timer
    private float RoundTime = 60;
    private float roundCountdown = 60;
    public Text RoundTimerText;    

    //FOR PROTOTYPE
    public Button conga;
    public Button boogaloo;

    public GameObject Afro;
    public GameObject AfroBackup;

    public GameObject Rock;
    public GameObject RockBackup;

    List<Move> moveOriginList = new List<Move>();
    public float LaunchForce = 50;

    //FOR MONKEYS
    public Toggle disablecolliders;

    //Undo button
    public struct DanceStep
    {
        public Dancer d;
        public Vector2 pos;
        public int range;
    }

    public Stack<DanceStep> _backStack = new Stack<DanceStep>();
    private Vector2 _dancerStartMovePos; //The position of selected dancer at the start of a drag

    //flag to detirmine if a game is currently being played
    public bool GameActive { get; private set;}

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

        //Setup references
        painter = GetComponent<Painter>();
        busta = new BustAMove(this);
    }

    //Gamestate call
    void OnGameStart()
    {
        if (GameActive) return; 

        //Player 1 Dancers
        GenerateDancers(1, 5, Player1);
        //Player 2 dancers
        GenerateDancers(9, 5, Player2);

        //Let player 1 move for first move
        EnableMove(Player1, true);

        //Put the movement pie in the oven
        BakeMovement(turn, false);

        //UI
        UI.ResetGameUI();
        UI.UpdateTurn(turn);

        GameActive = true;
    }

    //Gamestate call
    //Resetti the spaghettis
    //(Resets the board state)
    void OnMainMenu()
    {
        //Afro genocide
        foreach (KeyValuePair<Vector2, Dancer> d in _Dancers)
        {
            if(d.Value)
                Destroy(d.Value.gameObject);
        }

        _Dancers.Clear();
        _validPositions.Clear();
        painter.ClearLayer(1);
        painter.ClearLayer(0);
        moveOriginList.Clear();
        turn = false;
        UI.ResetGameUI();
        GameActive = false;
    }

    void OnTutorial()
    {
        SpawnDancer(true, new Vector2(3, 1), Player1, "tutorial afro");
        BakeMovement(turn, false);
        EnableMove(Player1, true);        
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

                    //Hit dancer
                    _dancerSelected = hit.transform.GetComponent<Dancer>();
                    if (!_dancerSelected)
                    {
                        //Hit board
                        var hitpos = hit.transform.position;
                        var boardPos = new Vector2(hitpos.x, hitpos.z);
                        _dancerSelected = GetDancer(boardPos);
                    }

                    //If valid selection
                    if (_dancerSelected && _dancerSelected.canMove)
                    {
                        _dancerSelected.Select();           //Select and paint selected tiles
                        PaintSelection(_dancerSelected);    //paint selected layer
                        _dancerStartMovePos = _dancerSelected.GetBoardPos();
                    }
                    else //cancel coz we can't move
                    {
                        _dancerSelected = null;
                    }
                }
            }
            else //Dancer is selected
            {
                //Move that booty
                if (Physics.Raycast(ray, out hit, 999, moverLayer.value))
                {
                    Vector2 hitBoardPos = new Vector2(hit.transform.position.x, hit.transform.position.z);

                    List<Vector2> poslist;
                    _validPositions.TryGetValue(_dancerSelected, out poslist);
                    Debug.Assert(_validPositions.ContainsKey(_dancerSelected));

                    if (poslist.Contains(hitBoardPos) || GameState.me.debug)
                        Move(_dancerSelected, hitBoardPos);                    
                }
            }
        }
        else if(_dancerSelected) //Deselect
        {
            //Add to back stack &  update its ui
            if(_dancerStartMovePos != _dancerSelected.GetBoardPos())
            {
                var step = new DanceStep();
                step.d = _dancerSelected;
                step.pos = _dancerStartMovePos;
                step.range = _dancerSelected.rangePoints;
                _backStack.Push(step);

                UI.UpdateInteractable(this);
            }

            //Checky moves!
            CheckMoves();

            //Cleanup
            _dancerSelected.DeSelect();
            painter.ClearLayer(0);
            BakeMovement(turn, false);

            //Laterssss
            _dancerSelected = null;
        }

        //Round timer
        if (GameActive && GameState.me.State == eGameState.GAME)
        {
            roundCountdown -= Time.deltaTime;
            RoundTimerText.text = roundCountdown.ToString("00");
            if (roundCountdown < 0)
            {
                EndTurn(true);
            }
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

            SpawnDancer(i == 2, v, p, "Dancer " + i);
        }
    }
    /// <summary>
    /// Spawns a dancer
    /// </summary>
    /// <param name="islead">if the dancer is the lead</param>
    /// <param name="pos">position</param>
    /// <param name="p">what player s</param>
    /// <param name="objname"></param>
    /// <returns></returns>
    public Dancer SpawnDancer(bool islead, Vector2 pos, Player p, string objname)
    {
        GameObject dancerObj;
        if (islead) //Lead
        {
            dancerObj = Instantiate(p == Player1 ? Afro : Rock, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }
        else //backup
        {
            dancerObj = Instantiate(p == Player1 ? AfroBackup : RockBackup, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        }

        var dancer = dancerObj.GetComponent<Dancer>();

        dancer.Player = p;
        _Dancers.Add(pos, dancer);
        dancer.Initialize(pos);
        dancerObj.name = name;

        dancerObj.transform.localRotation = p == Player1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        if (islead) dancer.SetLead(true);

        //FOR MOMKEY
        dancer.myCollider.enabled = !disablecolliders.isOn;

        return dancer;
    }

    /// <summary>
    /// Moves a dancer to a postion. Will knock the dancer out if moved off the floor
    /// </summary>
    /// <param name="d">dancer to move</param>
    /// <param name="newpos">position to move to</param>
    public void Move(Dancer d, Vector2 newpos)
    {
        //Knock out!
        if (!InBounds(newpos) && d.isDancing)
        {
            Vector2 dir = (newpos - GetDancerPos(d));
            RemoveDancer(d,  dir * LaunchForce);

            //Win state check
            if(CountDancers(turn ? Player2 : Player1) < 3 || d.IsLead)
            {
                GameState.me.ChangeState(eGameState.GAME_END);
            } 
        }
        else if(InBounds(newpos) && d.isDancing) //All good
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
    void CheckMoves()
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
            //if(GameState.me.debug) Debug.Log(m.origin);
            GlowDancer(m);
        }        
    }

    /// <summary>
    /// Indicates that a group of dancers are ready to perform a move
    /// </summary>
    /// <param name="d"></param>
    void GlowDancer(Move move)
    {
        //Common
        var m = move.GetFoundMove();
        var pos = move.origin;

        bool isBooty = false;

        //Glow Booty call
        if (move.GetFoundMove().Length >= BoardW ||
            move.GetFoundMove()[0].Length >= BoardW) 
        {
            isBooty = true;
        }

        for (int i = 0; i < m.Length; i++)
        {
            for (int j = 0; j < m[i].Length; j++)
            {
                if (isBooty)
                {
                    if (GetDancer(new Vector2(j,i) + pos))
                    {
                        painter.AddToLayer(1, pos + new Vector2(j, i), move.Color);
                    }
                }
                else if (m[i][j] == 'D' || m[i][j] == 'A')
                {
                    painter.AddToLayer(1, pos + new Vector2(j, i), move.Color);
                }
            }
        }
    }

    /// <summary>
    /// Called on move end, called via unity events
    /// </summary>
    public void EndTurn(bool changeUI)
    {
        if (UI.isMovingUI) return; //don't allow when moving

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
        CheckMoves();        
        BakeMovement(turn, false);

        //Get rid of back stack
        _backStack.Clear();

        if(changeUI)
            UI.UpdateTurn(turn);
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
                d.Value.PrevPos = d.Value.GetBoardPos();
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
    /// <param name="turn">What player's dancers's moves to bake</param>
    /// <param name="changeOrigin">This bakes the current movement state of dancers to prevent cheatin</param>
    public void BakeMovement(bool turn, bool changeOrigin)
    {
        _validPositions.Clear();
        foreach(Dancer d in _Dancers.Values.ToList()) //Get all dancers
        {
            if(d.Player == (turn ? Player2 : Player1)) //Get all dancers from player current turn
            {
                //make sure dancer is in dict
                if (!_validPositions.ContainsKey(d))
                    _validPositions.Add(d, new List<Vector2>());

                if (changeOrigin) //Locks in origin and points to prevent cheatin
                {
                    d.StartRoundPos = d.GetBoardPos();
                    d.StartrangePoints = d.rangePoints;

                    //Clear backstack
                    _backStack.Clear();
                    UI.UpdateInteractable(this);
                }
                
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
        UI.AnotherOneBitesTheDust(d.Player == Player2);
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
                UI.ActivateButton(0, turn, false);
                BakeMovement(turn, true);

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
                UI.ActivateButton(1, turn, false);
                BakeMovement(turn, true);
            }
        }

    }

	public void TestCrowdSurf()
	{
		foreach (Move m in moveOriginList)
		{
			if (m.MoveName.Contains("Crowd Surf"))
			{
				busta.CrowdSurf(m.origin, m.GetFoundMoveFiringPos() ,m.PushPower, m.GetFoundMove(), m.foundMoveCard);
                UI.ActivateButton(2, turn, false);
                BakeMovement(turn, true);
                CheckMoves();
            }
		}

    }

    public void TestBooty()
    {
        foreach (Move m in moveOriginList)
        {
            if (m.MoveName.Contains("Booty"))
            {
                //Cardinality is normalized since we have overlapping keys
                busta.BootyCall(m.origin, m.GetFoundMove(), m.foundMoveCard.normalized);
                UI.ActivateButton(3, turn, false);
                BakeMovement(turn, true);
                CheckMoves();
            }
        }

    }

    public GameObject[,] GetTiles()
    {
        return Tiles;
    }

    //Resets dancer positions back one step
    public void UndoMove()
    {
        if (!CanUndo()) return;
        DanceStep step = _backStack.Pop();
        Move(step.d, step.pos);
        step.d.ResettiTheSpaghetti(step.range);
        UI.UpdateInteractable(this);
        BakeMovement(turn,false);
        CheckMoves();
    }    

    public bool CanUndo()
    {
        return _backStack.Count > 0;
    }

    public static bool OnOuterEdge(Vector2 pos)
    {
        return pos.x == 0 || pos.y == 0 || pos.x == BoardW - 1 || pos.y == BoardH - 1;
    }

    public int CountDancers(Player p)
    {
        int count = 0;

        foreach (KeyValuePair<Vector2, Dancer> d in _Dancers)
        {
            if (d.Value.Player == p) count++;
        }

        return count;
    }
}
