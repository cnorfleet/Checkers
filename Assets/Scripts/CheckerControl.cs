using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CheckerControl : MonoBehaviour
{
    [HideInInspector]
    public int checkerNum = -1;
    [SerializeField]
    private Transform myTransform;
    private int[] myPos;

    [SerializeField]
    private SpriteRenderer mySprite;
    [SerializeField]
    private Sprite[] sprites;

    private bool isRed = false;
    private bool isKing = false;
    private bool isSelected = false;
    private bool isLocal;
    
    public void setRed(bool red)
    { isRed = red; updateSprite(); }

    public void setKing(bool king)
    { isKing = king; updateSprite(); }

    public bool getIsKing()
    { return isKing; }

    public void setLocal(bool local)
    { isLocal = local; }

    public void setSelected(bool selected)
    { isSelected = selected; updateSprite(); }

    private void updateSprite()
    {
        int idx = 0;
        if(isRed) { idx += 1; }
        if(isKing) { idx += 2; }
        if (isSelected) { idx += 4; }
        mySprite.sprite = sprites[idx];
    }

    void Start()
    { updateSprite(); }

    public void moveTo(int x, int y)
    {
        int[] temp = { x, y }; myPos = temp;
        myTransform.position = new Vector3(x - 3.5f, y - 3.5f, -1);
        if ((isLocal && myPos[1] == 7) || (!isLocal && myPos[1] == 0))
        { isKing = true; }
    }

    public int[] getPosition()
    { return myPos; }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && isLocal)
        {
            SendMessageUpwards("checkerClicked", checkerNum, SendMessageOptions.RequireReceiver);
        }
    }
}