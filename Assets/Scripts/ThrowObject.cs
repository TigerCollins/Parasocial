using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowObject : MonoBehaviour
{
    [SerializeField]
    AudioManager audioManager;
    [Header("Animation Settings")]
   

    [Header("Object Settings")]
   // public GameObject instantiatedObject;
    [SerializeField]
    private GameObject[] possibleObjects;   
    public Transform thrownObjectHolder;
    public Transform throwPoint;
    public GameObject targetTransform;
    [SerializeField]
    private float throwHeight;
    [SerializeField]
    private float throwTime;
    // Start is called before the first frame update


    public void ThrowRandomObject(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed && possibleObjects != null && Time.timeScale != 0)
        {
            audioManager.ThrowOneShot();
            int selectedID = Random.Range(0, possibleObjects.Length);
             GameObject instantiatedObject = Instantiate( possibleObjects[selectedID],throwPoint.position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), thrownObjectHolder);
            if(instantiatedObject.TryGetComponent(out  Projectile projectileScript))
            {
                projectileScript.HasFinished = true;
                projectileScript.targetTransform = targetTransform;
                projectileScript.thrownObject = instantiatedObject;
                projectileScript.EffectiveHeight = throwHeight;
                projectileScript.BaseAnimationTime = throwTime;
                projectileScript.RaycastDistance = GetComponent<BasicMovementScript>()._raycast.raycastDistance;
                projectileScript.HasFinished = false;

            }
        }
    }
}
