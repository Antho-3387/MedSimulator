using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Gauche")]
    public Image leftActorImage;
    public TMP_Text leftActorName;

    [Header("UI Droite")]
    public Image rightActorImage;
    public TMP_Text rightActorName;

    [Header("Dialogue")]
    public TMP_Text messageText;

    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive = false;

    // Variable statique pour sauvegarder la position entre les scènes
    public static int savedMessageIndex = -1;

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        isActive = true;

        leftActorImage.sprite = actors[0].sprite;
        leftActorName.text = actors[0].name;

        rightActorImage.sprite = actors[1].sprite;
        rightActorName.text = actors[1].name;

        // Reprise du dialogue là où on s'était arrêté
        if (savedMessageIndex >= 0)
        {
            activeMessage = savedMessageIndex + 1;
            savedMessageIndex = -1; // Réinitialisation après reprise
        }
        else
        {
            activeMessage = 0;
        }

        DisplayMessage();
    }

    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        messageText.text = messageToDisplay.message;

        int speaker = messageToDisplay.actorId;

        if (speaker == 0)
        {
            Highlight(leftActorImage);
            Dim(rightActorImage);
        }
        else
        {
            Highlight(rightActorImage);
            Dim(leftActorImage);
        }
    }

    void Highlight(Image img) { img.color = new Color(1f, 1f, 1f, 1f); }
    void Dim(Image img) { img.color = new Color(0.5f, 0.5f, 0.5f, 0.6f); }

    public void NextMessage()
    {
        // 1. On vérifie SI on est sur un index pivot AVANT de passer au message suivant
        if (activeMessage == 5) // Après le 6ème message (index 5)
        {
            savedMessageIndex = activeMessage;
            isActive = false; 
            Invoke("ChangeToDesin", 2f);
            return;
        }
        
        if (activeMessage == 7) // Après le 9ème message (index 8)
        {
            savedMessageIndex = activeMessage;
            isActive = false; 
            Invoke("ChangeToQTE", 2f);
            return;
        }

        // 2. Sinon, on avance normalement
        activeMessage++;

        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Fin de la conversation");
            isActive = false;
        }
    }

    void ChangeToQTE()
    {
        SceneManager.LoadScene("QTE");
    }

    void ChangeToDesin()
    {
        SceneManager.LoadScene("Desinfecter");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isActive)
        {
            NextMessage();
        }
    }
}