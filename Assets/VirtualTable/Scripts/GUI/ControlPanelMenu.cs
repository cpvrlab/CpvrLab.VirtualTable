using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable
{

    public class ControlPanelMenu : MonoBehaviour
    {
        public GameObject localPlayerSettings;
        public GameObject SpectatorSettings;
        public GameObject GameSettings;
        public GameObject adminSettings;
        public GameObject container;

        public float[] heights;

        public float edgeActivationRange = 10.0f;


        private bool _isVisible = false;
        private Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Start()
        {

        }

        void Update()
        {
            UpdateVisible();
        }

        void UpdateVisible()
        {
            var x = Input.mousePosition.x;
            var y = Input.mousePosition.y;

            if (_isVisible)
            {
                var rect = container.GetComponent<RectTransform>().rect;
                if (x < 0.0f || rect.width < x || y < 0.0f || rect.height < y)
                    HideMenu();
            } else
            {
                if (!(x < 0.0f || edgeActivationRange < x || y < 0.0f || Screen.height < y))
                    ShowMenu();
            }
        }
        

        void HideMenu()
        {
            Debug.Log("hide");
            _isVisible = false;
            //_animator.Stop();
            _animator.SetBool("isVisible", _isVisible);
        }

        void ShowMenu()
        {
            Debug.Log("show");
            _isVisible = true;
            //_animator.Stop();
            _animator.SetBool("isVisible", _isVisible);
        }
    }

}