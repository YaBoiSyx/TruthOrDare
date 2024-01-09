
using System;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using Random = UnityEngine.Random;

namespace Lastation.TOD
{
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GameManager : UdonSharpBehaviour
    {
        [Header("Required Udon")]
        [SerializeField] private URLLoader categoriesManager;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private string playerPlaceholder = "<player>";

        [Space] 
        
        [SerializeField] private VRCUrl defaultURL;
        [Space]
        [Header("Game Components")]
        [SerializeField] private TextMeshProUGUI questionDisplayedText;
        [SerializeField] private TextMeshProUGUI playerDisplayedText;
        [Space]
        [Header("Game Settings")] 
        public bool usePlaceholders;

        #region Variables & Data

        // Truth & Dares
        private string[] _truths = new[] { "null" };
        private string[] _dares = new[] { "null" };
        private string[] _pTruths = new[] { "null" };
        private string[] _pDares = new[] { "null" };

        // Custom Set Logic
        private string _customSetName;
        private string _customSetBy;

        // Game Logic & Data
        private VRCPlayerApi _player;
        private int _type;
        private int _id;
        private string _question;
        [UdonSynced] public int _playerID;
        
        

        #endregion Variables & Data


        void Start()
        {
            _player = Networking.LocalPlayer;
            VRCStringDownloader.LoadUrl(defaultURL, (IUdonEventReceiver)this);
        }


        #region Truth
        public void Truth()
        {
            Networking.SetOwner(_player, gameObject);
            _playerID = _player.playerId;

            if (usePlaceholders && playerManager.PlayerCount >= 3)
            {
                switch (Chance())
                {
                    case false:
                        _id = Random.Range(0, _truths.Length);
                        _type = 1;
                        _Update();
                        break;
                    case true:
                        _id = Random.Range(0, _pTruths.Length);
                        _type = 2;
                        _Update();
                        break;
                }
            }
            else
            {
                _id = Random.Range(0, _truths.Length);
                _type = 1;
                _Update();
            }
            RequestSerialization();
        }
        #endregion Truth

        public void Dare()
        {
            Networking.SetOwner(_player, gameObject);
            _playerID = _player.playerId;

            if (usePlaceholders && playerManager.PlayerCount >= 3)
            {
                switch (Chance())
                {
                    case false:
                        _id = Random.Range(0, _dares.Length);
                        _type = 3;
                        _Update();
                        break;
                    case true:
                        _id = Random.Range(0, _pDares.Length);
                        _type = 4;
                        _Update();
                        break;
                }
            }
            else
            {
                _id = Random.Range(0, _dares.Length);
                _type = 3;
                _Update();
            }
            
            RequestSerialization();
        }

        public bool Chance()
        {
            int chance = Random.Range(1, 2);

            switch (chance)
            {
                case 1:
                    return false;
                case 2:
                    return true;
            }

            return false;
        }

        public void _Update()
        {
            if (_type != 5) playerDisplayedText.text = VRCPlayerApi.GetPlayerById(_playerID).displayName;
            
            switch (_type)
            {
                case 1:
                    _question = _truths[_id];
                    break;
                case 2:
                    _question = _pTruths[_id];
                    _question.Replace(playerPlaceholder, playerManager.GetRandomPlayer());
                    break;
                case 3:
                    _question = _dares[_id];
                    break;
                case 4:
                    _question = _pDares[_id];
                    _question.Replace(playerPlaceholder, playerManager.GetRandomPlayer());
                    break;
            }
            questionDisplayedText.text = _question;
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            _Update();
        }
    }
}
