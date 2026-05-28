using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RpgSharp.Data;
using RpgSharp.Objects;

namespace RpgSharp.Management;

/// <summary>
/// The class that manages the data
/// </summary>
public static class DataManager
{
    public static List<DataActor>       DataActors       { get; private set; }
    public static List<DataClass>       DataClasses      { get; private set; }
    public static List<DataSkill>       DataSkills       { get; private set; }
    public static List<DataItem>        DataItems        { get; private set; }
    public static List<DataWeapon>      DataWeapons      { get; private set; }
    public static List<DataArmor>       DataArmors       { get; private set; }
    public static List<DataEnemy>       DataEnemies      { get; private set; }
    public static List<DataTroop>       DataTroops       { get; private set; }
    public static List<DataState>       DataStates       { get; private set; }
    public static List<DataAnimation>   DataAnimations   { get; private set; }
    public static List<DataTileset>     DataTilesets     { get; private set; }
    public static List<DataCommonEvent> DataCommonEvents { get; private set; }
    public static List<DataMapInfo>     DataMapInfos     { get; private set; }
    public static DataSystem            DataSystem       { get; private set; }
    public static DataMap               DataMap          { get; private set; }
    
    public static GameSwitches GameSwitches { get; set; }
    public static GameSelfSwitches GameSelfSwitches { get; set; }
    public static GameVariables GameVariables { get; set; }
    public static GameMap GameMap { get; set; }
    public static GameActors GameActors { get; set; }
    public static GameParty GameParty { get; set; }

    static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    /// <summary>
    /// Loads the database from the given path
    /// </summary>
    /// <param name="dataPath"></param>
    public static void LoadDatabase(string dataPath)
    {
        DataActors       = Load<List<DataActor>>      (dataPath, "Actors.json");
        DataClasses      = Load<List<DataClass>>      (dataPath, "Classes.json");
        DataSkills       = Load<List<DataSkill>>      (dataPath, "Skills.json");
        DataItems        = Load<List<DataItem>>       (dataPath, "Items.json");
        DataWeapons      = Load<List<DataWeapon>>     (dataPath, "Weapons.json");
        DataArmors       = Load<List<DataArmor>>      (dataPath, "Armors.json");
        DataEnemies      = Load<List<DataEnemy>>      (dataPath, "Enemies.json");
        DataTroops       = Load<List<DataTroop>>      (dataPath, "Troops.json");
        DataStates       = Load<List<DataState>>      (dataPath, "States.json");
        DataAnimations   = Load<List<DataAnimation>>  (dataPath, "Animations.json");
        DataTilesets     = Load<List<DataTileset>>    (dataPath, "Tilesets.json");
        DataCommonEvents = Load<List<DataCommonEvent>>(dataPath, "CommonEvents.json");
        DataMapInfos     = Load<List<DataMapInfo>>    (dataPath, "MapInfos.json");
        DataSystem       = Load<DataSystem>           (dataPath, "System.json");
    }

    public static void LoadMap(string dataPath, int mapId)
    {
        DataMap = Load<DataMap>(dataPath, $"Map{mapId:D3}.json");
    }

    private static T Load<T>(string dataPath, string filename)
    {
        var path = Path.Combine(dataPath, filename);
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }
}
