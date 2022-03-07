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
            Debug.Log(raycastHit.transform);
            if (raycastHit.transform.GetComponentInChildren<ButtonSelector>() != null)
            {
                raycastHit.transform.GetComponentInChildren<ButtonSelector>().Select();
                Debug.Log("Hit Button Selector");
                return;
            }
            if (GetFinalParent(raycastHit).GetComponent<MovableObjectStateMachine>() != null)
            {
                //if a movable was hit set position to target position and add movable to the dictionary
                MovableObjectStateMachine movableHit = GetFinalParent(raycastHit).GetComponent<MovableObjectStateMachine>();
                Ray ray2 = Camera.main.ScreenPointToRay(movableHit.transform.position);
                //Vector3 offset = new Vector3(movableHit.transform.position.x - raycastHit.point.x, 0, movableHit.transform.position.z - raycastHit.point.z);
                movableHit.SetTouched(index, raycastHit.point);
            }
            if (raycastHit.transform.GetComponent<BoxSelection>() != null)
            {
                BoxSelection boxSelection = raycastHit.transform.GetComponent<BoxSelection>();
                boxSelection.BeginDraggingGrid(index, position);
            }
            if (raycastHit.transform.GetComponentInChildren<BoxSelectionObject>() != null)
            {
                BoxSelectionObject boxSelection = raycastHit.transform.GetComponent<BoxSelectionObject>();
                boxSelection.SelectBox (index, raycastHit.point);
            }
            
        }
    }
    public Transform GetFinalParent(RaycastHit raycastHit)
    {
        Transform targetToMove = raycastHit.transform;
        while (targetToMove.parent != null)
        {
            targetToMove = targetToMove.transform.parent;
            if (targetToMove.GetComponent<MovableObjectStateMachine>() != null) return targetToMove;
        }
        return targetToMove;
    }
    
}
