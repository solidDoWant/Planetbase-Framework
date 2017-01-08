using System;
using System.Collections.Generic;

namespace PlanetbaseFramework
{
    public abstract class TitleButton
    {
        private String name;
        private Boolean enableGUI;

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

        public TitleButton(String nameKey, Boolean enableGUI)
        {
            this.name = Planetbase.StringList.get(nameKey);
            this.enableGUI = enableGUI;

            allTitleButtons.AddLast(this);
        }

        public String getName()
        {
            return this.name;
        }

        public Boolean allowEnableGUI()
        {
            return this.enableGUI;
        }

        public static LinkedList<TitleButton> getAllTitleButtons()
        {
            return allTitleButtons;
        }

        public abstract void handleAction(Planetbase.GameStateTitle gst);
    }
}
