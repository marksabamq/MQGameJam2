using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum QTEState { NONE, GENERATING, RUNNING, WRONG, COMPLETE};

public class QTEManager : MonoBehaviour
{
    public static QTEManager instance;

    [SerializeField] private TextMeshProUGUI letterText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private float timePerLetter = 1f;
    [SerializeField] private TextAsset textContent;


    [SerializeField] private GameObject minigameScreen;
    [SerializeField] private GameObject completedScreen;

    private QTEState currentState = QTEState.NONE;

    private string[] possibleTexts;

    private string currText;
    private int currIndex;
    private float currentTime;

    private int correct;

    private float wrongTimer;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        possibleTexts = textContent.text.Split("\n");
        minigameScreen.SetActive(false);
        completedScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case QTEState.GENERATING:
                minigameScreen.SetActive(true);
                currText = possibleTexts[Random.Range(0, possibleTexts.Length)];
                timerSlider.value = 1;
                currentTime = timePerLetter;
                currIndex = 0;
                letterText.text = currText[currIndex].ToString().ToUpper();

                timer = Time.time;
                SetState(QTEState.RUNNING);
                break;

            case QTEState.RUNNING:
                currentTime -= Time.deltaTime;

                string inStr = Input.inputString;
                if (inStr.Length > 0 && inStr[0] == currText[currIndex])
                {
                    correct++;

                    currIndex++;

                    if (currIndex >= currText.Length - 1)
                    {

                        Debug.Log("DONE");
                        Completed();
                        break;
                    }

                    letterText.text = currText[currIndex].ToString().ToUpper();
                    currentTime = timePerLetter;
                }

                if (currentTime <= 0)
                {
                    //skip letter
                    wrongTimer = 0;
                    letterText.color = Color.red;

                    correct--;
                    correct = correct < 0 ? 0 : correct;

                    SetState(QTEState.WRONG);
                }


                timerSlider.value = currentTime / timePerLetter;
                break;
            case QTEState.WRONG:
                wrongTimer += Time.deltaTime;

                if(wrongTimer > 1)
                {
                    currIndex++;
                    if(currIndex >= currText.Length) {
                        Completed();
                        break;
                    }
                    letterText.text = currText[currIndex].ToString().ToUpper();
                    currentTime = timePerLetter;

                    letterText.color = Color.white;

                    SetState(QTEState.RUNNING);
                }
                break;
        }

    }
    void Completed()
    {
        //completed
        minigameScreen.SetActive(false);
        completedScreen.SetActive(true);

        GameStateManager.instance.MonitorStats(correct / (float)(currText.Length - 1) * 100, Time.time - timer);
        GameStateManager.instance.SetGameState(GameState.GAMEOVER);

        SetState(QTEState.COMPLETE);
    }

    public void SetState(QTEState newState)
    {
        currentState = newState;
    } 

    public void QTEReset()
    {
        minigameScreen.SetActive(true);
        completedScreen.SetActive(false);

        correct = 0;
        SetState(QTEState.NONE);
    }
}
