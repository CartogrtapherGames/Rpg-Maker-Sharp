using System.Collections.Generic;
using RpgSharp.Data.RPG;

namespace RpgSharp.Data;

/// <summary>
/// The data object that shape a System model
/// </summary>
public class DataSystem
{
  public SystemAdvanced Advanced { get; set; }
  public Vehicle Airship { get; set; }
  public string[] ArmorTypes { get; set; }
  public List<AttackMotion> AttackMotions { get; set; }
  public AudioObject BattleBgm { get; set; }
  public string Battleback1Name { get; set; }
  public string Battleback2Name { get; set; }
  public int BattlerHue { get; set; }
  public string BattlerName { get; set; }
  public BattleSystem BattleSystem { get; set; }
  public Vehicle Boat { get; set; }
  public string CurrencyUnit { get; set; }
  public AudioObject DefeatMe { get; set; }
  public int EditMapId { get; set; }
  public string[] Elements { get; set; }
  public string[] EquipTypes { get; set; }
  public string GameTitle { get; set; }
  public AudioObject GameoverMe { get; set; }
  public bool[] ItemCategories { get; set; }
  public string Locale { get; set; }
  public int[] MagicSkills { get; set; }
  public bool[] MenuCommands { get; set; }
  public bool OptAutosave { get; set; }
  public bool OptDisplayTp { get; set; }
  public bool OptExtraExp { get; set; }
  public bool OptFloorDeath { get; set; }
  public bool OptFollowers { get; set; }
  public int OptKeyItemsNumber { get; set; }
  public bool OptSideView { get; set; }
  public bool OptSlipDeath { get; set; }
  public bool OptTransparent { get; set; }
  public int[] PartyMembers { get; set; }
  public Vehicle Ship { get; set; }
  public string[] SkillTypes { get; set; }
  public List<AudioObject> Sounds { get; set; }
  public int StartMapId { get; set; }
  public int StartX { get; set; }
  public int StartY { get; set; }
  public string[] Switches { get; set; }
  public Terms Terms { get; set; }
  public List<TestBattler> TestBattlers { get; set; }
  public int TestTroopId { get; set; }
  public string Title1Name { get; set; }
  public string Title2Name { get; set; }
  public AudioObject TitleBgm { get; set; }
  public TitleCommandWindow TitleCommandWindow { get; set; }
  public string[] Variables { get; set; }
  public int VersionId { get; set; }
  public AudioObject VictoryMe { get; set; }
  public string[] WeaponTypes { get; set; }
  public int[] WindowTone { get; set; }
  public SystemEditor Editor { get; set; }
  public int FaceSize { get; set; }
  public int IconSize { get; set; }
  public bool OptSplashScreen { get; set; }
  public bool OptMessageSkip { get; set; }
  public int TileSize { get; set; }
}

// Nested types extracted into their own classes
public class SystemAdvanced
{
  public int GameId { get; set; }
  public int ScreenWidth { get; set; }
  public int ScreenHeight { get; set; }
  public int UiAreaWidth { get; set; }
  public int UiAreaHeight { get; set; }
  public string NumberFontFilename { get; set; }
  public string FallbackFonts { get; set; }
  public int FontSize { get; set; }
  public string MainFontFilename { get; set; }
  public int ScreenScale { get; set; }
  public int WindowOpacity { get; set; }
  public int PicturesUpperLimit { get; set; }
}
public class TitleCommandWindow
{
  public int Background { get; set; }
  public int OffsetX { get; set; }
  public int OffsetY { get; set; }
}
public class SystemEditor
{
  public int MessageWidth1 { get; set; }
  public int MessageWidth2 { get; set; }
  public JsonFormatLevel JsonFormatLevel { get; set; }
}
public enum JsonFormatLevel
{
  Minified = 1,
  Prettified = 2
}
public enum BattleSystem
{
  TurnBased = 0,
  AtbActive = 1,
  AtbWait = 2
}
