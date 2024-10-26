using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Rewired;

namespace XLMultiMapVote.UI.Components
{
    public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float scrollSpeed = 10f;
        private bool mouseOver = false;

        private List<Selectable> m_Selectables = new List<Selectable>();
        private ScrollRect m_ScrollRect;
        private Dropdown m_DropDown;

        private Vector2 m_NextScrollPosition = Vector2.up;
        public int RewiredPlayerID = 0;
        private Player rePlayer;

        void OnEnable()
        {
            if (m_ScrollRect)
            {
                m_Selectables.Clear();
                m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
            }
        }
        void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
            m_DropDown = GetComponent<Dropdown>();
            //remove this line if not using Rewired
            rePlayer = ReInput.players.GetPlayer(RewiredPlayerID);
        }
        void Start()
        {
            if (m_ScrollRect)
            {
                m_Selectables.Clear();
                m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
            }
            ScrollToSelected(true);
        }
        void Update()
        {
            if (!m_ScrollRect || !m_ScrollRect.gameObject.activeSelf)
                return;

            // Scroll via input.
            if (InputScrollDetected())
            {
                ScrollToSelected(false);
            }

            if (!mouseOver)
            {
                // Lerp scrolling code.
                m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
            }
            else
            {
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
        }
        private bool InputScrollDetected()
        {
            if (m_Selectables.Count > 0)
            {
                return rePlayer.GetAxis("MoveHorizontal") != 0.0f || rePlayer.GetAxis("MoveVertical") != 0f
                    || Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f;
            }
            return false;
        }

        void ScrollToSelected(bool quickScroll)
        {
            int selectedIndex = -1;
            Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

            if (selectedElement)
            {
                selectedIndex = m_Selectables.IndexOf(selectedElement);
            }
            if (selectedIndex > -1)
            {
                float normalizedIndex = 1 - (selectedIndex / ((float)m_Selectables.Count - 1));
                normalizedIndex = Mathf.Clamp01(normalizedIndex);

                if (quickScroll)
                {
                    m_ScrollRect.normalizedPosition = new Vector2(0, normalizedIndex);
                    m_NextScrollPosition = m_ScrollRect.normalizedPosition;
                }
                else
                {
                    m_NextScrollPosition = new Vector2(0, normalizedIndex);
                }
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
            ScrollToSelected(false);
        }
    }
}
