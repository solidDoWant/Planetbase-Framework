using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class QuitTitleButton : TitleButton
    {
        public QuitTitleButton() : base("quit", true)
        {
        }

        public override void handleAction(GameStateTitle gst)
        {
            Application.Quit();
        }
    }
}
