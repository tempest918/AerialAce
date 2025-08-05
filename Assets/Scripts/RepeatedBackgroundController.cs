using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatedBackgroundController : MonoBehaviour
{
    public GameObject camera;
    public float BgHeight;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject CurrentChild = transform.GetChild(i).gameObject;
            if (camera.transform.position.y - CurrentChild.transform.position.y > BgHeight)
            {
                CurrentChild.transform.position = new Vector3(0,CurrentChild.transform.position.y + transform.childCount * BgHeight, CurrentChild.transform.position.z);
            }
        }
    }
}