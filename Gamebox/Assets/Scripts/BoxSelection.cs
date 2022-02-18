using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelection : MonoBehaviour
{
    private RectTransform selectionBox;
    private List<MovableObjectStateMachine> selectedMovableObjects = new List<MovableObjectStateMachine>();
    Vector3 startPosition;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void BeginDraggingGrid()
    {
        SubscribeToDelegates();
        Debug.Log("Begin draggin Grid");
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
        throw new NotImplementedException();
    }

    private void FingerMoved(Vector3 position, int index)
    {
        throw new NotImplementedException();
    }
}
