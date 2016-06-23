using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable {


    // todo:    Implement a custom line renderer that behaves like googles tilt brush line strokes
    //          The lines shouldn't try to orientate themselves towards the camera but just stay flat
    //          Or we could implement actual 3d brush strokes. Not sure yet. Has to be decided
    //          when we implement the light painting game.
    //          As of now this is just a proof of concept
    public class LightPainter : UsableItem {

        public float deltaPaint = 0.01f;
        public Transform paintPoint;
        public bool clearOnDrop = true;

        private List<Vector3> _currentLinePoints = new List<Vector3>();
        private LineRenderer _currentLine;
        private bool _pointsChanged;
        private List<GameObject> _lines = new List<GameObject>();
        private bool _drawing = false;


        void Start()
        {
            StartCoroutine(UpdateCurrentLine());
        }


        void Update()
        {
            if(_input == null)
                return;

            // todo: this seems tedious, can't we just subscribe to buttons and receive input?
            if(_input.GetActionDown(PlayerInput.ActionCode.Button0)) {
                // start a new line on button down
                StartNewLine();
            }

            _drawing = _input.GetAction(PlayerInput.ActionCode.Button0);

            if(_drawing) {
                var prevPosition = _currentLinePoints.Count > 0 ? _currentLinePoints[_currentLinePoints.Count - 1] : Vector3.zero;
                var pos = paintPoint.position;

                if(_currentLinePoints.Count == 0 || Vector3.Distance(prevPosition, pos) > deltaPaint) {
                    _pointsChanged = true;

                    _currentLinePoints.Add(pos);

                }
            }

            if(_input.GetAction(PlayerInput.ActionCode.Button1)) {
                Clear();
            }
        }

        void Clear()
        {
            foreach(var go in _lines) {
                DestroyImmediate(go);
            }
            _lines.Clear();
        }

        void StartNewLine()
        {
            _currentLinePoints.Clear();

            var go = new GameObject("Line");
            go.transform.SetParent(transform, false);

            _lines.Add(go);

            _currentLine = go.AddComponent<LineRenderer>();
            _currentLine.useWorldSpace = true;
            _currentLine.material = new Material(Shader.Find("Particles/Additive"));
            _currentLine.SetWidth(0.025F, 0.025F);
            var color = Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
            _currentLine.SetColors(color, color);
        }

        // clear the drawing when this item is dropped
        public override void OnUnequip()
        {
            base.OnUnequip();

            if(clearOnDrop)
                Clear();
        }

        IEnumerator UpdateCurrentLine()
        {
            while(true) {
                if(_currentLinePoints.Count < 1 || !_drawing)
                    yield return null;
                else {
                    // innefficient as hell
                    _currentLinePoints.Add(paintPoint.position);
                    _currentLine.SetVertexCount(_currentLinePoints.Count);
                    _currentLine.SetPositions(_currentLinePoints.ToArray());
                    _currentLinePoints.RemoveAt(_currentLinePoints.Count - 1);

                    _pointsChanged = false;

                    yield return null;
                }
            }
        }
    }

}