using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sudoku.Properties;

namespace Sudoku.Models
{
    internal struct SaveSlotStruct
    {
        internal DateTime DateAndTime;
        internal NumberListModel NumberList;
        internal MarkerListModel MarkerList;
        internal NumberColorListModel NumberColorsList;
        internal List<string> GeneratorNumberList;
    }
    internal class SaveSlotsModel
    {
        #region Constructors
        internal SaveSlotsModel(string _folderAppSettings)
        {
            folderAppSettings = _folderAppSettings;
        }
        #endregion Constructors

        #region Fields
        private readonly string folderAppSettings;
        #endregion Fields

        #region Methods
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

        internal void SaveAll(NumberListModel numberList, MarkerListModel markerList, NumberColorListModel numberColorsList, List<String> generatorNumberList, DateTime now, string slotNumber)
        {                        
            string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");

            List<List<List<string>>> markerListConverted = new List<List<List<string>>>();

            for (int i = 0; i < 9; i++)
            {
                List<List<string>> tempList1 = new List<List<string>>();
                for (int j = 0; j < 9; j++)
                {
                    List<string> tempList2 = new List<string>();
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            if (markerList[i][j][k][l] != "")
                            {
                                tempList2.Add(markerList[i][j][k][l]);
                            }
                        }
                    }
                    tempList1.Add(tempList2);
                }
                markerListConverted.Add(tempList1);
            }

            Dictionary<string, object> listsDict = new Dictionary<string, object>
            {
                ["DateAndTime"] = now,
                ["NumberList"] = numberList,
                ["MarkerList"] = markerListConverted,
                ["NumberColorList"] = numberColorsList,
                ["GeneratorNumberList"] = generatorNumberList
            };

            using (var file = File.CreateText(@filename))
            {
                var numberListDictJson = JsonConvert.SerializeObject(listsDict, Formatting.Indented);
                file.WriteLine(numberListDictJson);
            }
        }

        internal SaveSlotStruct LoadAll(string slotNumber)
        {
            string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");
            SaveSlotStruct saveSlot = new SaveSlotStruct
            {
                MarkerList = new MarkerListModel()
            };
            saveSlot.MarkerList.InitializeList();
            List<List<List<string>>> markerList = new List<List<List<string>>>();

            using (var file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, object> listsDict = (Dictionary<string, object>)serializer.Deserialize(file, typeof(Dictionary<string, object>));

                saveSlot.DateAndTime = (DateTime)listsDict["DateAndTime"];
                saveSlot.NumberList = ((JArray)listsDict["NumberList"]).ToObject<NumberListModel>();
                saveSlot.NumberColorsList = ((JArray)listsDict["NumberColorList"]).ToObject<NumberColorListModel>();
                saveSlot.GeneratorNumberList = ((JArray)listsDict["GeneratorNumberList"]).ToObject<List<string>>();
                try
                {
                    markerList = ((JArray)listsDict["MarkerList"]).ToObject<List<List<List<string>>>>();
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            for (int k = 0; k < markerList[i][j].Count; k++)
                            {
                                string number = markerList[i][j][k];
                                if      (number == "1") { saveSlot.MarkerList[i][j][0][0] = number; }
                                else if (number == "2") { saveSlot.MarkerList[i][j][1][0] = number; }
                                else if (number == "3") { saveSlot.MarkerList[i][j][2][0] = number; }
                                else if (number == "4") { saveSlot.MarkerList[i][j][3][0] = number; }
                                else if (number == "5") { saveSlot.MarkerList[i][j][0][1] = number; }
                                else if (number == "6") { saveSlot.MarkerList[i][j][3][1] = number; }
                                else if (number == "7") { saveSlot.MarkerList[i][j][0][2] = number; }
                                else if (number == "8") { saveSlot.MarkerList[i][j][1][2] = number; }
                                else if (number == "9") { saveSlot.MarkerList[i][j][2][2] = number; }
                            }
                        }
                    }
                }
                catch (JsonReaderException)
                {
                    saveSlot.MarkerList = ((JArray)listsDict["MarkerList"]).ToObject<MarkerListModel>();
                }

            }

            return saveSlot;
        }
        #endregion Methods
    }
}
