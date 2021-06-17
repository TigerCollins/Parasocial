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
    public GameObject targetTransform;
    public GameObject thrownObject;
    bool finished = false;
    public LayerMask playerLayer;


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
        if(Animation <= animationTime && finished == false)
        {
            Animation += Time.deltaTime;
        }
        else if(rb.isKinematic && finished == false)
        {
            rb.isKinematic = false;
            finished = true;
        }

        //Animation = Animation % animationTime;

        if (thrownObject != null && targetTransform != null && finished == false)
        {
            thrownObject.transform.position = MathParabola.Parabola(throwObjectScript.throwPoint.transform.position, targetTransform.transform.position, 5f, Animation / animationTime );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            finished = true;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            finished = true;
        }
    }
}
