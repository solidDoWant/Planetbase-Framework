using Planetbase;

namespace PlanetbaseFramework
{
    public class TutorialTitleButton : TitleButton
    {
        public TutorialTitleButton() : base("tutorial", true, true)
        {
        }

        public override void HandleAction(GameStateTitle gst)
        {
            GameManager.getInstance().setGameStateGameNew(1, 0, true, (Challenge)null);
        }
    }
}