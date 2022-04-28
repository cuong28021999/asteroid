using UnityEngine;
    
[CreateAssetMenu]
public class CharacterDatabase : ScriptableObject
{
    public Character[] characters;

    public int CharacterAmount
    {
        get
        {
            return characters.Length;
        }
    }

    public Character GetCharacter(int i)
    {
        return characters[i];
    }
}
