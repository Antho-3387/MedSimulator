using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Message[] messages;
    public Actor[] actors;

    public Button startConversationButton; // optionnel

    void Start()
    {
        // Lance automatiquement le dialogue au début de la scène du jeu
        StartDialogue();
    }

    public void StartDialogue()
    {
        FindObjectOfType<DialogueManager>().OpenDialogue(messages, actors);

        // Désactive le bouton s'il existe
        if (startConversationButton != null)
            startConversationButton.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class Message
{
    public int actorId;
    public string message;
}

[System.Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}