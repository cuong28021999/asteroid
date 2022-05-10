using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetwork : NetworkManager
{

    public static string SERVER_IP = "localhost";
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }
}
