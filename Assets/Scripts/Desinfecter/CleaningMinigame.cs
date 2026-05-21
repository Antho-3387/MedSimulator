using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ← AJOUTÉ : Pour pouvoir changer de scène

public class CleaningMinigame : MonoBehaviour
{
    public CanvasGroup dirtCanvas;   // Le groupe qui contient la saleté
    public float cleanSpeed = 0.5f;  // Vitesse de nettoyage
    public float requiredClean = 0.1f; // Seuil pour considérer la zone propre

    private bool isCleaned = false;

    void Update()
    {
        if (isCleaned)
            return;

        // Si clic gauche maintenu
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;

            // Vérifie si la souris est sur la zone sale
            if (RectTransformUtility.RectangleContainsScreenPoint(
                GetComponent<RectTransform>(), mousePos))
            {
                // Réduit l'opacité progressivement
                dirtCanvas.alpha -= cleanSpeed * Time.deltaTime;

                // Empêche alpha de descendre sous 0
                dirtCanvas.alpha = Mathf.Clamp01(dirtCanvas.alpha);

                // Si la zone est propre
                if (dirtCanvas.alpha <= requiredClean)
                {
                    isCleaned = true;

                    // 🔥 Rendre la tache totalement invisible
                    dirtCanvas.alpha = 0f;

                    Debug.Log("Zone désinfectée !");
                    OnCleaned();
                }
            }
        }
    }

    void OnCleaned()
    {
        // COCHÉ : Charge la scène principale pour reprendre le dialogue
        // Remplace "SampleScene" par le nom exact de ta scène de dialogue si nécessaire
        SceneManager.LoadScene("Dialogue");
    }
}