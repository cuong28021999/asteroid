using UnityEngine;

public class Energy : MonoBehaviour
{
    public float value = 60f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = FindObjectOfType<Player>();
            if (player.currentMana + value <= player.maxMana)
            {
                player.currentMana += value;
            } else
            {
                player.currentMana = player.maxMana;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        FindObjectOfType<GameManager>().YellowDestroy(this);
        Destroy(this.gameObject, 0.2f);
    }
}
