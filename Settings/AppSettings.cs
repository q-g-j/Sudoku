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
    internal struct AppSettingsStruct
    {
        internal bool SingleSolution;
    }

    internal class AppSettings
    {
        #region Constructors
        internal AppSettings(string _folderAppSettings)
        {
            folderAppSettings = _folderAppSettings;
        }
        #endregion Constructors

        #region Fields
        internal string folderAppSettings;
        #endregion Fields

        #region Methods
        internal AppSettingsStruct LoadSettings()
        {
            AppSettingsStruct appSettingsStruct = new AppSettingsStruct();

            string settingsFilename = Path.Combine(folderAppSettings, "settings.json");
            if (File.Exists(settingsFilename))
            {
                using (var settingsFile = File.OpenText(settingsFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Dictionary<string, object> settingsDict = (Dictionary<string, object>)serializer.Deserialize(settingsFile, typeof(Dictionary<string, object>));
                    if ((bool)settingsDict["SingleSolution"])
                    {
                        appSettingsStruct.SingleSolution = true;
                    }
                    else
                    {
                        appSettingsStruct.SingleSolution = false;
                    }
                }
            }
            else
            {
                Dictionary<string, object> settingsDict = new Dictionary<string, object>
                {
                    ["SingleSolution"] = false,
                };

                using (var file = File.CreateText(settingsFilename))
                {
                    var settingsDictJson = JsonConvert.SerializeObject(settingsDict, Formatting.Indented);
                    file.WriteLine(settingsDictJson);
                }
                appSettingsStruct.SingleSolution = false;
            }

            return appSettingsStruct;
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
                var numberListDictJson = JsonConvert.SerializeObject(listsDict, Formatting.Indented);
                    fileSave.WriteLine(numberListDictJson);
            }
        }
        #endregion Methods
    }
}
