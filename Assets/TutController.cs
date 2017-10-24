using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TutController : MonoBehaviour {

    private int Stage = -1; //index for the tutorial step

    public Text speech;
    public Transform afrodotjpeg;
    public Button advanceButt;

    public float speed;
    public float jiggleMagnitude;
    private Vector2 originpos;

    public Board _board;

    private Dancer lead;

    //steps where text won't advance until something happens
    //Array value is the index of the speech line
    private readonly int[] interaction = {6,9 };
    //private readonly int[] UI = 

    const int aaa = 3;

    private bool pressedBack;

    string[] Text =
    {
        "hey kiddo. it's me. your friendly neighbourhood disco teacher",
        "so i hear you wanna beat that punk, rock mullet in a dance fight huh?", 
        "well you're gunna have to learn how to get down on the d-floor and boogie first",
        "but don't worry. stick with me and you'll be booty callin' and boogalooin' in no time",
        "lets start on your footwork first",
        //"you can't make any sweet moves unless you're bouncin' around that floor",
        "each turn you have, you can move each of your boys two spaces",
        "give that a shot now.", //I-6

        "groovy man",
        "and don't worry if you messup",
        "it might not be too stylin', but just press the undo button to go back a step",  //I-9 UI-0

        "no biggie right?", //I-10

        "uh oh",
        "there's some dude over there crampin your style",
        "we better show him who's boss",
        "around here, we do that by bustin' moves",
        "lets try a conga line to start off with",
        "first arrange your boys in a line aiming at your target",
        "and then bust that move, homie", //I-17 //UI-1

        "nice. that was a good shove",
        "lets get him out of here and knock him off the floor",
        "end our turn, and we'll pull off another one",

        "now just line up in the other direction...",

        "groovy.",
        "look at him go.",

        "to win a dance battle knock out three other dudes, or the other lead dancer",
        "you'll know what the lead dancer looks like when you see them",
        
    };

	// Use this for initialization
	void Start ()
    {
        NextStep();
        originpos = afrodotjpeg.position;
        lead = _board.GetDancer(new Vector2(3,1));
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch(Stage)
        {
            case 6:
                if (lead.StartRoundPos != lead.GetBoardPos() && !lead.selected)
                    NextStep();
                break;
        }
	}

    public void NextStep()
    {
        Stage++;

        StartCoroutine(SNESText(Text[Stage], speed, speech, Stage));

        if(interaction.Contains(Stage))
        {
            advanceButt.gameObject.SetActive(false);
        }
        else if(!advanceButt.gameObject.activeInHierarchy)
        {
            advanceButt.gameObject.SetActive(true);
        }
    }

    //advances second interaction
    public void BackwasPressed()
    {
        if(Stage == interaction[1]) NextStep();
    }

    IEnumerator SNESText(string text, float speed, Text t, int currstage)
    {
        string partialText = "";
        for(int i = 0; i < text.Length; i++)
        {
            if (currstage == Stage) //Check the user hasn't skipped
            {
                partialText += text[i]; //add char by char
                t.text = partialText;
                var rand = new Vector2(Random.Range(-jiggleMagnitude, jiggleMagnitude), Random.Range(-jiggleMagnitude, jiggleMagnitude));
                afrodotjpeg.position = originpos + rand;
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(speed);
        }
    }
}
