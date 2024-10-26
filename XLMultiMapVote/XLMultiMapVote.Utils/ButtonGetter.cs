using System.Linq;
using UnityEngine;

namespace XLMultiMapVote.Utils
{
   public static class ButtonGetter
   {
        public static GameObject GetMultiplayerMenuButton()
        {
            //var firstActiveGO = MultiplayerManager.Instance.menuController.mainMenu.options
            //.FirstOrDefault(go => go.buttonGO.GetComponent<MenuButton>());

            //var firstActiveGO = MultiplayerManager.Instance.menuController.mainMenu.options
            //.LastOrDefault(go => go.buttonGO.GetComponent<MenuButton>());

            var firstActiveGO = MultiplayerManager.Instance.menuController.mainMenu.options
                .ElementAtOrDefault(7);

            return firstActiveGO?.buttonGO;
        }

    }
}