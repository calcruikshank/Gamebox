using Gameboard.EventArgs;
using Gameboard.Tools;
using Gameboard.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameboard.Examples{

    public class PlayerPresenceDrawer : UserPresenceSceneObject
    {
        // Start is called before the first frame update
        void Start()
        {
           // userPresenceObserver.OnUserPresence += OnUserPresence;
        }

        void OnUserPresence(GameboardUserPresenceEventArgs userPresence)
        {
            // Handle User Presence here.
        }
    }
}
