using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;

    public SpriteRenderer artworkSprite;

    public int selectedOption = 0;
    
    public InputField nameInput;

    public string playerName;


    private void Start()
    {
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        } else
        {
            Load();
        }

        UpdateCharacter(selectedOption);
    }

    public void SetPlayerName(string n)
    {
        playerName = n;
    }

    public void NextOption()
    {
        selectedOption++;

        if (selectedOption > characterDB.CharacterAmount - 1)
        {
            selectedOption = 0;
        }
        

        UpdateCharacter(selectedOption);
        Save();
    }

    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = characterDB.CharacterAmount - 1;
        } 

        UpdateCharacter(selectedOption);
        Save();
    }

    private void UpdateCharacter(int i)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Character character = characterDB.GetCharacter(i);
        artworkSprite = Instantiate(character.characterSprite, transform);
        artworkSprite.transform.parent = gameObject.transform;
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
        playerName = PlayerPrefs.GetString("playerName");
        nameInput.text = PlayerPrefs.GetString("playerName");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("selectedOption", selectedOption);
    }

    public void SavePlayerName()
    {
        PlayerPrefs.SetString("playerName", playerName);
    }

    public void ChangeScene(int sceneID)
    {
        SavePlayerName();
        SceneManager.LoadScene(sceneID);
    }
}
