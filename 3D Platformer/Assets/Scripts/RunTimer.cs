using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RunTimer : MonoBehaviour
{
    private bool _runIsStarted = false;
    private int _runNumber;
    private float _runTime;

    [SerializeField] private TextMeshProUGUI _timerUI;
    [SerializeField] private TextMeshProUGUI _recordsUI;

    private List<string> _records = new();

    void Start()
    {
        _timerUI.gameObject.SetActive(false);
        _runNumber = 1;
    }

    void Update()
    {
        if (_runIsStarted)
        {
            _runTime += Time.deltaTime;
            _timerUI.text = String.Format("{0:0.00}", _runTime);
        }
    }

    public void StartRun()
    {
        if (!_runIsStarted)
        {
            _timerUI.gameObject.SetActive(true);
            _runIsStarted = true;
        }
    }

    public void FinishRun()
    {
        if (_runIsStarted)
        {
            _runIsStarted = false;
            _records.Add($"Run {_runNumber}: {String.Format("{0:0.00}", _runTime)} sec");
            _recordsUI.text += $"\nRun {_runNumber}: {String.Format("{0:0.00}", _runTime)} sec";
            _runNumber++;
            _runTime = 0f;
            _timerUI.gameObject.SetActive(false);
        }
    }
}
