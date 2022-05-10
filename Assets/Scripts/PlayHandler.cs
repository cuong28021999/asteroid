using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayHandler : MonoBehaviour
{
    public CharacterManager characterManager;
    [SerializeField]
    private GameNetwork gameNetwork = null;
    [SerializeField]
    private GameObject lobbyUI;

    public void OnPlayGame()
    {
        // set name player
        characterManager.SetPlayerName(characterManager.nameInput.text);
        characterManager.SavePlayerName();

        gameNetwork.networkAddress = GameNetwork.SERVER_IP;
        gameNetwork.StartClient();

        lobbyUI.SetActive(false);
    }

    public void OnHostServer()
    {
        // set name player
        characterManager.SetPlayerName(characterManager.nameInput.text);
        characterManager.SavePlayerName();
        gameNetwork.StartHost();

        lobbyUI.SetActive(false);
    }
}
