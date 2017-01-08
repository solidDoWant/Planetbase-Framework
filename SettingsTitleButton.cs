using Planetbase;

namespace PlanetbaseFramework
{
    class SettingsTitleButton : TitleButton
    {
        public SettingsTitleButton() : base("settings", true)
        {
        }

        public override void handleAction(GameStateTitle gst)
        {
            GameManager.getInstance().setGameStateSettings();
        }
    }
}
