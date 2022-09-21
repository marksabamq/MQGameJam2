using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { NONE, TRANSITIONING, CUTSCENE, KEYBOARD, TOWER, MONITOR };
[System.Serializable]
public struct GameStateCamera
{
    public GameState state;
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    [SerializeField] private Camera cam;

    [SerializeField] private float cameraMoveSpeed = 3;
    [SerializeField] private float cameraRotateSpeed = 3;
    [SerializeField] private float transitionDelay = 1;
    [SerializeField] private GameStateCamera[] cameraStates;

    private GameState gameState;
    private GameStateCamera currentCameraState;

    private GameState settingState;

    private float transitionTime = 0f;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        SetGameState(GameState.KEYBOARD);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.TRANSITIONING)
        {
            if (Time.time - transitionTime >= transitionDelay)
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, currentCameraState.cameraPosition, cameraMoveSpeed);
                if (Vector3.Distance(cam.transform.position, currentCameraState.cameraPosition) < 0.1f)
                {
                    TransitionComplete();
                }
                cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, Quaternion.Euler(currentCameraState.cameraRotation), cameraRotateSpeed);
            }
        }
    }

    void TransitionComplete()
    {
        cam.transform.position = currentCameraState.cameraPosition;
        cam.transform.rotation = Quaternion.Euler(currentCameraState.cameraRotation);

        gameState = settingState;

        switch (gameState)
        {
            case GameState.KEYBOARD:
                KeyboardManager.instance.SetState(KeyboardState.EXPLODE);
                break;
            case GameState.MONITOR:
                QTEManager.instance.SetState(QTEState.GENERATING);
                break;
        }
    }

    public void SetGameState(GameState newState)
    {
        //gameState = newState;
        gameState = GameState.TRANSITIONING;
        settingState = newState;

        transitionTime = Time.time;

        for (int i = 0; i < cameraStates.Length; i++)
        {
            if (cameraStates[i].state == newState)
            {
                currentCameraState = cameraStates[i];
            }
        }
    }
}
