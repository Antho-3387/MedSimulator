using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    public Canvas monCanvas;
    public GameObject monPrefab;

    private Sprite sprite_a;
    private Sprite sprite_e;
    private Sprite sprite_f;
    public Text comboText; 
    public Text scoreText;

    void Start()
    {
        sprite_a = Resources.Load<Sprite>("Images/QTE/idee_bouton_a");
        sprite_e = Resources.Load<Sprite>("Images/QTE/idee_bouton_e");
        sprite_f = Resources.Load<Sprite>("Images/QTE/idee_bouton_f");

        StartCoroutine(QTE_game());
    }

    void Update() { }

    Sprite choose_sprite()
    {
        Sprite sprite_ran = sprite_a;
        int nombre = Random.Range(0, 3);
        if (nombre == 1) sprite_ran = sprite_e;
        else if (nombre == 2) sprite_ran = sprite_f;
        return sprite_ran;
    }

    string letter(Sprite sprite)
    {
        if (sprite == sprite_e) return "e";
        if (sprite == sprite_f) return "f";
        return "a";
    }

 IEnumerator QTE_game()
    {
        int score = 0;
        int combo = 0;
        for (int i = 0; i < 10; i++)
        {
            GameObject qteObject = Instantiate(monPrefab, monCanvas.transform);
            Image qteImage = qteObject.GetComponent<Image>();

            Sprite spriteChoisi = choose_sprite();
            qteImage.sprite = spriteChoisi;
            qteImage.color = new Color(1f, 1f, 1f, 0f);

            RectTransform rect = qteObject.GetComponent<RectTransform>();
            RectTransform canvasRect = monCanvas.GetComponent<RectTransform>();

            float margeX = canvasRect.rect.width * 0.1f;
            float margeY = canvasRect.rect.height * 0.1f;

            float x = Random.Range(-canvasRect.rect.width / 2f + margeX, canvasRect.rect.width / 2f - margeX);
            float y = Random.Range(-canvasRect.rect.height / 2f + margeY, canvasRect.rect.height / 2f - margeY);

            rect.anchoredPosition = new Vector2(x, y);
            string lettreAttendue = letter(spriteChoisi);
            KeyCode touche = (KeyCode)System.Enum.Parse(typeof(KeyCode), lettreAttendue.ToUpper());

            float alpha = 0f;
            float scale = 0.5f;
            float timer = 2f;

            while (timer > 0)
            {
                timer -= Time.deltaTime;

                if (qteObject == null) break; 

                if (alpha < 1f)
                {
                    alpha += 0.005f;
                    scale += 0.005f;
                    qteImage.color = new Color(1f, 1f, 1f, alpha);
                    qteObject.transform.localScale = new Vector3(scale, scale, 1f);
                }

                if (Input.GetKeyDown(touche))
                {
                    combo += 1;
                    if (combo == 1) score += 100;
                    else if (combo == 2) score += 50 * combo - 1;
                    else score += 50 * combo;

                    comboText.text = combo.ToString();
                    scoreText.text = score.ToString();
                    break;
                }

                yield return null;
            }
            if (timer <= 0)
            {
                combo = 0;
                score -= 50;
                scoreText.text = score.ToString();
                comboText.text = "0";
            }

            if (qteObject != null)
                Destroy(qteObject);

            yield return new WaitForSeconds(0.5f);
        }
    }
}