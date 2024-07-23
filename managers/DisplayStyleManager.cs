using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IpisCentralDisplayController.Managers
{
    public class DisplayStyleManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _displayStylesKey = "displayStyles";

        public DisplayStyleManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<DisplayStyle> LoadDisplayStyles()
        {
            return _jsonHelper.Load<List<DisplayStyle>>(_displayStylesKey) ?? new List<DisplayStyle>();
        }

        public void SaveDisplayStyles(List<DisplayStyle> displayStyles)
        {
            _jsonHelper.Save(_displayStylesKey, displayStyles);
        }

        public void AddDisplayStyle(DisplayStyle displayStyle)
        {
            var displayStyles = LoadDisplayStyles();
            if (displayStyles.Any(ds => ds.StyleName == displayStyle.StyleName))
            {
                throw new Exception("DisplayStyle with this name already exists.");
            }
            displayStyles.Add(displayStyle);
            SaveDisplayStyles(displayStyles);
        }

        public void UpdateDisplayStyle(DisplayStyle displayStyle)
        {
            var displayStyles = LoadDisplayStyles();
            var existingDisplayStyle = displayStyles.FirstOrDefault(ds => ds.StyleName == displayStyle.StyleName);
            if (existingDisplayStyle == null)
            {
                throw new Exception("DisplayStyle not found.");
            }
            // Update displayStyle properties here
            existingDisplayStyle.Sno = displayStyle.Sno;
            existingDisplayStyle.Language = displayStyle.Language;
            existingDisplayStyle.FontSize = displayStyle.FontSize;
            existingDisplayStyle.FontWeight = displayStyle.FontWeight;
            existingDisplayStyle.FontStyle = displayStyle.FontStyle;
            existingDisplayStyle.MarginTop = displayStyle.MarginTop;
            existingDisplayStyle.MarginLeft = displayStyle.MarginLeft;
            existingDisplayStyle.AlignmentH = displayStyle.AlignmentH;
            existingDisplayStyle.AlignmentV = displayStyle.AlignmentV;

            SaveDisplayStyles(displayStyles);
        }

        public void DeleteDisplayStyle(string styleName)
        {
            var displayStyles = LoadDisplayStyles();
            var displayStyle = displayStyles.FirstOrDefault(ds => ds.StyleName == styleName);
            if (displayStyle == null)
            {
                throw new Exception("DisplayStyle not found.");
            }
            displayStyles.Remove(displayStyle);
            SaveDisplayStyles(displayStyles);
        }

        public void DeleteAllDisplayStyles()
        {
            var displayStyles = LoadDisplayStyles();
            displayStyles.Clear();
            SaveDisplayStyles(displayStyles);
        }

        public DisplayStyle FindDisplayStyleByName(string styleName)
        {
            var displayStyles = LoadDisplayStyles();
            return displayStyles.FirstOrDefault(ds => ds.StyleName == styleName);
        }
    }
}
