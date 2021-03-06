using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.InputSystem; //Relates to the new input system and not the default unity system (as of Unity 2021.4)

[RequireComponent(typeof(CharacterController))]
public class BasicMovementScript : MonoBehaviour
{
  
    [SerializeField]
    Score score;
    [SerializeField]
    Transform enemyPos;
    public GameObject targetPrefab;
    [SerializeField]
    private ThrowObject throwObject;
    [SerializeField]
    private LayerMask raycastIgnore1;
    [SerializeField]
    private LayerMask raycastIgnore2;
    [SerializeField]
    ReticleScript reticleScript;

    [SerializeField]
    PlayerState playerState;
    [SerializeField]
    EnemyAnimation enemyAnimationScript;
    [SerializeField]
    float distanceScoreThreshold;
    public float enemyDistance;

    /// <summary>
    /// Visible to inspector
    /// </summary>
    [Header("BASIC MOVEMENT SCRIPT")]

    [Space(30)]

    [SerializeField]
    private CharacterController _characterController;
    private InputActionAsset _inputAction;


    [Header("Control Details")]
    [SerializeField]
    private GameObject _playerAvatarObject;
    [SerializeField]
    private ControlTypeEnum _controlType;
    public bool isPlayer = true;
    [SerializeField]
    private MovementDetails _movementControl;
    public FirstPersonDetails _firstPersonControl;
    [SerializeField]
    private JumpDetails jumpDetails;
        [SerializeField]
    private SprintDetails sprintDetails;


    [Header("World Interaction")]
    [ReadOnly]
    [SerializeField]
    private Vector3 _currentMovement; //debug purposes only
    [SerializeField]
    private GravityOptions _gravitySettings;
    public RaycastDetails _raycast;
    [SerializeField]
    private float _pushPower;


    [Header("Audio Settings")]
    [SerializeField]
    float minPitch;
    [SerializeField]
    float maxPitch;
    [SerializeField]
    AudioClip footstepNoise;
    [SerializeField]
    AudioClip jumpNoise;
    [SerializeField]
    float walkFrequency;
    [SerializeField]
    float walkProgress;
    [SerializeField]
    AudioSource footStepsAudioSource;
    readonly float baseAudioMultiplier = 1;
    float currentAudioMultiplier;

    [Header("None Player")]
    [SerializeField]
    BasicAI basicAI;
    [SerializeField]
    float stunTime;
    float originalSpeed;
    Coroutine stunCoroutine;

    // Don't set this too high, or NavMesh.SamplePosition() may slow down
    float onMeshThreshold = 8;

    /// <summary>
    /// Invisble to Inspector
    /// </summary>
    private Vector3 desiredMovementDirection;
    private float _horizontalAxis;
    private float _verticalAxis;

    private float _horizontalLookAxis;
    private float _verticalLookAxis;

    Vector3 velocity;
    float gravity;


    float baseFOV;
    float newFOV;
    float baseSprintMultipler = 1;
    float actualSprintMultipler = 1;
    bool isSprinting;
    bool hasJumped;

    //cam tilt
    float currentTilt = 0f;

    //time
    [ReadOnly]
    [SerializeField]
    float distance;
    [SerializeField]
    float distanceThreshold;
    bool canTriggerPathCoroutine;
    Coroutine currentCoroutine;
    public bool isVisible;


    // Start is called before the first frame update
    void Awake()
    {
        CheckForNullReference();
        AwakeGravity();
      ///  Cursor.lockState = CursorLockMode.Locked;
        SprintSetup();
        currentAudioMultiplier = baseAudioMultiplier;
        if(!isPlayer && basicAI.useNavMeshOn)
        {
            basicAI.navMeshAgent.SetDestination(basicAI.moveToTarget.position);
            StartCoroutine(AutoChangeTargetLocation());
            originalSpeed = basicAI.navMeshAgent.speed;
        }
    }


    public void FootSteps()
    {
        if (_movementControl.moveAxis != Vector2.zero)
        {
            if (walkProgress <= walkFrequency && jumpDetails.isGrounded)
            {
                walkProgress += Time.deltaTime * currentAudioMultiplier * actualSprintMultipler;
            }

            else if (walkProgress >= walkFrequency && jumpDetails.isGrounded)
            {
                footStepsAudioSource.clip = footstepNoise;
                footStepsAudioSource.pitch = Random.Range(minPitch, maxPitch);
                footStepsAudioSource.Play();
                walkProgress = 0;
            }
        }
    }

    public void Jump()
    {
            footStepsAudioSource.Stop();
            footStepsAudioSource.pitch = 1;
            footStepsAudioSource.clip = jumpNoise;
            footStepsAudioSource.Play();
     
    }


    private void OnCollisionEnter(Collision hit)
        
    {
        if (!isPlayer && hit.collider.TryGetComponent(out PlayerState playerState))
        {
            playerState.triggeredByCollision = true;
            StartCoroutine(playerState.GameOverScreen());
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
       

        if (hit.collider.TryGetComponent(out Projectile projectile))
        {
            if(!projectile.isInAnimation)
            {
                RigidBodyPhysics(hit);
            }
        }

        else
        {
            RigidBodyPhysics(hit);
        }

      

        
    }

    public bool IsAgentOnNavMesh(GameObject agentObject)
    {
        Vector3 agentPosition = agentObject.transform.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
        {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(agentPosition.x, hit.position.x)
                && Mathf.Approximately(agentPosition.z, hit.position.z))
            {
                // Lastly, check if object is below navmesh
                return agentPosition.y >= hit.position.y;
            }
        }

        return false;
    }

    public void RigidBodyPhysics(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;


            // no rigidbody
            if (body == null || body.isKinematic)
                return;

            // We dont want to push objects below us
            if (hit.moveDirection.y< -0.3f)
                return;

            // Calculate push direction from move direction,
            // we only push objects to the sides never up and down
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

    // If you know how fast your character is trying to move,
    // then you can also multiply the push velocity by that.

    // Apply the push
    body.velocity = pushDir* _pushPower;

}

    private void SprintSetup()
    {
        if(isPlayer == true)
        {
            baseFOV = _firstPersonControl.firstPersonCamera.fieldOfView;
            newFOV = baseFOV;
        }
       
    }

    public enum ControlTypeEnum
    {
        SideScroller,
        Topdown,
        FirstPerson
    }

    public void CheckForNullReference()
    {
        //If the user hasn't assigned the CharacterController script...
        if (_characterController == null)
        {
            //A more effecient version of GetComponent<>
            if (TryGetComponent(out CharacterController newCharacterController))
            {
                _characterController = newCharacterController;
            }
        }

        //If the user hasn't assigned the InputActionAsset...
        if (_inputAction == null)
        {
            PlayerInput tempPlayerInput = FindObjectOfType<PlayerInput>();
            //If no PlayerInput script is in scene
            if (tempPlayerInput == null)
            {
                Debug.LogWarning("Could not find PlayerInput script for " + gameObject.name + ". Please add component to scene or assign InputAction manually");
            }

            //If Player input hasn't been assign a InputActionAsset...
            else if (tempPlayerInput.actions == null)
            {
                Debug.LogWarning("The Actions variable for " + tempPlayerInput.gameObject.name + " is null. Please assign variable on " + tempPlayerInput.gameObject.name + " or assign component on BasicMovementScript");
            }

            //If everything is assigned on other objects...
            else
            {
                _inputAction = tempPlayerInput.actions;

            }
        }

        if(playerState != null)
        {
            //If the Raycast starting point hasn't been assigned...
            if (_raycast.raycastPoint == null && _raycast.useRaycast && playerState.DeathState == false)
            {
                Debug.LogWarning("Could not find Raycast Point transform. Raycast may not be in the desired position as a result, add a reference if it is inaccurate");
                _raycast.raycastPoint = transform;
            }
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        CheckGravity();
        ApplyGravity();
        Movement(_movementControl.moveAxis);
        CheckGrounded();
        FOVLerp();
        ReticleRaycast();
        FootSteps();
        AdjustLookVectorChill();


        if (isPlayer && enemyDistance > distanceScoreThreshold)
        {
            enemyDistance = Vector3.Distance(enemyPos.position, gameObject.transform.position);
            score.HowFar = true;
        }

        else if(isPlayer && enemyDistance < distanceScoreThreshold)
        {
            enemyDistance = Vector3.Distance(enemyPos.position, gameObject.transform.position);
            score.HowFar = false;
        }


        if (IsAgentOnNavMesh(gameObject) && isPlayer)
        {
            score.IsOnMesh = false;
        }

        else if(!IsAgentOnNavMesh(gameObject) && isPlayer)
        {
            score.IsOnMesh = true;
        }


        if (playerState !=null)
        {
            if (_raycast.useRaycast && playerState.DeathState == false)
            {
                Debug.DrawRay(_raycast.raycastPoint.position, _raycast.raycastPoint.forward * _raycast.raycastDistance, _raycast.raycastColour, Time.deltaTime);
                //Raycast();
            }
        }
       

        if (distance >= distanceThreshold && !isPlayer && basicAI.useNavMeshOn)
        {
            if(canTriggerPathCoroutine)
            {
               currentCoroutine = StartCoroutine(AutoChangeTargetLocation());
            }
        }
    
    }

    public IEnumerator AutoChangeTargetLocation()
    {
        canTriggerPathCoroutine = false;
        distance = Vector3.Distance(basicAI.moveToTarget.position, gameObject.transform.position);

        while (distance > distanceThreshold)
        {
            isVisible = false;

           // Debug.Log(distance);
            distance = Vector3.Distance(basicAI.moveToTarget.position, gameObject.transform.position);
            yield return new WaitForSeconds(3);
            basicAI.navMeshAgent.SetDestination(basicAI.moveToTarget.position);
          //  currentCoroutine = StartCoroutine(AutoChangeTargetLocation());
        }
            while (distance< distanceThreshold)
        {
            distance = Vector3.Distance(basicAI.moveToTarget.position, gameObject.transform.position);

            isVisible = true;
            basicAI.navMeshAgent.SetDestination(basicAI.moveToTarget.position);
            yield return null;
        }

        StartCoroutine(AutoChangeTargetLocation());
       
    }

    void FixedUpdate()
    {
        CameraTilter();

    }
    /// <summary>
    /// Gravity specific functions
    /// </summary>

    void AwakeGravity()
    {
        if (_gravitySettings.useCustomGravity && _gravitySettings.customGravityDetails != _gravitySettings.previousGravityStrength)
        {
            gravity = _gravitySettings.customGravityDetails.y;
            _gravitySettings.previousGravityStrength = _gravitySettings.customGravityDetails;
        }

        else if (Physics.gravity != _gravitySettings.previousGravityStrength)
        {
            _gravitySettings.previousGravityStrength = Physics.gravity;
            gravity = Physics.gravity.y;
        }
    }

    void ApplyGravity()
    {
        if(isPlayer)
        {
            //Reset the MoveVector
            //  Debug.Log(gameObject.name + ": Character controller is currently " + _characterController.isGrounded);
            if (_gravitySettings.useCustomGravity)
            {
                _currentMovement = Vector3.zero;
                velocity += _gravitySettings.customGravityDetails * Time.deltaTime;
                _currentMovement += _gravitySettings.customGravityDetails;
            }

            else if (!_gravitySettings.useCustomGravity)
            {
                _currentMovement = Vector3.zero;
                velocity.y += Physics.gravity.y * Time.deltaTime;
                _currentMovement += Physics.gravity;
            }
            _characterController.Move(velocity * Time.deltaTime);
        }
      

    }

    void CheckGravity()
    {
        if (_gravitySettings.useCustomGravity && _gravitySettings.customGravityDetails != _gravitySettings.previousGravityStrength)
        {
            _gravitySettings.previousGravityStrength = _gravitySettings.customGravityDetails;
            gravity = _gravitySettings.customGravityDetails.y;
            _gravitySettings.onCustomGravityStrengthChange.Invoke();
        }

        else if (Physics.gravity != _gravitySettings.previousGravityStrength)
        {
            _gravitySettings.previousGravityStrength = Physics.gravity;
            gravity = Physics.gravity.y;

            _gravitySettings.onCustomGravityStrengthChange.Invoke();
        }
    }

    /// <summary>
    /// Movement related functions.
    /// <param name="context"></param> relates to the use of the PlayerInputModule
    /// </summary>

    public void AdjustMovementVector(InputAction.CallbackContext context)
    {
        _movementControl.moveAxis = context.ReadValue<Vector2>();  //Mainly for debugging. You can place Context.ReadValue directly into the Movement argument if desired.
    }

    public void AdjustLookVector(InputAction.CallbackContext context)
    {
        if (_controlType == ControlTypeEnum.FirstPerson && Time.timeScale != 0)
        {
            _firstPersonControl.lookAxis = context.ReadValue<Vector2>();  //Mainly for debugging. You can place Context.ReadValue directly into the Movement argument if desired.
           
           // transform.Rotate(Vector3.up * _firstPersonControl.lookAxis.x);
        }
    }

    void AdjustLookVectorChill()
    {
        if(isPlayer)
        {
            _horizontalLookAxis += (_firstPersonControl.xSensitivity / 10) * _firstPersonControl.lookAxis.x;

            float newVerticalLookAxis = _verticalLookAxis + ((_firstPersonControl.ySensitivity / 10) * _firstPersonControl.lookAxis.y);
            _verticalLookAxis = Mathf.Clamp(newVerticalLookAxis, _firstPersonControl.yAxis.bottomClamp, _firstPersonControl.yAxis.topClamp);
            // CameraTilter();
            if (_firstPersonControl.yAxis.invertY)
            {
                _firstPersonControl.firstPersonCamera.transform.localEulerAngles = new Vector3(_verticalLookAxis, 0, 0);
                _firstPersonControl.firstPersonCamera.transform.parent.transform.localEulerAngles = new Vector3(0, _horizontalLookAxis, 0);

            }

            else
            {
                _firstPersonControl.firstPersonCamera.transform.localEulerAngles = new Vector3(-_verticalLookAxis, 0, 0);
                _firstPersonControl.firstPersonCamera.transform.parent.transform.localEulerAngles = new Vector3(0, _horizontalLookAxis, 0);


            }
        }
       
    }




    public void JumpFunction(InputAction.CallbackContext callbackContext)
    {
        if(jumpDetails.canJump && callbackContext.performed && jumpDetails.isGrounded && !hasJumped && Time.timeScale != 0)
        {
            hasJumped = true;
            Jump();
            if (_gravitySettings.useCustomGravity)
            {
                velocity.y = Mathf.Sqrt(jumpDetails.jumpHeight * -2f * gravity);

            }

            else
            {
                velocity.y = Mathf.Sqrt(jumpDetails.jumpHeight * -2f * gravity);

            }
            
        }

    }

    public void CheckGrounded()
    {
        if(jumpDetails.groudCheck != null)
        {
            jumpDetails.isGrounded = Physics.CheckSphere(jumpDetails.groudCheck.position, jumpDetails.groundDistance, jumpDetails.groundLayerMask);
            if(jumpDetails.isGrounded == false)
            {
                hasJumped = false;
            }
        }
    }

    public void CameraTilter()
    {
        if (_controlType == ControlTypeEnum.FirstPerson && isPlayer == true)
        {
          //  localTilt = 0;

            // if (-_firstPersonControl.lookAxis.x > -1f && _firstPersonControl.lookAxis.x > 1f)
            // {
            float localTilt = -_firstPersonControl.lookAxis.x * 6 * _firstPersonControl.tiltStrengthMultiplier;
          //  }


            currentTilt = Mathf.Lerp(currentTilt, -_firstPersonControl.lookAxis.x, _firstPersonControl.camLerpTime * Time.fixedDeltaTime);
          

            float camTilt = Mathf.Lerp(currentTilt, localTilt, _firstPersonControl.camLerpTime * Time.fixedDeltaTime);
           // currentTilt = camTilt;
            Vector3 newTiltAngle = new Vector3(0, 0, camTilt);

            _firstPersonControl.firstPersonCamera.transform.parent.parent.transform.localRotation = Quaternion.Euler(newTiltAngle);
          //  _firstPersonControl.firstPersonCamera.transform.localRotation = Quaternion.Euler((Vector3.left * _verticalLookAxis) + (Vector3.forward * _horizontalLookAxis * _firstPersonControl.cameraTilt) + (Vector3.forward * currentTilt));
        }
    }

    //This allows for events on Get and Set functions for MaxMovementClamp.
    public float MaxMovementClamp
    {
        get
        {
            return _movementControl.maxMovementClamp;
        }

        set
        {
            //to save performance, this lives in a if statement
            if (value != _movementControl.maxMovementClamp)
            {
                _movementControl.maxMovementClamp = value;
            }
        }
    }

    //This allows for events on Get and Set functions for MovementMultiplier.
    public float MovementMultiplier
    {
        get
        {
            return _movementControl.movementMultipler;
        }

        set
        {
            //to save performance, this lives in a if statement
            if (value != _movementControl.movementMultipler)
            {
                _movementControl.movementMultipler = value;
            }
        }
    }

    void ReticleRaycast()
    {
        RaycastHit hit;
        if(playerState != null)
        {
            if (_raycast.useRaycast && _controlType == ControlTypeEnum.FirstPerson && playerState.DeathState == false)
            {
                Ray ray;
                ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                if (Physics.Raycast(ray, out hit, _raycast.raycastDistance))
                {
                    if (hit.collider.gameObject.TryGetComponent(out NavMeshAgent enemy) && hit.transform != gameObject.transform)
                    {
                        ReticleState(true);
                    }

                    else
                    {
                        ReticleState(false);
                    }
                }
            }
        }
        
    }

    public void Raycast(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if(Time.timeScale != 0)
        {
            if (_raycast.useRaycast && playerState.DeathState == false)
            {
                if (_controlType == ControlTypeEnum.FirstPerson)
                {
                    Ray ray;
                    ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                    if (Physics.Raycast(ray, out hit, _raycast.raycastDistance) && context.performed)
                    {
                        if (hit.collider && hit.collider.gameObject.layer != raycastIgnore1 && hit.collider.gameObject.layer != raycastIgnore2)
                        {
                            GameObject newPoint = Instantiate(targetPrefab, hit.point, Quaternion.identity);
                            throwObject.targetTransform = newPoint;
                        }
                    }

                    //If nothing hit
                    else if (context.performed)
                    {
                        GameObject newPoint = Instantiate(targetPrefab, ray.GetPoint(_raycast.raycastDistance), Quaternion.identity);
                        throwObject.targetTransform = newPoint;

                    }
                }

                else
                {
                    if (Physics.Raycast(_raycast.raycastPoint.position, _raycast.raycastPoint.forward, out hit, _raycast.raycastDistance) && context.performed)
                    {
                        //Below is the if statement to find objects. Can be used from Unity 2017 onwards, otherwise use GetComponent instead of TryGetComponent()
                        /*
                         if (hit.collider.TryGetComponent(out QuipScript newQuipScript))
                         {
                             newQuipScript.UpdateText();
                         }
                         */
                    }
                }
            }
        
        }
        
       
    }

    void Movement(Vector2 _inputTranslation)
    {
        if(isPlayer == true)
        {
            //Sets up movement
            _horizontalAxis = _inputTranslation.x;
            _verticalAxis = _inputTranslation.y;

            //Sets up Rotation
            Vector3 _rotationDirection;
            float rotationX = -_inputTranslation.x;
            float rotationY = _inputTranslation.y;

            //Sets movement and then calls rotation function
            switch (_controlType)
            {
                case ControlTypeEnum.SideScroller:
                    desiredMovementDirection = Vector3.ClampMagnitude(new Vector3(_horizontalAxis, 0, 0), _movementControl.maxMovementClamp);
                    _rotationDirection = Vector3.ClampMagnitude(new Vector3(0, 0, rotationX), _movementControl.maxMovementClamp);
                    _characterController.Move(desiredMovementDirection * Time.deltaTime * MovementMultiplier * actualSprintMultipler);
                    MovementRotation(_rotationDirection);
                    break;
                case ControlTypeEnum.Topdown:
                    desiredMovementDirection = Vector3.ClampMagnitude(new Vector3(_horizontalAxis, 0, _verticalAxis), _movementControl.maxMovementClamp);
                    _rotationDirection = Vector3.ClampMagnitude(new Vector3(rotationY, 0, rotationX), _movementControl.maxMovementClamp);
                    _characterController.Move(desiredMovementDirection * Time.deltaTime * MovementMultiplier * actualSprintMultipler);
                    MovementRotation(_rotationDirection);
                    break;
                case ControlTypeEnum.FirstPerson:
                    if (_firstPersonControl.firstPersonCamera != null)
                    {
                        // Vector3 camera
                        desiredMovementDirection = _firstPersonControl.firstPersonCamera.transform.right * _horizontalAxis + _firstPersonControl.firstPersonCamera.transform.parent.transform.forward * _verticalAxis;
                        _characterController.Move(desiredMovementDirection * Time.deltaTime * MovementMultiplier * actualSprintMultipler);
                    }

                    else
                    {
                        Debug.LogError("Couldn't find first person camera, assign the missing variable.");
                    }

                    break;
                default:
                    break;
            }
        }
       
    }

    void MovementRotation(Vector3 _desiredMoveDirection)
    {
        if (_desiredMoveDirection != Vector3.zero && _playerAvatarObject != null)
        {
            _playerAvatarObject.transform.rotation = Quaternion.Slerp(_playerAvatarObject.transform.rotation, Quaternion.LookRotation(_desiredMoveDirection), _movementControl.rotationSpeed / 10);
        }

        else if (_desiredMoveDirection != Vector3.zero)
        {
            Debug.LogError("Could not find Player Avatar Object as part of the BasicMovementScript " + gameObject.name + ". Resolve null reference to get player rotation");
        }
    }

    public void SprintMovement(InputAction.CallbackContext callbackContext)
    {
        if (Time.timeScale != 0)
        {
            if (callbackContext.performed || callbackContext.started)
            {
                if (_controlType == ControlTypeEnum.FirstPerson && sprintDetails.canSprint)
                {
                    if (_movementControl.moveAxis.y > 0)
                    {
                        isSprinting = true;

                        newFOV = baseFOV * sprintDetails.fovMultiplier;
                        actualSprintMultipler = sprintDetails.sprintMultiplier;
                    }
                }

            }

            else if (callbackContext.canceled && sprintDetails.canSprint)
            {
                if (_controlType == ControlTypeEnum.FirstPerson)
                {
                    isSprinting = false;
                    newFOV = baseFOV;
                    actualSprintMultipler = baseSprintMultipler;
                }
            }
        }
    }

    public void IsSprintingCheck()
    {
        if(_movementControl.moveAxis.y < 0 || _movementControl.moveAxis.x != 0)
        {
            isSprinting = false;
        }
    }

    public void FOVLerp()
    {
        if(_controlType == ControlTypeEnum.FirstPerson && isPlayer == true)
        {
            if(_movementControl.moveAxis.y > 0 && isSprinting)
            {
                _firstPersonControl.firstPersonCamera.fieldOfView = Mathf.Lerp(_firstPersonControl.firstPersonCamera.fieldOfView, newFOV, sprintDetails.fovLerpTime * Time.deltaTime);

            }

            else if(!isSprinting)
            {
                _firstPersonControl.firstPersonCamera.fieldOfView = Mathf.Lerp(_firstPersonControl.firstPersonCamera.fieldOfView, baseFOV, sprintDetails.fovLerpTime * Time.deltaTime);

            }
        }
    }

    public void StartStunCoroutine()
    {
        if(!isPlayer)
        {
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
            }
            originalSpeed = basicAI.navMeshAgent.speed;
            stunCoroutine = StartCoroutine(Stun());
        }
            
     
    }

    IEnumerator Stun()
    {

            basicAI.navMeshAgent.speed = basicAI.navMeshAgent.speed / 50;
        enemyAnimationScript.Stun = true;
     
        yield return new WaitForSeconds(stunTime);

        enemyAnimationScript.Stun = false;
        basicAI.navMeshAgent.speed = originalSpeed;
    }

    void ReticleState(bool hitState)
    {
        if(isPlayer)
        {
            reticleScript.canHitEnemy = hitState;
        }
    }
}

[System.Serializable]
public class GravityOptions
{
    public bool useCustomGravity;
    public Vector3 customGravityDetails = new Vector3(0, .2f, 0);
    [HideInInspector]
    public Vector3 previousGravityStrength;

    [Space(10)]

    public UnityEvent onCustomGravityStrengthChange;
}

[System.Serializable]
public class RaycastDetails
{
    public bool useRaycast;
    [Tooltip("This decides where the raycast comes from Leave this variable blank for it to default to this gameobject.")]
    public Transform raycastPoint;
    public float raycastDistance;
    public Color raycastColour;
}

[System.Serializable]
public class MovementDetails
{
    public float movementMultipler = 2;
    [Tooltip("If set correctly, the idea is that it stops diagonal movement from simply combining Horizontal and Vertical speeds.")]
    public float maxMovementClamp = 1;
    [ReadOnly]
    public Vector2 moveAxis; //debug purposes only

    [Space(10)]

    [Range(0, 5)]
    public float rotationSpeed = .75f;
}

[System.Serializable]
public class FirstPersonDetails
{ 
    public Vector2 lookAxis;
    public Camera firstPersonCamera;

    [Space(5)]

    [Range(0, 10)]
    public float ySensitivity = 3;
    [Range(0, 10)]
    public float xSensitivity = 3;
    public YAxisDetails yAxis;

    [Space(5)]


    public float tiltStrengthMultiplier = 5f;
    public float camLerpTime = 5f;
    public float cameraTilt = 0.2f;

}

[System.Serializable]
public class YAxisDetails
{
    public bool invertY;

    [Space(5)]

    public float bottomClamp;
    public float topClamp;
}


[System.Serializable]
public class JumpDetails
{
    public bool canJump;
    public float jumpHeight;
    public bool isGrounded;

    [Space(10)]

    public Transform groudCheck;
    public float groundDistance = .4f;
    public LayerMask groundLayerMask;
}

[System.Serializable]
public class SprintDetails
{
    public bool canSprint;
    public float sprintMultiplier;
    public float fovMultiplier;
    public float fovLerpTime;
}

[System.Serializable]
public class BasicAI
{
    public bool useNavMeshOn;
    public Transform moveToTarget;
    public NavMeshAgent navMeshAgent;
}