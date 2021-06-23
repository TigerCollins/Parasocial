using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleScript : MonoBehaviour
{
    public bool canHitEnemy;
    bool previousEnemyState = true;
    [SerializeField]
    Color baseColour;
    [SerializeField]
    Color enemyColour;
    [SerializeField]
    Image reticleImage;
    // Start is called before the first frame update
    void Start()
    {
        canHitEnemy = false;   
    }

    // Update is called once per frame
    void Update()
    {
        if(canHitEnemy != previousEnemyState)
        {
            previousEnemyState = canHitEnemy;
            if(canHitEnemy == true)
            {
                reticleImage.color = enemyColour;
            }

            else
            {
                reticleImage.color = baseColour;
            }
           
        }
    }
}
