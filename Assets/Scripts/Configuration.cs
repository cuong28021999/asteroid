using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration : MonoBehaviour
{
	public BuildType buildType;
	public string ipAddress = "";
	public ushort port = 0;
}

public enum BuildType
{
	LOCAL_CLIENT,
	REMOTE_CLIENT,
	LOCAL_SERVER,
	REMOTE_SERVER
}