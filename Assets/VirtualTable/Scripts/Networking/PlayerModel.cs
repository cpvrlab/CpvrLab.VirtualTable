using UnityEngine;
using System.Collections;

namespace CpvrLab.VirtualTable
{

    public abstract class PlayerModel : MonoBehaviour
    {
        public abstract void RenderPreview(RenderTexture target);
        /// <summary>
        /// Called by the owning GamePlayer
        /// </summary>
        /// <param name="player"></param>
        public abstract void InitializeModel(GamePlayer player);
    }

}