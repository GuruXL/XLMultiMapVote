using HarmonyLib;
using System;
using GameManagement;
using System.Reflection;
using UnityEngine;
using XLMultiMapVote.State;

namespace XLMultiMapVote.UI
{
    
    public class PopupMenuManager : MonoBehaviour
    {
        GameState popUpState;

        RectTransform popupOptionsRect;

        public MultiplayerGameModePopup popupUI
        {
            get
            {
                return MultiplayerManager.Instance.gameModePopup;
            }
        }
        private void Awake()
        {
        }
        private void Start()
        {
            popUpState = popupUI.gameObject.AddComponent<PopUpState>();

            popupOptionsRect = GetPopupOptionsRect();
            SetPopUpOptionsRectSize(0, 720);
        }
        private RectTransform GetPopupOptionsRect() 
        {
            RectTransform rect = null;
            try
            {
                rect = MultiplayerManager.Instance.gameModePopup.optionsListView.gameObject.GetComponent<RectTransform>();
                return rect;
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Finding PopupOptionsRect - {ex.Message}");
                return null;
            }
            finally
            {
                if (rect != null)
                {
                    Main.Logger.Log("PopupOptionsRect has been found");
                }
                else
                {
                    Main.Logger.Error("PopupOptionsRect Not Found");
                }
            }
        }    
        private void SetPopUpOptionsRectSize(float x, float y)
        {
            if (popupOptionsRect == null)
                return;

            popupOptionsRect.sizeDelta = new Vector2(x, y);
        }

    }
}
