using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;

[AddComponentMenu("Network/NetworkManagerHUD")]
[RequireComponent(typeof(NetworkManager))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class connect : NetworkBehaviour
{
    public NetworkManager manager;
    private NetworkClient myClient;
    [SerializeField]
    public bool showGUI = false;
    [SerializeField]
    public int offsetX;
    [SerializeField]
    public int offsetY;

    [SerializeField]
    public GameManager gm;

    bool clientSetup = false;
    bool showServer = false;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    void Update()
    {
        /*if (!showGUI)
            return;

        if (NetworkServer.active && NetworkClient.active)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                manager.StopHost();
            }
        }*/

        if (!NetworkServer.active && !NetworkClient.active && manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }
    }

    void OnGUI()
    {
        if (!showGUI)
            return;

        int xpos = 10 + offsetX;
        int ypos = 10 + offsetY;
        int spacing = 24;

        if (NetworkClient.active && !ClientScene.ready)
        {
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Client Ready"))
            {
                ClientScene.Ready(manager.client.connection);

                if (ClientScene.localPlayers.Count == 0)
                {
                    ClientScene.AddPlayer(0);
                }
            }
            ypos += spacing;
        }

        if (NetworkServer.active || NetworkClient.active)
        {
            if (!clientSetup)
            { myClient = manager.client; SetupClient(); clientSetup = true; }

            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Disconnect"))
            {
                manager.StopHost();
                gm.leaveGame();
            }
            ypos += spacing;
        }

        if (!NetworkServer.active && !NetworkClient.active)
        {
            clientSetup = false;
            if (manager.matchInfo == null)
            {
                if (manager.matches == null)
                {
                    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Create Match"))
                    {
                        manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
                        gm.begin(1);
                    }
                    ypos += spacing;

                    GUI.Label(new Rect(xpos, ypos, 100, 20), "Room Name:");
                    manager.matchName = GUI.TextField(new Rect(xpos + 100, ypos, 100, 20), manager.matchName);
                    ypos += spacing;

                    ypos += 10;

                    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Find Match"))
                    {
                        manager.matchMaker.ListMatches(0, 20, "", true, 0, 0, manager.OnMatchList);
                    }
                    ypos += spacing;
                }
                else
                {
                    foreach (var match in manager.matches)
                    {
                        if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Join Match:" + match.name))
                        {
                            manager.matchName = match.name;
                            manager.matchSize = (uint)match.currentSize;
                            manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
                            gm.begin(2);
                        }
                        ypos += spacing;
                    }

                    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Back"))
                    {
                        manager.matches = null;
                    }
                    ypos += spacing;
                }
            }

            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Change MM server"))
            {
                showServer = !showServer;
            }
            if (showServer)
            {
                ypos += spacing;
                if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Local"))
                {
                    manager.SetMatchHost("localhost", 1337, false);
                    showServer = false;
                }
                ypos += spacing;
                if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Internet"))
                {
                    manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
                    showServer = false;
                }
                ypos += spacing;
                if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Staging"))
                {
                    manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
                    showServer = false;
                }
            }

            ypos += spacing;

            try { GUI.Label(new Rect(xpos, ypos, 300, 20), "MM Uri: " + manager.matchMaker.baseUri); }
            catch { }
            ypos += spacing;
        }
    }

    public class MyMsgType
    {
        public static short Move = 99;
    };

    public class MessageUpdate : MessageBase
    {
        public int winCondition = 0;
        public int checkerMoved;
        public int[] movedTo = new int[2];
    }

    public void sendMove(int checker, int[] pos)
    {
        var msg = new MessageUpdate();
        msg.checkerMoved = checker;
        msg.winCondition = 0;
        msg.movedTo = pos;
        manager.client.Send(MyMsgType.Move, msg);
        Debug.Log("sent");
    }

    public void OnMove(NetworkMessage msg)
    {
        Debug.Log("msg");
    }

    public void SetupClient()
    {
        myClient.RegisterHandler(MyMsgType.Move, OnMove);
        myClient.Connect("127.0.0.1", 4444);
    }
}