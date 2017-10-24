using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Singleton that stores gamestate and coordinates managers
/// Maybe also global variables sometimes but don't tell anyone
/// </summary>
public class GameState : MonoBehaviour
{
    public static GameState me;
    public eGameState State { get; private set; }

    //Global debug switch
    public bool debug;

    //Enforce singleton
    //Also may be interpreted as some sort of deep metaphor
    void Start()
    {
        if (!me) me = this;
        if (this != me) Destroy(gameObject);

        ChangeState(eGameState.MENU);
	}

    public void ChangeState(eGameState newState)
    {
        State = newState;

        switch(newState)
        {
            case eGameState.MENU:
                BroadcastMessage("OnMainMenu", SendMessageOptions.DontRequireReceiver);
                break;

            case eGameState.GAME:
                BroadcastMessage("OnGameStart", SendMessageOptions.DontRequireReceiver);
                break;

            case eGameState.PAUSE:
                BroadcastMessage("OnPause", SendMessageOptions.DontRequireReceiver);
                break;

            case eGameState.GAME_END:
                BroadcastMessage("OnGameEnd", SendMessageOptions.DontRequireReceiver);
                break;

            case eGameState.TUTORIAL:
                BroadcastMessage("OnTutorial", SendMessageOptions.DontRequireReceiver);
                break;
        }

        BroadcastMessage("OnStateChange", SendMessageOptions.DontRequireReceiver);
    }

    //Overload for int conversion
    //Called by unity events button click
    public void ChangeState(int i)
    {
        ChangeState((eGameState)i);
    }
}

public enum eGameState
{
    MENU,
    GAME,
    GAME_END,
    TUTORIAL,
    PAUSE,
}


