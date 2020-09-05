using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]
    private int _killedEnemies;
    public bool finishCheck;
    private bool finishPlayed = false;
    public Transform finishPoint;
    public Transform player;
    public GameObject door;
    public PlayableDirector director;

    private float _distance;
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

        _killedEnemies = 0;
        UIManager.Instance.UpdateScoreUI();
        finishCheck = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!finishPlayed)
        {
            FinishCheck();
            Finish();
        }
    }

    public void AddKilledEnemy(int value)
    {
        _killedEnemies += value;
        UIManager.Instance.UpdateScoreUI();
    }
    public int GetKilledEnemies()
    {
        return _killedEnemies;
    }

    public void FinishCheck()
    {
        _distance = Mathf.Abs(finishPoint.position.x - player.position.x);
        if (_distance < 0.5f)
        {
            finishCheck = true;
        }
    }
    public void Finish()
    {
        if (finishCheck)
        {
            finishPlayed = true;
            director.Play();
            AudioManager.Instance.Stop("mainTheme");
            AudioManager.Instance.Play("endTheme");
            UIManager.Instance.FinishUI();
        }
    }

    public void UnlockDoor()
    {
        if (door)
        {
            Destroy(door);
            AudioManager.Instance.Play("activate");
        }
    }


}
