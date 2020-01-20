using System;
using UnityEngine;
using Planetbase;

namespace PlanetbaseFramework
{
    class GuiRenderer : Planetbase.GuiRenderer
    {

        public bool renderBigButton(Rect rect, string text, FontSize fontSize, GUIStyle style, SoundDefinition sound = null)
        {
            if (style == null)
            {
                float aspect = rect.width / rect.height;
                style = (aspect <= 1.5f) ? Singleton<GuiStyles>.getInstance().getBigButtonStyle(fontSize, aspect) : Singleton<GuiStyles>.getInstance().getBigTextButtonStyle(fontSize, aspect);
            }
            if (!GUI.Button(rect, text, style))
            {
                return false;
            }
            Singleton<AudioPlayer>.getInstance().play((sound != null) ? sound : SoundListMenu.getInstance().ButtonClick, null);
            return true;
        }

        public bool renderTitleButton(Rect rect, string text, FontSize fontSize, GUIStyle style, bool playSound = false)
        {
            return this.renderBigButton(rect, text, fontSize, style, null);
        }

    }
}


