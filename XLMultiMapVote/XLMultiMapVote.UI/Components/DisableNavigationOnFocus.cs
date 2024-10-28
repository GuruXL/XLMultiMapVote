using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace XLMultiMapVote.UI.Components
{
    public class DisableNavigationOnFocus : MonoBehaviour, ISelectHandler, IDeselectHandler, ICancelHandler, ISubmitHandler, IPointerClickHandler
    {
        private TMP_InputField inputField;
        private bool isNavigationDisabled = false;

        private void Start()
        {
            inputField = GetComponent<TMP_InputField>();
            inputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            inputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string text)
        {
            if (text.Length > 0 && !isNavigationDisabled)
            {
                DisableNav();
            }
        }
        private void DisableNav()
        {
            Navigation nav = inputField.navigation;
            if (nav.mode != Navigation.Mode.None)
            {
                nav.mode = Navigation.Mode.None;
                inputField.navigation = nav;
                isNavigationDisabled = true; // Mark that navigation is now disabled
            }
        }
        private void EnableNav()
        {
            Navigation nav = inputField.navigation;
            if (nav.mode == Navigation.Mode.None)
            {
                nav.mode = Navigation.Mode.Explicit; // Or whatever mode suits the UI layout
                inputField.navigation = nav;
                isNavigationDisabled = false; // Mark that navigation is now enabled
            }
        }
        public void OnSelect(BaseEventData eventData)
        {
            if (inputField.text.Length == 0)
            {
                EnableNav(); // Ensure navigation is enabled when selected if the field is empty
            }
        }
        public void OnDeselect(BaseEventData eventData)
        {
            EnableNav(); // Enable navigation when deselected
        }

        public void OnCancel(BaseEventData eventData)
        {
            EnableNav(); // Enable navigation if canceled
        }
        public void OnSubmit(BaseEventData eventData)
        {
            EnableNav(); // Enable navigation if submitted
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (inputField.text.Length == 0)
            {
                EnableNav(); // Make sure navigation is enabled if the user clicks and the input is empty
            }
        }
    }
}