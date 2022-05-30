using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameNetwork : NetworkManager
{

    public float spawnDistance = 5f;
    public UIHandler uIHandler;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("----> OnStartServer");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        uIHandler.OnStartClient();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        uIHandler.OnClientConnected();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        uIHandler.OnClientDisconnect();
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
    // add player at correct spawn position
        Vector3 spawnPoint = GetRandomSpawnPoint();
        Debug.Log(spawnPoint);
        GameObject player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        // base.OnServerAddPlayer(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }

    public Vector3 GetRandomSpawnPoint() {
        // random a point direction in a circle
        Vector3 spawnDirection = Random.insideUnitCircle.normalized * this.spawnDistance;
        Vector3 spawnPoint = this.transform.position + spawnDirection; // offset point
        return spawnPoint;
    }
}
