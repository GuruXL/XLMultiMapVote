using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLMultiMapVote.Utils
{
	public class CustomFixFirstSelected : MonoBehaviour
	{
		public GameObject selected;

		public GameObject[] fallbackSelectables;

		public bool cacheLastSelected = true;

		private bool doSet;

		private void OnEnable()
		{
			EventSystem.current.SetSelectedGameObject(null);
			if (selected == null)
			{
				selected = EventSystem.current.firstSelectedGameObject;
			}
			doSet = true;
		}

		private void Update()
		{
			if (doSet)
			{
				Selectable selectable = ((selected == null) ? null : selected.GetComponent<Selectable>());
				if (selected == null || !selected.activeInHierarchy || (selectable != null && !selectable.interactable))
				{
					selected = Enumerable.FirstOrDefault(fallbackSelectables, delegate (GameObject s)
					{
						Selectable component = s.GetComponent<Selectable>();
						return s.activeInHierarchy && (component == null || component.interactable);
					});
				}
				EventSystem.current.SetSelectedGameObject(selected);
				Main.Logger.Log("Current Selected SeT To: " + selected.name);
			}
			doSet = false;
		}

		private void OnDisable()
		{
			if (cacheLastSelected && EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.activeSelf && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(base.transform))
			{
				selected = EventSystem.current.currentSelectedGameObject;
			}
		}

	}
}