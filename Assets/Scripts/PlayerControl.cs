using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private GameObject checkerPrefab;
    private GameObject[] myCheckerObjs = new GameObject[12];
    private CheckerControl[] myCheckers = new CheckerControl[12];
    [SerializeField]
    private GameObject movePrefab;
    private GameObject[] myMoveObjs = new GameObject[4];
    private MoveControl[] myMoves = new MoveControl[4];

    private bool amRed = false;
    private bool isLocal = true;
    private GameManager gm;
    private int selectedChecker = -1;
    private bool myTurn = false;

    public void setRed(bool red)
    { amRed = red; }

    public void setLocal(bool local)
    { isLocal = local; }

    public void setGM(GameManager g)
    { gm = g; }

    void Awake()
    { }

    void Update()
    { myTurn = true; }

	public void resetCheckers()
    {
        myTurn = false;
        for (int i = 0; i < 12; i++)
        {
            if (myCheckers[i] == null)
            {
                myCheckerObjs[i] = Instantiate(checkerPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
                myCheckerObjs[i].transform.parent = transform;
                myCheckers[i] = myCheckerObjs[i].GetComponent<CheckerControl>();
                myCheckers[i].checkerNum = i;
            }
        }
        for (int idx = 0; idx < 12; idx++)
        {
            CheckerControl checker = myCheckers[idx];
            checker.setLocal(isLocal);
            checker.setRed(amRed);
            checker.setKing(false);
            checker.setSelected(false);
            
            int x = idx * 2;
            int y = x / 8;
            x %= 8; if (y == 1) { x++; }
            if (isLocal) { checker.moveTo(x, y); }
            else { checker.moveTo(7 - x, 7 - y); }
        }
        clearMoves();
        if(!amRed) { myTurn = true; } ///make red go first :) ////////////////////////////////////////////////////
    }

    public void checkerClicked(int checkerNum)
    {
        if(!myTurn || !isLocal) { return; }

        if (selectedChecker == checkerNum) { selectedChecker = -1; }
        else { selectedChecker = checkerNum; }

        for (int idx = 0; idx < 12; idx++)
        {
            if (idx == selectedChecker) { myCheckers[idx].setSelected(true); }
            else { myCheckers[idx].setSelected(false); }
        }

        clearMoves();

        if (selectedChecker == -1)
        { return; }

        gm.generateMoves(selectedChecker);
    }

    private void clearMoves()
    {
        for (int i = 0; i < 4; i++)
        {
            if (myMoves[i] != null)
            {
                Destroy(myMoveObjs[i]);
                myMoves[i] = null;
            }
        }
    }

    public void updateMoves(int[] locs, int checkerNum)
    {
        int[] checkerPos = myCheckers[checkerNum].getPosition();

        int[][] directions = new int[4][];
        for (int i = 0; i < 4; i++) { directions[i] = new int[2]; }
        directions[0][0] = -1; directions[0][1] =  1;
        directions[1][0] =  1; directions[1][1] =  1;
        directions[2][0] = -1; directions[2][1] = -1;
        directions[3][0] =  1; directions[3][1] = -1;

        for (int i = 0; i < 4; i++)
        {
            if (locs[i] == 0) { continue; }
            myMoveObjs[i] = Instantiate(movePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            myMoveObjs[i].transform.parent = transform;
            myMoves[i] = myMoveObjs[i].GetComponent<MoveControl>();
            myMoves[i].setLocalIsRed(amRed);
            myMoves[i].setCheckerNum(checkerNum);
            myMoves[i].setPosition(checkerPos[0] + directions[i][0] * locs[i], checkerPos[1] + directions[i][1] * locs[i]);
        }
    }

    public CheckerControl[] getCheckers()
    { return myCheckers; }

    public bool getIsRed()
    { return amRed; }

    public void moveClicked(int[] message)
    {
        if(!myTurn || !isLocal) { return; }

        Debug.Log("move, " + message[0] + ", " + message[1] + ", " + message[2]);
        myCheckers[selectedChecker].setSelected(false);
        selectedChecker = -1;
        clearMoves();
        myTurn = false;
        SendMessageUpwards("UpdateMessageText");
        int checker = message[0];
        int[] pos = { message[1], message[2] };
        myCheckers[checker].moveTo(pos[0], pos[1]);
        gm.sendMove(checker, pos);
    }

    public bool getIsTurn()
    { return myTurn; }

    public void UpdateMessageText()
    { gm.UpdateMessageText(); }
}