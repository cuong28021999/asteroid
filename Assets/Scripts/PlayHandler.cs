using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayHandler : MonoBehaviour
{
    public CharacterManager characterManager;
    [SerializeField]
    private GameNetwork gameNetwork = null;

    public void OnPlayGame()
    {
        // set name player
        characterManager.SetPlayerName(characterManager.nameInput.text);
        characterManager.SavePlayerName();

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        characterManager.ChangeScene(currentSceneIndex + 1);
    }

    public void OnHostServer()
    {
        gameNetwork.StartHost();
    }
}
