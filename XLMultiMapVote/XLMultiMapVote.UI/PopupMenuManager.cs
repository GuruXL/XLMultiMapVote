using HarmonyLib;
using MapEditor;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using XLMultiMapVote.Data;

namespace XLMultiMapVote.UI
{
    public class PopupMenuManager : MonoBehaviour
    {
        MultiplayerGameModePopup popupMenu;
        RectTransform popupOptionsRect;

        private void Awake()
        {
            popupMenu = GetPopupMenu();
            popupOptionsRect = GetPopupOptionsRect();
        }
        private void Start()
        {
            SetPopUpOptionsRectSize(0, 720);
        }
        private MultiplayerGameModePopup GetPopupMenu()
        {
            MultiplayerGameModePopup menu = null;
            try
            {
                menu = MultiplayerManager.Instance.gameModePopup;
                return menu;
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Error Finding popupMenu - {ex.Message}");
                return null;
            }
            finally
            {
                if (menu != null)
                {
                    Main.Logger.Log("popupMenu has been found");
                }
                else
                {
                    Main.Logger.Error("menuButtonPrefab Not Found");
                }
            }
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
