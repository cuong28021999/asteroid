using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{
    public GameManager gameManager;
    public Text playerScore;

    private void Update()
    {
        playerScore.text = "Score: " + gameManager.score.ToString();
    }
}
