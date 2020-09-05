using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator imageAnim;

    public void NewGame()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        imageAnim.SetTrigger("Load");
        StartCoroutine(LoadAsync());
       // SceneManager.LoadScene("SampleScene");
    }

    IEnumerator LoadAsync() {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SampleScene");
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (imageAnim.gameObject.transform.GetChild(0).gameObject.active)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
