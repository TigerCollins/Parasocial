using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public bool destroyOnCollision;

    [Space(5)]

    public bool canDestroyFromCountdown;
    public float destroyCountdown;

    // Update is called once per frame
    void Start()
    {
        CountDown();
    }

    void CountDown()
    {
        Destroy(gameObject, destroyCountdown);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(destroyOnCollision)
        {
            Destroy(gameObject);
        }
    }
}
