using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;

public class ServerStartUp : MonoBehaviour
{

	public Configuration configuration;
	// private List<ConnectedPlayer> _connectedPlayers;
	public NetworkManager networkManager;

	void Start()
	{
		if (configuration.buildType == BuildType.REMOTE_SERVER)
		{
			Debug.Log("StartServer??? ---->");
			networkManager.StartServer();
			// networkManager.StartHost();
			Debug.Log("StartServer DONE??? ---->");
		}
	}

	public void OnStartLocalServerButtonClick()
	{
		if (configuration.buildType == BuildType.LOCAL_SERVER)
		{
			networkManager.StartHost();
		} else {
			networkManager.StartServer();
		}
	}


}
