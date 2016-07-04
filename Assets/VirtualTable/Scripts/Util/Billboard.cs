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

            transform.LookAt(Camera.main.transform.position);
        }
    }

}