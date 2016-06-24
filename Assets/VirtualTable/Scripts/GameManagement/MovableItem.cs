using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable {
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
    }
}