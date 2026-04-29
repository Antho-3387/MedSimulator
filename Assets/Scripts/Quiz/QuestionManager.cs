using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text questionText;
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text timerText;
    public TMP_Text explanationText;

    [Header("Buttons")]
    public Button[] replyButtons;

    [Header("Data")]
    public QtsData qtsData;

    [Header("Panels")]
    public GameObject rightPanel;
    public GameObject wrongPanel;
    public GameObject gameFinishedPanel;

    private int currentQuestion = 0;
    private int score = 0;

    private float timer = 25f;
    private bool timerRunning = true;

    void Start()
    {
        if (!ValidateSetup()) return;

        ResetGameState();
        SetQuestion(currentQuestion);
    }

    void Update()
    {
        if (!timerRunning) return;

        timer -= Time.deltaTime;
        timer = Mathf.Max(0, timer);

        timerText.text = "Temps : " + timer.ToString("F0") + "s";

        if (timer <= 0)
        {
            EndGame();
        }
    }

    bool ValidateSetup()
    {
        if (qtsData == null || qtsData.questions.Length == 0)
        {
            Debug.LogError("QtsData manquant ou vide !");
            return false;
        }

        return true;
    }

    void SetQuestion(int index)
    {
        explanationText.text = "";

        questionText.text = qtsData.questions[index].questionText;

        for (int i = 0; i < replyButtons.Length; i++)
        {
            int localIndex = i;

            replyButtons[i].interactable = true;
            replyButtons[i].onClick.RemoveAllListeners();

            replyButtons[i].GetComponentInChildren<TMP_Text>().text =
                qtsData.questions[index].replies[i];

            replyButtons[i].onClick.AddListener(() =>
            {
                CheckReply(localIndex);
            });
        }
    }

    void CheckReply(int replyIndex)
    {
        bool isCorrect =
            replyIndex == qtsData.questions[currentQuestion].correctReplyIndex;

        explanationText.text = qtsData.questions[currentQuestion].explanation;

        foreach (Button b in replyButtons)
            b.interactable = false;

        if (isCorrect)
        {
            score++;
            scoreText.text = score.ToString();
            rightPanel.SetActive(true);
            timer += 10f;
        }
        else
        {
            wrongPanel.SetActive(true);
            timer -= 15f;

            if (timer <= 0)
            {
                timer = 0;
                EndGame();
                return;
            }
        }

        StartCoroutine(NextQuestion());
    }

    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(3f);

        rightPanel.SetActive(false);
        wrongPanel.SetActive(false);
        explanationText.text = "";

        currentQuestion++;

        if (currentQuestion < qtsData.questions.Length)
        {
            SetQuestion(currentQuestion);
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        timerRunning = false;
        gameFinishedPanel.SetActive(true);

        float percent =
            (float)score / qtsData.questions.Length * 100f;

        finalScoreText.text = "Score : " + percent.ToString("F0") + "%";

        if (percent < 50)
            finalScoreText.text += "\nDommage";
        else if (percent < 60)
            finalScoreText.text += "\nEssaie encore";
        else if (percent < 70)
            finalScoreText.text += "\nPas mal";
        else if (percent < 80)
            finalScoreText.text += "\nT'es chaud !";
        else
            finalScoreText.text += "\nT'es un monstre !";
    }

    public void ResetGame()
    {
        rightPanel.SetActive(false);
        wrongPanel.SetActive(false);
        gameFinishedPanel.SetActive(false);

        score = 0;
        currentQuestion = 0;
        timer = 25f;
        timerRunning = true;

        scoreText.text = "0";

        SetQuestion(currentQuestion);
    }

    void ResetGameState()
    {
        rightPanel.SetActive(false);
        wrongPanel.SetActive(false);
        gameFinishedPanel.SetActive(false);

        score = 0;
        currentQuestion = 0;
        timer = 25f;

        scoreText.text = "0";
        timerRunning = true;
    }
}