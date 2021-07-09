using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    internal GameManager game;
    [SerializeField]
    PlayerState playerState;

    [SerializeField]
    int highestScore;

    [Header("Score")]

    [SerializeField]
    int currentScore;
    [SerializeField]
    int displayScore;
    [SerializeField]
    TextMeshProUGUI uiScore;
    [SerializeField]
    float numberChangeSpeed;

    [Header("Viewer Count")]
    [SerializeField]
    TextMeshProUGUI viewerCountText;
    [SerializeField]
    int baseViewers;
    [SerializeField]
    int currentViewers;
    [SerializeField]
    int maxViewers;
    [SerializeField]
    int previousViewers;
    [SerializeField]
    bool tooFar;

    [Space(5)]

    [SerializeField]
    float declineRate = 2;
    [SerializeField]
    float declineAmount = 30;
    float baseDeclineRate;

    int previousScore;
    float scoreLerpTime = 0;
    float viewerLerpTime = 0;
    bool isOnMesh;

    /*  [Header("Animation")]
      [SerializeField]
      AnimationCurve textAnimationPath;
    */
    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        displayScore = 0;
        previousScore = 0;
        baseDeclineRate = declineRate;
        ViewerCount = baseViewers;
        uiScore.text = displayScore.ToString();
        StartCoroutine(ViewCountCountdown(declineRate));
    }

    public int ViewerCount
    {
        get
        {
            return currentViewers;
        }

        set
        {
                currentViewers = Mathf.Clamp(value, 0, maxViewers);
                if (currentViewers <= 0)
                {
                playerState.triggeredByCollision = false;
                StartCoroutine(playerState.GameOverScreen());
                }
        }
    }

    public bool IsOnMesh
    {
        get
        {
            return isOnMesh;
        }

        set
        {
            isOnMesh = value;
        }
    }

    public bool HowFar
    {
        get
        {
            return tooFar;
        }

        set
        {
            tooFar = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ScoreUpdater();
        ViewerUpdater();
        

        if(IsOnMesh == true)
        {
            declineRate = .5f;
        }

        else if(HowFar == true)
        {
            declineRate = .5f;
        }

        else
        {
            declineRate = baseDeclineRate;
        }
    }

    IEnumerator ViewCountCountdown(float time)
    {
        float thisTime = time;
        yield return new WaitForSeconds(thisTime);
        float viewCountValue = declineAmount;
        ViewerCount = ViewerCount - (int)viewCountValue;
        StartCoroutine(ViewCountCountdown(declineRate));
    }

    private void ScoreUpdater()
    {
            if (displayScore != currentScore)
            {
            scoreLerpTime += Time.deltaTime / numberChangeSpeed;
                displayScore = (int)Mathf.Lerp(previousScore, currentScore,  scoreLerpTime); //Increment the display score by 1
                uiScore.text = displayScore.ToString(); //Write it to the UI
            }

            else if (previousScore != currentScore)
            {
                previousScore = currentScore;
            scoreLerpTime = 0;
            }
        
    }

    private void ViewerUpdater()
    {
        if (viewerCountText.text != ViewerCount.ToString())
        {
            viewerLerpTime += Time.deltaTime / numberChangeSpeed;
            int newValue = (int)Mathf.Lerp(previousViewers, ViewerCount, viewerLerpTime);
            viewerCountText.text = newValue.ToString();
        }

        else if (ViewerCount != previousViewers)
        {
            previousViewers = ViewerCount;
            viewerLerpTime = 0;
        }
    }

    public int SubscriberCount
    {
        get
        {
            return currentScore;
        }

        set
        {
            
            if(value != currentScore && playerState.DeathState == false)
            {
                //score loss
                if (currentScore < previousScore)
                {
                    currentScore = Mathf.Clamp(value, 0,previousScore);
                 //   uiScore.c
                }

                // Score gain
                else
                {
                    currentScore = Mathf.Clamp(value, previousScore, value);

                }
               // playerState.PreviousScore = currentScore;
            }
            
           
        }
    }


}

