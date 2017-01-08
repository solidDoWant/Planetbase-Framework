using Planetbase;

namespace PlanetbaseFramework
{
    class NewGameTitleButton : TitleButton
    {
        public NewGameTitleButton() : base("new_game", true)
        {
        }

        public override void handleAction(GameStateTitle gst)
        {
            if (gst.canAlreadyPlay())
                GameManager.getInstance().setGameStateLocationSelection();
            else
                gst.renderTutorialRequestWindow(new GuiDefinitions.Callback(gst.onWindowCancelNewGame));
        }
    }
}
