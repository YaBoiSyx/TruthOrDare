
using AvocadoVR.TOD.Runtime.Manager;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace AvocadoVR.TOD.Runtime.Interact
{
    public class Dare : UdonSharpBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        
        public override void Interact()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            Networking.SetOwner(player, gameObject);
            Networking.SetOwner(player, _gameManager.gameObject);
            _gameManager.Dare();
        }
    }
}
