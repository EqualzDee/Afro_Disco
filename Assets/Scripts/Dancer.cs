using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dancer : MonoBehaviour
{
    public float moveSpeed = 100;

    private bool _selected = false;
    private Vector3 _target;
    private Rigidbody _RB;


    public float _switchInterval = 1;
    public float noiseFactor = 0.2f;
    private Vector3 rand;
   
    private float _switchTime;

    public bool canMove = false;

    public Player Player;

    public Vector2 StartRoundPos; //My Position at the start of the round

    bool isDancing = true; //is Alive basically


    /// <summary>
    /// Called on spawn
    /// </summary>
    /// <param name="pos">My start pos</param>
    /// <param name="b">The controlling board</param>
    public void Initialize(Vector2 pos)
    {
        StartRoundPos = pos;
        _target = new Vector3(pos.x,0,pos.y);        
    }

	// Use this for initialization
	void Start ()
	{
	    _RB = GetComponent<Rigidbody>();
	    StartRoundPos = new Vector2((int)transform.position.x, (int)transform.position.z);
	    _switchTime = Time.time + _switchTime;

        //Listen to the beet mon
        AudioMan.OnBeat += Groove;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isDancing)
        {
            _RB.MovePosition(Vector3.Lerp(transform.position, _target + rand, Time.deltaTime*moveSpeed));
        }
    }
 
    /// <summary>
    /// When the player clicks on this dancer to move
    /// </summary>
    public void Select()
    {
        _selected = true;

        //Change color
        foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
        {
            m.material.color = Color.yellow;
        }        
    }

    public void UpdateTarget(Vector3 pos)
    {
        _target = pos;
    }

    /// <summary>
    /// Deselects the dancer
    /// </summary>
    public void DeSelect()
    {
        _selected = false;

        foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
        {
            m.material.color = Color.white;
        }
    }

    /// <summary>
    /// Sets up for next round
    /// </summary>
    public void NextRound()
    {
        
    }

    /// <summary>
    /// Gets the dancer's board pos from WORLD pos, not board
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPosition()
    {
        return new Vector2(_target.x, _target.z);
    }

    /// <summary>
    /// Move randomly to the beat
    /// </summary>
    private void Groove()
    {
        rand = new Vector3(Random.Range(-noiseFactor, noiseFactor), 0, Random.Range(-noiseFactor, noiseFactor));
    }

    /// <summary>
    /// This kills the afro
    /// </summary>
    public void KnockOut(Vector2 launchVec)
    {
        _RB.constraints = RigidbodyConstraints.None;
        _RB.AddForce(new Vector3(launchVec.x, 0, launchVec.y)*300);
        _RB.AddTorque(Random.rotation.eulerAngles*1000);
        isDancing = false;
        Destroy(gameObject, 3);

    }

}
