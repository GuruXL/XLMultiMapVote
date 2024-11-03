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
                isNavigationDisabled = true; 
            }
        }
        private void EnableNav()
        {
            Navigation nav = inputField.navigation;
            if (nav.mode == Navigation.Mode.None)
            {
                nav.mode = Navigation.Mode.Explicit;
                inputField.navigation = nav;
                isNavigationDisabled = false;
            }
        }
        public void OnSelect(BaseEventData eventData)
        {
            if (inputField.text.Length == 0)
            {
                EnableNav();
            }
        }
        public void OnDeselect(BaseEventData eventData)
        {
            EnableNav();
        }

        public void OnCancel(BaseEventData eventData)
        {
            EnableNav();
        }
        public void OnSubmit(BaseEventData eventData)
        {
            EnableNav();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (inputField.text.Length == 0)
            {
                EnableNav();
            }
        }
    }
}