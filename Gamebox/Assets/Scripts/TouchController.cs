using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    private void Awake()
    {
        TouchScript.touchedDown += FingerDown;
        TouchScript.allTouchesReleased += AllFingersReleased;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void AllFingersReleased(Vector3 position, int index)
    {
    }

   

    private void FingerDown(Vector3 position, int index)
    {
        //shoot a ray from the position sent
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity))
        {
            if (GetFinalParent(raycastHit).GetComponent<MovableObject>() != null)
            {
                //if a movable was hit set position to target position and add movable to the dictionary
                MovableObject movableHit = GetFinalParent(raycastHit).GetComponent<MovableObject>();
                Ray ray2 = Camera.main.ScreenPointToRay(movableHit.transform.position);
                Vector3 offset = new Vector3(movableHit.transform.position.x - raycastHit.point.x, 0, movableHit.transform.position.z - raycastHit.point.z);
                movableHit.SetSelected(index, offset);
            }
        }
    }
    public Transform GetFinalParent(RaycastHit raycastHit)
    {
        Transform targetToMove = raycastHit.transform;
        while (targetToMove.parent != null)
        {
            targetToMove = targetToMove.transform.parent;
        }
        return targetToMove;
    }
}
