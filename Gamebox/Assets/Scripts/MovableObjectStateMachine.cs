using Shared.UI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectStateMachine : MonoBehaviour
{
    public State state;
    public int id;
    Vector3 startingTouchPosition;
    Vector3 fingerMovePosition;
    Vector3 offset;
    float heldDownTimer = 0f;
    float heldDownThreshold = .5f;
    float doubleTapTimer = 0f;
    float doubleTapThreshold = .25f;
    [SerializeField] float distanceThreshold = 1f;
    Deck deck;
    public bool faceUp;
    bool lowering;
    bool snappingToThreeOnY;
    int numOfFingersOnCard = 0;
    PlayerContainer playerOwningCard;
    public enum State
    {
        Idle,
        Indeterminate,
        Selected,
        Moving
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
        this.id = -1;
        if (this.GetComponentInChildren<Deck>() != null)
        {
            deck = this.GetComponentInChildren<Deck>();
        }
        faceUp = true;
        lowering = true;
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
                break;
            case State.Selected:
                HandleSelected();
                HandleLowering();
                break;
            case State.Moving:
                Move();
                HandleRaising();
                break;
        }

        HandleFlipCard();
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
    }


    public void SetTouched(int id, Vector3 positionSent)
    {
        numOfFingersOnCard++;
        if (doubleTapTimer < doubleTapThreshold)
        {
            FlipObject();
        }
        doubleTapTimer = 0f;
        if (state == State.Selected)
        {
            state = State.Idle;
            return;
        }
        if (this.id != -1)
            return;

        if (state != State.Idle)
            return;
        this.id = id;
        if (playerOwningCard != null)
        {
            playerOwningCard.RemoveCardFromHand(this.gameObject);
        }
        startingTouchPosition = positionSent;
        offset = new Vector3(this.transform.position.x - positionSent.x, 0, this.transform.position.z - positionSent.z);
        heldDownTimer = 0f;
        lowering = false;
        snappingToThreeOnY = true;
        SubscribeToDelegates();
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

    void SetStateToIdle()
    {

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

    public void MeasureDistanceUntilRelease(int id, Vector3 offset)
    {
        if (this.id != -1)
            return;
        if (state != State.Idle)
            return;
    }

    void QuickRelease()
    {
        state = State.Selected;
        Debug.Log(state);
    }

    #region delegates
    private void SubscribeToDelegates()
    {
        TouchScript.touchMoved += FingerMoved;
        TouchScript.fingerReleased += FingerReleased;
    }
    private void UnsubscribeToDelegates()
    {
        TouchScript.touchMoved -= FingerMoved;
        TouchScript.fingerReleased -= FingerReleased;
    }
    private void FingerReleased(Vector3 position, int index)
    {
        numOfFingersOnCard--;
        if (id != index) return;
        if (deck != null)
        {
            deck.CheckToSeeIfDeckShouldBeAdded();
        }
        lowering = true;
        snappingToThreeOnY = false;
        this.id = -1;
        UnsubscribeToDelegates();
        if (state == State.Indeterminate)
        {
            QuickRelease();
        }
        if (state == State.Moving)
        {
            state = State.Idle;
        }
    }

    private void FingerMoved(Vector3 position, int index)
    {
        if (id != index) return;
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
}
