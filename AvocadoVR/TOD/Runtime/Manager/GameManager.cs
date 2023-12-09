
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

namespace AvocadoVR.TOD.Runtime.Manager
{
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GameManager : UdonSharpBehaviour
    {
        [Header("Required Udon")]
        [SerializeField] private CategoriesManager categoriesManager;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlaceholderManager placeholderManager;
        [Space] 
        [SerializeField] private VRCUrl defaultURL;
        [Space]
        [Header("Game Components")]
        [SerializeField] private TextMeshProUGUI question;
        [SerializeField] private TextMeshProUGUI player;
        [Space]
        [Header("Game Settings")] 
        public bool usePlaceholders;
        public bool getAllPlayers;

        #region Variables & Data

        // Truth & Dares
        [UdonSynced] private string[] _truths = new[] { "null" };
        [UdonSynced] private string[] _dares = new[] { "null" };
        [UdonSynced] private string[] _pTruths = new[] { "null" };
        [UdonSynced] private string[] _pDares = new[] { "null" };

        // Custom Set Logic
        [UdonSynced] private string _customSetName;
        [UdonSynced] private string _customSetBy;

        // Game Logic & Data
        [UdonSynced] private int _type;
        [UdonSynced] private string _question;
        [UdonSynced] private int _id;
        [UdonSynced] private int _playerID;

        private VRCPlayerApi _player;
        
        #endregion
        

        void Start()
        {
            _player = Networking.LocalPlayer;
            VRCStringDownloader.LoadUrl(defaultURL, (IUdonEventReceiver)this);
        }

        public void LoadURL(VRCUrl url)
        {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

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
        
        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            string[] sections = result.Result.Split(new string[] { "#name=", "#by=", "#type=truth_reg", "#type=truth_ph", "#type=dare_reg", "#type=dare_ph" }, StringSplitOptions.RemoveEmptyEntries);
            
            _customSetName = sections[0];
            _customSetBy = sections[1];
            _truths = sections[2].Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            _pTruths = sections[3].Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            _dares = sections[4].Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            _pDares = sections[5].Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            _type = 5;
            RequestSerialization();
            _Update();
        }
        
        public override void OnStringLoadError(IVRCStringDownload result)
        {
            
        }

        public void _Update()
        {
            if (_type != 5) player.text = VRCPlayerApi.GetPlayerById(_playerID).displayName;
            
            switch (_type)
            {
                case 1:
                    _question = _truths[_id];
                    question.text = _truths[_id];
                    break;
                case 2:
                    _question = _pTruths[_id];
                    _question.Replace(placeholderManager.playerPlaceholder, playerManager.GetRandomPlayer());
                    question.text = _pTruths[_id];
                    break;
                case 3:
                    _question = _dares[_id];
                    question.text = _dares[_id];
                    break;
                case 4:
                    _question = _pDares[_id];
                    _question.Replace(placeholderManager.playerPlaceholder, playerManager.GetRandomPlayer());
                    question.text = _pDares[_id];
                    break;
                case 5:
                    categoriesManager.customSet.text = $"{_customSetName}" + " By " + $"{_customSetBy}";
                    break;
            }
        }

        public override void OnDeserialization()
        {
            _Update();
        }
    }
}
