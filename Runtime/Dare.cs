using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Lastation.TOD
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Dare : UdonSharpBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        
        public override void Interact()
        {
            _gameManager.Dare();
        }
    }
}
