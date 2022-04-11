using System.Linq;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public RowUI rowUi;
    public ScoreManager scoreManager;

    private void Start()
    {
        scoreManager.AddScore(new Score("Alexxxxx", 6000));
        scoreManager.AddScore(new Score("Johnnnnnn", 10000));
        scoreManager.AddScore(new Score("Buppyyyyy", 999999));

        var scores = scoreManager.GetHighScores().ToArray();
        for (int i = 0; i < scores.Length; i++)
        {
            // top 10
            if (i < 10)
            {
                var row = Instantiate(rowUi, transform).GetComponent<RowUI>();
                row.rank.text = "#" + (i + 1).ToString();
                row.playerName.text = scores[i].playerName;
                row.score.text = scores[i].score.ToString();
            }
        }
    }
}
