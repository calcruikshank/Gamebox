using Shared.UI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectStateMachine : MonoBehaviour
{
    public State state;
    public List<int> idList = new List<int>();
    Vector3 startingTouchPosition;
    Vector3 fingerMovePosition;
    Vector3 offset;
    float heldDownTimer = 0f;
    float heldDownThreshold = .25f;
    float doubleTapTimer = 0f;
    float doubleTapThreshold = .25f;
    float distanceThreshold = .25f;
    Deck deck;
    public bool faceUp;
    bool lowering;
    bool snappingToThreeOnY;
    int numOfFingersOnCard = 0;

    Vector3 startingTouchPositionFinger2;
    Vector3 fingerMovePosition2;
    PlayerContainer playerOwningCard;
    Vector3 targetRotation;
    Vector3 startingRotation;

    bool showSelectedWheel = false;
    GameObject selectedWheelGO;

    public bool boxSelected = false;
    public enum State
    {
        Idle,
        Indeterminate,
        Selected,
        Moving,
        Rotating
    }
    private void Awake()
    {
        InitializeMovableObject();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    private void InitializeMovableObject()
    {
        this.state = State.Idle;
        this.idList.Clear();
        if (this.GetComponentInChildren<Deck>() != null)
        {
            deck = this.GetComponentInChildren<Deck>();
        }
        faceUp = true;
        doubleTapTimer = doubleTapThreshold;
    }

    protected virtual void Update()
    {
        switch (state)
        {
            case State.Idle:
                HandleIdle();
                HandleLowering();
                break;
            case State.Indeterminate:
                CheckForInputCommands();
                CheckToSeeIfShouldBeginRotating();
                break;
            case State.Selected:
                HandleSelected();
                HandleLowering();
                break;
            case State.Moving:
                Move();
                HandleRaising();
                CheckToSeeIfShouldBeginRotating();
                break;
            case State.Rotating:
                Move();
                HandleRaising();
                HandleRotating();
                break;
        }

        HandleFlipCard();
    }

    private void CheckToSeeIfShouldBeginRotating()
    {
        if (idList.Count >= 2)
        {
            startingRotation = transform.GetChild(0).forward;
            state = State.Rotating;
        }
    }

    private void HandleRotating()
    {

        /*if (idList.Count >= 2)
        {
            Vector3 worldPosition = Camera.main.WorldToScreenPoint(transform.GetChild(0).position);
            Vector3 normalizedChildPosition = (fingerMovePosition - worldPosition).normalized;
            targetRotation = new Vector3(transform.GetChild(0).localEulerAngles.x, normalizedChildPosition.y, transform.GetChild(0).localEulerAngles.z);
            Debug.Log(fingerMovePosition2 + " " + targetRotation);
        }*/
        float speed = 1;
        //transform.GetChild(0).localEulerAngles = targetRotation;
        // Determine which direction to rotate towards
        if (fingerMovePosition2 != Vector3.zero)
        {
            Vector3 targetDirection = new Vector3(Camera.main.ScreenToWorldPoint(fingerMovePosition2).x - transform.GetChild(0).position.x, Camera.main.ScreenToWorldPoint(fingerMovePosition2).y - transform.GetChild(0).position.y, Camera.main.ScreenToWorldPoint(fingerMovePosition2).z - transform.GetChild(0).position.z);
            // The step size is equal to speed times frame time.
            float singleStep = speed * Time.deltaTime;

            // Draw a ray pointing at our target in
            //Vector3 newDirection = Vector3.RotateTowards(transform.GetChild(0).forward, targetDirection, singleStep, 0.0f);
            // Calculate a rotation a step closer to the target and applies rotation to this object
            if (faceUp)
            {
                transform.GetChild(0).forward = new Vector3(targetDirection.x, targetDirection.y * -1, targetDirection.z);
            }
            if (!faceUp)
            {
                transform.GetChild(0).forward = new Vector3(targetDirection.x, targetDirection.y, targetDirection.z);
            }
        }
    }

    void HandleFlipCard()
    {
        if (doubleTapTimer > doubleTapThreshold)
        {
            return;
        }
        doubleTapTimer += Time.deltaTime;
    }
    public void HandleIdle()
    {
    }

    void HandleSelected()
    {
        Vector3 targetScale = Vector3.one;
        if (showSelectedWheel)
        {
        }
    }


    public void SetTouched(int id, Vector3 positionSent)
    {
        if (!this.idList.Contains(id))
        {
            this.idList.Add(id);
        }
        if (state == State.Selected)
        {
            SetUnselected();
        }
        if (doubleTapTimer < doubleTapThreshold && idList.Count == 1)
        {
            SetSelected();
        }
        

        if (state != State.Idle)
            return;

        doubleTapTimer = 0f;
        if (playerOwningCard != null)
        {
            playerOwningCard.RemoveCardFromHand(this.gameObject);
        }

        if (this.transform.GetComponentInChildren<CardTilter>() != null)
        {
            this.transform.GetComponentInChildren<CardTilter>().SetRotationToNotZero();
        }
        SubscribeToDelegates();
        startingTouchPosition = positionSent;
        offset = new Vector3(this.transform.position.x - positionSent.x, 0, this.transform.position.z - positionSent.z);
        heldDownTimer = 0f;
        lowering = false;
        snappingToThreeOnY = true;
        state = State.Indeterminate;
    }
    public void FlipObject()
    {
        if (faceUp)
        {
            transform.GetChild(0).localEulerAngles = new Vector3(270, transform.GetChild(0).localEulerAngles.y, transform.GetChild(0).localEulerAngles.z);
            faceUp = false;
        }
        else
        {
            transform.GetChild(0).localEulerAngles = new Vector3(90, transform.GetChild(0).localEulerAngles.y, transform.GetChild(0).localEulerAngles.z);
            faceUp = true;
        }
    }
    public void CheckForInputCommands()
    {
        if (fingerMovePosition == Vector3.zero)
        {
            return;
        }
        heldDownTimer += Time.deltaTime;
        Vector3 differenceBetweenStartingPositionAndMovePosition = startingTouchPosition - fingerMovePosition;

        //if held down timer is greater than helddowntimerthreshold then start moving entire entity
        if (heldDownTimer >= heldDownThreshold)
        {
            LongPress();
        }
        //if differencebetweenstartingpositionandmoveposition is greater than distancethreshold then move top card only
        if (differenceBetweenStartingPositionAndMovePosition.magnitude > distanceThreshold)
        {
            QuickDrag();
        }
        //if release before either are triggered than bring up context menu and select 
    }
    void HandleLowering()
    {
        if (lowering)
        {
            SnapToLowestPointHit();
        }
    }
    void HandleRaising()
    {
        if (snappingToThreeOnY)
        {
            SnapPositionToThreeOnY();
        }
    }
    void Move()
    {
        Vector3 targetPosition = new Vector3(fingerMovePosition.x, this.transform.position.y, fingerMovePosition.z);
        targetPosition = targetPosition + offset;
        this.transform.position = targetPosition;
    }


    public void SnapPositionToThreeOnY()
    {
        Transform targetToMove = this.transform;
        if (targetToMove.transform.position.y == 3)
        {
            snappingToThreeOnY = false;
        }
        targetToMove.position = Vector3.MoveTowards(targetToMove.position, new Vector3(targetToMove.position.x, 3, targetToMove.position.z), .05f * 1000 * Time.deltaTime);
    }
    public void SnapToLowestPointHit()
    {
        Transform targetToMove = this.transform;

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

    public void LongPress()
    {
        state = State.Moving;
    }

    public void QuickDrag()
    {
        //Draw top card 
        if (deck != null)
        {
            deck.PickUpCards(1);
            state = State.Moving;
        }
    }

    void SetSelected()
    {
        ShowSelectedWheel();
        state = State.Selected;
    }
    void SetUnselected()
    {
        HideSelectedWheel();
        state = State.Idle;
    }
    void QuickRelease()
    {
        /*ShowSelectedWheel();
        state = State.Selected;
        Debug.Log(state);*/
        state = State.Idle;
    }
    
    void ShowSelectedWheel()
    {
        showSelectedWheel = true;
        SpawnInNewSelectedWheelTransform(this.transform.GetChild(0));
    }
    void HideSelectedWheel()
    {
        if (showSelectedWheel)
        {
            DestroySpecificSelectedWheel(this.transform);
            showSelectedWheel = false;
        }
    }

    #region delegates
    private void SubscribeToDelegates()
    {
        TouchScript.touchMoved += FingerMoved;
        TouchScript.fingerReleased += FingerReleased;
        TouchScript.rotateRight += RotateRight;
    }

    private void RotateRight(Vector3 position, int index)
    {
        targetRotation = new Vector3(transform.GetChild(0).localEulerAngles.x, transform.GetChild(0).localEulerAngles.y + 90, transform.GetChild(0).localEulerAngles.z);
        state = State.Rotating;
    }

    private void UnsubscribeToDelegates()
    {
        TouchScript.touchMoved -= FingerMoved;
        TouchScript.fingerReleased -= FingerReleased;
        TouchScript.rotateRight -= RotateRight;
    }
    private void FingerReleased(Vector3 position, int index)
    {
        
        Debug.Log(idList.Count);
        if (!idList.Contains(index)) return;
        if (idList.Count == 1)
        {
            UnsubscribeToDelegates();
            if (state == State.Indeterminate)
            {
                QuickRelease();
            }
            if (state == State.Moving || state == State.Rotating)
            {
                lowering = true;
                snappingToThreeOnY = false;
                state = State.Idle;
                ResetRotationOnX();
            }
            if (deck != null)
            {
                deck.CheckToSeeIfDeckShouldBeAdded();
            }
        }


        idList.Remove(index);
    }
    void ResetRotationOnX()
    {
        if (faceUp)
        {
            transform.GetChild(0).localEulerAngles = new Vector3(90, transform.GetChild(0).localEulerAngles.y, transform.GetChild(0).localEulerAngles.z);
        }
        if (!faceUp)
        {
            transform.GetChild(0).localEulerAngles = new Vector3(270, transform.GetChild(0).localEulerAngles.y, transform.GetChild(0).localEulerAngles.z);
        }
    }

    private void FingerMoved(Vector3 position, int index)
    {
        if (idList.Count >= 2)
        {
            if (index == idList[1])
            {
                fingerMovePosition2 = position;
            }
        }
        if (idList[0] != index) return;

        Ray ray = Camera.main.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            fingerMovePosition = raycastHit.point;
        }
    }
    #endregion

    public void RemovePlayerOwnership(PlayerContainer playerContainer)
    {
        playerOwningCard = null;
    }

    internal void GivePlayerOwnership(PlayerContainer playerContainer)
    {
        playerOwningCard = playerContainer;
    }
    public void SpawnInNewSelectedWheelTransform(Transform transformToSpawnOn)
    {
        selectedWheelGO = Instantiate(Crutilities.singleton.SelectedWheelTransform.gameObject, new Vector3(transformToSpawnOn.position.x, 4, transformToSpawnOn.position.z), Quaternion.identity);
    }
    public void DestroySpecificSelectedWheel(Transform transformToSpawnOn)
    {
        Destroy(selectedWheelGO);
    }

    public void Highlight()
    {
        if (boxSelected) return;
        ShowSelectedWheel();
        boxSelected = true;
    }
    public void UnHighlight()
    {
        HideSelectedWheel();
        boxSelected = false;
    }
}
