using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    private float length, startposX, startposY, height;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startposX = transform.position.x;
        startposY = transform.position.y;
       /* length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float distX = (cam.transform.position.x * parallaxEffect);
        float distY = (cam.transform.position.y * parallaxEffect);

        transform.position = new Vector3(startposX + distX, startposY + distY, transform.position.z);

        // infinity loop background
        /*float tempX = (cam.transform.position.x * (1 - parallaxEffect));
        float tempY = (cam.transform.position.y * (1 - parallaxEffect));*/

        /*if (tempX > startposX + length) startposX += length;
        else if (tempX < startposX - length) startposX -= length;

        if (tempY > startposY + height) startposY += height;
        else if (tempY < startposY - height) startposY -= height;*/
    }
}
