using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class ClientStartUp : MonoBehaviour
{
	public Configuration configuration;
	public ServerStartUp serverStartUp;
	public NetworkManager networkManager;

	public void OnLoginUserButtonClick()
	{
		if (configuration.buildType == BuildType.REMOTE_CLIENT)
		{
			networkManager.networkAddress = configuration.ipAddress;
			// kcpTransport.port = configuration.port;
			Debug.Log("connect to server --->" + configuration.ipAddress);
			networkManager.StartClient();
		}
		else if (configuration.buildType == BuildType.LOCAL_CLIENT)
		{
			networkManager.StartClient();
		}
	}
}