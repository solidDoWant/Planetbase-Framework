using Planetbase;
using UnityEngine;
using System.Collections.Generic;

namespace PlanetbaseFramework
{
    //This class was thrown together fairly quickly, but should be a decent example on making new gamestates.
    //qp: followed instructions above..
    public class ModListGameState : GameState
    {
        private GuiRenderer Renderer { get; } = new GuiRenderer();

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

            List<IModMetaData> datas = ModManager.getInstance().GetMetaDatas();
            PrintLine("Loaded Mods:", 0);
            for(var i = 0; i < datas.Count; i++)
            {
                string name = datas[i].GetName();
                string text = string.Format("{0} - ({1})", name, datas[i].GetVersion().ToString());
                if (name.Equals(Utils.GetFrameworkMod().ModName))
                    PrintLine(text, i + 1, (ModManager.getInstance().isModActive(name)) ? Color.green : Color.red, FontSize.Normal);
                else
                    PrintLine(text, i + 1, (ModManager.getInstance().isModActive(name)) ? Color.green : Color.gray, FontSize.Normal, name);
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

        private void PrintLine(string text, int lineNumber) {
            PrintLine(text, lineNumber, Color.blue);
        }

        private void PrintLine(string text, int lineNumber, Color color, FontSize size = FontSize.Huge, string modName = null)
        {
            float deltaY = (GuiRenderer.getMenuButtonSize(FontSize.Huge).y);
            
            if (modName != null) {
                Vector2 startLocation = new Vector2(50, 200);
                Rect rect = new Rect(startLocation.x, startLocation.y + deltaY * lineNumber, GuiRenderer.getMenuButtonSize(FontSize.Huge).x - 30, GuiRenderer.getMenuButtonSize(FontSize.Huge).y - 30);
                float aspect = rect.width / rect.height;
                GUIStyle btnStyle = Singleton<GuiStyles>.getInstance().getBigTextButtonStyle(FontSize.Normal, aspect);
                Color c = btnStyle.normal.textColor;
                btnStyle.normal.textColor = color;
                if (Renderer.renderTitleButton(rect, text, FontSize.Normal)) {
                    ModManager.getInstance().setModActive(modName, !(color == Color.green));
                }
                btnStyle.normal.textColor = c;
            }
            else {
                Vector2 startLocation = new Vector2(55, 200);
                GUIStyle labelStyle = Renderer.getLabelStyle(size, FontStyle.Bold, TextAnchor.LowerLeft, FontType.Title);
                labelStyle.normal.textColor = color;
                GUI.Label(new Rect(startLocation.x, startLocation.y + deltaY * lineNumber, Screen.width, GuiRenderer.getMenuButtonSize(FontSize.Huge).y - 30), text, labelStyle);

                labelStyle.normal.textColor = Color.white;
            }
        }
    }
}