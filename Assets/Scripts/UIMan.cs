using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIMan : MonoBehaviour {

    public Sprite NotKOd;
    public Sprite KOd;

    public Transform Left;
    public Transform Right;

    public int MoveOffset = 34;
    public float buttonMoveSpeed = 0.3f;

    private bool first = true;

    private int deadCountP1;
    private int deadCountP2;
    public Transform P1Score;
    public Transform P2Score;

    // Update is called once per frame
    void Start () {
        ShowUI(0);
	}

    public void UpdateTurn(bool turn)
    {       
        var child = turn ? Right : Left;    //Kid that gets turned off
        var other = !turn ? Right : Left;   //Kid that gets turned on

        //Child
        foreach (Button b in child.GetComponentsInChildren<Button>())
        {
            ActivateButton(b.transform, !turn, false);
        }

        if(first)
        {
            first = false;
            return;
        }

        //Other
        foreach (Button b in other.GetComponentsInChildren<Button>())
        {
            ActivateButton(b.transform, turn, true);
        }

    }

    //Move a butt
    private IEnumerator MovePos(Transform t, Vector3 startPos, int offset, bool isLeft, bool isOn, float time)
    {
        float elapsedTime = 0;

        Vector3 offsetVec = new Vector3(offset, 0);
        Vector2 endPos;

        if (isLeft)
        {
            if (isOn) endPos = startPos + offsetVec;
            else endPos = startPos - offsetVec;
        }
        else
        {
            if (isOn) endPos = startPos - offsetVec;
            else endPos = startPos + offsetVec;
        }        

        while (elapsedTime < time)
        {
            t.position = Vector3.Lerp(startPos, endPos, elapsedTime/time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        t.position = endPos;
    }

    //Activate butts
    private void ActivateButton(Transform t, bool isLeft, bool isOn)
    {
        if (t.GetComponent<Button>().interactable == isOn) return;

        t.GetComponent<Button>().interactable = isOn;
        StartCoroutine(MovePos(t.transform, t.transform.position, MoveOffset, isLeft, isOn, buttonMoveSpeed));
    }

    public void ActivateButton(int i, bool isLeft, bool isOn)
    {
        Transform t = isLeft ? Left : Right;
        ActivateButton(t.GetChild(i), isLeft, isOn);
    }

    public void AnotherOneBitesTheDust(bool isPlayerTwo)
    {
        if (isPlayerTwo)
            deadCountP2 += 1;
        else
            deadCountP1 += 1;

        for(int i = 0; i < deadCountP1; i++)
        {
            P1Score.GetChild(i).GetComponent<Image>().sprite = KOd;
        }

        for (int i = 0; i < deadCountP2; i++)
        {
            P2Score.GetChild(i).GetComponent<Image>().sprite = KOd;
        }
    }

    //Clear sprites and reset counts
    public void ResetGameUI()
    {
        deadCountP1 = 0;
        deadCountP2 = 0;

        for (int i = 0; i < 3; i++)
        {
            P1Score.GetChild(i).GetComponent<Image>().sprite = NotKOd;
            P2Score.GetChild(i).GetComponent<Image>().sprite = NotKOd;
        }
    }

    //for gamestate message
    private void OnGameStart()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Invoke("menudelayhack", 0.5f);
    }

    //Switches between menu and game UI
    private void ShowUI(int i)
    {
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
        transform.GetChild(i).gameObject.SetActive(true);
    }

    private void menudelayhack()
    {
        ShowUI(1);
    }

    //Update UI for state
    void OnStateChange()
    {
        var state = (int)GameState.me.State;
        if (state != 1)
            ShowUI(state);
    }
}
