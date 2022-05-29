using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sudoku.Properties;

namespace Sudoku.Models
{
    internal class SaveSlotsModel
    {
        internal SaveSlotsModel(string _folderAppSettings)
        {
            folderAppSettings = _folderAppSettings;
        }
        private readonly string folderAppSettings;

        internal struct SaveSlotStruct
        {
            internal DateTime DateAndTime;
            internal NumbersListModel NumbersList;
            internal MarkersListModel MarkersList;
            internal NumbersColorsListModel NumbersColorsList;
            internal List<string> GeneratorNumbers;
        }

        internal List<string> GetSaveTexts()
        {
            List<string> saveSlotList = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                if      (i == 0) saveSlotList.Add(Resources.MenuGameSaveSlotsSaveToSlot1);
                else if (i == 1) saveSlotList.Add(Resources.MenuGameSaveSlotsSaveToSlot2);
                else if (i == 2) saveSlotList.Add(Resources.MenuGameSaveSlotsSaveToSlot3);
                else if (i == 3) saveSlotList.Add(Resources.MenuGameSaveSlotsSaveToSlot4);
                else if (i == 4) saveSlotList.Add(Resources.MenuGameSaveSlotsSaveToSlot5);
                string saveSlotFilename = Path.Combine(folderAppSettings, "slot" + (i + 1).ToString() + ".json");
                if (File.Exists(saveSlotFilename))
                {
                    using (var saveSlotFile = File.OpenText(saveSlotFilename))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Dictionary<string, object> saveSlotDict = (Dictionary<string, object>)serializer.Deserialize(saveSlotFile, typeof(Dictionary<string, object>));

                        DateTime dateAndTime = (DateTime)saveSlotDict["DateAndTime"];
                        saveSlotList[i] += " (" + dateAndTime.ToString() + ")";
                    }
                }
            }
            return saveSlotList;
        }

        internal List<string> GetLoadTexts()
        {
            List<string> saveSlotList = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                if      (i == 0) saveSlotList.Add(Resources.MenuGameSaveSlotsLoadFromSlot1);
                else if (i == 1) saveSlotList.Add(Resources.MenuGameSaveSlotsLoadFromSlot2);
                else if (i == 2) saveSlotList.Add(Resources.MenuGameSaveSlotsLoadFromSlot3);
                else if (i == 3) saveSlotList.Add(Resources.MenuGameSaveSlotsLoadFromSlot4);
                else if (i == 4) saveSlotList.Add(Resources.MenuGameSaveSlotsLoadFromSlot5);
                string saveSlotFilename = Path.Combine(folderAppSettings, "slot" + (i + 1).ToString() + ".json");
                if (File.Exists(saveSlotFilename))
                {
                    using (var saveSlotFile = File.OpenText(saveSlotFilename))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Dictionary<string, object> saveSlotDict = (Dictionary<string, object>)serializer.Deserialize(saveSlotFile, typeof(Dictionary<string, object>));

                        DateTime dateAndTime = (DateTime)saveSlotDict["DateAndTime"];
                        saveSlotList[i] += " (" + dateAndTime.ToString() + ")";
                    }
                }
            }
            return saveSlotList;
        }

        internal void SaveAll(NumbersListModel numbersList, MarkersListModel markersList, NumbersColorsListModel numbersColorsList, List<String> generatorNumbers, DateTime now, string slotNumber)
        {
            string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");

            Dictionary<string, object> listsDict = new Dictionary<string, object>
            {
                ["DateAndTime"] = now,
                ["NumbersList"] = numbersList,
                ["MarkersList"] = markersList,
                ["NumbersColorsList"] = numbersColorsList,
                ["GeneratorNumbers"] = generatorNumbers
            };

            using (var file = File.CreateText(@filename))
            {
                var numbersListDictJson = JsonConvert.SerializeObject(listsDict, Formatting.Indented);
                file.WriteLine(numbersListDictJson);
            }
        }

        internal SaveSlotStruct LoadAll(string slotNumber)
        {
            string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");
            SaveSlotStruct listsStruct = new SaveSlotStruct();

            using (var file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, object> listsDict = (Dictionary<string, object>)serializer.Deserialize(file, typeof(Dictionary<string, object>));

                listsStruct.DateAndTime = (DateTime)listsDict["DateAndTime"];
                listsStruct.NumbersList = ((JArray)listsDict["NumbersList"]).ToObject<NumbersListModel>();
                listsStruct.MarkersList = ((JArray)listsDict["MarkersList"]).ToObject<MarkersListModel>();
                listsStruct.NumbersColorsList = ((JArray)listsDict["NumbersColorsList"]).ToObject<NumbersColorsListModel>();
                listsStruct.GeneratorNumbers = ((JArray)listsDict["GeneratorNumbers"]).ToObject<List<string>>();
            }

            return listsStruct;
        }
    }
}
