using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelection : MonoBehaviour
{
    [SerializeField] private RectTransform selectionBox;
    private List<MovableObjectStateMachine> selectedMovableObjects = new List<MovableObjectStateMachine>();
    Vector3 startPosition, currentPosition;
    private Camera cam;
    int id = -1;
    float width;
    float height;
    List<RaycastHit> movableObjects = new List<RaycastHit>();

    Vector3 fingerMovePosition;
    Vector3 offset;
    Vector3 raycastStartPos;

    float distanceFromStartToCurrent;


    bool moving = false;
    public static BoxSelection singleton;
    //Travis was here
    private void Awake()
    {
        singleton = this;
        cam = Camera.main;
    }

    public void BeginDraggingGrid(int indexSent, Vector3 positionSent)
    {
        if (id != -1)
        {
            return;
        }
        id = indexSent;
        currentPosition = positionSent;
        startPosition = positionSent;
        SubscribeToDelegates();
        Debug.Log("Begin draggin Grid");
        raycastStartPos = startPosition;
        UpdateSelectionBox();
    }
    void SubscribeToDelegates()
    {
        TouchScript.touchMoved += FingerMoved;
        TouchScript.fingerReleased += FingerReleased;
    }
    void UnsubscribeToDelegates()
    {
        TouchScript.touchMoved -= FingerMoved;
        TouchScript.fingerReleased -= FingerReleased;
    }

    private void FingerReleased(Vector3 position, int index)
    {
        if (id != index)
        {
            return;
        }
        id = -1;
        UnsubscribeToDelegates();

        ShootRayCastToCheckForMovableObjects();
        SelectAllMovableObjectsWithinList();
    }

    private void SelectAllMovableObjectsWithinList()
    {
        for (int i = 0; i < movableObjects.Count; i++)
        {
            Transform finalParent = Crutilities.singleton.GetFinalParent(movableObjects[i].transform);
            if (finalParent != null)
            {
                finalParent.GetComponentInChildren<MovableObjectStateMachine>().SetBoxSelected(selectionBox.position);
            }
        }
    }

    private void FingerMoved(Vector3 position, int index)
    {
        if (id != index)
        {
            return;
        }
        if (!moving)
        {
            UpdateSelectionBox();
            currentPosition = position;
        }
        if (moving)
        {
            Ray ray = Camera.main.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                fingerMovePosition = raycastHit.point;
            }
            Move();
        }
    }

    void UpdateSelectionBox()
    {
        if (!selectionBox.gameObject.activeInHierarchy)
        {
            selectionBox.gameObject.SetActive(true);
        }
        Vector3 currentWorldPosition = Camera.main.ScreenToWorldPoint(currentPosition);
        Vector3 startingWorldPosition = Camera.main.ScreenToWorldPoint(startPosition);
        width = currentWorldPosition.x - startingWorldPosition.x;
        height = currentWorldPosition.z - startingWorldPosition.z;

        selectionBox.localScale = new Vector3(MathF.Abs(width), MathF.Abs(height), 1);
        selectionBox.anchoredPosition3D = new Vector3(startingWorldPosition.x + width / 2, 1, startingWorldPosition.z + height / 2);

        distanceFromStartToCurrent = Vector3.Distance(raycastStartPos, currentPosition);
        if (distanceFromStartToCurrent > 10f)
        {
            ShootRayCastToCheckForMovableObjects();
            raycastStartPos = currentPosition;
        }
    }

    void ShootRayCastToCheckForMovableObjects()
    {
        //foreach movableobject if it isnt in objectshit remove it from movableobjects

        RaycastHit[] objectsHit = Physics.BoxCastAll(selectionBox.position, new Vector3(selectionBox.localScale.x / 2, selectionBox.localScale.z / 2, selectionBox.localScale.y / 2), Vector3.down, Quaternion.identity, 10f);


        for (int j = 0; j < movableObjects.Count; j++)
        {
            if (!ArrayContains(objectsHit, movableObjects[j]))
            {
                Transform finalParent = Crutilities.singleton.GetFinalParent(movableObjects[j].transform);
                if (finalParent != null)
                {
                    finalParent.GetComponentInChildren<MovableObjectStateMachine>().SetBoxUnselected();
                }
            }
        }

        for (int i = 0; i < objectsHit.Length; i++)
        {
            Transform finalParent = Crutilities.singleton.GetFinalParent(objectsHit[i].transform);
            while (finalParent.parent != null)
            {
                finalParent = finalParent.parent;

            }
            if (finalParent.GetComponentInChildren<MovableObjectStateMachine>() != null)
            {
                if (!movableObjects.Contains(objectsHit[i]))
                {
                    movableObjects.Add(objectsHit[i]);
                }
            }
        }
    }

    bool ArrayContains(RaycastHit[] arraySent, RaycastHit rayToCheck)
    {
        foreach (RaycastHit ray in arraySent)
        {
            if (rayToCheck.transform == ray.transform)
            {
                return true;
            }
        }
        return false;
    }

    void Move()
    {
        Debug.Log("Moving");
        Vector3 targetPosition = new Vector3(fingerMovePosition.x, this.selectionBox.transform.position.y, fingerMovePosition.z);
        targetPosition = targetPosition + offset;
        selectionBox.transform.position = targetPosition;
    }

    public void MoveAllObjectsWithinSelection()
    {

        for (int i = 0; i < movableObjects.Count; i++)
        {
            Crutilities.singleton.GetFinalParent(movableObjects[i].transform).GetComponentInChildren<MovableObjectStateMachine>().transform.position = Vector3.zero;
        }
    }
    public void SetBoxSelected(int index, Vector3 positionSent)
    {
        this.id = index;
        SubscribeToDelegates();
        offset = new Vector3(this.selectionBox.position.x - positionSent.x, 0, this.selectionBox.position.z - positionSent.z);
        moving = true;
        Debug.Log("SelectBox");
    }
}
