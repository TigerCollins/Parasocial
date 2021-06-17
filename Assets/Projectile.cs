using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public ThrowObject throwObjectScript;
    protected float Animation;
    [SerializeField]
    float animationTime = 3f;
    [SerializeField]
    private Transform targetTransform;


    [SerializeField]
    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        throwObjectScript = FindObjectOfType<ThrowObject>();
        if(rb == null && TryGetComponent(out Rigidbody newRB))
        {
            rb = newRB;
        }
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Animation <= animationTime)
        {
            Animation += Time.deltaTime;
        }

        //Animation = Animation % animationTime;

        if (throwObjectScript.instantiatedObject != null)
        {
            throwObjectScript.instantiatedObject.transform.position = MathParabola.Parabola(throwObjectScript.throwPoint.transform.position, targetTransform.position, 5f, Animation / animationTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = false;
    }
}
