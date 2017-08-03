using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    private bool localIsRed = false;
    private int myCheckerNum;
    private int[] myPos;
    [SerializeField]
    private Transform myTransform;

    void Start ()
    {
		
	}

    public void setPosition(int x, int y)
    {
        int[] temp = { x, y }; myPos = temp;
        myTransform.position = new Vector3(x - 3.5f, y - 3.5f, -2);
    }

    public int[] getPosition()
    { return myPos; }

    public void setCheckerNum(int num)
    { myCheckerNum = num; }

    public int getCheckerNum()
    { return myCheckerNum;  }

    public void setLocalIsRed(bool isRed)
    { localIsRed = isRed; }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int[] message = { myCheckerNum, myPos[0], myPos[1] };
            SendMessageUpwards("moveClicked", message, SendMessageOptions.RequireReceiver);
        }
    }
}