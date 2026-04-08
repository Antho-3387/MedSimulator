using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceManager : MonoBehaviour
{
    [Header("References")]
    public PatientStatsManager statsManager;
    public StressSpikeController spikeController;

    [Header("Timer")]
    public float totalTime = 60f;

    // ----- Sequences principales (3 phases) -----
    static readonly List<List<KeyCode>> MainSequences = new List<List<KeyCode>>
    {
        new List<KeyCode>{ KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow,
                           KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow },
        new List<KeyCode>{ KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow,
                           KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow },
        new List<KeyCode>{ KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.UpArrow,
                           KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.DownArrow }
    };

    // ----- Sequences de correction (echec) -----
    static readonly List<List<KeyCode>> FailSequences = new List<List<KeyCode>>
    {
        new List<KeyCode>{ KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,
                           KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.RightArrow },
        new List<KeyCode>{ KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.UpArrow,
                           KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.LeftArrow }
    };

    // ----- Etat -----
    public int  CurrentPhase   { get; private set; } = 0;
    public int  CurrentStep    { get; private set; } = 0;
    public bool InFailMode     { get; private set; } = false;
    public int  FailSeqIndex   { get; private set; } = 0;
    public float TimeLeft      { get; private set; }
    public bool GameRunning    { get; private set; } = false;

    public List<KeyCode> ActiveSequence =>
        InFailMode ? FailSequences[FailSeqIndex] : MainSequences[CurrentPhase];

    // ----- Events -----
    public UnityEvent<int, int>  OnStepSuccess;       // phase, step
    public UnityEvent<int>       OnStepError;          // step
    public UnityEvent<int>       OnPhaseComplete;      // phase index
    public UnityEvent            OnFailModeEnter;
    public UnityEvent            OnFailModeExit;
    public UnityEvent            OnGameComplete;
    public UnityEvent            OnTimeUp;
    public UnityEvent<float>     OnTimeTick;           // temps restant

    // ----- Stress deltas -----
    [Header("Stress changes")]
    public float stressOnGoodKey   = -6f;
    public float stressOnBadKey    = +20f;
    public float stressOnFailGood  = -14f;
    public float stressOnTimeUp    = +8f;
    public float stressOnPhaseOk   = -10f;

    private Coroutine _timerCoroutine;

    void Update()
    {
        if (!GameRunning) return;
        ReadKeyboard();
    }

    public void StartGame()
    {
        CurrentPhase = 0;
        CurrentStep  = 0;
        InFailMode   = false;
        TimeLeft     = totalTime;
        GameRunning  = true;
        spikeController?.StartSpikes();
        _timerCoroutine = StartCoroutine(TimerLoop());
    }

    public void StopGame()
    {
        GameRunning = false;
        spikeController?.StopSpikes();
        if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
    }

    IEnumerator TimerLoop()
    {
        while (TimeLeft > 0f && GameRunning)
        {
            yield return new WaitForSeconds(1f);
            TimeLeft = Mathf.Max(0f, TimeLeft - 1f);
            OnTimeTick?.Invoke(TimeLeft);
            if (TimeLeft <= 0f)
            {
                statsManager.ApplyStressDelta(stressOnTimeUp);
                OnTimeUp?.Invoke();
            }
        }
    }

    void ReadKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))    HandleInput(KeyCode.UpArrow);
        else if (Input.GetKeyDown(KeyCode.DownArrow))  HandleInput(KeyCode.DownArrow);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))  HandleInput(KeyCode.LeftArrow);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) HandleInput(KeyCode.RightArrow);
    }

    public void HandleInput(KeyCode key)
    {
        if (!GameRunning) return;

        var seq = ActiveSequence;
        if (key == seq[CurrentStep])
        {
            // Bonne touche
            statsManager.ApplyStressDelta(InFailMode ? stressOnFailGood : stressOnGoodKey);
            if (spikeController != null && spikeController.SpikeActive)
                spikeController.AbsorbSpike(10f);

            OnStepSuccess?.Invoke(CurrentPhase, CurrentStep);
            CurrentStep++;

            if (CurrentStep >= seq.Count)
                AdvancePhase();
        }
        else
        {
            // Mauvaise touche
            statsManager.ApplyStressDelta(stressOnBadKey);
            OnStepError?.Invoke(CurrentStep);
            CurrentStep = 0;

            if (!InFailMode)
            {
                InFailMode   = true;
                FailSeqIndex = Random.Range(0, FailSequences.Count);
                OnFailModeEnter?.Invoke();
            }
        }
    }

    void AdvancePhase()
    {
        if (InFailMode)
        {
            InFailMode  = false;
            CurrentStep = 0;
            statsManager.ApplyStressDelta(stressOnPhaseOk);
            OnFailModeExit?.Invoke();
            return;
        }

        statsManager.ApplyStressDelta(stressOnPhaseOk);
        OnPhaseComplete?.Invoke(CurrentPhase);
        CurrentPhase++;
        CurrentStep = 0;

        if (CurrentPhase >= MainSequences.Count)
        {
            GameRunning = false;
            spikeController?.StopSpikes();
            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
            OnGameComplete?.Invoke();
        }
    }
}