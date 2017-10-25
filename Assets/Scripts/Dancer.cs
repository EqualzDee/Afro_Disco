using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dancer : MonoBehaviour
{
    public float moveSpeed;

    public bool selected { get; private set; }
    public Vector3 _target { get; private set;} //aka board position
    private Rigidbody _RB;

    public bool IsLead { get; private set; }
    
    public float noiseFactor = 0.2f;
    private Vector3 rand;    

    public bool canMove = false;

    public Player Player;

    public Vector2 StartRoundPos; //My Position at the start of the round
    public Vector2 PrevPos;

    public bool isDancing { get; private set; }

    private Animator _myAnimator;

    public int rangePoints = 2;
    public int StartrangePoints = 2;

	public CapsuleCollider myCollider;
    private float outOfRangeTimer;
    public float outOfRangeTimeOut = 0.5f;
    
    private Vector2 koLaunchVec;


    /// <summary>
    /// Initalize dancer to board
    /// </summary>
    /// <param name="pos">My start pos</param>
    /// <param name="b">The controlling board</param>
    public void Initialize(Vector2 pos)
    {
        StartRoundPos = pos;
        PrevPos = pos;
        _target = new Vector3(pos.x,0,pos.y);
        isDancing = true;

        myCollider = GetComponentInChildren<CapsuleCollider>();
    }

	// Use this for initialization
	void Start ()
	{
	    _RB = GetComponent<Rigidbody>();
	    StartRoundPos = new Vector2((int)transform.position.x, (int)transform.position.z);

	    _myAnimator = GetComponentInChildren<Animator>();
        _myAnimator.enabled = false;

	    Invoke("animOffset", Random.Range(0f, 0.2f));

        //Disable rag doll colliders
        //EnableRagdoll(false);

		//Set random offset for collider to prevent weird collisions    
		myCollider.transform.position += new Vector3(Random.Range(-0.1f,0.1f),0,
			Random.Range(-0.1f,0.1f));

		//Listen to the beet mon
		//AudioMan.OnBeat += Groove;
	}

    void animOffset()
    {
        _myAnimator.enabled = true;
    }

    private void Update()
    {
        //Scheduled for D E A T H
        if (!isDancing)
            if (Board.OnOuterEdge(GetWorldPosition()))
            {
                KnockOutInner(koLaunchVec);
                Invoke("Melt", 5);
            }
    }


    // Update is called once per frame
    void FixedUpdate ()
    {
        Vector3 lerpval = Vector3.Lerp(transform.position, _target, moveSpeed);            
        transform.position = lerpval;     
    }
 
    /// <summary>
    /// When the player clicks on this dancer to move
    /// </summary>
    public void Select()
    {
        selected = true;

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
        selected = false;
        var delta = PrevPos - GetBoardPos();
        var rangeLoss = (int) Mathf.Abs(delta.x) + (int)Mathf.Abs(delta.y);
        rangePoints -= rangeLoss;
        rangePoints = Mathf.Clamp(rangePoints, 0, 999);
        PrevPos = GetBoardPos();
    }

    /// <summary>
    /// Gets the dancer's board pos from WORLD pos, not board
    /// Use GetBoardPos() for board pos
    /// </summary>
    /// <returns></returns>
    public Vector2 GetWorldPosition()
    {
        return new Vector2(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.z));
    }

    /// <summary>
    /// This kills the afro
    /// </summary>
    public void KnockOut(Vector2 launchVec)
    {
        isDancing = false;
        koLaunchVec = launchVec;        
    }

    private void KnockOutInner(Vector2 launchVec)
    {
        _RB.constraints = RigidbodyConstraints.None;
        Vector3 forward = new Vector3(launchVec.x, 0, launchVec.y);
        if(!IsLead) _RB.AddForce(forward); //Make lead go spinny
        _RB.AddTorque(new Vector3(0,5000,0));

        //Rag doll!
        //EnableRagdoll(true);
        _myAnimator.StopPlayback();
        //Destroy(gameObject, 3);
    }

    private void Melt()
    {
        myCollider.enabled = false;
        Destroy(gameObject,5);
    }

    /// <summary>
    /// Set this dancer as lead (only 1 per team) (NOT VALIDATED!!!!)
    /// </summary>
    /// <param name="b"></param>
    public void SetLead(bool b)
    {
        IsLead = true;
    }

    //Animation to play when pushed
    public void Stumble()
    {
        _myAnimator.SetTrigger("Stumble");
    }


    //When a dancer brushes this dancer, play shove animation
    void OnCollisionEnter(Collision c)
    {        
        if (c.collider.tag == "Dancer" && !selected)
        {            
            _myAnimator.SetTrigger("Shove");
        }
    }

    void EnableRagdoll(bool b)
    {
        //Rag doll!
        foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
        {
            r.isKinematic = !b;
        }
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = b;
        }

		myCollider.enabled = !b;		
		_RB.isKinematic = b;
    }

    public void ResettiTheSpaghetti(int range)
    {
        rangePoints = range;
        PrevPos = StartRoundPos;
    }

    public Vector2 GetBoardPos()
    {        
        return new Vector2(_target.x, _target.z);
    }

    

}
