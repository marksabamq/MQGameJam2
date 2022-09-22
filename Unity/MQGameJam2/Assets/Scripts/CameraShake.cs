using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private float magnitude = 0.5f;
    private float shakeDuration;

    private Vector3 preshakePos;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        preshakePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeDuration > 0)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = preshakePos + new Vector3(x, y);
            shakeDuration -= Time.deltaTime;

            if (shakeDuration <= 0)
            {
                transform.position = preshakePos;
            }
        }
    }

    public void AddShake(float duration)
    {
        if (shakeDuration <= 0)
        {
            preshakePos = transform.position;
        }
        shakeDuration += duration;

    }
}
