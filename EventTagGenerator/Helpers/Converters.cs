using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using EventTagGenerator.Model;

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
            const string root = "Files";
            return Path.Combine(root, Enum.GetName(typeof(RoleType), role));
        }
        
        public static Brush GetColor(this RoleType role)
        {
            switch (role)
            {
                case RoleType.Attendee:
                    return new SolidBrush(Color.FromArgb(85, 122, 185));
                    break;
                case RoleType.Organizer:
                    return new SolidBrush(Color.FromArgb(242, 189, 68));
                    break;
                case RoleType.Speaker:
                    return new SolidBrush(Color.FromArgb(220, 69, 59));
                    break;
                case RoleType.Volunteer:
                    return new SolidBrush(Color.FromArgb(203, 231, 209));
                    break;
                default:
                    return new SolidBrush(Color.Black);
            }
        }
    }
}