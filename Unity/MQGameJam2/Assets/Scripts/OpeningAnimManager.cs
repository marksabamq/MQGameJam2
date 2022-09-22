using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OpeningAnimManager : MonoBehaviour
{
    public UnityEvent animationComplete;
    public UnityEvent startGame;

    public Animator startAnim;
    public string animTrigger = "Spill";

    private void Start()
    {
        startAnim.SetTrigger(animTrigger);
    }

    public void AnimationComplete()
    {
        animationComplete.Invoke();
    }

    public void StartGame()
    {
        startGame.Invoke();
    }
}
