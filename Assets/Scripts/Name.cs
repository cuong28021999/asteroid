using UnityEngine;
using UnityEngine.UI;

public class Name : MonoBehaviour
{

    public Text playerName;

    private void Start()
    {
        // get name
        if (!PlayerPrefs.HasKey("playerName"))
        {
            playerName.text = "";
        } else
        {
            playerName.text = PlayerPrefs.GetString("playerName");
        }
    }

    private void Update()
    {
        this.transform.rotation = Quaternion.Euler(0f,0f,0f);
    }
}
