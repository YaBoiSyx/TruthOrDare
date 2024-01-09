using System.Net;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace Lastation.TOD
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class URLLoader : UdonSharpBehaviour
    {
        #region Variables & Data
        [SerializeField] private TODSetContainer[] _setContainers;

        [Space]

        [Header("Game Instance")]
        private GameManager gameManager;

        [Space]

        [Header("Button Instancing")]
        [SerializeField] private Transform _buttonParent;
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private Button[] _SetButtons;

        [Space]

        [Header("URL Input")]
        private VRCUrlInputField _urlInputField;

        [Space]

        [Header("Loaded Set Info")]
        [SerializeField] private TextMeshProUGUI _setname;
        [SerializeField] private TextMeshProUGUI _setby;

        //Private & Synced Variables
        private VRCPlayerApi _player;
        [UdonSynced] private bool _IsMasterLocked = true;
        [UdonSynced] private VRCUrl _tempUrl;
        #endregion Variables & Data

        #region Start & Serialization
        void Start()
        {
            _player = Networking.LocalPlayer;
            GenerateButtons();
            #region Button Caching
            _SetButtons = new Button[_setContainers.Length];
            for (int i = 0; i < _setContainers.Length; i++)
            {
                _SetButtons[i] = _setContainers[i].SetButton.GetComponent<Button>();
            }
            #endregion Button Caching
        }

        public override void OnDeserialization()
        {
            LoadURL(_tempUrl);
        }
        #endregion Start & Serialization

        #region Button Generation
        public void GenerateButtons()
        {
            foreach (TODSetContainer containerInstance in _setContainers)
            {
                if (containerInstance.SetButton == null)
                {
                    GameObject button = Instantiate(_buttonPrefab, _buttonParent);
                    button.GetComponent<SetButton>().SetTODSetContainer(containerInstance);
                    containerInstance.SetButton = button.GetComponent<SetButton>();
                    button.SetActive(true);
                }
            }
        }
        #endregion Button Generation

        #region URL Loading
        public void LoadURL(VRCUrl url)
        {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

        public void LoadSetDataContainer(TODSetContainer containerInstance)
        {
            Networking.SetOwner(_player, gameObject);
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EnableRateLimit));
            _tempUrl = containerInstance.VRCUrl;
            LoadURL(_tempUrl);
            RequestSerialization();
        }

        public void RequestURL()
        {
            if (_IsMasterLocked && !_player.isMaster) return;
            Networking.SetOwner(_player, gameObject);
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EnableRateLimit));
            _tempUrl = _urlInputField.GetUrl();
            LoadURL(_tempUrl);
            RequestSerialization();
        }
        #endregion URL Loading

        #region String Load Events
        public override void OnStringLoadSuccess(IVRCStringDownload WebRequest)
        {
            string json = WebRequest.Result;
            Debug.Log($"Successfully downloaded json {json}");

            if (VRCJson.TryDeserializeFromJson(json, out DataToken result))
            {
                Debug.Log($"Successfully deserialized as a list with {result.DataDictionary.Count} items.");

                result.DataDictionary.TryGetValue("SetName", out DataToken name);
                result.DataDictionary.TryGetValue("SetBy", out DataToken setBy);
                result.DataDictionary.TryGetValue("Truths", out DataToken truths);
                result.DataDictionary.TryGetValue("Dares", out DataToken dares);
                result.DataDictionary.TryGetValue("Player_Truths", out DataToken pTruths);
                result.DataDictionary.TryGetValue("Player_Dares", out DataToken pDares);

                //Import these values to the game manager and the URLLoader UI
                SendCustomEventDelayedSeconds(nameof(DisableRateLimit), 10);
            }

        }

        public override void OnStringLoadError(IVRCStringDownload WebRequest)
        {
            SendCustomEventDelayedSeconds(nameof(DisableRateLimit), 10);
        }
        #endregion String Load Events

        #region Rate Limiting
        public void EnableRateLimit()
        {
            _urlInputField.interactable = false;
            for (int i = 0; i < _SetButtons.Length; i++)
            {
                _SetButtons[i].interactable = false;
            }
        }

        public void DisableRateLimit()
        {
            _urlInputField.interactable = true;
            for (int i = 0; i < _SetButtons.Length; i++)
            {
                _SetButtons[i].interactable = true;
            }
        }

        #endregion Rate Limiting
    }

}
