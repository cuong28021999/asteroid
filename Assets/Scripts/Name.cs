using UnityEngine;
using UnityEngine.UI;

public class Name : MonoBehaviour
{

    public Text playerName;
    public GameManager gameManager;

    private void Start()
    {
        playerName.text = gameManager.playerName;
    }

    private void Update()
    {
        this.transform.rotation = Quaternion.Euler(0f,0f,0f);
    }
}
