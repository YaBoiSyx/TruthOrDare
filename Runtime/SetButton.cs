
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using VRC.SDKBase;
using VRC.Udon;


namespace Lastation.TOD
{
    public class SetButton : UdonSharpBehaviour
    {
        public URLLoader UIController;
        public TODSetContainer assignedSet;
        public TextMeshProUGUI buttonText;

        public void OnClicked()
        {
            UIController.LoadSetDataContainer(assignedSet);
            //loads the set data for this button instance into the UI
        }

        //sets the container this button is for on start()
        public void SetTODSetContainer(TODSetContainer InputWorldData)
        {
            assignedSet = InputWorldData;
            buttonText.text = assignedSet.setName;
        }
    }
}
