using Gameboard.EventArgs;
using Gameboard.Examples;
using Gameboard.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserPresenceTest : MonoBehaviour
{
    private List<PlayerPresenceDrawer> playerList = new List<PlayerPresenceDrawer>();
    private List<string> onScreenLog = new List<string>();
    [SerializeField]GameObject playerPresenceSceneObject;

    void Start()
    {
        GameObject presenceObserverObj = GameObject.FindWithTag("UserPresenceObserver");
        UserPresenceObserver userPresenceObserver = presenceObserverObj.GetComponent<UserPresenceObserver>();
        userPresenceObserver.OnUserPresence += OnUserPresence;
    }

    void OnUserPresence(GameboardUserPresenceEventArgs userPresence)
    {
        PlayerPresenceDrawer myObject = playerList.Find(s => s.userId == userPresence.userId);
        if (myObject == null)
        {
            Debug.Log("my object is not null");
            // Add it here, and when adding also populate myObject
            // If the user doesn't exist in our player list, add them now.
            if (playerList.Find(s => s.userId == userPresence.userId) == null)
            {

                Debug.Log(userPresence.userId + " user presence id");
                /*UserPresencePlayer testPlayer = new UserPresencePlayer()
                {
                    gameboardId = userPresence.userId
                };*/

                GameObject scenePrefab = Instantiate(playerPresenceSceneObject, userPresence.boardUserPosition.screenPosition, Quaternion.identity);


                myObject = scenePrefab.GetComponent<PlayerPresenceDrawer>();
                myObject.InjectDependencies(userPresence);

                if (!string.IsNullOrEmpty(userPresence.userId))
                {
                    myObject.InjectUserId(userPresence.userId);
                }


                playerList.Add(myObject);
                AddToLog("--- === New player added: " + userPresence.userId);
            }

        }
        if (myObject != null)
        {
            myObject.UpdateUserPresence(userPresence);
        }
        
    }


    void AddToLog(string logMessage)
    {
        onScreenLog.Add(logMessage);
        Debug.Log(logMessage);
    }

    void OnGUI()
    {
        foreach (string thisString in onScreenLog)
        {
            GUILayout.Label(thisString);
        }
    }

}
