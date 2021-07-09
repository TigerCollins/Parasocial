using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLight : MonoBehaviour
{
    [SerializeField]
    Light fireLight;

    [Space(5)]

    [SerializeField]
    float minStrength;
    [SerializeField]
    float maxtrength;
    [SerializeField]
    float updateFrequency;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireFlicker(true));

    }

    IEnumerator FireFlicker(bool changeHeat)
    {
        if(changeHeat)
        {
            fireLight.intensity = Random.Range(minStrength, maxtrength);
        }

        yield return new WaitForSeconds(updateFrequency);
        int randNum = Random.Range(0, 10);
        if(randNum >7)
        {
            StartCoroutine(FireFlicker(true));
        }

        else
        {
            StartCoroutine(FireFlicker(false));
        }
     
    }


}
