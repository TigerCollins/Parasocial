using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    GameOverScript gameOverScript;
    [SerializeField]
    Score scoreScript;

    [Space(10)]

    [SerializeField]
    CanvasGroup gameOverCanvasGroup;

    [Space(10)]

    [SerializeField]
    bool isDead;
    [SerializeField]
    LayerMask enemyLayer;
   
    [Header("Score")]
    [SerializeField]
    float highscore;
    [SerializeField]
    float previousScore;
    [SerializeField]
    TextMeshProUGUI newScoreText;

    float scoreLerpTime = 0;
    [SerializeField]
    float numberChangeSpeed;
    int currentScore;
    int newScore;

    float lastScoreLerpTime = 0;
    int newLastScore;

    float highScoreLerpTime = 0;
    int newHighscore;
    [SerializeField]
    TextMeshProUGUI highScoreText;

    Coroutine fadeInCoroutine;
    [SerializeField]
    float timeBetweenGameOver = .75f;

    public bool DeathState
    {
        set
        {
            isDead = value;
        }

        get
        {
            return isDead;
        }
    }

    public int Highscore
    {
        get
        {
            int currentHighscore = PlayerPrefs.GetInt("Highscore");
            return currentHighscore;     
        }

        set
        {
            if(value > highscore)
            {
                highscore = value;
                PlayerPrefs.SetInt("Highscore", value);
            }
        }
    }

    public int PreviousScore
    {
        get
        {
            previousScore = PlayerPrefs.GetInt("PreviousScore");
            return PlayerPrefs.GetInt("PreviousScore");
        }

        set
        {
            PlayerPrefs.SetInt("PreviousScore", value);
        }
    }
    
    public void Start()
    {
        gameOverCanvasGroup.alpha = 0;
        newScoreText.text = "Well, at least you gained <color=red>" + newScore + "</color> new followers on this stream. Last Stream you gained <color=red>0</color> new followers...";
        Cursor.visible = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.TryGetComponent(out PlayerState playerState))
        {
            fadeInCoroutine = StartCoroutine(playerState.GameOverScreen());
        }

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.collider.gameObject.layer == 12 && fadeInCoroutine == null)
        {
          
                fadeInCoroutine = StartCoroutine(GameOverScreen());
            
        }
    }

    public IEnumerator GameOverScreen()
    {
        isDead = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Time has paused for gameover sequence");
        while (gameOverCanvasGroup.alpha < 1)
        {
            gameOverCanvasGroup.alpha += Time.unscaledDeltaTime;
            yield return null;
        }
        currentScore = scoreScript.SubscriberCount ;

        yield return new WaitForSecondsRealtime(timeBetweenGameOver);
        float t = 0;
        while(t<numberChangeSpeed)
        {
            t += Time.unscaledDeltaTime;
            scoreLerpTime += Time.unscaledDeltaTime / numberChangeSpeed;
            newScore = (int)Mathf.Lerp(0, currentScore, scoreLerpTime); //Increment the display score by 1
            newScoreText.text = "Well, at least you gained <color=red>" + newScore + "</color> new followers on this stream. Last Stream you gained <color=red>0</color> new followers...";
            yield return null; 
        }

        yield return new WaitForSecondsRealtime(timeBetweenGameOver);
        float tt = 0;
        while (tt < numberChangeSpeed)
        {
            tt += Time.unscaledDeltaTime;
            lastScoreLerpTime += Time.unscaledDeltaTime / numberChangeSpeed;
            newLastScore = (int)Mathf.Lerp(0, PreviousScore, lastScoreLerpTime); //Increment the display score by 1
            newScoreText.text = "Well, at least you gained <color=red>" + newScore + "</color> new followers on this stream. Last Stream you gained <color=red>" + newLastScore + "</color> new followers...";
            
            yield return null;
        }

        yield return new WaitForSecondsRealtime(timeBetweenGameOver);

        float ttt = 0;
        while (ttt < numberChangeSpeed * 2)
        {
            Highscore = scoreScript.SubscriberCount;
            ttt += Time.unscaledDeltaTime;
            highScoreLerpTime += Time.unscaledDeltaTime / numberChangeSpeed;
            newHighscore = (int)Mathf.Lerp(0, Highscore, highScoreLerpTime); //Increment the display score by 1
            highScoreText.text = "The most subscribers you've gained in a stream was <color=green>" + newHighscore + "</color>.";
            yield return null;
        }
        PreviousScore = scoreScript.SubscriberCount;
       
        yield return null;
    }
}
