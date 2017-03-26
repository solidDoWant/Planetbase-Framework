using System;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    class FrameworkMod : ModBase
    {
        public FrameworkMod() : base("Planetbase Framework")
        {
        }
        public override void Init()
        {
            ModTitleButton modTitleButton = new ModTitleButton();
        }

        private class ModTitleButton : TitleButton
        {
            public ModTitleButton() : base("mod_titlemenu", true)
            {
            }

            public override void handleAction(GameStateTitle gst)
            {
                GameManager.getInstance().setNewState(new ModListGameState());
            }
        }
    }
}
