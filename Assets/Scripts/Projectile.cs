using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public bool hasTriggered = false;

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
    protected float Animation = 0;

    [SerializeField]
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
    Vector3 startingPos;
    // Start is called before the first frame update
    void Awake()
    {
        HasFinished = false;
        throwObjectScript = FindObjectOfType<ThrowObject>();
        scoreScript = FindObjectOfType<Score>();
        if(rb == null && TryGetComponent(out Rigidbody newRB))
        {
            rb = newRB;
        }
       rb.isKinematic = true;
        startingPos = transform.position;
        Animation = 0;
        StartCoroutine(NewAnimation());
    }

    public float EffectiveHeight
    {
        get
        {
            return effectiveHeight;
        }

        set
        {
            effectiveHeight = value * Vector3.Distance(targetTransform.transform.position, startingPos) / 10f;
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

    public IEnumerator NewAnimation()
    {
        while(Animation < BaseAnimationTime && finished == false)
        {
            if (thrownObject != null && targetTransform != null && finished == false)
            {
                Animation += Time.deltaTime / Vector3.Distance(targetTransform.transform.position, startingPos) * RaycastDistance;
          
                thrownObject.transform.position = MathParabola.Parabola(startingPos, targetTransform.transform.position, EffectiveHeight, Animation / BaseAnimationTime);
            }
            yield return null;
        }

        rb.isKinematic = false;
        finished = true;
        isInAnimation = false;
        hasTriggered = true;

     
    }
    // Update is called once per frame
    void Update()
    {

        //Animation = Animation % animationTime;

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out BasicMovementScript basicMovementScript) && collision.gameObject.layer == 12 && finished != true)
        {
            if(!basicMovementScript.isPlayer && hasTriggered == false)
            {
                hasTriggered = true;
             HasFinished = true;
                scoreScript.SubscriberCount += scoreModifier;
                enemyScript = basicMovementScript;
                basicMovementScript.StartStunCoroutine();

            }

        }

        if (finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            HasFinished = true;
            isInAnimation = false;
          //  hasTriggered = true;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            HasFinished = true;
            isInAnimation = false;

        }
    }
}
