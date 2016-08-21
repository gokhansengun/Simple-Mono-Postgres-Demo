using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace UniversityRegistrationSystem.Utility
{
    public class FileHandler
    {
        static List<string> _sqlFileLocations;

        static FileHandler()
        {
            _sqlFileLocations = ConfigurationManager.AppSettings["SqlFileLocations"].Split(',').ToList();
        }

        public static string ReadFileContent(string fileName)
        {
            foreach (string fileLocation in _sqlFileLocations)
            {
                string filePath = "./" + fileLocation + "/" + fileName;

                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }

            throw new FileNotFoundException("fileName: " + fileName);
        }
    }
}