using Photon.Pun;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using XLMultiMapVote;

namespace XLMultiMapVote.UI.Components
{
    public class SelectableSounds : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler
    {
        public void OnSelectSound()
        {
            UISounds.Instance.PlayOneShotSelectionChange();
        }

        public void OnSubmitSound() 
        {
            UISounds.Instance.PlayOneShotSelectMajor();
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectSound();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnSelectSound();
        }
        public void OnSubmit(BaseEventData eventData)
        {
            OnSubmitSound();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            OnSubmitSound();
        }
    }
}