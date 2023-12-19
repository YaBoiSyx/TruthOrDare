using AvocadoVR.TOD.Runtime.Interact;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace AvocadoVR.TOD.Runtime.Manager
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class CategoriesManager : UdonSharpBehaviour
    {
        [Header("Required Udon")] [SerializeField]
        private GameManager gameManager;

        [Header("Custom Set Components")] [SerializeField]
        private VRCUrlInputField customTodUrl;
        [HideInInspector] public TextMeshProUGUI customSet; // Name of set & who its by.
        [SerializeField] private PrebakedSet[] _customSets;

        private VRCPlayerApi _player;
        [UdonSynced] private bool _IsMasterLocked = true;

        private VRCUrl[] customSets;

        [UdonSynced] private VRCUrl _tempUrl;
        private int currentPage;

        void Start()
        {
            _player = Networking.LocalPlayer;
            _Calculate();
        }

        public void _Calculate()
        {
            var entries = customSets.Length;
            
        }

        public void EnableRateLimit() => customTodUrl.interactable = false;
        public void RateLimit() => customTodUrl.interactable = true;

        public void Load(int index)
        {
            if (_IsMasterLocked && !_player.isMaster) return;
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EnableRateLimit));

            _tempUrl = customSets[index];
            
            RequestSerialization();
            SyncURL();
        }

        public void _Request()
        {
            if (_IsMasterLocked && !_player.isMaster) return;
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EnableRateLimit));
            
            _tempUrl = customTodUrl.GetUrl();
            RequestSerialization();
            SyncURL();
        }

        public void SyncURL()
        {
            Networking.SetOwner(_player, gameManager.gameObject);
            gameManager.LoadURL(customTodUrl.GetUrl());
            SendCustomEventDelayedSeconds(nameof(RateLimit), 10);
        }

        public override void OnDeserialization()
        {
            SyncURL();
        }
    }

}
