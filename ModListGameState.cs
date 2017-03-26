using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    class ModListGameState : GameState
    {
        public GuiRenderer mGuiRenderer = new GuiRenderer();
        public override bool isTitleState()
        {
            return true;
        }

        public override void onGui()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                return;
            }

            printLine("Loaded Mods:", 0);
            for(int i = 0; i < Modloader.modList.Count; i++)
            {
                printLine(Modloader.modList[i].ModName, i + 1);
            }

            if(this.mGuiRenderer.renderBackButton(new Vector2(Screen.width - GuiRenderer.getMenuButtonSize(FontSize.Huge).x, Screen.height - GuiRenderer.getMenuButtonSize(FontSize.Huge).y))){
                GameManager.getInstance().setGameStateTitle();
            }
        }

        private void printLine(string text, int lineNumber)
        {
            Vector2 textLocation = new Vector2(50, 80);
            GUIStyle labelStyle = mGuiRenderer.getLabelStyle(FontSize.Huge, FontStyle.Bold, TextAnchor.LowerLeft, FontType.Title);
            labelStyle.normal.textColor = Color.blue;

            GUI.Label(new Rect(textLocation.x, textLocation.y + (GuiRenderer.getMenuButtonSize(FontSize.Huge).y )* lineNumber, Screen.width, GuiRenderer.getMenuButtonSize(FontSize.Huge).y - 30), text, labelStyle);

            labelStyle.normal.textColor = Color.white;
        }
    }
}
