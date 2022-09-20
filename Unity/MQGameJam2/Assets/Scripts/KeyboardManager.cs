using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyboardState { NONE, EXPLODE, EXPLODING, BROKEN, FIXED };
[System.Serializable]
class KeySocket
{
    public string key;
    public Transform keyObject;
    public Transform keySocket;
}

public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager instance;

    [SerializeField] private Camera cam;

    [SerializeField] private KeySocket[] keys;
    [SerializeField] private Transform[] keyExplosionPositions;

    [SerializeField] private int minKeysExplode = 5;
    [SerializeField] private int maxKeysExplode = 10;

    [SerializeField] private float explodeSpeed = 2;

    [SerializeField] private bool explode = false;

    [SerializeField] private float keySnapPos = 0.25f;

    private List<KeySocket> explodingKeys = new List<KeySocket>();
    private List<Transform> explodePositions = new List<Transform>();
    private List<float> tVals = new List<float>();

    [SerializeField] private LayerMask keyMask;

    private KeyboardState keyboardState = KeyboardState.NONE;

    private Transform draggingObject;
    private int brokenKeys;
    private int correctPlacements;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        maxKeysExplode = maxKeysExplode > keyExplosionPositions.Length ? keyExplosionPositions.Length : maxKeysExplode;
    }

    // Update is called once per frame
    void Update()
    {
        switch (keyboardState)
        {
            case KeyboardState.EXPLODE:
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

                brokenKeys = explodingKeys.Count;
                keyboardState = KeyboardState.EXPLODING;

                break;

            case KeyboardState.EXPLODING:
                bool allComplete = true;
                for (int i = 0; i < explodingKeys.Count; i++)
                {
                    explodingKeys[i].keyObject.position = Vector3.Lerp(explodingKeys[i].keySocket.position, explodePositions[i].position, tVals[i]);
                    explodingKeys[i].keyObject.rotation = Quaternion.Euler(tVals[i] * 270, (1 - tVals[i]) * 360, (tVals[i]) * Random.Range(-90, 90));

                    if (tVals[i] < 1)
                    {
                        allComplete = false;
                    }
                    else
                    {
                        tVals[i] = 1;
                        explodingKeys[i].keyObject.position = explodePositions[i].position;
                        explodingKeys[i].keyObject.rotation = Quaternion.Euler(270, 0, Random.Range(-90, 90));
                    }
                    tVals[i] += Time.deltaTime * explodeSpeed;
                    // explodingKeys[i].keyObject.position = Vector3.MoveTowards(explodingKeys[i].keyObject.position, explodePositions[i].position, explodeSpeed);
                }

                if (allComplete)
                {
                    SetState(KeyboardState.BROKEN);
                }

                break;

            case KeyboardState.BROKEN:
                //allow dragging pieces 
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log(hit.transform.name);
                        if (hit.transform.gameObject.layer == 6)
                        {
                            draggingObject = hit.transform;
                        }
                    }
                }

                if (draggingObject != null)
                {
                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 100, keyMask))
                    {
                        Debug.Log(hit.transform.name);
                        Vector3 movePos = hit.point - (ray.direction * 0.1f);
                        draggingObject.position = movePos;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (draggingObject != null)
                    {

                        //see if dragging object is close to slot
                        KeySocket key = null;
                        Transform socket = null;

                        for (int i = 0; i < keys.Length; i++)
                        {
                            Vector3 transformed = keys[i].keySocket.InverseTransformPoint(draggingObject.position);
                            //Debug.Log(transformed + " " + keys[i].key);

                            float offset = Mathf.Abs(transformed.x) + Mathf.Abs(transformed.z);
                            if (offset < keySnapPos)
                            {
                                socket = keys[i].keySocket;
                                draggingObject.transform.position = keys[i].keySocket.position;
                            }

                            if (keys[i].keyObject == draggingObject)
                            {
                                key = keys[i];
                            }
                        }

                        if (key != null)
                        {
                            Debug.Log("Set in: " + key.key);

                            int explodeIndex = explodingKeys.IndexOf(key);
                            if (explodeIndex >= 0)
                            {
                                draggingObject.position = explodePositions[explodeIndex].position;
                            }

                            if (socket != null)
                            {
                                explodingKeys.Remove(key);
                                key.keyObject.transform.position = socket.position;
                                key.keyObject.transform.eulerAngles = new Vector3(-90, 0, 0);

                                if (socket == key.keySocket)
                                {
                                    //correct
                                    correctPlacements++;
                                }
                                if (explodingKeys.Count == 0)
                                {
                                    Debug.Log("Correct: " + (correctPlacements / (float)brokenKeys * 100) + "%");
                                    SetState(KeyboardState.FIXED);
                                }
                            }
                        }

                        draggingObject = null;
                    }
                }
                break;
        }
    }

    public void SetState(KeyboardState newState)
    {
        keyboardState = newState;
    }
}
