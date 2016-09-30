﻿using UnityEngine;
using EA4S.Tobogan;
using System.Collections.Generic;

public class ToboganFeedbackGraphics : MonoBehaviour
{
    Queue<bool> answersResults = new Queue<bool>();
    
    public LettersTower tower;
    //public WrongTubesSet wrongTubes;

    bool waitingForTowerRelease = false;
    bool waitingForTowerCrash = false;

    public void ShowPoorPlayerPerformanceFeedback()
    {
        answersResults.Clear();
        waitingForTowerCrash = true;
        tower.RequestCrash();
        // antura.Howl();
    }

    void OnResult(bool result)
    {
        answersResults.Enqueue(result);
    }
    
    void OnLetterGoodReleased()
    {
        waitingForTowerRelease = false;
    }

    void OnTowerCrashed()
    {
        waitingForTowerCrash = false;
    }
    
    public void Initialize(QuestionsManager questionsManager)
    {
        tower.onCrashed += OnTowerCrashed;
        questionsManager.onAnswered += OnResult;
    }

    void Update()
    {
        if (waitingForTowerRelease || waitingForTowerCrash)
            return;

        if (answersResults.Count > 0)
        {
            var nextValue = answersResults.Dequeue();

            if (nextValue)
            {
                waitingForTowerRelease = true;
                tower.AddLetter(OnLetterGoodReleased);
            }
            else
            {
                waitingForTowerCrash = true;
                tower.RequestCrash();
            }
        }
    }
}
