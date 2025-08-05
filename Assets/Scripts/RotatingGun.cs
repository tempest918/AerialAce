using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingGun : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float oscillationAngle = 15f;
    public bool invertRotation = false;
    private float startingAngle;
    public float randomSpeedVariation = 0.1f; 

    void Start()
    {
        startingAngle = transform.eulerAngles.z;
        
        float randomAdjustment = Random.Range(-randomSpeedVariation, randomSpeedVariation);
        rotationSpeed += randomAdjustment;

    }

    void Update()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        float angle = startingAngle + Mathf.Sin(Time.time * rotationSpeed) * oscillationAngle;

        if (invertRotation)
        {
            angle = -angle;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
