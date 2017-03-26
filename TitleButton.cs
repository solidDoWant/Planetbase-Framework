using System;
using System.Collections.Generic;

namespace PlanetbaseFramework
{
    public abstract class TitleButton
    {
        public String name { get; }
        public Boolean enableGUI { get; }
        public Boolean isBaseGameTitleButton { get; }

        private static LinkedList<TitleButton> allTitleButtons = new LinkedList<TitleButton>();

        static TitleButton()
        {
            new NewGameTitleButton();
            new ContinueGameTitleButton();
            new LoadGameTitleButton();
            new TutorialTitleButton();
            new ChallengesTitleButton();
            new SettingsTitleButton();
            new QuitTitleButton();
        }

        protected internal TitleButton(String nameKey, Boolean enableGUI, Boolean isBaseGameTitleButton)
        {
            this.name = Planetbase.StringList.get(nameKey);
            this.enableGUI = enableGUI;
            this.isBaseGameTitleButton = isBaseGameTitleButton;

            allTitleButtons.AddLast(this);
        }

        public TitleButton(String nameKey, Boolean enableGUI) : this(nameKey, enableGUI, false)
        {
            
        }

        public static LinkedList<TitleButton> getAllTitleButtons()
        {
            return allTitleButtons;
        }

        public abstract void handleAction(Planetbase.GameStateTitle gst);
    }
}
