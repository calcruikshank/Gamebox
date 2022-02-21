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
    //Travis was here
    private void Awake()
    {
        cam = Camera.main;
    }

    public void BeginDraggingGrid(int indexSent, Vector3 positionSent)
    {
        currentPosition = positionSent;
        startPosition = positionSent;
        SubscribeToDelegates();
        Debug.Log("Begin draggin Grid");
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
        UnsubscribeToDelegates();
    }

    private void FingerMoved(Vector3 position, int index)
    {
        UpdateSelectionBox();
        currentPosition = position;
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

        ShootRayCastToCheckForMovableObjects();
    }

    void ShootRayCastToCheckForMovableObjects()
    {

        //check to see if an object in movable objects is not in 
        RaycastHit[] objectsHitArray = Physics.BoxCastAll(selectionBox.position, new Vector3(selectionBox.localScale.x, selectionBox.localScale.y, selectionBox.localScale.z), Vector3.down, Quaternion.identity, Mathf.Infinity);
        List<RaycastHit> objectsHit = new List<RaycastHit>();
        foreach (RaycastHit objectHit in objectsHitArray)
        {
            objectsHit.Add(objectHit);
        }
        for (int j = 0; j < movableObjects.Count; j++)
        {
            if (!objectsHit.Contains(movableObjects[j]))
            {
                Transform finalPar = GetFinalParent(movableObjects[j].transform);
                finalPar.GetComponentInChildren<MovableObjectStateMachine>().UnHighlight();
            }
        }
        for (int i = 0; i < objectsHit.Count; i++)
        {
            Transform finalParent = objectsHit[i].transform;
            while (finalParent.parent != null)
            {
                finalParent = finalParent.parent;

            }
            if (finalParent.GetComponentInChildren<MovableObjectStateMachine>() != null)
            {
                if (!movableObjects.Contains(objectsHit[i]))
                {
                    finalParent.GetComponentInChildren<MovableObjectStateMachine>().Highlight();
                    movableObjects.Add(objectsHit[i]);
                }
            }
        }


        for (int j = 0; j < movableObjects.Count; j++)
        {

        }
    }

    public Transform GetFinalParent(Transform transformSent)
    {
        Transform finalParent = transformSent;
        while (finalParent.parent != null)
        {
            finalParent = finalParent.parent;
        }
        return finalParent;
    }
}
