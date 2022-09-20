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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
