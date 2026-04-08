using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArrowRowController : MonoBehaviour
{
    [Header("References")]
    public SequenceManager   seqManager;
    public Transform         arrowContainer;
    public GameObject        arrowTilePrefab;

    [Header("Phase label")]
    public TextMeshProUGUI   phaseLabel;
    public Color             labelNormal  = new Color(0.65f, 0.70f, 0.75f, 1f);
    public Color             labelWarning = new Color(0.85f, 0.30f, 0.30f, 1f);

    static readonly string[] PhaseNames =
        { "Serrage du brassard", "Lecture de pression", "Relachement controle" };

    static readonly KeyCode[] Keys =
        { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    static readonly string[] Symbols = { "↑", "↓", "←", "→" };

    private List<ArrowTile> _tiles = new List<ArrowTile>();
    private int  _lastPhase    = -1;
    private bool _lastFailMode = false;
    private int  _lastStep     = -1;

    void Start()
    {
        seqManager.OnStepSuccess.AddListener(OnSuccess);
        seqManager.OnStepError.AddListener(OnError);
        seqManager.OnPhaseComplete.AddListener((_) => RebuildRow());
        seqManager.OnFailModeEnter.AddListener(RebuildRow);
        seqManager.OnFailModeExit.AddListener(RebuildRow);
        seqManager.OnGameComplete.AddListener(OnGameComplete);
    }

    void OnSuccess(int phase, int step)
    {
        if (step < _tiles.Count)
            _tiles[step].SetSuccess();

        int next = step + 1;
        if (next < _tiles.Count)
            _tiles[next].SetActive();
    }

    void OnError(int step)
    {

        if (step < _tiles.Count)
            _tiles[step].SetError();

    }

    void RebuildRow()
    {
        foreach (Transform child in arrowContainer)
            Destroy(child.gameObject);
        _tiles.Clear();

        var seq = seqManager.ActiveSequence;

        for (int i = 0; i < seq.Count; i++)
        {
            int ki  = System.Array.IndexOf(Keys, seq[i]);
            string sym = ki >= 0 ? Symbols[ki] : "?";

            var go   = Instantiate(arrowTilePrefab, arrowContainer);
            var tile = go.GetComponent<ArrowTile>();
            if (tile == null) tile = go.AddComponent<ArrowTile>();

            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = sym;

            if (i < seqManager.CurrentStep)
                tile.SetDone();
            else if (i == seqManager.CurrentStep)
                tile.SetActive();
            else
                tile.SetIdle();

            _tiles.Add(tile);
        }

        UpdatePhaseLabel();
    }

    void UpdatePhaseLabel()
    {
        if (!phaseLabel) return;
        if (seqManager.InFailMode)
        {
            phaseLabel.text  = "Mauvais geste — Correction " + (seqManager.FailSeqIndex + 1);
            phaseLabel.color = labelWarning;
        }
        else if (seqManager.CurrentPhase < PhaseNames.Length)
        {
            phaseLabel.text  = "Phase " + (seqManager.CurrentPhase + 1) + " / 3  —  "
                             + PhaseNames[seqManager.CurrentPhase];
            phaseLabel.color = labelNormal;
        }
    }

    void OnGameComplete()
    {
        foreach (var t in _tiles) t.SetSuccess();
        if (phaseLabel)
        {
            phaseLabel.text  = "Mesure terminee";
            phaseLabel.color = new Color(0.30f, 0.75f, 0.50f, 1f);
        }
    }

    public void ForceRebuild() => RebuildRow();
}