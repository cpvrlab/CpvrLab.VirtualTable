using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable
{

    /// <summary>
    /// Generic score board that can hold any kind of table data and sync over the network
    /// </summary>
    public class ScoreBoard : NetworkBehaviour
    {
        private int _rows;
        private int _columns;

        private bool _dirty = false;

        private string _title;
        private List<string> _headers;
        private List<List<string>> _rowData;

        [Server] public void AddRow(string[] data)
        {
            var row = new List<string>(data);
            _rowData.Add(new List<string>());
        }

        [Server] public void SetRowData(int rowNum, string[] data)
        {
            for (int i = 0; i < data.Length; i++)
                SetFieldData(rowNum, i, data[i]);
        }

        [Server] public void SetFieldData(int rowNum, int colNum, string data)
        {
            if (_rowData[rowNum] == null || _rowData[rowNum][colNum] == null)
                return;

            _rowData[rowNum][colNum] = data;
            _dirty = true;
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            // todo:    make sure all clients see the same data!
            return base.OnSerialize(writer, initialState);
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            // todo:    make sure all clients see the same data!
            base.OnDeserialize(reader, initialState);
        }

        /// <summary>
        /// very simple debug output of the scoreboard content for testing purposes
        /// </summary>
        public void DebugDisplay()
        {
            Debug.Log("------------------------------------------");
            Debug.Log("------------{ " + _title + " }------------");
            Debug.Log("------------------------------------------");

            // print headers
            string line = "| ";
            foreach (var val in _headers)
                line += val + " | ";
            Debug.Log(line);

            // print content
            foreach(var row in _rowData)
            {
                line = "| ";
                foreach (var val in row)
                    line += val + " | ";
                Debug.Log(line);
            }

            Debug.Log("------------------------------------------");
        }
    }

}