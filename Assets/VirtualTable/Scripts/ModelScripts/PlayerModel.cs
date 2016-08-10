using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CpvrLab.VirtualTable
{
    /// <summary>
    /// Base class for visual player representations. GamePlayer contains multiple of
    /// these for local and remote clients to use.
    /// </summary>
    public abstract class PlayerModel : MonoBehaviour
    {
        /// <summary>
        /// The name tag billboard that will normally be displayed above the player models head.
        /// </summary>
        public Text playerText;

        public abstract void RenderPreview(RenderTexture target);
        /// <summary>
        /// Called by the owning GamePlayer
        /// </summary>
        /// <param name="player"></param>
        public abstract void InitializeModel(GamePlayer player);
        
    }

}