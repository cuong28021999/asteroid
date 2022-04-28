using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour
{
    private int score;

    public Text scoreUI;

    private void Start() {
        if (PlayerPrefs.HasKey("score")) {
            Load();
            scoreUI.text = score.ToString();
        }
    }

    private void Load() {
        score = PlayerPrefs.GetInt("score");
    }

    public void OnAgain() {
        // set score to 0
        PlayerPrefs.DeleteKey("score");
        score = 0;

        // change to gameplay scene
        int sceneId = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneId - 1);
    }

    public void OnBackMenu() {
         // change to menu scene
        SceneManager.LoadScene(0);
    }
}
