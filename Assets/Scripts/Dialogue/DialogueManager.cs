using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        isActive = true;

        // Affiche les deux acteurs dès le début
        leftActorImage.sprite = actors[0].sprite;
        leftActorName.text = actors[0].name;

        rightActorImage.sprite = actors[1].sprite;
        rightActorName.text = actors[1].name;

        activeMessage = 0;
        DisplayMessage();
    }

    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        messageText.text = messageToDisplay.message;

        int speaker = messageToDisplay.actorId;

        // Celui qui parle = normal
        // Celui qui ne parle pas = sombre + transparent
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

    void Highlight(Image img)
    {
        img.color = new Color(1f, 1f, 1f, 1f); // normal
    }

    void Dim(Image img)
    {
        img.color = new Color(0.5f, 0.5f, 0.5f, 0.6f); // sombre + transparent
    }

    public void NextMessage()
    {
        activeMessage++;

        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("End of conversation");
            isActive = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isActive)
        {
            NextMessage();
        }
    }
}