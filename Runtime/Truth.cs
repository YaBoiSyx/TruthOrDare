using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Lastation.TOD
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Truth : UdonSharpBehaviour
    {
        [SerializeField] private GameManager _gameManager;

        public override void Interact()
        {
            _gameManager.Truth();
        }
    }
}
