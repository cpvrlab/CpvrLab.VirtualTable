using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


namespace CpvrLab.VirtualTable
{
    /// <summary>
    /// Used for a networked transform tree
    /// </summary>
    public class NetworkedTransformTree : NetworkBehaviour
    {

        public Transform root;
        private Transform[] _children;
        

        // todo: optimize the syncing by only syncing the rotations that actually changed!
        //       
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if(_children == null)
                _children = (Transform[])root.GetComponentsInChildren<Transform>();

            if (initialState)
            {
                // always write initial state, no dirty bits
            }
            else if (syncVarDirtyBits == 0)
            {
                writer.WritePackedUInt32(0);
                return false;
            }
            else
            {
                // dirty bits
                writer.WritePackedUInt32(1);
            }
            
            // todo: we should probably be compressing this data?
            for(int i = 0; i < _children.Length; i++)            
                writer.Write(_children[i].rotation);
            

            return true;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (_children == null)
                _children = (Transform[])root.GetComponentsInChildren<Transform>();

            if (isServer && NetworkServer.localClientActive)
                return;

            if (!initialState)
            {
                if (reader.ReadPackedUInt32() == 0)
                    return;
            }

            for (int i = 0; i < _children.Length; i++)
                _children[i].rotation = reader.ReadQuaternion();
        }
    }

}