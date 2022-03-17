using Gameboard.EventArgs;
using Gameboard.Examples;
using Gameboard.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameboard.Utilities
{
    public class PlayerPresenceController : UserPresenceSceneObject
    {

        public List<PlayerPresenceDrawer> playerPresenceDrawers = new List<PlayerPresenceDrawer>();
        [SerializeField] UserPresenceTool userPresenceTool;
        public static PlayerPresenceController singleton;

        // Start is called before the first frame update
        void Start()
        {
            if (singleton != null)
            {
                Destroy(this);
            }
            singleton = this;
            userPresenceTool = FindObjectOfType<UserPresenceTool>();
            userPresenceTool.RequestUserPresenceUpdate();
        }



        // Update is called once per frame
        void Update()
        {
            UpdateWithRecievedUserPresence();
        }

        private void UpdateWithRecievedUserPresence()
        {
            Queue<GameboardUserPresenceEventArgs> presenceUpdates = userPresenceTool.DrainQueue();
            lock (playerPresenceDrawers)
            {
                int presenceCount = presenceUpdates.Count;

                for (int i = 0; i < presenceCount; i++)
                {
                    Debug.Log("Presence count " + presenceCount);

                }
            }
        }
    }
}

