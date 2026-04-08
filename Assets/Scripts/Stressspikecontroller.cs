using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StressSpikeController : MonoBehaviour
{
    [Header("References")]
    public PatientStatsManager statsManager;

    [Header("Timing des pics")]
    [Tooltip("Delai min entre deux pics (secondes)")]
    public float minDelay = 4f;
    [Tooltip("Delai max entre deux pics (secondes)")]
    public float maxDelay = 14f;

    [Header("Amplitude du pic")]
    [Tooltip("Stress ajoute minimum (%)")]
    public float minSpikeAmount = 30f;
    [Tooltip("Stress ajoute maximum (%)")]
    public float maxSpikeAmount = 60f;

    [Header("Duree du pic")]
    [Tooltip("Secondes avant que le stress redescende automatiquement")]
    public float spikeDuration = 5f;
    [Tooltip("Stress retire apres expiration du pic")]
    public float spikeDecay = 15f;

    [Header("Events")]
    public UnityEvent<float> OnSpikeStart;   
    public UnityEvent<int>   OnSpikeCountdown; 
    public UnityEvent        OnSpikeEnd;

    public bool SpikeActive { get; private set; }
    public float SpikeTimeLeft { get; private set; }

    private Coroutine _schedulerCoroutine;
    private Coroutine _spikeCoroutine;
    private bool _running = false;

    public void StartSpikes()
    {
        _running = true;
        _schedulerCoroutine = StartCoroutine(ScheduleLoop());
    }

    public void StopSpikes()
    {
        _running = false;
        if (_schedulerCoroutine != null) StopCoroutine(_schedulerCoroutine);
        if (_spikeCoroutine    != null) StopCoroutine(_spikeCoroutine);
        SpikeActive = false;
    }

    public void AbsorbSpike(float stressReduction)
    {
        if (!SpikeActive) return;
        statsManager.ApplyStressDelta(-stressReduction);
        if (_spikeCoroutine != null) StopCoroutine(_spikeCoroutine);
        SpikeActive = false;
        OnSpikeEnd?.Invoke();
    }

    IEnumerator ScheduleLoop()
    {
        while (_running)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            if (!_running) yield break;
            _spikeCoroutine = StartCoroutine(RunSpike());
        }
    }

    IEnumerator RunSpike()
    {
        float amp = Random.Range(minSpikeAmount, maxSpikeAmount);
        statsManager.ApplyStressDelta(amp);
        SpikeActive = true;
        SpikeTimeLeft = spikeDuration;
        OnSpikeStart?.Invoke(amp);

        while (SpikeTimeLeft > 0f)
        {
            int secondsLeft = Mathf.CeilToInt(SpikeTimeLeft);
            OnSpikeCountdown?.Invoke(secondsLeft);
            yield return new WaitForSeconds(1f);
            SpikeTimeLeft -= 1f;
        }

        statsManager.ApplyStressDelta(-spikeDecay);
        SpikeActive = false;
        OnSpikeEnd?.Invoke();
    }
}