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

    bool faceUp = true;
    bool hasReachedTargetRotation = true;
    public Vector3 targetRotation;
    private void Start()
    {
        if (GetFinalParent().GetComponent<Deck>() != null)
        {
            deck = GetFinalParent().GetComponent<Deck>();
        }
        targetRotation = transform.GetChild(0).transform.localEulerAngles;
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
        TouchScript.flipObject += FlipObject;
        TouchScript.rotateRight += RotateObject;
        TouchScript.rotateLeft += RotateObjectLeft;
        this.offset = offset;
        Transform targetToMove = GetFinalParent();
        if (targetToMove.transform.parent == null)
        {
            targetToMove.eulerAngles = new Vector3(0, targetToMove.eulerAngles.y, 0);
            if (targetToMove.GetComponentInChildren<CardTilter>() != null)
            {
                targetToMove.GetComponentInChildren<CardTilter>().SetRotationToNotZero();
            }
            if (targetToMove.GetComponentInChildren<Deck>() != null)
            {
                targetToMove.GetComponentInChildren<Deck>().SetSelected(id, offset);
                targetToMove.GetComponentInChildren<Deck>().PickUpCards(1);
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
        if (!hasReachedTargetRotation)
        {
            Vector3 angles = transform.GetChild(0).localEulerAngles;
           
            if (angles.y != targetRotation.y)
            {
                if (targetRotation.y < 0)
                {
                    targetRotation = new Vector3(targetRotation.x, 270, targetRotation.z);
                }
                angles.y = Mathf.MoveTowards(angles.y, targetRotation.y, 1500f * Time.deltaTime);
                
            }
            transform.GetChild(0).localEulerAngles = angles;

            if (angles.y == targetRotation.y)
            {
                if (targetRotation.y >= 360)
                {
                    targetRotation = new Vector3(targetRotation.x, 0, targetRotation.z);
                }
                
                hasReachedTargetRotation = true;
            }

        }
    }
    public void FingerReleased(Vector3 position, int index)
    {
        if (id != index)
        {
            return;
        }
        lowering = true;
        snappingToOneOnY = false;
        this.id = -1;
        if (deck != null)
        {
            deck.CheckToSeeIfDeckShouldBeAdded();
        }
        TouchScript.touchMoved -= FingerMoved;
        TouchScript.fingerReleased -= FingerReleased;
        TouchScript.flipObject -= FlipObject;
        TouchScript.rotateRight -= RotateObject;
        TouchScript.rotateLeft -= RotateObjectLeft;
        Transform targetToMove = GetFinalParent();
        if (targetToMove.transform.parent == null)
        {
            if (targetToMove.GetComponentInChildren<Deck>() != null)
            {
                targetToMove.GetComponentInChildren<Deck>().SetUnselected(id, offset);
            }
        }
    }

    public void Unsub()
    {
        this.id = -1;
        TouchScript.touchMoved -= FingerMoved;
        TouchScript.fingerReleased -= FingerReleased;
    }

    public int GetId()
    {
        return id;
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
            targetToMove.position = targetPosition;
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
            targetToMove.position = Vector3.MoveTowards(targetToMove.position, new Vector3(targetToMove.position.x, 3, targetToMove.position.z), .05f * 1000 * Time.deltaTime);
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
            targetToMove.position = Vector3.MoveTowards(targetToMove.position, new Vector3(targetToMove.position.x, lowestPointHit, targetToMove.position.z), .05f * 1000 * Time.deltaTime);
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
        Collider colliderHit = transform.GetComponentInChildren<Collider>();
        RaycastHit hit;
        bool hitDetected = Physics.BoxCast(colliderHit.bounds.center, new Vector3(colliderHit.bounds.extents.x, colliderHit.bounds.extents.y, colliderHit.bounds.extents.z), Vector3.down, out hit, Quaternion.identity, Mathf.Infinity);

        if (hitDetected)
        {
            if (hit.transform.GetComponent<Collider>() != null && this.transform.GetComponentInChildren<Collider>() != null)
            {
                return hit.transform.GetComponent<Collider>().bounds.extents.y + hit.transform.position.y + this.transform.GetComponentInChildren<Collider>().bounds.extents.y;
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


    public void FlipObject(Vector3 position, int index)
    {
        if (faceUp)
        {
            transform.GetChild(0).localEulerAngles = new Vector3(270, targetRotation.y, targetRotation.z);
            faceUp = false;
        }
        else
        {
            transform.GetChild(0).localEulerAngles = new Vector3(90, targetRotation.y, targetRotation.z);
            faceUp = true;
        }
    }
    public void RotateObject(Vector3 position, int index)
    {
        targetRotation = new Vector3(targetRotation.x, targetRotation.y + 90, targetRotation.z);

        hasReachedTargetRotation = false;
    }
    public void RotateObjectLeft(Vector3 position, int index)
    {
        targetRotation = new Vector3(targetRotation.x, targetRotation.y - 90, targetRotation.z);

        hasReachedTargetRotation = false;
    }
}
