﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sudoku.Models;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Sudoku.SaveGame
{
    internal class SaveSlots
    {
        internal SaveSlots(string _folderAppSettings)
        {
            folderAppSettings = _folderAppSettings;
        }
        private readonly string folderAppSettings;

        internal struct ListsStruct
        {
            internal NumbersListModel NumbersList;
            internal MarkersListModel MarkersList;
            internal NumbersColorsListModel NumbersColorsList;
            internal List<string> GeneratorNumbers;
        }

        internal void SaveAll(NumbersListModel numbersList, MarkersListModel markersList, NumbersColorsListModel numbersColorsList, List<String> generatorNumbers, string slotNumber)
        {
            string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");

            Dictionary<string, object> listsDict = new Dictionary<string, object>
            {
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

        internal ListsStruct LoadAll(string slotNumber)
        {
            string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");
            ListsStruct listsStruct = new ListsStruct();

            using (var file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, object> listsDict = (Dictionary<string, object>)serializer.Deserialize(file, typeof(Dictionary<string, object>));

                listsStruct.NumbersList = ((JArray)listsDict["NumbersList"]).ToObject<NumbersListModel>();
                listsStruct.MarkersList = ((JArray)listsDict["MarkersList"]).ToObject<MarkersListModel>();
                listsStruct.NumbersColorsList = ((JArray)listsDict["NumbersColorsList"]).ToObject<NumbersColorsListModel>();
                listsStruct.GeneratorNumbers = ((JArray)listsDict["GeneratorNumbers"]).ToObject<List<string>>();
            }

            return listsStruct;
        }
    }
}
