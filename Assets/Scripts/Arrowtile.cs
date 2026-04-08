using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A placer sur chaque prefab de fleche genere par HUDController.
/// Gere les etats visuels : normal, actif, succes (transparent), erreur (rouge).
/// Style : realiste / sombre.
/// </summary>
[RequireComponent(typeof(Image))]
public class ArrowTile : MonoBehaviour
{
    [Header("References")]
    public Image          background;
    public Image          border;
    public TextMeshProUGUI arrowText;

    // ----- Couleurs style sombre -----
    [Header("Couleurs")]
    public Color colorIdle    = new Color(0.12f, 0.14f, 0.16f, 1f);   // fond gris tres sombre
    public Color colorActive  = new Color(0.18f, 0.26f, 0.36f, 1f);   // bleu acier (fleche courante)
    public Color colorSuccess = new Color(0.10f, 0.12f, 0.14f, 0.15f);// presque transparent (reussi)
    public Color colorError   = new Color(0.55f, 0.06f, 0.06f, 1f);   // rouge sombre
    public Color colorDone    = new Color(0.10f, 0.12f, 0.14f, 0.30f);// fleches passees, fades

    public Color borderIdle    = new Color(0.30f, 0.34f, 0.38f, 1f);
    public Color borderActive  = new Color(0.35f, 0.55f, 0.80f, 1f);
    public Color borderSuccess = new Color(0.20f, 0.55f, 0.35f, 0.40f);
    public Color borderError   = new Color(0.80f, 0.15f, 0.15f, 1f);

    public Color textIdle    = new Color(0.75f, 0.78f, 0.82f, 1f);
    public Color textActive  = new Color(0.85f, 0.92f, 1.00f, 1f);
    public Color textSuccess = new Color(0.45f, 0.55f, 0.50f, 0.35f);
    public Color textError   = new Color(1.00f, 0.45f, 0.45f, 1f);
    public Color textDone    = new Color(0.35f, 0.37f, 0.40f, 0.50f);

    [Header("Durees transitions (secondes)")]
    public float fadeSuccessDuration = 0.35f;
    public float flashErrorDuration  = 0.25f;
    public float holdErrorDuration   = 0.40f;

    public enum TileState { Idle, Active, Done, Success, Error }
    private TileState _state = TileState.Idle;
    private Coroutine _anim;

    void Awake()
    {
        if (!background) background = GetComponent<Image>();
    }

    // ----------------------------------------------------------------
    // API publique appelee par HUDController
    // ----------------------------------------------------------------

    public void SetIdle()
    {
        StopAnim();
        _state = TileState.Idle;
        Apply(colorIdle, borderIdle, textIdle);
    }

    public void SetActive()
    {
        StopAnim();
        _state = TileState.Active;
        Apply(colorActive, borderActive, textActive);
    }

    public void SetDone()
    {
        StopAnim();
        _state = TileState.Done;
        Apply(colorDone, borderIdle, textDone);
    }

    public void SetSuccess()
    {
        StopAnim();
        _state = TileState.Success;
        _anim = StartCoroutine(AnimSuccess());
    }

    public void SetError()
    {
        StopAnim();
        _state = TileState.Error;
        _anim = StartCoroutine(AnimError());
    }

    // ----------------------------------------------------------------
    // Animations
    // ----------------------------------------------------------------

    IEnumerator AnimSuccess()
    {
        // Fondu rapide vers transparent
        Color bgStart  = background.color;
        Color brdStart = border ? border.color : borderSuccess;
        Color txtStart = arrowText ? arrowText.color : textSuccess;

        float t = 0f;
        while (t < fadeSuccessDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / fadeSuccessDuration);
            background.color = Color.Lerp(bgStart,  colorSuccess, p);
            if (border)     border.color     = Color.Lerp(brdStart, borderSuccess, p);
            if (arrowText)  arrowText.color  = Color.Lerp(txtStart, textSuccess,  p);
            yield return null;
        }
        Apply(colorSuccess, borderSuccess, textSuccess);
    }

    IEnumerator AnimError()
    {
        // Flash rouge immediat
        Apply(colorError, borderError, textError);
        yield return new WaitForSeconds(holdErrorDuration);

        // Fondu retour vers idle (la fleche reste en place, on attend la prochaine sequence)
        Color bgStart  = colorError;
        Color brdStart = borderError;
        Color txtStart = textError;

        float t = 0f;
        while (t < flashErrorDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / flashErrorDuration);
            background.color = Color.Lerp(bgStart,  colorIdle,  p);
            if (border)    border.color    = Color.Lerp(brdStart, borderIdle,  p);
            if (arrowText) arrowText.color = Color.Lerp(txtStart, textIdle, p);
            yield return null;
        }
        Apply(colorIdle, borderIdle, textIdle);
    }

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    void Apply(Color bg, Color brd, Color txt)
    {
        background.color = bg;
        if (border)    border.color    = brd;
        if (arrowText) arrowText.color = txt;
    }

    void StopAnim()
    {
        if (_anim != null) { StopCoroutine(_anim); _anim = null; }
    }
}