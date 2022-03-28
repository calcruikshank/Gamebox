using Gameboard.EventArgs;
using Gameboard.Tools;
using Gameboard.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameboard.Examples{

    public class PlayerPresenceDrawer : UserPresenceSceneObject
    {
        protected override void ScenePositionUpdated(Vector3 inNewPosition) 
        {
            this.transform.position = inNewPosition;
            Debug.Log("UpdatedPosition = " + inNewPosition);
        }
        protected override void LocalEulerAnglesUpdated(Vector3 inNewEulers)
        {
            this.transform.localEulerAngles = inNewEulers;
        }
        protected override void PlayerAdded() 
        {
            Debug.Log("Adding Player");
        }
        protected override void PlayerRemoved()
        {
            Debug.Log("Player Removed");
            Destroy(this.gameObject);
        }
        protected override void PlayerNameChanged()
        {

            Debug.Log("Player name changed");
        }
        protected override void PlayerColorChanged() 
        {
            Debug.Log("Player Color Changed ");
        }

    }


}
