using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EventTagGenerator.Helpers;
using EventTagGenerator.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace EventTagGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ExportTags();
        }

        private static List<General> ALlList { get; set; }
        private static void ExportTags()
        {
            ALlList = new List<General>();
            var csvFilePath = Path.Combine("Files","DevFestTagExport.csv");
            var exportFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "DevFestTags");
            if (!File.Exists(csvFilePath))
            {
                
            }
            #region Populate List
            using(var reader = new StreamReader(csvFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    General general = new General();
                    var name = values[0];
                    var nameSplit = name.Split();
                    general.FirstName = nameSplit[0].ToTitle();
                    if (nameSplit.Length > 1)
                    {
                        general.LastName = nameSplit[1].ToTitle();
                        if (nameSplit.Length > 2)
                        {
                            general.LastName += $" {nameSplit[2]}".ToTitle();
                        }
                    }
                    var summary = values[1];
                    summary = summary.Replace("Not a Student", string.Empty).Replace("~", ",");
                    general.Summary = summary;
                    
                    var footer = values[2];
                    general.Footer = footer;
                    general.TRoleType = footer switch
                    {
                        "Attendee" => RoleType.Attendee,
                        "Speaker" => RoleType.Speaker,
                        "Volunteer" => RoleType.Volunteer,
                        "Organizer" => RoleType.Organizer,
                        _ => general.TRoleType
                    };

                    //Remove duplicates. 
                    if(!ALlList.Any(d=>d.FirstName == general.FirstName && d.LastName == general.LastName))
                        ALlList.Add(general);
                }
            }
            #endregion

            #region Sort

            ALlList.Where(d => d.TRoleType == RoleType.Volunteer).OrderBy(d => d.Footer);
            

            #endregion
            #region Render

            
            
            if (!Directory.Exists(exportFolder))
                Directory.CreateDirectory(exportFolder);
            var counter = 1;
            
            PointF firstNameLocation = new PointF(45f, 220f);
            PointF lastNameLocation = new PointF(45f, 250f);
            PointF summaryLocation = new PointF(45f, 300f);
            PointF tagNumberLocation = new PointF(280f, 400f);

            FontCollection collection = new FontCollection();
            FontFamily family = collection.Add(Path.Combine("Files","Fonts","Roboto-Medium.ttf"));
            Font font = family.CreateFont(25, FontStyle.Regular);
            Font summaryFont = family.CreateFont(12, FontStyle.Regular);

            var nameColor = Color.Black;
            var summaryColor = Color.FromRgb(101, 109, 116);
            
            foreach (var item in ALlList)
            {
                var tagNumber = counter.ToString("000");
                var templateFile = item.TRoleType.GetTemplate();
                var exportFileName = Path.Combine(exportFolder,$"Tag{tagNumber}.png");
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
                
                //return;
            }

            Console.WriteLine($"Done with {counter} images. Check {exportFolder} for files");
            #endregion
        }
    }
}