using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private float magnitude = 0.5f;
    private float shakeDuration;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 orignalPosition = transform.position;

        if (shakeDuration > 0)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position += new Vector3(x, y);
            shakeDuration -= Time.deltaTime;

            if (shakeDuration <= 0)
            {
                transform.position = orignalPosition;
            }
        }
    }

    public void AddShake(float duration, float newMagnitude = 0.5f)
    {
        shakeDuration += duration;
        magnitude = newMagnitude;
    }
}
