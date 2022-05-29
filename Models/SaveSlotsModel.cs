using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
