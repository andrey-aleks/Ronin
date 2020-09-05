using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text scoreText;
    public int score;
    public GameObject winMenu;
    public GameObject blackPanel;
    public Text deathText;
    public Text scoreTextDoor;
    public GameObject UI;
    public Text scoreTextFinish;

    private Image _blackPanelImg;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        winMenu.SetActive(false);
        blackPanel.SetActive(true);
        _blackPanelImg = blackPanel.GetComponent<Image>();
        deathText.enabled = false;
        _blackPanelImg.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScoreUI() {
        score = GameManager.Instance.GetKilledEnemies();
        scoreText.text = "Score: " + score;
        scoreTextDoor.text = score + "/4";
        scoreTextFinish.text = "Your score: " + score;

        if (score > 3)
        {
            scoreTextDoor.color = Color.green;
            GameManager.Instance.UnlockDoor();
        }
        else scoreTextDoor.color = Color.white;
    }

    public void FinishUI() {
        UI.SetActive(false);
        winMenu.SetActive(true);
    }

    public void Finish() { 
    
    }

    public void StartUI() {
        _blackPanelImg.color = Color.Lerp(_blackPanelImg.color, Color.clear, Time.deltaTime * 8f);
    }

    public void DeathUI() {
        UI.SetActive(false);
        blackPanel.SetActive(true);
        deathText.enabled = true;
        _blackPanelImg.color = Color.Lerp(_blackPanelImg.color, Color.black, Time.deltaTime * 5f);
    }
}
