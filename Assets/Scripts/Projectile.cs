using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    float easeOutTime = .5f;
    [SerializeField]
    float timeDelay = .5f;
    [SerializeField]
    MeshCollider newCollider;
    public bool hasTriggered = false;

    [SerializeField]
    private Score scoreScript;
    [Header("Audio Settings")]
    [SerializeField]
    float minPitch;
    [SerializeField]
    float maxPitch;
    [SerializeField]
    AudioSource thisAudio;
    [SerializeField]
    AudioClip treeCollision;
    [SerializeField]
    AudioClip waterCollision;
    [SerializeField]
    AudioClip terrainCollision;

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
    [Space(15)]
    [SerializeField]
    float remainingCountdownTime;



    [Header("Enemy Interaction")]
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private int scoreModifier;
    [SerializeField]
    private int viewerModifier = 45;
    [SerializeField]
    private BasicMovementScript enemyScript;


    [SerializeField]
    private Rigidbody rb;
    Vector3 startingPos;
    Coroutine endCoroutine;
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

    public void Start()
    {

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
        if (endCoroutine == null)
        {
            endCoroutine = StartCoroutine(LeanTweenCoroutine(easeOutTime));
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
        Debug.Log("Hit");
        if(collision.gameObject.TryGetComponent(out BasicMovementScript basicMovementScript) && collision.gameObject.layer == 12 && finished != true)
        {
            if(!basicMovementScript.isPlayer && hasTriggered == false)
            {
                hasTriggered = true;
             HasFinished = true;
                scoreScript.SubscriberCount += scoreModifier;
                scoreScript.ViewerCount = scoreScript.ViewerCount + viewerModifier;
                enemyScript = basicMovementScript;
                basicMovementScript.StartStunCoroutine();
                if(endCoroutine == null)
                {
                    endCoroutine = StartCoroutine(LeanTweenCoroutine(easeOutTime));
                }

           //     leanTween.scale(gameObject, new Vector3(0, 0, 0), 1).setDestroyOnComplete(true).setEaseInOutBounce();
            }

        }

        if (finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            HasFinished = true;
            isInAnimation = false;
            if (endCoroutine == null)
            {
                endCoroutine = StartCoroutine(LeanTweenCoroutine(easeOutTime));
            }
            thisAudio.pitch = Random.Range(minPitch, maxPitch);
            //  hasTriggered = true;
            if (collision.gameObject.CompareTag("Tree"))
            {
                Debug.Log(collision.gameObject.name);
                Debug.Log("Tree Audio Played");
              
                thisAudio.PlayOneShot(treeCollision);
            }

            else if(collision.gameObject.CompareTag("Terrain"))
            {
                Debug.Log("Terrain Audio Played");
                thisAudio.PlayOneShot(terrainCollision);
            }

            else if (collision.gameObject.CompareTag("Water"))
            {
                Debug.Log("Water Audio Played");
                thisAudio.PlayOneShot(waterCollision);
            }

            else
            {
                Debug.Log("Terrain Audio Played");
                thisAudio.PlayOneShot(terrainCollision);
            }

        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (finished == false && collision.collider.gameObject.layer != gameObject.layer && collision.collider.gameObject.layer != playerLayer)
        {
            rb.isKinematic = false;
            HasFinished = true;
            isInAnimation = false;
            if (endCoroutine == null)
            {
                endCoroutine = StartCoroutine(LeanTweenCoroutine(easeOutTime));
            }
        }
    }

    public IEnumerator LeanTweenCoroutine(float time)
    {
        float countdownTime = time + timeDelay;
        bool hasStarted = false;
        Destroy(gameObject,countdownTime + 3f);
        while (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            remainingCountdownTime = countdownTime;
            if(gameObject.transform.localScale == Vector3.zero)
            {
                newCollider.enabled = false;
                rb.isKinematic = true;
                Destroy(gameObject);
            }

            if (!hasStarted)
            {
                LeanTween.scale(gameObject, Vector3.zero, .4f).setDelay(timeDelay).setEaseInBack().setOvershoot(1.5f);
                hasStarted = true;
            }
         
             if(countdownTime <= .2f)
             {
                 newCollider.enabled = false;
                rb.isKinematic = true;
             }
            
            yield return null;
        }
        Destroy(gameObject);
       
    }
}
