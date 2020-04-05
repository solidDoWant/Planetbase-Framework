using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    //This class was thrown together fairly quickly, but should be a decent example on making new gamestates.
    public class ModListGameState : GameState
    {
        private GuiRenderer Renderer { get; } = new GuiRenderer();

        public override bool isTitleState() => true;

        public override void onGui()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                return;
            }


            PrintLine("Loaded Mods:", 0);
            // This should be split to a method
            for (var i = 0; i < ModLoader.ModList.Count; i++)
            {
                PrintLine(ModLoader.ModList[i].ModName, i + 1);
            }

            if (Renderer.renderBackButton(
                new Vector2(
                    Screen.width - GuiRenderer.getMenuButtonSize(FontSize.Huge).x,
                    Screen.height - GuiRenderer.getMenuButtonSize(FontSize.Huge).y
                )
            ))
            {
                GameManager.getInstance().setGameStateTitle();
            }
        }

        private void PrintLine(string text, int lineNumber)
        {
            Vector2 textLocation = new Vector2(50, 80);
            GUIStyle labelStyle = Renderer.getLabelStyle(FontSize.Huge, FontStyle.Bold, TextAnchor.LowerLeft, FontType.Title);
            labelStyle.normal.textColor = Color.blue;

            GUI.Label(new Rect(textLocation.x, textLocation.y + (GuiRenderer.getMenuButtonSize(FontSize.Huge).y )* lineNumber, Screen.width, GuiRenderer.getMenuButtonSize(FontSize.Huge).y - 30), text, labelStyle);

            labelStyle.normal.textColor = Color.white;
        }
    }
}