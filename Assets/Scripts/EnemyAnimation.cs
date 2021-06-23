using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimation : MonoBehaviour
{
    [Header("Skin")]
    [SerializeField]
    List<Texture> enemySprites = new List<Texture>();
    [SerializeField]
    Material skinnedMesh;

    [Header("Animation")]
    [SerializeField]
    Animator enemyAnimator;
    [SerializeField]
    NavMeshAgent nav;
    [SerializeField]
    [ReadOnly]
    Vector3 previousVelocity;
    [SerializeField]
    [ReadOnly]
    float currentSpeed;
    [SerializeField]
    bool isStunned;


    public void Awake()
    {
        if(enemySprites.Count > 0)
        {
            skinnedMesh.mainTexture = enemySprites[Random.Range(0, enemySprites.Count)];
        }
    }

    public bool Stun
    {
        get
        {
            return isStunned;
        }

        set
        {
            isStunned = value;
            enemyAnimator.SetBool("isStunned", value);
        }
    }

    public void FixedUpdate()
    {
        if(previousVelocity != nav.velocity)
        {
            currentSpeed = nav.velocity.magnitude;
            ChangeBlendTree();
            previousVelocity = nav.velocity;
        }
    }

    void ChangeBlendTree()
    {
        enemyAnimator.SetFloat("BlendTree", currentSpeed);
    }
}
