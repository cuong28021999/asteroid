using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Configuration configuration;
    public CharacterManager characterManager;
    [SerializeField]
    private GameObject lobbyUI;
    public GameObject connectUI;
    public GameObject disconnectUI;
    public RectTransform rectTransformCharacter;
    public ClientStartUp clientStartUp;
    public ServerStartUp serverStartUp;

    public void Awake() {
        if(configuration.buildType == BuildType.REMOTE_SERVER) {
            lobbyUI.SetActive(false);
        } else {
            lobbyUI.SetActive(true);
            connectUI.SetActive(false);
            disconnectUI.SetActive(false);
        }
    }
    public void OnPlayGame()
    {
        // set name player
        characterManager.SetPlayerName(characterManager.nameInput.text);
        characterManager.SavePlayerName();

        clientStartUp.OnLoginUserButtonClick();
    }

    public void OnStartClient() {
        lobbyUI.SetActive(false);
        connectUI.SetActive(true);
        disconnectUI.SetActive(false);
        characterManager.artworkSprite.gameObject.SetActive(false);
    }

    public void OnClientConnected() {
        lobbyUI.SetActive(false);
        connectUI.SetActive(false);
        disconnectUI.SetActive(false);
        characterManager.artworkSprite.gameObject.SetActive(false);
    }

    public void OnClientDisconnect() {
        lobbyUI.SetActive(true);
        connectUI.SetActive(false);
        disconnectUI.SetActive(true);
        characterManager.artworkSprite.gameObject.SetActive(true);

        Invoke("HideDisconect", 2);
    }

    public void Update() {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(rectTransformCharacter.transform.position);
        worldPoint.z = 0;
        characterManager.gameObject.transform.position = worldPoint;
        characterManager.gameObject.transform.localScale = rectTransformCharacter.transform.localScale * 0.3f;
    }

    void HideDisconect() {
        disconnectUI.SetActive(false);
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
