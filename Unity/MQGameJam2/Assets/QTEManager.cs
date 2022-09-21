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

    private QTEState currentState = QTEState.NONE;

    private string[] possibleTexts;

    private string currText;
    private int currIndex;
    private float currentTime;

    private float wrongTimer;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        possibleTexts = textContent.text.Split("\n");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case QTEState.GENERATING:
                currText = possibleTexts[Random.Range(0, possibleTexts.Length)];
                timerSlider.value = 1;
                currentTime = timePerLetter;
                currIndex = 0;
                letterText.text = currText[currIndex].ToString().ToUpper();

                SetState(QTEState.RUNNING);
                break;

            case QTEState.RUNNING:
                currentTime -= Time.deltaTime;

                if (currentTime <= 0)
                {
                    //skip letter
                    wrongTimer = 0;
                    letterText.color = Color.red;

                    SetState(QTEState.WRONG);
                }

                if (currIndex >= currText.Length)
                {
                    //completed
                    Debug.Log("Yes");
                    SetState(QTEState.COMPLETE);
                }

                string inStr = Input.inputString;
                if(inStr.Length > 0 && inStr[0] == currText[currIndex])
                {
                    currIndex++;
                    letterText.text = currText[currIndex].ToString().ToUpper();
                    currentTime = timePerLetter;
                }

                timerSlider.value = currentTime / timePerLetter;
                ///check input for that key
                break;
            case QTEState.WRONG:
                wrongTimer += Time.deltaTime;

                if(wrongTimer > 1)
                {
                    currIndex++;
                    letterText.text = currText[currIndex].ToString().ToUpper();
                    currentTime = timePerLetter;

                    letterText.color = Color.white;

                    SetState(QTEState.RUNNING);
                }
                break;
        }

    }
    public void SetState(QTEState newState)
    {
        currentState = newState;
    } 
}
