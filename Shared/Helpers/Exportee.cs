using System.Diagnostics;

namespace EventTagGenerator.Helpers;

public class Exportee
{
    private static List<General> ALlList { get; set; }

    public static void ExportTags()
    {
        ALlList = new List<General>();
        var csvFilePath = Path.Combine("Files", "DevFestTagExport2023.csv");
        var exportFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "DevFestTags");
        if (!File.Exists(csvFilePath)) { }

        #region Populate List

        using (var reader = new StreamReader(csvFilePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                var general = new General
                {
                    FirstName = values[0].ToLower().ToTitle(),
                    LastName = values[1].ToLower().ToTitle(),
                    Footer = values[4],
                    Index = int.Parse(values[5])
                };
                general.TRoleType = general.Footer switch
                {
                    "Attendee" => RoleType.Attendee,
                    "Speaker" => RoleType.Speaker,
                    "Organizer" => RoleType.Organizer,
                    "Volunteer" => RoleType.Volunteer,
                    "Partner" => RoleType.Partner,
                    _ => general.TRoleType
                };
                general.Summary = $"{values[3]}, {values[2]}";
                if (string.IsNullOrEmpty(values[3]))
                    general.Summary = values[2];
                if (string.IsNullOrEmpty(values[2]))
                    general.Summary = values[3];

             
                ALlList.Add(general);
            }
        }

        #endregion

        #region Sort

        ALlList.Where(d => d.TRoleType == RoleType.Volunteer).OrderBy(d => d.Index);

        #endregion

        #region Render

        if (!Directory.Exists(exportFolder))
            Directory.CreateDirectory(exportFolder);

        PointF firstNameLocation = new PointF(55f, 280f);
        PointF lastNameLocation = new PointF(55f, 320f);
        PointF summaryLocation = new PointF(55f, 380f);
        PointF tagNumberLocation = new PointF(240f, 415f);

        FontCollection collection = new FontCollection();
        FontFamily family = collection.Add(Path.Combine("Files", "Fonts", "Roboto-Medium.ttf"));
        Font font = family.CreateFont(30, FontStyle.Regular);
        Font summaryFont = family.CreateFont(14, FontStyle.Regular);

        var nameColor = Color.Black;
        var summaryColor = Color.FromRgb(101, 109, 116);
        int counter = 0;
        foreach (var item in ALlList)
        {
            var tagNumber = item.Index.ToString("000");
            var templateFile = item.TRoleType.GetTemplate();
            var exportFileName = Path.Combine(exportFolder, $"Tag{tagNumber}.png");
            var footerColor = item.TRoleType.GetColor();

            //write on the photo now. 

            using (Image image = Image.Load(templateFile))
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(item.FirstName))
                        image.Mutate(x => x.DrawText(item.FirstName, font, nameColor, firstNameLocation));

                    if (!string.IsNullOrWhiteSpace(item.LastName))
                        image.Mutate(x => x.DrawText(item.LastName, font, nameColor, lastNameLocation));


                    if (!string.IsNullOrWhiteSpace(item.Summary))
                        image.Mutate(x => x.DrawText(item.Summary, summaryFont, summaryColor, summaryLocation));

                    if (item.TRoleType != RoleType.Organizer || item.TRoleType != RoleType.Volunteer)
                        image.Mutate(x => x.DrawText(tagNumber, font, footerColor, tagNumberLocation));

                    image.Save(exportFileName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            counter++;
            Console.WriteLine($"Done with {counter} images.");
        }

        Console.WriteLine($"Done with {counter} images. Check {exportFolder} for files");
        OpenPathInExplorer(exportFolder);
        #endregion
    }
    
    public static void OpenPathInExplorer(string path)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();

        startInfo.FileName = "explorer"; // On Windows
        startInfo.Arguments = path;

        if (OperatingSystem.IsMacOS())
        {
            // Use the "open" command on macOS
            startInfo.FileName = "open";
            startInfo.Arguments = $"\"{path}\"";
        }

        process.StartInfo = startInfo;
        process.Start();
    }
}