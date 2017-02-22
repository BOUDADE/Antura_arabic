﻿using EA4S.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EA4S.PlayerBook
{
    /// <summary>
    /// Button that allows access to the PlayerBook.
    /// </summary>
    // refactor: should be grouped with Map scripts
    public class OpenPlayerBookScene : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            AppManager.I.NavigationManager.GoToPlayerBook();
        }

    }
}