using UnityEngine;
using UnityEngine.Events;

namespace CpvrLab.VirtualTable
{

    public class VectorEvent : UnityEvent<Vector3> { }

    public class Shootable : MonoBehaviour
    {
        public VectorEvent OnHit = new VectorEvent();
        

        public void Hit(Vector3 position)
        {
            if(OnHit != null)
                OnHit.Invoke(position);
        }
    }

}