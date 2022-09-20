using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct KeySocket
{
    public string key;
    public Transform keyObject;
    public Transform keySocket;
}

public class KeyboardManager : MonoBehaviour
{
    [SerializeField] private KeySocket[] keys;
    [SerializeField] private Transform[] keyExplosionPositions;

    [SerializeField] private int minKeysExplode = 5;
    [SerializeField] private int maxKeysExplode = 10;

    [SerializeField] private float explodeSpeed = 2;

    [SerializeField] private bool explode = false;

    private List<KeySocket> explodingKeys = new List<KeySocket>();
    private List<Transform> explodePositions = new List<Transform>();
    private List<float> tVals = new List<float>();

    private KeyboardState keyboardState = KeyboardState.NONE;
    enum KeyboardState { NONE, EXPLODING, BROKEN, FIXED };

    // Start is called before the first frame update
    void Start()
    {
        maxKeysExplode = maxKeysExplode > keyExplosionPositions.Length ? keyExplosionPositions.Length : maxKeysExplode;
    }

    // Update is called once per frame
    void Update()
    {
        if (explode)
        {
            explode = false;
            keyboardState = KeyboardState.EXPLODING;

            //pick keys
            int keyAmountToExplode = Random.Range(minKeysExplode, maxKeysExplode);
            explodingKeys.Clear();
            explodePositions.Clear();
            tVals.Clear();

            while (explodingKeys.Count < keyAmountToExplode)
            {
                int rndIndex = Random.Range(0, keys.Length);
                if (!explodingKeys.Contains(keys[rndIndex]))
                {
                    explodingKeys.Add(keys[rndIndex]);
                    tVals.Add(0);
                }
            }
            while (explodePositions.Count < keyAmountToExplode)
            {
                int rndIndex = Random.Range(0, keyExplosionPositions.Length);
                if (!explodePositions.Contains(keyExplosionPositions[rndIndex]))
                {
                    explodePositions.Add(keyExplosionPositions[rndIndex]);
                }
            }


            for (int i = 0; i < explodingKeys.Count; i++)
            {
                explodingKeys[i].keyObject.position = explodePositions[i].position;
            }
        }

        switch (keyboardState)
        {
            case KeyboardState.EXPLODING:
                bool allComplete = true;
                for (int i = 0; i < explodingKeys.Count; i++)
                {
                    explodingKeys[i].keyObject.position = Vector3.Lerp(explodingKeys[i].keySocket.position, explodePositions[i].position, tVals[i]);
                    explodingKeys[i].keyObject.rotation = Quaternion.Euler(tVals[i] * 270, (1 - tVals[i]) * 360, (tVals[i]) * 360);
                    tVals[i] += Time.deltaTime * explodeSpeed;

                    if (tVals[i] < 1)
                    {
                        allComplete = false;
                    }
                    // explodingKeys[i].keyObject.position = Vector3.MoveTowards(explodingKeys[i].keyObject.position, explodePositions[i].position, explodeSpeed);
                }

                if (allComplete)
                {
                    keyboardState = KeyboardState.BROKEN;
                }

                break;
        }
    }
}
