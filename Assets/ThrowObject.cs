using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowObject : MonoBehaviour
{
    [Header("Animation Settings")]
   

    [Header("Object Settings")]
   // public GameObject instantiatedObject;
    [SerializeField]
    private GameObject[] possibleObjects;   
    public Transform thrownObjectHolder;
    public Transform throwPoint;
    public GameObject targetTransform;
    // Start is called before the first frame update


    public void ThrowRandomObject(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed && possibleObjects != null)
        {
            int selectedID = Random.Range(0, possibleObjects.Length);
             GameObject instantiatedObject = Instantiate( possibleObjects[selectedID],throwPoint.position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), thrownObjectHolder);
            if(instantiatedObject.TryGetComponent(out  Projectile projectileScript))
            {
                projectileScript.targetTransform = targetTransform;
                projectileScript.thrownObject = instantiatedObject;
            }   
        }
    }
}
