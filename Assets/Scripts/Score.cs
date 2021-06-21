using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    
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
   
    int previousScore;
    float scoreLerpTime = 0;

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
        uiScore.text = displayScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreUpdater();
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

    public int SubscriberCount
    {
        get
        {
            return currentScore;
        }

        set
        {
            
            if(value != currentScore)
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
            }
            
           
        }
    }
}

