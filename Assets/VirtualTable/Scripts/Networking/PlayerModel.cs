using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CpvrLab.VirtualTable
{

    public abstract class PlayerModel : MonoBehaviour
    {
        public Text playerText;

        public abstract void RenderPreview(RenderTexture target);
        /// <summary>
        /// Called by the owning GamePlayer
        /// </summary>
        /// <param name="player"></param>
        public abstract void InitializeModel(GamePlayer player);
    }

}