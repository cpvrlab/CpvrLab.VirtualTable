using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace CpvrLab.VirtualTable {


    /// <summary>
    /// Simple "light painter" that allows the user to draw in the air.
    ///
    /// todo:    Implement a custom line renderer that behaves like googles tilt brush line strokes
    ///          The lines shouldn't try to orientate themselves towards the camera but just stay flat
    ///          Or we could implement actual 3d brush strokes. Not sure yet. Has to be decided
    ///          when we implement the light painting game.
    ///          As of now this is just a proof of concept
    /// 
    /// todo:    Network optimizations if needed: We currently send a command every time we add
    ///          a point to the line renderer. This could bog down the network. A better approach 
    ///          would be to use the position on each client to draw the line. And periodically
    ///          send a list of correct line points to the server for the other clients to 
    ///          correct their local drawings.
    /// </summary>
    public class LightPainter : UsableItem {

        public float deltaPaint = 0.01f;
        public Transform paintPoint;
        public bool clearOnDrop = true;

        private List<Vector3> _currentLinePoints = new List<Vector3>();
        private LineRenderer _currentLine;
        private bool _pointsChanged;
        private List<GameObject> _lines = new List<GameObject>();
        [SyncVar]
        private bool _drawing = false;
        private bool _lineChanged = false;


        public override void OnStartClient()
        {
            base.OnStartClient();
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

            bool drawing = _input.GetAction(PlayerInput.ActionCode.Button0);
            if(_drawing != drawing) {
                _drawing = drawing;
                CmdSetDrawing(_drawing);
            }

            if(_drawing) {
                var prevPosition = _currentLinePoints.Count > 0 ? _currentLinePoints[_currentLinePoints.Count - 1] : Vector3.zero;
                var pos = paintPoint.position;

                if(_currentLinePoints.Count == 0 || Vector3.Distance(prevPosition, pos) > deltaPaint) {
                    AddLinePoint(pos);
                }
            }

            if(_input.GetAction(PlayerInput.ActionCode.Button1)) {
                Clear();
            }
        }
        [Command] void CmdSetDrawing(bool value) { _drawing = value; }

        
        void Clear()
        {

            // todo: is it really necessary to immediately execute on local client?
            //       probably yes, but we should still make sure.
            ClearClient();
            CmdClear();
        }
        
        [Command] void CmdClear() { RpcClear(); }
        [ClientRpc] void RpcClear() { if(!hasAuthority) ClearClient(); }

        void ClearClient()
        {
            foreach(var go in _lines) {
                DestroyImmediate(go);
            }
            _lines.Clear();
        }

        void AddLinePoint(Vector3 position)
        {
            AddLinePointClient(position);
            CmdAddLinePoint(position);
        }
        [Command] void CmdAddLinePoint(Vector3 position) { RpcAddLinePoint(position); }
        [ClientRpc] void RpcAddLinePoint(Vector3 position) { if(!hasAuthority) AddLinePointClient(position); }
        void AddLinePointClient(Vector3 position)
        {
            _currentLinePoints.Add(position);
            _lineChanged = true;
        }

        void StartNewLine()
        {
            var color = Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
            StartNewLineClient(color);
            CmdStartNewLine(color);
        }

        [Command] void CmdStartNewLine(Color color) { RpcStartNewLine(color); }
        [ClientRpc] void RpcStartNewLine(Color color) { if(!hasAuthority) StartNewLineClient(color); }
        void StartNewLineClient(Color color)
        {
            _currentLinePoints.Clear();

            var go = new GameObject("Line");
            go.transform.SetParent(transform, false);

            _lines.Add(go);

            _currentLine = go.AddComponent<LineRenderer>();
            _currentLine.useWorldSpace = true;
            _currentLine.material = new Material(Shader.Find("Particles/Additive"));
            _currentLine.SetWidth(0.025F, 0.025F);
            _currentLine.SetColors(color, color);
        }

        // clear the drawing when this item is dropped
        protected override void OnUnequip()
        {
            base.OnUnequip();

            if(clearOnDrop)
                Clear();
        }

        IEnumerator UpdateCurrentLine()
        {
            while(true) {
                if(_currentLinePoints.Count > 0 && (_drawing || _lineChanged)) {
                    // innefficient as hell
                    _currentLinePoints.Add(paintPoint.position);
                    _currentLine.SetVertexCount(_currentLinePoints.Count);
                    _currentLine.SetPositions(_currentLinePoints.ToArray());
                    _currentLinePoints.RemoveAt(_currentLinePoints.Count - 1);

                    _lineChanged = false;
                }

                yield return null;
            }
        }
    }

}