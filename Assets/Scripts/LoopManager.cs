using UnityEngine;
using UnityEngine.SceneManagement;

public class LoopManager : MonoBehaviour
{
    public static int deathCount = 0; // "static" pour qu'il survive au rechargement de scène

    public void OnDeath()
    {
        deathCount++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // On recommence la boucle
    }

    public string GetXeronDialogue()
    {
        if (deathCount == 0) return "Soignez ma jambe, vite !";
        if (deathCount < 5) return "Encore toi ? Applique-toi, j'en ai marre de cette odeur d'hôpital.";
        return "Tentative n°" + deathCount + ". Tu finiras bien par y arriver, petit mortel.";
    }
}