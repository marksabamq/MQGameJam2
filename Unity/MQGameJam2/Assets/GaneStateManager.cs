using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { NONE, CUTSCENE, KEYBOARD, TOWER, MONITOR };
[System.Serializable]
public struct GameStateCamera
{
    public GameState state;
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
}

public class GaneStateManager : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float cameraMoveSpeed = 3;
    [SerializeField] private float cameraRotateSpeed = 3;
    [SerializeField] private GameStateCamera[] cameraStates;
    
    private GameState gameState;
    private GameStateCamera currentCameraState;

    private void Start()
    {
        SetGameState(GameState.KEYBOARD);
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = Vector3.MoveTowards(cam.transform.position, currentCameraState.cameraPosition, cameraMoveSpeed);
        cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, Quaternion.Euler(currentCameraState.cameraRotation), cameraRotateSpeed);
    }

    void SetGameState(GameState newState)
    {
        gameState = newState;

        for (int i = 0; i < cameraStates.Length; i++)
        {

            if (cameraStates[i].state == newState)
            {
                currentCameraState = cameraStates[i];
            }
        }
    }
}
