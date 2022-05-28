using UnityEngine;
using Mirror;

public class OnlinePlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName = "";
    
    public Character character; 

    [SyncVar]
    public int selectedOption = 0;
    public int score = 0;

    public override void OnStartAuthority()
    {
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        CmdSetPlayerNameAndOption(PlayerPrefs.GetString("playerName"), selectedOption);
    }

    [Command]
    public void CmdSetPlayerNameAndOption(string name, int option)
    {
        playerName = name;
        selectedOption = option;
        // ClientRpcSetPlayerInfo(strPlayerName, selectedOption);

        //spawn new player

    }

    void OnPlayerNameChanged(string newValue, string oldValue) {

    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("score", FindObjectOfType<GameManager>().score);
    }
}
