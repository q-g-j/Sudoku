using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sudoku.Settings
{
    internal class AppSettings
    {
        internal AppSettings(string _folderAppSettings)
        {
            folderAppSettings = _folderAppSettings;
        }

        internal string folderAppSettings;
        
        internal struct AppSettingsStruct
        {
            internal bool SingleSolution;
        }

        internal void ChangeSingleSolution(bool action)
        {
            string filename = Path.Combine(folderAppSettings, "settings.json");

            AppSettingsStruct appSettingsStruct = new AppSettingsStruct();

            Dictionary<string, object> listsDictOld;
            using (var fileLoad = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                listsDictOld = (Dictionary<string, object>)serializer.Deserialize(fileLoad, typeof(Dictionary<string, object>));

                appSettingsStruct.SingleSolution = (bool)listsDictOld["SingleSolution"];
            }

            Dictionary<string, object> listsDict = new Dictionary<string, object>(listsDictOld)
            {
                ["SingleSolution"] = action
            };

            using (var fileSave = File.CreateText(@filename))
            {
                var numbersListDictJson = JsonConvert.SerializeObject(listsDict, Formatting.Indented);
                    fileSave.WriteLine(numbersListDictJson);
            }
        }
    }
}
