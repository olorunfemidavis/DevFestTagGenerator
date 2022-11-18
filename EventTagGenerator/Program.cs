using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using EventTagGenerator.Helpers;
using EventTagGenerator.Model;

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

            #region Populate List
            using(var reader = new StreamReader(@"Files/DevFestTagExport.csv"))
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

            var exportFolder = Path.Combine(Environment.SpecialFolder.Desktop.ToString(),
                "DevFestTags");
            if (!Directory.Exists(exportFolder))
                Directory.CreateDirectory(exportFolder);
            File.WriteAllText(Path.Combine(exportFolder, "test.txt"), "Hello World");
            int counter = 0;
            foreach (var item in ALlList)
            {
                var tagNumber = counter.ToString("000");
                var templateFile = item.TRoleType.GetTemplate();
                var exportFileName = $"Tag{tagNumber}";
                var footerColor = item.TRoleType.GetColor();
                var nameColor = new SolidBrush(Color.Black);
                var summaryColor = new SolidBrush(Color.FromArgb(101, 109, 116));
                
                //write on the photo now. 
                PointF firstNameLocation = new PointF(10f, 10f);
                PointF lastNameLocation = new PointF(10f, 50f);
                PointF summaryLocation = new PointF(10f, 10f);
                PointF roleLocation = new PointF(10f, 50f);
                PointF tagNumberLocation = new PointF(10f, 10f);

                Bitmap exportBitmap;
                using (var bitmap = (Bitmap)Image.FromFile(templateFile))
                {
                    using(Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        using (Font arialFont =  new Font("Arial", 10))
                        {
                            graphics.DrawString(item.FirstName, arialFont, nameColor, firstNameLocation);
                            graphics.DrawString(item.LastName, arialFont, nameColor, lastNameLocation);
                            graphics.DrawString(item.Summary, arialFont, summaryColor, summaryLocation);
                            graphics.DrawString(item.Footer, arialFont, footerColor, roleLocation);
                            graphics.DrawString(tagNumber, arialFont, footerColor, tagNumberLocation);
                          
                        }
                    }

                    exportBitmap = new Bitmap(bitmap);
                }
                exportBitmap.Save(exportFileName);

                counter++;
                
                return;
            }


            #endregion
        }
    }
}