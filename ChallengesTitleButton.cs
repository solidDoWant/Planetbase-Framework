using Planetbase;

namespace PlanetbaseFramework
{
    public class ChallengesTitleButton : TitleButton
    {
        public ChallengesTitleButton() : base("challenges", true, true)
        {
        }

        public override void handleAction(GameStateTitle gst)
        {
            if (gst.canAlreadyPlay())
            {
                GameManager.getInstance().setGameStateChallengeSelection();
            }
            else
            {

                gst.renderTutorialRequestWindow(new GuiDefinitions.Callback(gst.onWindowCancelChallenges));
            }
        }
    }
}
