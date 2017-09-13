using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    private bool _oddEven;

    //TILES SHOULD BE THE ONLY PLACE WHERE X AND Y IS REVERSED (i and j)
    private GameObject[,] Tiles = new GameObject[BoardH, BoardW];

    //False is player 1 turn
    //True is player 2 turn
    public bool turn = false;

    //Create player structs
    Player Player1 = new Player();
    Player Player2 = new Player();

    //flashing tiles
    bool flashThisTurn = true;
    private int RecurseDepth = 0;
    public Color SelectedColor;
    public Color[] colorssss;
    private int colCounter;
    public float colLerptime = 0.2f;
    
    Dictionary<Vector2, Dancer> _Dancers = new Dictionary<Vector2, Dancer>(); //Big papa

    private Dancer _dancerSelected;
    Dictionary<Vector2, GameObject> _validPositions = new Dictionary<Vector2,GameObject>(); //Grid postions

    private MoveChecker CheckyBoy = new MoveChecker();

    private bool speedUp; //true when either team has less than 3 dancers

    //Paint layers: Colors here won't be overridden;
    private Dictionary<Vector2, Color> SelectedPaint = new Dictionary<Vector2, Color>();
    private Dictionary<Vector2, Color> MovePaint = new Dictionary<Vector2, Color>();

    public Text TurnIndicator;

    //Round timer
    private float RoundTime = 60;
    private float roundCountdown = 60;
    public Text RoundTimerText;

    List<Move> moveOriginList;

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

                t.name = "Tile " + _tileCounter + " (" + i + "x" + j + ")";
                _tileCounter++;
            }
        }

        //Player 1 Dancers
        GenerateDancers(1,Player1);
        //Player 2 dancers
        GenerateDancers(9,Player2);

        //Let player 1 move for first move
        EnableMove(Player1, true);

        //like share subscribe
        AudioMan.OnBeat += ChangeFloorCol;
        ChangeFloorCol();
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
                    
                    //Select and paint selected tiles
                    _dancerSelected.Select();
                    ShowValidTiles(_dancerSelected);
                    PaintLayer(SelectedPaint);
                }
            }
            else //Dancer is selected
            {
                //Move that booty
                if (Physics.Raycast(ray, out hit, 999, moverLayer))
                {
                    Vector2 hitBoardPos = new Vector2(hit.transform.position.x, hit.transform.position.z);
                    if (_validPositions.ContainsKey(hitBoardPos))
                        Move(_dancerSelected, hitBoardPos);
                    //Push(_dancerSelected, hitBoardPos - GetDancerPos(_dancerSelected));
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
            _validPositions.Clear();
            SelectedPaint.Clear();
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

    /// <summary>
    /// Resets the tiles back to white
    /// </summary>
    void ResetTileCol()
    {
        for (int i = 0; i < BoardW; i++)
        {
            for (int j = 0; j < BoardH; j++)
            {
                Tiles[j, i].GetComponent<MeshRenderer>().material.SetColor("_Color",Color.white);

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
    //todo override flashing tiles with valid tiles
    void CastValidTile(int dX, int dY, Vector2 v, int length, Dancer d)
    {        
        for (int i = 0; i < length + 1; i++)
        {
            Vector2 check = new Vector2(dX, dY) + (v*i);            
            if (IsValidPos(check, d))
            {
                //Change tile col
                var t = Tiles[(int) check.y, (int) check.x]; //The valid tile
                Color c = i == 0 ? Color.blue : SelectedColor;
                //t.GetComponent<MeshRenderer>().material.SetColor("_Color", c);
                if(!SelectedPaint.ContainsKey(check))
                    SelectedPaint.Add(check, c); //Paint color

                //Add to dict if not already
                if (!_validPositions.ContainsKey(check))
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
        MovePaint.Clear();

        //Only get dancers of the current player
        Dictionary<Vector2, Dancer> playerMaskedDancers = new Dictionary<Vector2, Dancer>();
        foreach (KeyValuePair<Vector2, Dancer> d in _Dancers)
        {
            if (d.Value.Player == (turn ? Player2 : Player1)) //Add masked players to turn if 
                playerMaskedDancers.Add(d.Key,d.Value);
        }

        moveOriginList = CheckyBoy.CheckForMoves(playerMaskedDancers,BoardW,BoardH);

        //Glow valid moves
        foreach (Move m in moveOriginList)
        {    
            Debug.Log(m.origin);
            GlowDancer(m);
        }

        PaintLayer(MovePaint);
    }

    /// <summary>
    /// Indicates that a group of dancers are ready to perform a move
    /// </summary>
    /// <param name="d"></param>
    void GlowDancer(Move move)
    {
        var m = move.FoundMove();
        var pos = move.origin;
        for (int i = 0; i < m.Length; i++)
        {
            for (int j = 0; j < m[i].Length; j++)
            {
                if (m[i][j] == 'D' || m[i][j] == 'A')
                {
                    var offset = new Vector2(j, i);
                    if(!MovePaint.ContainsKey(pos + offset))
                        MovePaint.Add(pos + offset, move.Color);   
                }
            }
        }
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

        TurnIndicator.text = (turn ? "Player 2" : "Player 1") + "'s turn";

        roundCountdown = RoundTime;
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
                d.Value.StartRoundPos = d.Key;
            }
        }
    }

    private void PaintLayer(Dictionary<Vector2,Color> d)
    {
        foreach (KeyValuePair<Vector2, Color> paint in d)
        {
            var tile = Tiles[(int)paint.Key.y, (int)paint.Key.x];
            tile.GetComponent<MeshRenderer>().material.SetColor("_Color", paint.Value);
        }
    }

    /// <summary>
    /// Called on the beat to change the tile color all flashy like
    /// </summary>
    private void ChangeFloorCol()
    {
        //Flashy on snare
        if (!speedUp)
        {
            if (!flashThisTurn)
            {
                flashThisTurn = true;
                return;
            }
        }

        ResetTileCol();

        _tileCounter = 0;
        _oddEven = !_oddEven;
       
        List<Material> tilesOn = new List<Material>();
        List<Material> tilesOff = new List<Material>();

        //Paint floor tiles
        for (int i = 0; i < BoardW; i++)
        {
            for(int j = 0; j < BoardH; j++)
            {
                var indexVec = new Vector2(i, j);
                if (!(SelectedPaint.ContainsKey(indexVec) ||
                    MovePaint.ContainsKey(indexVec)))
                {
                    if (_tileCounter % 2 == (_oddEven ? 0 : 1))
                        tilesOn.Add(Tiles[j, i].GetComponent<MeshRenderer>().material);
                    if (_tileCounter % 2 == (_oddEven ? 1 : 0))
                        tilesOff.Add(Tiles[j, i].GetComponent<MeshRenderer>().material);
                }
                _tileCounter++;
            }
        }

        //Start routines
        StartCoroutine(ColChange(colorssss[colCounter], Color.white, tilesOff, colLerptime/2));

        colCounter++;
        if (colCounter >= colorssss.Length)
            colCounter = 0;

        StartCoroutine(ColChange(Color.white, colorssss[colCounter], tilesOn, colLerptime));

        //Paint layers over the top
        PaintLayer(SelectedPaint);
        PaintLayer(MovePaint);

        flashThisTurn = false;
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

    /// <summary>
    /// Coroutine to change color
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="Tiles"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator ColChange(Color start, Color end, List<Material> Tiles, float time)
    {
        float elapsedTime = 0;
        Color current = start;

        while (elapsedTime < time)
        {
            current = Color.Lerp(start, end, elapsedTime / time);
            
            foreach (Material m in Tiles)
            {
                m.SetColor("_Color",current);
            }

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
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

    public void TestConga()
    {
        foreach(Move m in moveOriginList)
        {
            if(m.MoveName.Contains("Conga"))
            {
                Conga(m.origin, 2, 2, m.FoundMove());
            }
        }
    }

    //Moves here -- may move to XML/file read/unity editor tool
    void Conga(Vector2 pos, int range, int push, string[] move)
    {
        //Vertical conga
        //Get just the tips (we only need one dancer for push to work)
        Vector2 top = new Vector2(pos.x, move.Length + pos.y);
        Vector2 bottom = pos - new Vector2(0,1);

        Dancer dtop = null; //null if not found
        Dancer dBot = null;

        //find targets in range (only one)
        for (int i = 0; i < range; i++)
        {
            if(!dtop) //top
                dtop = GetDancer(top + new Vector2(0, i),!turn); //Look for enemies

            if (!dBot) //bot
                dBot = GetDancer(pos - new Vector2(0, i),!turn);
        }

        //Push
        for(int i = 0; i < push; i++)
        {
            if (dtop)
                Push(dtop, new Vector2(0, 1));
            if (dBot)
                Push(dBot, new Vector2(0, -1));
        }
    }
}
