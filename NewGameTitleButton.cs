using Planetbase;

namespace PlanetbaseFramework
{
    public class NewGameTitleButton : TitleButton
    {
        public NewGameTitleButton() : base("new_game", true, true)
        {
        }

        public override void HandleAction(GameStateTitle gst)
        {
            if (gst.canAlreadyPlay())
                GameManager.getInstance().setGameStateLocationSelection();
            else
                gst.renderTutorialRequestWindow(gst.onWindowCancelNewGame);
        }
    }
}