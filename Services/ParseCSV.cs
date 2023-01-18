using DotNetNuke.Services.Log.EventLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Arkix.Modules.SellingPoints.Services
{
    public class ParseCSV
    {
        public static List<string[]> Parse(string path, int length, char separador)
        {
            List<string[]> parsedData = new List<string[]>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(separador);
                        EventLogController.Instance.AddLog("Parser data", $"Line: {line.Split(separador)}.", EventLogController.EventLogType.ADMIN_ALERT);
                        if (row.Length == length)
                        {
                            parsedData.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return parsedData;
        }
    }
}