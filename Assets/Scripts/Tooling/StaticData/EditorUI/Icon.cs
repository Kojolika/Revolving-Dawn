using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData.Data
{
    public class Icon : VisualElement
    {
        public const int Width  = 16;
        public const int Height = 16;

        public Icon(string iconPath)
        {
            Add(new Image
            {
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath),
                style =
                {
                    alignSelf = Align.Center,
                    width     = Width,
                    minWidth  = Width,
                    height    = Height,
                    minHeight = Height,
                }
            });
        }
    }

    public class ButtonIcon : VisualElement
    {
        private const int ButtonPadding = 2;
        private const int ExtraSpace    = 4;

        public ButtonIcon(Action clickEvent, string iconPath)
        {
            var icon = new Icon(iconPath);
            var button = new Button(clickEvent)
            {
                style =
                {
                    alignContent  = Align.Center,
                    alignItems    = Align.Center,
                    paddingLeft   = ButtonPadding,
                    paddingBottom = ButtonPadding,
                    paddingRight  = ButtonPadding,
                    paddingTop    = ButtonPadding,
                    minWidth      = Icon.Width + ButtonPadding + ExtraSpace,
                    minHeight     = Icon.Height + ButtonPadding + ExtraSpace,
                    maxWidth      = Icon.Width + ButtonPadding + ExtraSpace,
                    maxHeight     = Icon.Height + ButtonPadding + ExtraSpace,
                    marginLeft    = 0,
                    marginRight   = 0,
                    marginTop     = 0,
                    marginBottom  = 0,
                }
            };
            button.Add(icon);
            Add(button);

            style.marginBottom = 4;
            style.marginTop    = 4;
            style.marginLeft   = 4;
            style.marginRight  = 4;
            style.minWidth     = button.style.minWidth;
        }
    }
}