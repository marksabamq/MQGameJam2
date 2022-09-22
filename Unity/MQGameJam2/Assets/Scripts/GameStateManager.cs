using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public enum GameState { NONE, TRANSITIONING, CUTSCENE, KEYBOARD, TOWER, MONITOR, GAMEOVER };
[System.Serializable]
public struct GameStateCamera
{
    public GameState state;
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;

    public UnityEvent transitionEvent;
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    [SerializeField] private Camera cam;

    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI gameoverStats;

    [SerializeField] private float cameraMoveSpeed = 3;
    [SerializeField] private float cameraRotateSpeed = 3;
    [SerializeField] private float transitionDelay = 1;
    [SerializeField] private GameStateCamera[] cameraStates;

    public UnityEvent resetEvent;

    private GameState gameState;
    private GameStateCamera currentCameraState;

    private GameState settingState;

    private float transitionTime = 0f;

    private float keyboardAcc;
    private float keyboardTime;

    private float towerAcc;
    private float towerTime;

    private float monitorAcc;
    private float monitorTime;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        objectiveText.text = "RIP COMPUTER ;(";
        SetGameState(GameState.CUTSCENE);
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.TRANSITIONING:
                if (Time.time - transitionTime >= transitionDelay)
                {
                    cam.transform.position = Vector3.MoveTowards(cam.transform.position, currentCameraState.cameraPosition, cameraMoveSpeed);
                    if (Vector3.Distance(cam.transform.position, currentCameraState.cameraPosition) < 0.1f)
                    {
                        TransitionComplete();
                    }
                    cam.transform.rotation = Quaternion.RotateTowards(cam.transform.rotation, Quaternion.Euler(currentCameraState.cameraRotation), cameraRotateSpeed);
                }
                break;

            case GameState.GAMEOVER:
                gameoverStats.text = $"KEYBOARD: \n- Accuracy: {(int)keyboardAcc}%\n- Time: {keyboardTime.ToString("0.00")} seconds\n\nTOWER:\n- Accuracy: {(int)towerAcc}%\n- Time {towerTime.ToString("0.00")} seconds\n\nMONITOR:\n- Accuracy: {(int)monitorAcc}%\n- Time: {monitorTime.ToString("0.00")} seconds";
                break;
        }
    }

    void TransitionComplete(bool setTransform = true)
    {
        if (setTransform)
        {
            cam.transform.position = currentCameraState.cameraPosition;
            cam.transform.rotation = Quaternion.Euler(currentCameraState.cameraRotation);
        }

        gameState = settingState;

        switch (gameState)
        {
            case GameState.NONE:
                objectiveText.text = "RIP COMPUTER ;(";
                break; 
            case GameState.KEYBOARD:
                objectiveText.text = "REPAIR YOUR KEYBOARD";
                KeyboardManager.instance.SetState(KeyboardState.BROKEN);
                break;

            case GameState.TOWER:
                objectiveText.text = "REWIRE YOUR MOTHERBOARD";
                CPUConnectionManager.instance.Activate();
                break;
            case GameState.MONITOR:
                objectiveText.text = "ENTER YOUR PASSWORD";
                QTEManager.instance.SetState(QTEState.GENERATING);
                break;
        }
    }

    public void SetGameState(GameState newState)
    {
        //gameState = newState;
        objectiveText.text = gameState != GameState.CUTSCENE ? "COMPLETE" : "";
        gameState = GameState.TRANSITIONING;
        settingState = newState;

        transitionTime = Time.time;

        bool found = false;
        for (int i = 0; i < cameraStates.Length; i++)
        {
            if (cameraStates[i].state == newState)
            {
                found = true;
                currentCameraState = cameraStates[i];
                currentCameraState.transitionEvent.Invoke();
            }
        }

        if (!found)
        {
            TransitionComplete(false);
        }
    }

    public void KeyboardStats(float acc, float time)
    {
        keyboardAcc = acc;
        keyboardTime = time;
    }
    public void TowerStats(float acc, float time)
    {
        towerAcc = acc;
        towerTime = time;
    }

    public void MonitorStats(float acc, float time)
    {
        monitorAcc = acc;
        monitorTime = time;
    }

    public void StartGame()
    {
        SetGameState(GameState.KEYBOARD);
    }

    public void ResetGame()
    {
        resetEvent.Invoke();
        SetGameState(GameState.CUTSCENE);

        KeyboardManager.instance.ResetKeyboard();
        CPUConnectionManager.instance.ResetConnections();
        QTEManager.instance.QTEReset();
    }
}
