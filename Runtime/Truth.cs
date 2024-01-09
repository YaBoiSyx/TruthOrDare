using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Lastation.TOD
{
    public class Truth : UdonSharpBehaviour
    {
        [SerializeField] private GameManager _gameManager;

        public override void Interact()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            Networking.SetOwner(player, gameObject);
            Networking.SetOwner(player, _gameManager.gameObject);
            _gameManager.Truth();
        }
    }
}
