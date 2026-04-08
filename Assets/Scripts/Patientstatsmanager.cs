using UnityEngine;
using UnityEngine.Events;

public class PatientStatsManager : MonoBehaviour
{
    [Header("Valeurs initiales")]
    public float baseSys = 120f;
    public float baseDia = 80f;
    public float baseBpm = 80f;

    [Header("Valeurs actuelles (lecture seule)")]
    [SerializeField] private float currentSys;
    [SerializeField] private float currentDia;
    [SerializeField] private float currentBpm;
    [SerializeField] private float currentStress = 30f;

    [Header("Limites critiques BPM")]
    public float bpmMin = 80f;
    public float bpmMax = 150f;
    public float criticalDuration = 6f;

    [Header("Events")]
    public UnityEvent OnCriticalStart;
    public UnityEvent OnCriticalEnd;
    public UnityEvent OnPatientDeath;

    private float criticalTimer = 0f;
    private bool inCritical = false;

    public float Stress => currentStress;
    public float Bpm => currentBpm;
    public float Sys => currentSys;
    public float Dia => currentDia;
    public bool InCritical => inCritical;
    public float CriticalProgress => inCritical ? (criticalDuration - criticalTimer) / criticalDuration : 0f;

    public string TensionDisplay =>
        RoundToFive(currentSys / 10f) + "/" + RoundToFive(currentDia / 10f);

    void Start()
    {
        currentSys = baseSys;
        currentDia = baseDia;
        currentBpm = baseBpm;
    }

    void Update()
    {
        RecalculateFromStress();
        HandleCritical();
    }

    void RecalculateFromStress()
    {
        float s = currentStress;
        currentBpm = Mathf.Clamp(baseBpm + s * 0.6f, bpmMin, 160f);
        currentSys  = Mathf.Clamp(baseSys  + s * 0.55f, 80f, 180f);
        currentDia  = Mathf.Clamp(baseDia  + s * 0.3f,  50f, 110f);
    }

    void HandleCritical()
    {
        bool isCritNow = currentBpm < bpmMin || currentBpm > bpmMax;

        if (isCritNow && !inCritical)
        {
            inCritical = true;
            criticalTimer = criticalDuration;
            OnCriticalStart?.Invoke();
        }
        else if (!isCritNow && inCritical)
        {
            inCritical = false;
            criticalTimer = 0f;
            OnCriticalEnd?.Invoke();
        }

        if (inCritical)
        {
            criticalTimer -= Time.deltaTime;
            if (criticalTimer <= 0f)
            {
                inCritical = false;
                OnPatientDeath?.Invoke();
            }
        }
    }

    public void ApplyStressDelta(float delta)
    {
        currentStress = Mathf.Clamp(currentStress + delta, 0f, 100f);
    }

    public void SetStress(float value)
    {
        currentStress = Mathf.Clamp(value, 0f, 100f);
    }


    public static int RoundToFive(float value)
    {
        return Mathf.FloorToInt(value / 5f) * 5;
    }

    public string GetDiagnosis()
    {
        int s = RoundToFive(currentSys / 10f);
        int d = RoundToFive(currentDia / 10f);
        if (s >= 14 || d >= 9) return "Hypertension";
        if (s < 9  || d < 6)  return "Hypotension";
        return "Tension normale";
    }
}