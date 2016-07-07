using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {

    /// <summary>
    /// Base class for any item that can be picked up by the player. 
    /// 
    /// note:   at the time of writing this comment this class is still unfinished and unused.
    /// 
    /// todo:   there is a lot of shared functionality between UsableItem and MovableItem
    ///         we should consider to maybe combine the two into a single base class.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class MovableItem : MonoBehaviour {
        // todo:    implement functionality to be grabbed
        //          one should be able to grab an object already held by someone
        //          else. Or for example pass an object from one hand to an other
        //          the object should remember who's holding it and provide functions to
        //          be picked up.
        //          (or should it not?)

        // todo:    furthermore we should be able to restrict an item to only be picked up
        //          by a specific GamePlayer. However that functionality should be implemented
        //          in a child class. The base class should provide the necessary methods
        //          to achieve that altered functionality.

        public void Attach(Rigidbody rb)
        {
            
        }

        public void Detach(Rigidbody rb)
        {

        }
    }
}