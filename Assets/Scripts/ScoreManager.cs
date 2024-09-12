using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _winningPointPerEvent;
    [SerializeField] private int _timePointFactor;
    [SerializeField] private int _timeBeforeUpgrade;
    [SerializeField] private TextMeshProUGUI _scoreTMP;

    public void ReportScore(float score, int scoreIndex)
    {
        
    }
}
