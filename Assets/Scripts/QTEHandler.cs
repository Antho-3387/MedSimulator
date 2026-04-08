using System.Collections;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class QTEHandler : MonoBehaviour
{
    [Header("Références UI")]
    public Canvas monCanvas;
    public GameObject monPrefab;
    public Text comboText; 
    public Text scoreText;

    [Header("Sons")]
    public AudioClip sonReussi;
    public AudioClip sonRate;
    
    [Header("Contrôle Vidéo")]
    public QTEVideoController videoController;
    public bool utiliserVideo = true;
    
    [Header("Effets Visuels")]
    public float apparitionDuration = 0.5f;
    public float successEffectDuration = 0.3f;
    public float failEffectDuration = 0.3f;

    private AudioSource audioSource;
    private Sprite sprite_a;
    private Sprite sprite_e;
    private Sprite sprite_f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;
        audioSource.mute = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        
        sprite_a = Resources.Load<Sprite>("Images/QTE/idee_bouton_a");
        sprite_e = Resources.Load<Sprite>("Images/QTE/idee_bouton_e");
        sprite_f = Resources.Load<Sprite>("Images/QTE/idee_bouton_f");

        StartCoroutine(QTE_game());
    }

    Sprite choose_sprite()
    {
        int nombre = Random.Range(0, 3);
        if (nombre == 1) return sprite_e;
        if (nombre == 2) return sprite_f;
        return sprite_a;
    }

    string letter(Sprite sprite)
    {
        if (sprite == sprite_e) return "e";
        if (sprite == sprite_f) return "f";
        return "a";
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, 1f);
        }
    }

    IEnumerator AnimateAppearance(GameObject qteObject, Image qteImage, Vector2 finalPosition)
    {
        RectTransform rect = qteObject.GetComponent<RectTransform>();
        float elapsed = 0f;
        Vector2 startPos = finalPosition + new Vector2(0, 100f);
        float startRotation = Random.Range(-15f, 15f);

        while (elapsed < apparitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / apparitionDuration;
            rect.anchoredPosition = Vector2.Lerp(startPos, finalPosition, progress);
            float alpha = Mathf.Lerp(0f, 1f, progress);
            qteImage.color = new Color(1f, 1f, 1f, alpha);
            float scale = Mathf.Lerp(0.3f, 1f, progress);
            if (progress > 0.7f)
                scale += Mathf.Sin(progress * Mathf.PI * 3) * 0.1f;
            qteObject.transform.localScale = new Vector3(scale, scale, 1f);
            float rotation = Mathf.Lerp(startRotation, 0f, progress);
            qteObject.transform.rotation = Quaternion.Euler(0, 0, rotation);
            yield return null;
        }

        rect.anchoredPosition = finalPosition;
        qteObject.transform.rotation = Quaternion.identity;
        qteObject.transform.localScale = Vector3.one;
        qteImage.color = Color.white;
    }

    IEnumerator AnimateSuccess(GameObject qteObject, Image qteImage)
    {
        float elapsed = 0f;
        Vector3 initialScale = qteObject.transform.localScale;

        while (elapsed < successEffectDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / successEffectDuration;
            float scaleFactor = 1f + Mathf.Sin(progress * Mathf.PI) * 0.5f;
            qteObject.transform.localScale = initialScale * scaleFactor;
            Color successColor = Color.Lerp(Color.white, new Color(0.3f, 1f, 0.3f), 
                                             Mathf.Sin(progress * Mathf.PI));
            qteImage.color = successColor;
            float rotation = progress * 360f * 2f;
            qteObject.transform.rotation = Quaternion.Euler(0, 0, rotation);
            if (progress > 0.6f)
            {
                float alpha = Mathf.Lerp(1f, 0f, (progress - 0.6f) / 0.4f);
                qteImage.color = new Color(successColor.r, successColor.g, successColor.b, alpha);
            }
            yield return null;
        }
    }

    IEnumerator AnimateFail(GameObject qteObject, Image qteImage)
    {
        float elapsed = 0f;
        Vector3 initialPosition = qteObject.transform.localPosition;
        Vector3 initialScale = qteObject.transform.localScale;

        while (elapsed < failEffectDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / failEffectDuration;
            float shakeAmount = (1f - progress) * 10f;
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount), 0);
            qteObject.transform.localPosition = initialPosition + shakeOffset;
            Color failColor = Color.Lerp(Color.white, new Color(1f, 0.2f, 0.2f), 
                                          Mathf.Sin(progress * Mathf.PI));
            float scale = Mathf.Lerp(1f, 0.7f, progress);
            qteObject.transform.localScale = initialScale * scale;
            float alpha = Mathf.Lerp(1f, 0f, progress);
            qteImage.color = new Color(failColor.r, failColor.g, failColor.b, alpha);
            yield return null;
        }
    }

    IEnumerator AnimatePulse(GameObject qteObject, float duration)
    {
        float elapsed = 0f;
        Vector3 baseScale = qteObject.transform.localScale;

        while (elapsed < duration && qteObject != null)
        {
            elapsed += Time.deltaTime;
            float pulse = 1f + Mathf.Sin(elapsed * 5f) * 0.05f;
            qteObject.transform.localScale = baseScale * pulse;
            float rotation = Mathf.Sin(elapsed * 2f) * 5f;
            qteObject.transform.rotation = Quaternion.Euler(0, 0, rotation);
            yield return null;
        }
    }

    IEnumerator QTE_game()
    {
        int score = 0;
        int combo = 0;
        
        for (int i = 0; i < 10; i++)
        {
            // PAUSE la vidéo avant chaque QTE
            if (utiliserVideo && videoController != null)
            {
                videoController.PauseVideo();
            }

            GameObject qteObject = Instantiate(monPrefab, monCanvas.transform);
            Image qteImage = qteObject.GetComponent<Image>();
            Sprite spriteChoisi = choose_sprite();
            qteImage.sprite = spriteChoisi;
            qteImage.color = new Color(1f, 1f, 1f, 0f);
            RectTransform rect = qteObject.GetComponent<RectTransform>();
            RectTransform canvasRect = monCanvas.GetComponent<RectTransform>();
            float margeX = canvasRect.rect.width * 0.1f;
            float margeY = canvasRect.rect.height * 0.1f;
            float x = Random.Range(-canvasRect.rect.width / 2f + margeX, 
                                   canvasRect.rect.width / 2f - margeX);
            float y = Random.Range(-canvasRect.rect.height / 2f + margeY, 
                                   canvasRect.rect.height / 2f - margeY);
            Vector2 finalPosition = new Vector2(x, y);
            rect.anchoredPosition = finalPosition;
            string lettreAttendue = letter(spriteChoisi);
            KeyCode touche = (KeyCode)System.Enum.Parse(typeof(KeyCode), lettreAttendue.ToUpper());

            StartCoroutine(AnimateAppearance(qteObject, qteImage, finalPosition));
            yield return new WaitForSeconds(apparitionDuration);
            Coroutine pulseCoroutine = StartCoroutine(AnimatePulse(qteObject, 2f));
            float timer = 2f;
            bool actionTaken = false;

            while (timer > 0 && !actionTaken)
            {
                timer -= Time.deltaTime;
                if (qteObject == null) break;

                // Mauvaise touche
                if (Input.anyKeyDown && !Input.GetKeyDown(touche))
                {
                    StopCoroutine(pulseCoroutine);
                    combo = 0;
                    score -= 50;
                    scoreText.text = score.ToString();
                    comboText.text = "0";
                    audioSource.PlayOneShot(sonRate, 0.25f);
                    
                    // Animation d'échec
                    yield return StartCoroutine(AnimateFail(qteObject, qteImage));
                    
                    // RECULE et joue la vidéo brièvement
                    if (utiliserVideo && videoController != null)
                    {
                        yield return StartCoroutine(videoController.JouerReculer(videoController.dureeReculEchec));
                    }
                    
                    actionTaken = true;
                }

                // Bonne touche
                if (Input.GetKeyDown(touche))
                {
                    StopCoroutine(pulseCoroutine);
                    combo += 1;
                    if (combo == 1) score += 100;
                    else if (combo == 2) score += 50 * combo - 1;
                    else score += 50 * combo;
                    audioSource.PlayOneShot(sonReussi, 0.25f);
                    comboText.text = combo.ToString();
                    scoreText.text = score.ToString();
                    
                    // Animation de succès
                    yield return StartCoroutine(AnimateSuccess(qteObject, qteImage));
                    
                    // JOUE la vidéo pendant X secondes
                    if (utiliserVideo && videoController != null)
                    {
                        yield return StartCoroutine(videoController.JouerAvancer(videoController.dureeAvanceSucces));
                    }
                    
                    actionTaken = true;
                }

                yield return null;
            }

            // Timeout
            if (timer <= 0 && !actionTaken)
            {
                StopCoroutine(pulseCoroutine);
                combo = 0;
                score -= 50;
                scoreText.text = score.ToString();
                comboText.text = "0";
                
                yield return StartCoroutine(AnimateFail(qteObject, qteImage));
                
                // RECULE pour timeout
                if (utiliserVideo && videoController != null)
                {
                    yield return StartCoroutine(videoController.JouerReculer(videoController.dureeReculEchec));
                }
            }

            if (qteObject != null)
                Destroy(qteObject);

            yield return new WaitForSeconds(0.5f);
        }

        // Fin du jeu - joue la vidéo jusqu'au bout
        if (utiliserVideo && videoController != null)
        {
            videoController.JouerJusquAuBout();
        }
    }
}