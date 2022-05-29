using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayHandler : MonoBehaviour
{
    public CharacterManager characterManager;
    [SerializeField]
    private GameObject lobbyUI;
    public ClientStartUp clientStartUp;
    public ServerStartUp serverStartUp;

    public void OnPlayGame()
    {
        // set name player
        characterManager.SetPlayerName(characterManager.nameInput.text);
        characterManager.SavePlayerName();

        clientStartUp.OnLoginUserButtonClick();

        lobbyUI.SetActive(false);
        characterManager.artworkSprite.gameObject.SetActive(false);
    }

    public void OnHostServer()
    {
        // set name player
        characterManager.SetPlayerName(characterManager.nameInput.text);
        characterManager.SavePlayerName();

        serverStartUp.OnStartLocalServerButtonClick();
        // gameNetwork.StartHost();

        lobbyUI.SetActive(false);
        characterManager.artworkSprite.gameObject.SetActive(false);
    }
}
