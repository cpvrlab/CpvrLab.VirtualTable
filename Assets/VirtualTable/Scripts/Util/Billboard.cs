using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable
{

    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            if (Camera.main == null)
                return;
            
            Vector3 position = transform.position + Camera.main.transform.rotation * Vector3.forward;
            transform.LookAt(position, Camera.main.transform.rotation * Vector3.up);
        }
    }

}