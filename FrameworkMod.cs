﻿using System;
using Planetbase;

namespace PlanetbaseFramework
{
    public class FrameworkMod : ModBase
    {
        public const string AssemblyVersion = "2.3.2.0";
        public new static readonly Version ModVersion = new Version(AssemblyVersion);
        public const string DefaultModName = "Planetbase Framework";

        public FrameworkMod()
        {
            Utils.ErrorTexture = ModTextures.Find(x => x.name.Equals("error.png"));
        }

        public override string ModName { get; } = DefaultModName;

        public override void Init()
        {
            RegisterTitleButton(new ModTitleButton());

            InjectPatches();
        }

        private class ModTitleButton : TitleButton
        {
            public ModTitleButton() : base("fmod_titlemenu") { }

            public override void HandleAction(GameStateTitle gst)
            {
                GameManager.getInstance().setNewState(new ModListGameState());
            }
        }
    }
}