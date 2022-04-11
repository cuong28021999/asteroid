using System;

[Serializable]
public class Score
{
    public string playerName;
    public int score;    

    public Score(string n, int s)
    {
        this.playerName = n;
        this.score = s;
    }
}
