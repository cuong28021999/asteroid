using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cracked : MonoBehaviour
{
    public Sprite[] sprites;

    public Asteroid asteroid;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        sr.sprite = sprites[asteroid.countingShoot];
    }
}
