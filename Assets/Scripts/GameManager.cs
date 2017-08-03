using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int playerNum = 0;
    [HideInInspector]
    public bool started = false;
    
    [SerializeField]
    private connect NetworkManager;

    //[SerializeField]
    //private Camera[] Cameras = new Camera[2];

    public GameObject playerPrefab;

    [SerializeField]
    private Text messageText;

    private GameObject localPlayer;
    private PlayerControl localPlayerCtrl;
    private GameObject opponentPlayer;
    private PlayerControl opponentPlayerCtrl;

    void Start ()
    {
		
	}

    public void begin (int playerNumber)
    {
        playerNum = playerNumber;

        if (!started)
        {
            localPlayer = Instantiate(playerPrefab, transform.position, transform.rotation);
            localPlayerCtrl = localPlayer.GetComponent<PlayerControl>();
            localPlayerCtrl.setGM(gameObject.GetComponent<GameManager>());
            opponentPlayer = Instantiate(playerPrefab, transform.position, transform.rotation);
            opponentPlayerCtrl = opponentPlayer.GetComponent<PlayerControl>();
            //opponentPlayerCtrl.setGM(gameObject.GetComponent<GameManager>());
        }

        //load camera
        if (playerNum == 2)
        {
            //Cameras[1].enabled = true;
            //Cameras[0].enabled = false;
            localPlayerCtrl.setRed(true);
            opponentPlayerCtrl.setRed(false);
        }
        else
        {
            //Cameras[0].enabled = true;
            //Cameras[1].enabled = false;
            localPlayerCtrl.setRed(false);
            opponentPlayerCtrl.setRed(true);
        }
        localPlayerCtrl.setLocal(true);
        opponentPlayerCtrl.setLocal(false);

        localPlayerCtrl.resetCheckers();
        opponentPlayerCtrl.resetCheckers();

        started = true;
        UpdateMessageText();
    }

	void Update ()
    { }

    public void leaveGame ()
    {
        //started = false;
    }

    public void generateMoves(int checkerNum)
    {
        CheckerControl[] localCheckers = localPlayerCtrl.getCheckers();
        CheckerControl[] opponentCheckers = opponentPlayerCtrl.getCheckers();
        int[] checkerPos = localCheckers[checkerNum].getPosition();
        bool isKing = localCheckers[checkerNum].getIsKing();
        int[] moves = { 1, 1, 1, 1 };

        //check direct adjacency
        for (int i = 0; i < 12; i++)
        {
            if (localCheckers[i] != null)
            {
                int[] localCheckerPos = localCheckers[i].getPosition();

                if ((localCheckerPos[0] == checkerPos[0] - 1 && localCheckerPos[1] == checkerPos[1] + 1) || notInRange(checkerPos[0] - 1, checkerPos[1] + 1))
                { moves[0] = 0; }
                if ((localCheckerPos[0] == checkerPos[0] + 1 && localCheckerPos[1] == checkerPos[1] + 1) || notInRange(checkerPos[0] + 1, checkerPos[1] + 1))
                { moves[1] = 0; }
                if (!isKing || (localCheckerPos[0] == checkerPos[0] - 1 && localCheckerPos[1] == checkerPos[1] - 1) || notInRange(checkerPos[0] - 1, checkerPos[1] - 1))
                { moves[2] = 0; }
                if (!isKing || (localCheckerPos[0] == checkerPos[0] + 1 && localCheckerPos[1] == checkerPos[1] - 1) || notInRange(checkerPos[0] + 1, checkerPos[1] - 1))
                { moves[3] = 0; }
            }
            if (opponentCheckers[i] != null)
            {
                int[] oppCheckerPos = opponentCheckers[i].getPosition();

                if (moves[0] == 1 && oppCheckerPos[0] == checkerPos[0] - 1 && oppCheckerPos[1] == checkerPos[1] + 1)
                { moves[0] = 2; }
                if (moves[1] == 1 && oppCheckerPos[0] == checkerPos[0] + 1 && oppCheckerPos[1] == checkerPos[1] + 1)
                { moves[1] = 2; }
                if (moves[2] == 1 && isKing && oppCheckerPos[0] == checkerPos[0] - 1 && oppCheckerPos[1] == checkerPos[1] - 1)
                { moves[2] = 2; }
                if (moves[3] == 1 && isKing && oppCheckerPos[0] == checkerPos[0] + 1 && oppCheckerPos[1] == checkerPos[1] - 1)
                { moves[3] = 2; }
            }
        }

        //confirm can jump
        if (moves[0] == 2 && notInRange(checkerPos[0] - 2, checkerPos[1] + 2)) { moves[0] = 0; }
        if (moves[1] == 2 && notInRange(checkerPos[0] + 2, checkerPos[1] + 2)) { moves[1] = 0; }
        if (moves[2] == 2 && notInRange(checkerPos[0] - 2, checkerPos[1] - 2)) { moves[2] = 0; }
        if (moves[3] == 2 && notInRange(checkerPos[0] + 2, checkerPos[1] - 2)) { moves[3] = 0; }

        for (int i = 0; i < 12; i++)
        {
            if (localCheckers[i] != null)
            {
                int[] localCheckerPos = localCheckers[i].getPosition();

                if (moves[0] == 2 && (localCheckerPos[0] == checkerPos[0] - 2 && localCheckerPos[1] == checkerPos[1] + 2))
                { moves[0] = 0; }
                if (moves[1] == 2 && (localCheckerPos[0] == checkerPos[0] + 2 && localCheckerPos[1] == checkerPos[1] + 2))
                { moves[1] = 0; }
                if (moves[2] == 2 && (localCheckerPos[0] == checkerPos[0] - 2 && localCheckerPos[1] == checkerPos[1] - 2))
                { moves[2] = 0; }
                if (moves[3] == 2 && (localCheckerPos[0] == checkerPos[0] + 2 && localCheckerPos[1] == checkerPos[1] - 2))
                { moves[3] = 0; }
            }
            if (opponentCheckers[i] != null)
            {
                int[] oppCheckerPos = opponentCheckers[i].getPosition();

                if (moves[0] == 2 && (oppCheckerPos[0] == checkerPos[0] - 2 && oppCheckerPos[1] == checkerPos[1] + 2))
                { moves[0] = 0; }
                if (moves[1] == 2 && (oppCheckerPos[0] == checkerPos[0] + 2 && oppCheckerPos[1] == checkerPos[1] + 2))
                { moves[1] = 0; }
                if (moves[2] == 2 && (oppCheckerPos[0] == checkerPos[0] - 2 && oppCheckerPos[1] == checkerPos[1] - 2))
                { moves[2] = 0; }
                if (moves[3] == 2 && (oppCheckerPos[0] == checkerPos[0] + 2 && oppCheckerPos[1] == checkerPos[1] - 2))
                { moves[3] = 0; }
            }
        }

        localPlayerCtrl.updateMoves(moves, checkerNum);
    }

    private bool notInRange(int x, int y)
    { return (x < 0 || x > 7 || y < 0 || y > 7); }

    public bool isLocalTurn()
    { return localPlayerCtrl.getIsTurn(); }

    public void UpdateMessageText()
    {
        Debug.Log("aa");
        if (isLocalTurn()) { messageText.text = "Your Turn"; }
        else { messageText.text = "Opponent's Turn"; }
    }

    public void sendMove(int checker, int[] pos)
    { NetworkManager.sendMove(checker, pos); }
}