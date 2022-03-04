using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        moving = false;
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

        ChangeRectSizeToFitAllMovableObjects();
    }

    private void ChangeRectSizeToFitAllMovableObjects()
    {
        List<Collider> movableObjectColliders = new List<Collider>();
        for (int i = 0; i < selectedMovableObjects.Count; i++)
        {
            movableObjectColliders.Add(selectedMovableObjects[i].transform.GetComponentInChildren<Collider>());
        }

        float mostNegativeX = 0, mostPositiveX = 0, mostNegativeY = 0, mostPositiveY = 0;
        for (int i = 0; i < movableObjectColliders.Count; i++)
        {
            //position on the x - halfwidth
            float leftBounds = movableObjectColliders[i].transform.position.x - (movableObjectColliders[i].bounds.size.x / 2);
            float rightBound = movableObjectColliders[i].transform.position.x + (movableObjectColliders[i].bounds.size.x / 2);
            float topBounds = movableObjectColliders[i].transform.position.z + (movableObjectColliders[i].bounds.size.z / 2);
            float bottomBounds = movableObjectColliders[i].transform.position.z - (movableObjectColliders[i].bounds.size.z / 2);

            if (leftBounds < mostNegativeX || mostNegativeX == 0)
            {
                mostNegativeX = leftBounds;
            }
            if (rightBound > mostPositiveX || mostPositiveX == 0)
            {
                mostPositiveX = rightBound;
            }
            if (topBounds > mostPositiveY || mostPositiveY == 0)
            {
                mostPositiveY = topBounds;
            }
            if (bottomBounds < mostNegativeY || mostNegativeY == 0)
            {
                mostNegativeY = bottomBounds;
            }
        }
        width = mostPositiveX - mostNegativeX;
        height = mostPositiveY - mostNegativeY;

        selectionBox.localScale = new Vector3(MathF.Abs(width), MathF.Abs(height), 1);
        selectionBox.anchoredPosition3D = new Vector3(mostNegativeX + width / 2, 1, mostNegativeY + height / 2);
        for (int i = 0; i < selectedMovableObjects.Count; i++)
        {
            selectedMovableObjects[i].SetGridOffset(this.selectionBox.position);
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
        RaycastHit[] objectsHit = Physics.BoxCastAll(selectionBox.position, new Vector3(selectionBox.localScale.x / 2, selectionBox.localScale.z / 2, selectionBox.localScale.y / 2), Vector3.down, Quaternion.identity, 10f);

        List<MovableObjectStateMachine> newListOfMovablesToSelect = new List<MovableObjectStateMachine>();
        for (int i = 0; i < objectsHit.Length; i++)
        {
            if (Crutilities.singleton.GetFinalParent(objectsHit[i].transform).GetComponent<MovableObjectStateMachine>() != null)
            {
                newListOfMovablesToSelect.Add(Crutilities.singleton.GetFinalParent(objectsHit[i].transform).GetComponent<MovableObjectStateMachine>());
            }
        }


        List<MovableObjectStateMachine> result = selectedMovableObjects.Except(newListOfMovablesToSelect).ToList<MovableObjectStateMachine>();

        for (int i = 0; i < result.Count(); i++)
        {
            result[i].SetBoxDeselected();
        }
        List<MovableObjectStateMachine> resultToAdd = newListOfMovablesToSelect.Except(selectedMovableObjects).ToList<MovableObjectStateMachine>();

        for (int j = 0; j < resultToAdd.Count(); j++)
        {
            resultToAdd[j].SetBoxSelected();
        }

        selectedMovableObjects = newListOfMovablesToSelect;




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
        Vector3 targetPosition = new Vector3(fingerMovePosition.x, this.selectionBox.transform.position.y, fingerMovePosition.z);
        targetPosition = targetPosition + offset;
        selectionBox.transform.position = targetPosition;
        MoveAllObjectsWithinSelection(targetPosition);
    }

    public void MoveAllObjectsWithinSelection(Vector3 targetPosition)
    {
        for (int i = 0; i < selectedMovableObjects.Count; i++)
        {
            Crutilities.singleton.GetFinalParent(selectedMovableObjects[i].transform).GetComponentInChildren<MovableObjectStateMachine>().GridMove(targetPosition);
        }
    }
    public void SetBoxSelected(int index, Vector3 positionSent)
    {
        this.id = index;
        SubscribeToDelegates();
        offset = new Vector3(this.selectionBox.position.x - positionSent.x, 0, this.selectionBox.position.z - positionSent.z);
        moving = true;
    }
}
