using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Button startButton;
    [TextArea(2, 5)] public string[] dialogueLines;

    private int currentLine = 0;
    private bool isDialogueActive = false;

    void Start()
    {
        dialogueText.text = "";
        startButton.onClick.AddListener(StartDialogue);
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        startButton.gameObject.SetActive(false);
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
            currentLine++;
        }
        else
        {
            dialogueText.text = "";
            isDialogueActive = false;
        }
    }
}