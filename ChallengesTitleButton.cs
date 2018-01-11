using Planetbase;

namespace PlanetbaseFramework
{
    public class ChallengesTitleButton : TitleButton
    {
        public ChallengesTitleButton() : base("challenges", true, true)
        {
        }

        public override void HandleAction(GameStateTitle gst)
        {
            if (gst.canAlreadyPlay())
            {
                GameManager.getInstance().setGameStateChallengeSelection();
            }
            else
            {

                gst.renderTutorialRequestWindow(gst.onWindowCancelChallenges);
            }
        }
    }
}