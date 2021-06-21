using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private Score scoreScript;

    [Header("Throw Settings")]
    public ThrowObject throwObjectScript;

    public GameObject targetTransform;
    public GameObject thrownObject;
    bool finished = false;
    public LayerMask playerLayer;

    [Space(5)]

    public bool isInAnimation = true;

    float effectiveHeight;
    float raycastDistance;
    protected float Animation;

    float animationTime = 3f;



    [Header("Enemy Interaction")]
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private int scoreModifier;
    [SerializeField]
    private BasicMovementScript enemyScript;


    [SerializeField]
    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        HasFinished = true;
        throwObjectScript = FindObjectOfType<ThrowObject>();
        scoreScript = FindObjectOfType<Score>();
        if(rb == null && TryGetComponent(out Rigidbody newRB))
        {
            rb = newRB;
        }
        rb.isKinematic = true;
    }

    public float EffectiveHeight
    {
        get
        {
            return effectiveHeight;
        }

        set
        {
            effectiveHeight = value * Vector3.Distance(targetTransform.transform.position, throwObjectScript.throwPoint.transform.position) / 10f;
        }
    }

    public float BaseAnimationTime
    {
        get
        {
           
            return animationTime;
           
        }

        set
        {
            animationTime = value;
        }
    }

    public float RaycastDistance
    {
        set
        {
            raycastDistance = value;
        }

        get
        {
            return raycastDistance;
        }
    }

    public bool HasFinished
    {
        set
        {
            finished = value;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(Animation <= animationTime && finished == false)
        {
            Animation += Time.deltaTime / Vector3.Distance(targetTransform.transform.position, throwObjectScript.throwPoint.transform.position) * RaycastDistance ;
        }
        else if(rb.isKinematic && finished == false)
        {
            rb.isKinematic = false;
            finished = true;
            isInAnimation = false;
        }

        //Animation = Animation % animationTime;

        if (thrownObject != null && targetTransform != null && finished == false)
        {
            thrownObject.transform.position = MathParabola.Parabola(throwObjectScript.throwPoint.transform.position, targetTransform.transform.position, EffectiveHeight, Animation / BaseAnimationTime );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out BasicMovementScript basicMovementScript) && collision.gameObject.layer != enemyLayer && finished != false)
        {
            finished = true;
            scoreScript.SubscriberCount += scoreModifier;
            enemyScript = basicMovementScript;
            basicMovementScript.StartStunCoroutine();
        }
       
        if(finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            finished = true;
            isInAnimation = false;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            finished = true;
            isInAnimation = false;
        }
    }
}
