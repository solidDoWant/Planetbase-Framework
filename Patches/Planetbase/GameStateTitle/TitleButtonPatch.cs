using System.Collections.Generic;
using Harmony;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.GameStateTitle
{
    [HarmonyPatch(typeof(global::Planetbase.GameStateTitle))]
    [HarmonyPatch("onGui")]
    public class TitleButtonPatch
    {
        public static List<TitleButton> RegisteredTitleButtons { get; } = new List<TitleButton>();
        
        public static void Postfix(global::Planetbase.GameStateTitle __instance)
        {
            //Render the background image
            var backgroundImage = ResourceList.getInstance().Title.BackgroundRight;
            var textureHeight = Screen.height * backgroundImage.height / 1080f;
            var textureWidth = textureHeight * backgroundImage.width / backgroundImage.height;
            GUI.DrawTexture(
                new Rect(
                    textureWidth - __instance.mRightOffset,
                    (float)((Screen.height - (double)textureHeight) * 0.75),
                    -textureWidth,
                    textureHeight
                ),
                backgroundImage
            );

            //Render the buttons
            var menuLocation = Singleton<global::Planetbase.TitleScene>.getInstance().getMenuLocation();
            var menuButtonSize = GuiRenderer.getMenuButtonSize(FontSize.Huge);
            menuLocation.x -= menuButtonSize.x;
            menuLocation.x += __instance.mRightOffset;

            var modY = menuLocation.y;
            var num1 = menuButtonSize.y * 1.3f;
            foreach (var button in RegisteredTitleButtons)
            {
                if (__instance.mGuiRenderer.renderTitleButton(
                    new Rect(
                        Screen.width - menuLocation.x - menuButtonSize.x,
                        modY,
                        menuButtonSize.x,
                        menuButtonSize.y
                    ),
                    button.Name,
                    FontSize.Huge)
                )
                {
                    button.HandleAction(__instance);
                }

                modY += num1;
            }
        }
    }
}