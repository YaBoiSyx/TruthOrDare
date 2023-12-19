
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace AvocadoVR.TOD.Runtime.Interact
{
    public class PrebakedSet : UdonSharpBehaviour
    {
        [HideInInspector] public int Index;
        [SerializeField] private TextMeshProUGUI _setText;


        public void UpdateText(string name) => _setText.text = name;
        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);
    }
}
