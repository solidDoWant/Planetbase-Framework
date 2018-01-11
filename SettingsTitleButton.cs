using Planetbase;

namespace PlanetbaseFramework
{
    public class SettingsTitleButton : TitleButton
    {
        public SettingsTitleButton() : base("settings", true, true)
        {
        }

        public override void HandleAction(GameStateTitle gst)
        {
            GameManager.getInstance().setGameStateSettings();
        }
    }
}