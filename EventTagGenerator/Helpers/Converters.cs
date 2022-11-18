using System;
using System.Globalization;
using System.IO;
using EventTagGenerator.Model;
using SixLabors.ImageSharp;

namespace EventTagGenerator.Helpers
{
    public static class Converters
    {
        public static string ToTitle(this string input)
        {
            var culture = CultureInfo.InvariantCulture;
            return culture.TextInfo.ToTitleCase(input);
        }

        public static string GetTemplate(this RoleType role)
        {
            return Path.Combine("Files","Templates", $"{Enum.GetName(typeof(RoleType), role)}.png");
        }
        
        public static Color GetColor(this RoleType role)
        {
            switch (role)
            {
                case RoleType.Attendee:
                    return Color.FromRgb(85, 122, 185);
                    break;
                case RoleType.Organizer:
                    return Color.FromRgb(242, 189, 68);
                    break;
                case RoleType.Speaker:
                    return Color.FromRgb(220, 69, 59);
                    break;
                case RoleType.Volunteer:
                    return Color.FromRgb(203, 231, 209);
                    break;
                default:
                    return Color.Black;
            }
        }
    }
}