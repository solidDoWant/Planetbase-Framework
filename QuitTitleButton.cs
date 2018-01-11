using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class QuitTitleButton : TitleButton
    {
        public QuitTitleButton() : base("quit", true, true)
        {
        }

        public override void HandleAction(GameStateTitle gst)
        {
            Application.Quit();
        }
    }
}