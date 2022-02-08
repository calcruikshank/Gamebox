using Shared.UI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    int id = -1;
    Vector3 offset;
    bool snappingToOneOnY;
    bool lowering = false;
    Deck deck;

    private void Start()
    {
        if (GetFinalParent().GetComponent<Deck>() != null)
        {
            deck = GetFinalParent().GetComponent<Deck>();
        }
    }
    public void SetSelected(int id, Vector3 offset)
    {
        if (this.id != -1)
        {
            return;
        }
        this.id = id;
        snappingToOneOnY = true;
        lowering = false;
        TouchScript.touchMoved += FingerMoved;
        TouchScript.fingerReleased += FingerReleased;
        this.offset = offset;
        Transform targetToMove = GetFinalParent();
        if (targetToMove.transform.parent == null)
        {
            targetToMove.eulerAngles = new Vector3(0, targetToMove.eulerAngles.y, 0);
            if (targetToMove.GetComponentInChildren<CardTilter>() != null)
            {
                targetToMove.GetComponentInChildren<CardTilter>().SetRotationToNotZero();
            }
        }
    }
    private void Update()
    {
        if (snappingToOneOnY)
        {
            SnapPositionToOneOnY();
        }
        if (lowering)
        {
            SnapToLowestPointHit();
        }
    }
    private void FingerReleased(Vector3 position, int index)
    {
        lowering = true;
        snappingToOneOnY = false;
        this.id = -1;
        if (deck != null)
        {
            deck.CheckToSeeIfDeckShouldBeAdded();
        }
        TouchScript.touchMoved -= FingerMoved;
        TouchScript.fingerReleased -= FingerReleased;
    }

    private void FingerMoved(Vector3 position, int index)
    {
        if (id == index)
        {
            Ray ray = Camera.main.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                Move(raycastHit.point);
            }
        }
    }

    public void Move(Vector3 rayPoint)
    {
        Transform targetToMove = GetFinalParent();
        if (targetToMove.transform.parent == null)
        {
            Vector3 targetPosition = new Vector3(rayPoint.x + offset.x, targetToMove.position.y, rayPoint.z + offset.z);
            targetToMove.position = Vector3.MoveTowards(targetToMove.position, targetPosition, 1f);
        }
    }

    public void SnapPositionToOneOnY()
    {
        Transform targetToMove = GetFinalParent();
        if (targetToMove.transform.parent == null)
        {
            if (targetToMove.transform.position.y == 3)
            {
                snappingToOneOnY = false;
            }
            targetToMove.position = Vector3.MoveTowards(targetToMove.position, new Vector3(targetToMove.position.x, 3, targetToMove.position.z), .05f);
        }
    }
    public void SnapToLowestPointHit()
    {
        Transform targetToMove = GetFinalParent();
        if (targetToMove.transform.parent == null)
        {
            targetToMove.eulerAngles = new Vector3(0, targetToMove.eulerAngles.y, 0);
            if (targetToMove.GetComponentInChildren<CardTilter>() != null)
            {
                targetToMove.GetComponentInChildren<CardTilter>().SetRotationToZero();
            }
            float lowestPointHit = FindLowestPoint();
            targetToMove.position = Vector3.MoveTowards(targetToMove.position, new Vector3(targetToMove.position.x, lowestPointHit, targetToMove.position.z), .05f);
            if (targetToMove.transform.position.y == lowestPointHit)
            {
                lowering = false;
            }
        }
    }

    public Transform GetFinalParent()
    {
        Transform targetToMove = this.transform;
        while (targetToMove.parent != null)
        {
            targetToMove = targetToMove.transform.parent;
        }
        return targetToMove;
    }
    public float FindLowestPoint()
    {
        Collider colliderHit = transform.GetComponent<Collider>();
        RaycastHit hit;
        bool hitDetected = Physics.BoxCast(colliderHit.bounds.center, new Vector3(colliderHit.bounds.extents.x, colliderHit.bounds.extents.y, colliderHit.bounds.extents.z), Vector3.down, out hit, Quaternion.identity, Mathf.Infinity);

        if (hitDetected)
        {
            if (hit.transform.GetComponent<Collider>() != null && this.transform.GetComponent<Collider>() != null)
            {
                return hit.transform.GetComponent<Collider>().bounds.extents.y + hit.transform.position.y + this.transform.GetComponent<Collider>().bounds.extents.y;
            }
            else
            {
                return -.9f;
            }
        }
        else
        {
            return -.9f;
        }
    }
}
