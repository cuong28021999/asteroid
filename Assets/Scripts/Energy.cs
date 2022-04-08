using UnityEngine;

public class Energy : MonoBehaviour
{
    public float value = 60f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            FindObjectOfType<Player>().currentMana += value;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        FindObjectOfType<GameManager>().YellowDestroy(this);
        Destroy(this.gameObject, 0.2f);
    }
}
