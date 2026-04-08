using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HUDController : MonoBehaviour
{
    [Header("References managers")]
    public PatientStatsManager stats;
    public SequenceManager     seqManager;
    public StressSpikeController spikeCtrl;

    [Header("HUD - Tension / BPM / Timer")]
    public TextMeshProUGUI tensionText;
    public TextMeshProUGUI bpmText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI stressText;

    [Header("Couleurs critiques")]
    public Color normalColor  = Color.white;
    public Color warningColor = new Color(1f, 0.65f, 0f);
    public Color dangerColor  = new Color(0.89f, 0.18f, 0.18f);

    [Header("Barre de stress")]
    public Image stressBarFill;
    public Color stressLow    = new Color(0.11f, 0.62f, 0.46f);
    public Color stressMid    = new Color(0.94f, 0.62f, 0.15f);
    public Color stressHigh   = new Color(0.89f, 0.18f, 0.18f);

    [Header("Barre brassard (pression)")]
    public Image brassardFill;

    [Header("Barre critique (6s countdown)")]
    public GameObject criticalPanel;
    public Image      criticalCountdownBar;
    public TextMeshProUGUI criticalReasonText;

    [Header("Alerte pic de stress")]
    public GameObject spikeWarningPanel;
    public TextMeshProUGUI spikeCountdownText;

    [Header("Sequences - fleches")]
    public Transform arrowContainer;
    public GameObject arrowPrefab;     
    public Color arrowPending  = Color.white;
    public Color arrowActive   = new Color(0.22f, 0.54f, 0.85f);
    public Color arrowDone     = new Color(0.5f, 0.5f, 0.5f);
    public Color arrowError    = new Color(0.89f, 0.18f, 0.18f);

    [Header("Phase label")]
    public TextMeshProUGUI phaseLabel;
    string[] phaseNames = { "Serrage du brassard", "Lecture de pression", "Relachement controle" };

    private int _lastStep = -1;
    private bool _lastFailMode = false;
    private int _lastPhase = -1;

    void Update()
    {
        UpdateHUD();
        UpdateArrows();
        UpdateCritical();
        UpdateSpikeWarning();
    }

    void UpdateHUD()
    {
        if (tensionText) tensionText.text = stats.TensionDisplay;
        if (bpmText)
        {
            bpmText.text = Mathf.RoundToInt(stats.Bpm).ToString();
            bpmText.color = (stats.Bpm < stats.bpmMin || stats.Bpm > stats.bpmMax)
                ? dangerColor : normalColor;
        }
        if (timerText)
        {
            int t = Mathf.CeilToInt(seqManager.TimeLeft);
            timerText.text = t.ToString();
            timerText.color = t < 15 ? dangerColor : t < 30 ? warningColor : normalColor;
        }
        if (stressText)
            stressText.text = Mathf.RoundToInt(stats.Stress) + "%";

        if (stressBarFill)
        {
            float s = stats.Stress / 100f;
            stressBarFill.fillAmount = s;
            stressBarFill.color = s > 0.65f ? stressHigh : s > 0.4f ? stressMid : stressLow;
        }

        if (brassardFill)
        {
            float p = seqManager.InFailMode ? 0f :
                (seqManager.CurrentPhase / 3f) + (seqManager.CurrentStep /
                (float)Mathf.Max(1, seqManager.ActiveSequence.Count)) * (1f / 3f);
            brassardFill.fillAmount = Mathf.Clamp01(p);
        }
    }

    void UpdateCritical()
    {
        if (criticalPanel)
            criticalPanel.SetActive(stats.InCritical);

        if (stats.InCritical)
        {
            if (criticalCountdownBar)
                criticalCountdownBar.fillAmount = 1f - stats.CriticalProgress;
            if (criticalReasonText)
                criticalReasonText.text = stats.Bpm < stats.bpmMin
                    ? "BPM trop bas (" + Mathf.RoundToInt(stats.Bpm) + ")"
                    : "BPM trop eleve (" + Mathf.RoundToInt(stats.Bpm) + ")";
        }
    }

    void UpdateSpikeWarning()
    {
        if (spikeWarningPanel)
            spikeWarningPanel.SetActive(spikeCtrl != null && spikeCtrl.SpikeActive);
        if (spikeCtrl != null && spikeCtrl.SpikeActive && spikeCountdownText)
            spikeCountdownText.text = Mathf.CeilToInt(spikeCtrl.SpikeTimeLeft) + "s";
    }

    void UpdateArrows()
    {
        if (!arrowContainer || !arrowPrefab) return;

        bool changed = seqManager.CurrentStep != _lastStep
                    || seqManager.InFailMode   != _lastFailMode
                    || seqManager.CurrentPhase != _lastPhase;
        if (!changed) return;

        _lastStep     = seqManager.CurrentStep;
        _lastFailMode = seqManager.InFailMode;
        _lastPhase    = seqManager.CurrentPhase;

        foreach (Transform child in arrowContainer)
            Destroy(child.gameObject);

        if (phaseLabel)
        {
            if (seqManager.InFailMode)
            {
                phaseLabel.text  = "Mauvais geste — Correction " + (seqManager.FailSeqIndex + 1);
                phaseLabel.color = dangerColor;
            }
            else if (seqManager.CurrentPhase < phaseNames.Length)
            {
                phaseLabel.text  = "Phase " + (seqManager.CurrentPhase + 1) + " / 3 — "
                                 + phaseNames[seqManager.CurrentPhase];
                phaseLabel.color = normalColor;
            }
        }

        // Fleches
        var seq = seqManager.ActiveSequence;
        string[] symbols = { "↑", "↓", "←", "→" };
        KeyCode[] keys   = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

        for (int i = 0; i < seq.Count; i++)
        {
            int ki = System.Array.IndexOf(keys, seq[i]);
            string sym = ki >= 0 ? symbols[ki] : "?";

            var go  = Instantiate(arrowPrefab, arrowContainer);
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            if (txt)
            {
                txt.text  = sym;
                txt.color = i < seqManager.CurrentStep ? arrowDone
                          : i == seqManager.CurrentStep ? arrowActive
                          : arrowPending;
            }
        }
    }

    public void FlashError()
    {
        if (arrowContainer == null) return;
        var txts = arrowContainer.GetComponentsInChildren<TextMeshProUGUI>();
        if (seqManager.CurrentStep < txts.Length)
            txts[seqManager.CurrentStep].color = arrowError;
    }
}