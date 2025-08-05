using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public float verticalSpeed = 1.0f;

    void Update()
    {
        transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
    }
}
