using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused;

    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject HUD;
    public Animator panelAnim;

    private void Start()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }

    void Update()
    {

    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        HUD.SetActive(true);
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        HUD.SetActive(false);
        Time.timeScale = 0f;
    }
    public void Menu()
    {
        Time.timeScale = 1f;
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
        }
        panelAnim.SetTrigger("Load");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        StartCoroutine(LoadAsync());
        //SceneManager.LoadScene("TitleMenu");
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("TitleMenu");
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (panelAnim.gameObject.transform.GetChild(0).gameObject.active)
            {

                Destroy(AudioManager.Instance.gameObject);
                asyncOperation.allowSceneActivation = true;

            }
            yield return null;
        }
    }
}
