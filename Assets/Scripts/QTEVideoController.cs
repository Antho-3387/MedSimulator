using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class QTEVideoController : MonoBehaviour
{
    [Header("Configuration Vidéo")]
    public VideoPlayer videoPlayer;
    public float dureeAvanceSucces = 2f;     // Durée de lecture lors d'un succès
    public float dureeReculEchec = 1.5f;     // Durée à reculer lors d'un échec
    public bool loopVideo = false;

    private double videoDuration;
    private bool isPlaying = false;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.isLooping = loopVideo;
            videoPlayer.playOnAwake = false;
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogError("Aucun VideoPlayer assigné !");
        }
    }

    void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("Vidéo prête !");
        videoDuration = videoPlayer.length;
        Debug.Log($"Durée de la vidéo: {videoDuration} secondes");
        videoPlayer.Pause();
        isPlaying = false;
    }

    void Update()
    {
        // Force la pause si la vidéo essaie de jouer toute seule
        if (videoPlayer != null && videoPlayer.isPlaying && !isPlaying)
        {
            videoPlayer.Pause();
        }
    }

    // Joue la vidéo pendant X secondes (succès)
    public IEnumerator JouerAvancer(float duree)
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            double startTime = videoPlayer.time;
            Debug.Log($"▶️ Lecture de {startTime:F2}s à {startTime + duree:F2}s");
            
            isPlaying = true;
            videoPlayer.Play();
            
            yield return new WaitForSeconds(duree);
            
            videoPlayer.Pause();
            isPlaying = false;
            
            Debug.Log($"⏸️ Pause à {videoPlayer.time:F2}s");
        }
    }

    // Recule et joue brièvement (échec)
    public IEnumerator JouerReculer(float dureeRecul)
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            // Recule d'abord
            double currentTime = videoPlayer.time;
            double newTime = Mathf.Max(0, (float)(currentTime - dureeRecul));
            videoPlayer.time = newTime;
            
            Debug.Log($"⏪ Reculé de {currentTime:F2}s à {newTime:F2}s");
            
            // Joue brièvement pour montrer l'effet
            isPlaying = true;
            videoPlayer.Play();
            
            yield return new WaitForSeconds(0.3f); // Joue 0.3s pour montrer qu'on a reculé
            
            videoPlayer.Pause();
            isPlaying = false;
            
            Debug.Log($"⏸️ Pause à {videoPlayer.time:F2}s");
        }
    }

    // Met en pause la vidéo
    public void PauseVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
            isPlaying = false;
            Debug.Log($"⏸️ Vidéo en pause à {videoPlayer.time:F2}s");
        }
    }

    // Joue la vidéo jusqu'au bout
    public void JouerJusquAuBout()
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            isPlaying = true;
            videoPlayer.Play();
            Debug.Log("▶️ Lecture jusqu'à la fin");
        }
    }

    // Obtient le temps actuel
    public double GetCurrentTime()
    {
        return videoPlayer != null ? videoPlayer.time : 0;
    }

    // Vérifie si la vidéo est terminée
    public bool EstTerminee()
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            return videoPlayer.time >= videoDuration - 0.1;
        }
        return false;
    }
}