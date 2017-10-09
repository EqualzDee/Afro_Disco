using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the board go pretty colours on the beat. (Disco layer)
/// Other pretty colours can be painted on top via layers
/// colours for these layers are stored in other classes and painted here
/// </summary>
[RequireComponent(typeof(Board))]
public class Painter : MonoBehaviour
{    
    public Color[] colorssss;
    private int colCounter;
    public float colLerptime = 0.2f;
    public float emissionIntensity = 0.5f;
    public float secondaryEmission = 0.1f;
    private bool flashThisTurn = true;
    private int _tileCounter;
    private bool _oddEven;


    private bool routineRunning = false;
    List<Material> tilesOn = new List<Material>();
    List<Material> tilesOff = new List<Material>();

    //Paint layers: Colors here won't be overridden;
    public static readonly string[] layerNames =
    {
        "Selected",
        "Move"
    };

    private Layer[] Layers = new Layer[layerNames.Length];

    private GameObject[,] Tiles;

    Board board;

    public void Start()
    {
        //get Board
        board = GetComponent<Board>();

        //Generate layers
        for (int i = 0; i < layerNames.Length; i++)
        {
            Layers[i] = new Layer(layerNames[i]);
        }

        //like share subscribe
        AudioMan.OnBeat += ChangeFloorCol;
        ChangeFloorCol();
    }

    private void Update()
    {
        //Paint layers over the top
        ResetTileCol(tilesOn, tilesOff);
        PaintLayer(1);
        PaintLayer(0);
    }

    /// <summary>
    /// Adds a color to a layer at a position
    /// </summary>
    /// <param name="layerIndex">index of layer, see layer names</param>
    /// <param name="pos">pos to paint at</param>
    /// <param name="c">color to paint</param>
    public void AddToLayer(int layerIndex, Vector2 pos, Color c)
    {
        var l = Layers[layerIndex];
        if (!l.dict.ContainsKey(pos))
            l.dict.Add(pos, c);
    }

    public void ClearLayer(int i)
    {
        Layers[i].dict.Clear();
    }

    /// <summary>
    /// Resets the tiles back to white
    /// </summary>
    void ResetTileCol(List<Material> on, List<Material> off)
    {
        if (routineRunning) return;

        foreach(Material m in on)
        {
            m.SetColor("_Color", colorssss[colCounter]);
            m.SetColor("_EmissionColor", colorssss[colCounter] * secondaryEmission);
        }

        foreach (Material m in off)
        {
            m.SetColor("_Color", Color.white);
            m.SetColor("_EmissionColor", Color.white * secondaryEmission);
        }

    }

    /// <summary>
    /// Called on the beat to change the tile color all flashy like
    /// </summary>
    private void ChangeFloorCol()
    {
        Tiles = board.GetTiles();

        //Flashy on snare
        if (!flashThisTurn)
        {
            flashThisTurn = true;
            return;
        }

        _tileCounter = 0;
        _oddEven = !_oddEven;

        tilesOn.Clear();
        tilesOff.Clear();

        //Split tiles into two groups
        for (int i = 0; i < Board.BoardW; i++)
        {
            for (int j = 0; j < Board.BoardH; j++)
            {
                var indexVec = new Vector2(i, j);
                if (_tileCounter % 2 == (_oddEven ? 0 : 1))
                    tilesOn.Add(Tiles[j, i].GetComponent<MeshRenderer>().material);
                if (_tileCounter % 2 == (_oddEven ? 1 : 0))
                    tilesOff.Add(Tiles[j, i].GetComponent<MeshRenderer>().material);
                _tileCounter++;
            }
        }

        //Start routines
        //Turn old tiles off
        StartCoroutine(ColChange(colorssss[colCounter], Color.white, tilesOff, colLerptime / 2));

        colCounter++;
        if (colCounter >= colorssss.Length)
            colCounter = 0;

        //Turn new tiles on
        StartCoroutine(ColChange(Color.white, colorssss[colCounter], tilesOn, colLerptime));

        flashThisTurn = false;
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
        routineRunning = true;
        float elapsedTime = 0;
        Color current = start;

        while (elapsedTime < time)
        {
            current = Color.Lerp(start, end, elapsedTime / time);

            foreach (Material m in Tiles)
            {
                m.SetColor("_Color", current);
                m.SetColor("_EmissionColor", current * secondaryEmission);
            }

            PaintLayer(1);
            PaintLayer(0);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        routineRunning = false;
    }
    
    private void PaintLayer(int LayerIndex)
    {
        var d = Layers[LayerIndex].dict;
        foreach (KeyValuePair<Vector2, Color> paint in d)
        {
            var tile = Tiles[(int)paint.Key.y, (int)paint.Key.x];
            var m = tile.GetComponent<MeshRenderer>().material;
            m.SetColor("_Color", paint.Value);
            m.SetColor("_EmissionColor", paint.Value * emissionIntensity);
        }
    }
}

public class Layer
{
    string _name;
    public Dictionary<Vector2, Color> dict = new Dictionary<Vector2, Color>();

    public Layer(string name)
    {
        _name = name;
    }
}
