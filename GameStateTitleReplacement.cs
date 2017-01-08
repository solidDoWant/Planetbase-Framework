using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    class GameStateTitleReplacement : Planetbase.GameStateTitle
    {
        public GameStateTitleReplacement(GameState previousState) : base(previousState)
        {
        }

        public override void onGui()
        {
            if (Input.GetKey(KeyCode.Space))
                return;
            if (this.mGuiRenderer == null)
                this.mGuiRenderer = new GuiRenderer();
            ResourceList instance1 = ResourceList.getInstance();
            TitleTextures title = instance1.Title;
            Texture2D gameTitle = title.GameTitle;
            Vector2 menuButtonSize = GuiRenderer.getMenuButtonSize(FontSize.Huge);
            Vector2 titleLocation = Singleton<TitleScene>.getInstance().getTitleLocation();
            Vector2 menuLocation = Singleton<TitleScene>.getInstance().getMenuLocation();
            float height1 = (float)(Screen.height * gameTitle.height) / 1080f;
            float width1 = height1 * (float)gameTitle.width / (float)gameTitle.height;
            GUI.color = new Color(1f, 1f, 1f, this.mAlpha);
            GUI.DrawTexture(new Rect(titleLocation.x - width1 * 0.5f, titleLocation.y, width1, height1), (Texture)gameTitle);
            GUI.color = Color.white;
            Texture2D backgroundRight = title.BackgroundRight;
            float height2 = (float)(Screen.height * backgroundRight.height) / 1080f;
            float width2 = height2 * (float)backgroundRight.width / (float)backgroundRight.height;
            GUI.DrawTexture(new Rect((float)Screen.width - width2 + this.mRightOffset, (float)(((double)Screen.height - (double)height2) * 0.75), width2, height2), (Texture)backgroundRight);
            float y = menuLocation.y;
            float num1 = menuButtonSize.y * 1.3f;
            menuLocation.x -= menuButtonSize.x;
            menuLocation.x += this.mRightOffset;

            foreach (TitleButton button in TitleButton.getAllTitleButtons())
            {
                GUI.enabled = button.allowEnableGUI();

                if (this.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, y, menuButtonSize.x, menuButtonSize.y), button.getName(), FontSize.Huge, true))
                {
                    button.handleAction(this);
                }

                y += num1;
            }

            if (this.mConfirmWindow != null)
            {
                this.mGuiRenderer.renderWindow((GuiWindow)this.mConfirmWindow, (GuiDefinitions.Callback)null);

            }

            ServerInterfaceManager instance2 = Singleton<ServerInterfaceManager>.getInstance();
            int num2 = !instance2.isWorkshopEnabled() ? 3 : 4;
            float num3 = menuButtonSize.y * 0.75f;
            float num4 = menuButtonSize.y * 0.25f;
            Vector2 vector2 = new Vector2((float)(((double)Screen.width - (double)num2 * (double)num3 - (double)(num2 - 1) * (double)num4) * 0.5), (float)((double)Screen.height - (double)num3 - (double)num4 + (double)this.mRightOffset * 0.5));
            Rect rect = new Rect(vector2.x, vector2.y, num3, num3);
            if (instance2.isWorkshopEnabled())
            {
                if (this.mGuiRenderer.renderButton(rect, new GUIContent((string)null, (Texture)instance1.Icons.Workshop, StringList.get("steam_workshop")), (SoundDefinition)null))
                    instance2.openWorkshop();
                rect.x += num3 + num4;
            }
            if (this.mGuiRenderer.renderButton(rect, new GUIContent((string)null, (Texture)instance1.Icons.Credits, StringList.get("credits")), (SoundDefinition)null))
                GameManager.getInstance().setGameStateCredits();
            rect.x += num3 + num4;
            if (this.mGuiRenderer.renderButton(rect, new GUIContent((string)null, (Texture)instance1.Icons.SwitchPlanet, StringList.get("switch_planet")), (SoundDefinition)null))
                Singleton<TitleScene>.getInstance().switchPlanet();
            rect.x += num3 + num4;
            if (this.mGuiRenderer.renderButton(rect, new GUIContent((string)null, (Texture)instance1.Icons.Achievements, StringList.get("achievements")), (SoundDefinition)null))
                GameManager.getInstance().setGameStateAchievements();
            rect.x += num3 + num4;
            this.mGuiRenderer.renderTooltip();
        }
    }
}
