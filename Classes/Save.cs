﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;

namespace ACSE
{
    public enum SaveType
    {
        Unknown,
        Doubutsu_no_Mori,
        Doubutsu_no_Mori_Plus,  //Possible to remove this? I can't even find a valid file on the internet... Perhaps I'll just buy a copy
        Animal_Crossing,
        Doubutsu_no_Mori_e_Plus,
        Wild_World,
        City_Folk,
        New_Leaf,
        Welcome_Amiibo
    }

    public struct Offsets
    {
        public int Save_Size;
        public int Checksum;
        public int Town_Name;
        public int Player_Start;
        public int Player_Size;
        public int Town_Data;
        public int Town_Data_Size;
        public int Acre_Data;
        public int Acre_Data_Size;
        public int Buried_Data;
        public int Buried_Data_Size;
        public int Island_Acre_Data;
        public int Island_World_Data;
        public int Island_World_Size;
        public int Island_Buried_Data;
        public int Island_Buried_Size;
        public int Island_Buildings;
        public int House_Data;
        public int House_Data_Size;
        public int Villager_Data;
        public int Villager_Size;
        public int Islander_Data;
        public int Debt; //Only global in WW
        public int Buildings; //CF/NL
        public int Buildings_Count;
        public int Grass_Wear;
        public int Grass_Wear_Size;
        public int PWPs; //NL only
        public int Past_Villagers;
        public int[] CRC_Offsets;
    }

    public struct Save_Info
    {
        public bool Contains_Island;
        public bool Has_Islander;
        public int Pattern_Count;
        public bool Player_JPEG_Picture;
        public Offsets Save_Offsets;
        public int Acre_Count;
        public int X_Acre_Count;
        public int Town_Acre_Count;
        public int Town_Y_Acre_Start;
        public int Island_Acre_Count;
        public int Island_X_Acre_Count;
        public int Villager_Count;
        public string[] House_Rooms;
    }

    public enum SaveFileDataOffset
    {
        nafjfla = 0,
        gafegci = 0x26040,
        gafegcs = 0x26150,
        admeduc = 0x1F4,
        admedss = 0x1F4,
        admedsv = 0,
        admesav = 0,
        ruuedat = 0,
        edgedat = 0x80,
        edgebin = 0, //RAM Dump
        eaaedat = 0x80,
    }

    public static class SaveDataManager
    {
        public static Offsets Animal_Crossing_Offsets = new Offsets
        {
            Save_Size = 0x26000,
            Town_Name = 0x9120,
            Player_Start = 0x20,
            Player_Size = 0x2440,
            Villager_Data = 0x17438,
            Villager_Size = 0x988,
            Acre_Data = 0x173A8,
            Acre_Data_Size = 0x8C,
            Town_Data = 0x137A8,
            Town_Data_Size = 0x3C00,
            Buried_Data = 0x20F1C,
            Buried_Data_Size = 0x3C0,
            Island_World_Data = 0x22554,
            Island_World_Size = 0x400,
            Island_Buried_Data = 0x23DC8, //C9
            Island_Buried_Size = 0x40,
            Islander_Data = 0x23440,
            House_Data = 0x9CE8,
            House_Data_Size = 0x26B0,
            Island_Acre_Data = 0x17420,
            Buildings = -1,
            Debt = -1,
            Grass_Wear = -1,
            Past_Villagers = -1,
            PWPs = -1,
            Island_Buildings = -1,
            Checksum = 0x12
        };

        public static Offsets Wild_World_Offsets = new Offsets
        {
            Save_Size = 0x15FE0,
            Town_Name = 0x0004,
            Player_Start = 0x000C,
            Player_Size = 0x228C,
            Villager_Data = 0x8A3C,
            Villager_Size = 0x700,
            Acre_Data = 0xC330,
            Acre_Data_Size = 0x24,
            Town_Data = 0xC354,
            Town_Data_Size = 0x2000,
            Buried_Data = 0xE354,
            Buried_Data_Size = 0x200,
            House_Data = 0xE558,
            House_Data_Size = 0x15A0,
            Debt = 0xFAE8,
            Buildings = -1,
            Grass_Wear = -1,
            Islander_Data = -1,
            Island_Buried_Data = -1,
            Island_World_Data = -1,
            Past_Villagers = -1,
            PWPs = -1,
            Island_Acre_Data = -1,
            Island_Buildings = -1,
            Checksum = 0x15FDC
        };

        public static Offsets City_Folk_Offsets = new Offsets
        {
            Save_Size = 0x40F340,
            Player_Start = 0,
            Player_Size = 0x86C0,
            Buildings = 0x5EB0A,
            Buildings_Count = 0x33, //Not sure
            Acre_Data = 0x68414, //Don't forget about the additional acres before?
            Acre_Data_Size = 0x62,
            Town_Name = 0x640E8,
            Town_Data = 0x68476,
            Town_Data_Size = 0x3200,
            Buried_Data = 0x6B676,
            Buried_Data_Size = 400,
            Grass_Wear = 0x6BCB6,
            Grass_Wear_Size = 0x1900,
            House_Data = 0x6D5C0,
            House_Data_Size = 0x15C0,
            Checksum = -1,
            Debt = -1,
            Islander_Data = -1,
            Island_Buried_Data = -1,
            Island_World_Data = -1,
            Past_Villagers = -1,
            PWPs = -1,
            Villager_Data = -1, //finish this sometime
            Island_Acre_Data = -1,
            Island_Buildings = -1,
        };

        public static Offsets New_Leaf_Offsets = new Offsets
        {
            Save_Size = 0x7F980,
            Player_Start = 0x20,
            Player_Size = 0x9F10,
            Villager_Data = 0x27C90,
            Villager_Size = 0x24F8,
            Buildings = 0x49528,
            Buildings_Count = 58, //TODO: Add island buildings (Island Hut & Loaner Gyroid at 59 and 60)
            Acre_Data = 0x4DA04,
            Acre_Data_Size = 0x54,
            Town_Data = 0x4DA58,
            Town_Data_Size = 0x5000,
            Buried_Data = -1,
            Town_Name = 0x5C73A,
            Grass_Wear = 0x53E80,
            Grass_Wear_Size = 0x3000, //Extra row of "Invisible" X Acres
            Past_Villagers = 0x3F17E,
            Island_Acre_Data = 0x6A408,
            Island_World_Data = 0x6A428,
            Island_Buildings = 0x6B428,
            //Campsite_Villager = 0x3F1CA
        };

        public static Offsets Welcome_Amiibo_Offsets = new Offsets
        {
            Save_Size = 0x89A80,
            Player_Start = 0x20,
            Player_Size = 0xA480,
            Villager_Data = 0x29250,
            Villager_Size = 0x2518,
            Buildings = 0x4BE08,
            Buildings_Count = 58, //TODO: Add island buildings (Island Hut & Loaner Gyroid at 59 and 60)
            Acre_Data = 0x53404,
            Acre_Data_Size = 0x54,
            Town_Data = 0x53458,
            Town_Data_Size = 0x5000,
            Buried_Data = -1,
            Town_Name = 0x6213A,
            Grass_Wear = 0x59880,
            Grass_Wear_Size = 0x3000, //Extra row of "Invisible" X Acres
            Past_Villagers = 0x4087A,
            Island_Acre_Data = 0x6FE38,
            Island_World_Data = 0x6FE58,
            Island_Buildings = 0x70E58,
        };

        public static Save_Info Animal_Crossing = new Save_Info
        {
            Contains_Island = true,
            Has_Islander = true,
            Player_JPEG_Picture = false,
            Pattern_Count = 8,
            Save_Offsets = Animal_Crossing_Offsets,
            Acre_Count = 70,
            X_Acre_Count = 7,
            Town_Acre_Count = 30,
            Island_Acre_Count = 2,
            Island_X_Acre_Count = 2,
            Town_Y_Acre_Start = 1,
            Villager_Count = 16,
            House_Rooms = new string[3] { "Main Floor", "Upper Floor", "Basement" } //Don't forget about island house
        };

        public static Save_Info Wild_World = new Save_Info
        {
            Contains_Island = false,
            Has_Islander = false,
            Player_JPEG_Picture = false,
            Pattern_Count = 8,
            Save_Offsets = Wild_World_Offsets,
            Acre_Count = 36,
            X_Acre_Count = 6,
            Town_Acre_Count = 16,
            Town_Y_Acre_Start = 1,
            Villager_Count = 8,
            House_Rooms = new string[6] { "Main Floor", "Basement", "Upper Floor", "Left Wing", "Right Wing", "Back Wing" } //Confirm order
            //6 drawers with 15 items per
        };

        public static Save_Info City_Folk = new Save_Info
        {
            Contains_Island = false,
            Has_Islander = false,
            Player_JPEG_Picture = false,
            Pattern_Count = 8,
            Save_Offsets = City_Folk_Offsets,
            Acre_Count = 49,
            X_Acre_Count = 7,
            Town_Acre_Count = 25,
            Town_Y_Acre_Start = 1,
            Villager_Count = 10,
            House_Rooms = new string[3] { "Main Floor", "Upper Floor", "Basement" }
        };

        public static Save_Info New_Leaf = new Save_Info
        {
            Contains_Island = true,
            Has_Islander = false,
            Player_JPEG_Picture = true,
            Pattern_Count = 10,
            Save_Offsets = New_Leaf_Offsets,
            Acre_Count = 42,
            X_Acre_Count = 7,
            Town_Acre_Count = 20,
            Town_Y_Acre_Start = 1,
            Villager_Count = 10,
            Island_Acre_Count = 16,
            Island_X_Acre_Count = 4,
            House_Rooms = new string[6] { "Main Floor", "Upper Floor", "Basement", "Left Wing", "Right Wing", "Back Wing" } //Check order
            //3 drawers with 60 items per
        };

        public static Save_Info Welcome_Amiibo = new Save_Info
        {
            Contains_Island = true,
            Has_Islander = false,
            Player_JPEG_Picture = true,
            Pattern_Count = 10,
            Save_Offsets = Welcome_Amiibo_Offsets,
            Acre_Count = 42,
            X_Acre_Count = 7,
            Town_Acre_Count = 20,
            Town_Y_Acre_Start = 1,
            Villager_Count = 10,
            Island_Acre_Count = 16,
            Island_X_Acre_Count = 4,
            House_Rooms = new string[6] { "Main Floor", "Upper Floor", "Basement", "Left Wing", "Right Wing", "Back Wing" }
            //3 drawers with 60 items per
        };

        /// <summary>
        /// Used for Doubutsu no Mori. Every four bytes is reversed in the save, so we change it back.
        /// </summary>
        /// <param name="saveBuff"></param>
        /// <returns></returns>
        public static byte[] ByteSwap(byte[] saveBuff)
        {
            byte[] Corrected_Save = new byte[saveBuff.Length];
            for (int i = 0; i < saveBuff.Length; i += 4)
            {
                byte[] Temp = new byte[4];
                Buffer.BlockCopy(saveBuff, i, Temp, 0, 4);
                Array.Reverse(Temp);
                Temp.CopyTo(Corrected_Save, i);
            }
            return Corrected_Save;
        }

        public static SaveType GetSaveType(byte[] Save_Data)
        {
            if (Save_Data.Length == 0x20000)
                return SaveType.Doubutsu_no_Mori;
            else if (Save_Data.Length == 0x72040 || Save_Data.Length == 0x72150)
            {
                string Game_ID = Encoding.ASCII.GetString(Save_Data, Save_Data.Length == 0x72150 ? 0x110 : 0, 4);
                if (Game_ID == "GAFE")
                    return SaveType.Animal_Crossing;
                else if (Game_ID == "GAFJ")
                    return SaveType.Doubutsu_no_Mori_Plus;
                else if (Game_ID == "GAEJ")
                    return SaveType.Doubutsu_no_Mori_e_Plus;
            }
            else if (Save_Data.Length == 0x4007A || Save_Data.Length == 0x401F4 || Save_Data.Length == 0x40000)
                return SaveType.Wild_World;
            else if (Save_Data.Length == 0x40F340 || Save_Data.Length == 0x47A0DA)
                return SaveType.City_Folk;
            else if (Save_Data.Length == 0x7FA00 || Save_Data.Length == 0x80000)
                return SaveType.New_Leaf;
            else if (Save_Data.Length == 0x89B00)
                return SaveType.Welcome_Amiibo;
            return SaveType.Unknown;
        }

        public static int GetSaveDataOffset(string Game_ID, string Extension)
        {
            if (Enum.TryParse(Game_ID + Extension, out SaveFileDataOffset Extension_Enum))
                return (int)Extension_Enum;
            return 0;
        }

        public static string GetGameID(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Doubutsu_no_Mori:
                    return "NAFJ";
                case SaveType.Animal_Crossing:
                    return "GAFE";
                case SaveType.Doubutsu_no_Mori_Plus:
                    return "GAFJ";
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return "GAEJ";
                case SaveType.Wild_World: //Currently only supporting the English versions of WW+
                    return "ADME";
                case SaveType.City_Folk:
                    return "RUUE";
                case SaveType.New_Leaf:
                    return "EDGE";
                case SaveType.Welcome_Amiibo:
                    return "EAAE";
                default:
                    return "Unknown";
            }
        }

        public static Save_Info GetSaveInfo(SaveType Save_Type)
        {
            switch(Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return Animal_Crossing;
                case SaveType.Wild_World:
                    return Wild_World;
                case SaveType.City_Folk:
                    return City_Folk;
                case SaveType.New_Leaf:
                    return New_Leaf;
                case SaveType.Welcome_Amiibo:
                    return Welcome_Amiibo;
                default:
                    return Wild_World;
            }
        }

        public static Dictionary<ushort, string> GetItemInfo(SaveType Save_Type, string Language = "en")
        {
            StreamReader Contents = null;
            string Item_DB_Location = NewMainForm.Assembly_Location + "\\Resources\\";
            if (Save_Type == SaveType.Wild_World)
                Item_DB_Location += "WW_Items_" + Language + ".txt";
            else if (Save_Type == SaveType.Animal_Crossing)
                Item_DB_Location += "AC_Items_" + Language + ".txt";
            else if (Save_Type == SaveType.City_Folk)
                Item_DB_Location += "CF_Items_" + Language + ".txt";
            else if (Save_Type == SaveType.New_Leaf)
                Item_DB_Location += "NL_Items_" + Language + ".txt";
            else if (Save_Type == SaveType.Welcome_Amiibo)
                Item_DB_Location += "WA_Items_" + Language + ".txt";
            try { Contents = File.OpenText(Item_DB_Location); }
            catch (Exception e)
            {
                NewMainForm.Debug_Manager.WriteLine(string.Format("An error occured opening item database file:\n\"{0}\"\nError Info:\n{1}", Item_DB_Location, e.Message), DebugLevel.Error);
                return null;
            }
            Dictionary<ushort, string> Item_Dictionary = new Dictionary<ushort, string>();
            string Line;
            while ((Line = Contents.ReadLine()) != null)
            {
                if (!Properties.Settings.Default.DebuggingEnabled && Line.Contains("//"))
                    MessageBox.Show("Now loading item type: " + Line.Replace("//", ""));
                else if (Line.Contains("0x"))
                {
                    string Item_ID_String = Line.Substring(0, 6), Item_Name = Line.Substring(7).Trim();
                    if (ushort.TryParse(Item_ID_String.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out ushort Item_ID))
                        Item_Dictionary.Add(Item_ID, Item_Name);
                    else
                        NewMainForm.Debug_Manager.WriteLine("Unable to add item: " + Item_ID_String + " | " + Item_Name, DebugLevel.Error);
                }
            }
            return Item_Dictionary;
        }

        public static Dictionary<byte, string> GetAcreInfo(SaveType Save_Type, string Language = "en")
        {
            StreamReader Contents = null;
            string Acre_DB_Location = NewMainForm.Assembly_Location + "\\Resources\\";
            if (Save_Type == SaveType.Wild_World)
                Acre_DB_Location += "WW_Acres_" + Language + ".txt";
            try { Contents = File.OpenText(Acre_DB_Location); }
            catch (Exception e)
            {
                NewMainForm.Debug_Manager.WriteLine(string.Format("An error occured opening acre database file:\n\"{0}\"\nError Info:\n{1}", Acre_DB_Location, e.Message), DebugLevel.Error);
                return null;
            }
            Dictionary<byte, string> Acre_Dictionary = new Dictionary<byte, string>();
            string Line;
            while ((Line = Contents.ReadLine()) != null)
            {
                if (!Properties.Settings.Default.DebuggingEnabled && Line.Contains("//"))
                    MessageBox.Show("Now loading Acre type: " + Line.Replace("//", ""));
                else if (Line.Contains("0x"))
                {
                    string Acre_ID_String = Line.Substring(0, 4), Acre_Name = Line.Substring(5);
                    if (byte.TryParse(Acre_ID_String.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out byte Acre_ID))
                        Acre_Dictionary.Add(Acre_ID, Acre_Name);
                    else
                        NewMainForm.Debug_Manager.WriteLine("Unable to add Acre: " + Acre_ID_String + " | " + Acre_Name, DebugLevel.Error);
                }
            }
            return Acre_Dictionary;
        }

        public static Dictionary<ushort, string> GetAcreInfoUInt16(SaveType Save_Type, string Language = "en")
        {
            StreamReader Contents = null;
            string Acre_DB_Location = NewMainForm.Assembly_Location + "\\Resources\\";
            if (Save_Type == SaveType.Animal_Crossing)
                Acre_DB_Location += "AC_Acres_" + Language + ".txt";
            else if (Save_Type == SaveType.City_Folk)
                Acre_DB_Location += "CF_Acres_" + Language + ".txt";
            else if (Save_Type == SaveType.New_Leaf)
                Acre_DB_Location += "NL_Acres_" + Language + ".txt";
            else if (Save_Type == SaveType.Welcome_Amiibo)
                Acre_DB_Location += "WA_Acres_" + Language + ".txt";
            try { Contents = File.OpenText(Acre_DB_Location); }
            catch (Exception e)
            {
                NewMainForm.Debug_Manager.WriteLine(string.Format("An error occured opening acre database file:\n\"{0}\"\nError Info:\n{1}", Acre_DB_Location, e.Message), DebugLevel.Error);
                return null;
            }
            Dictionary<ushort, string> Acre_Dictionary = new Dictionary<ushort, string>();
            string Line;
            while ((Line = Contents.ReadLine()) != null)
            {
                if (!Properties.Settings.Default.DebuggingEnabled && Line.Contains("//"))
                    MessageBox.Show("Now loading Acre type: " + Line.Replace("//", ""));
                else if (Line.Contains("0x"))
                {
                    string Acre_ID_String = Line.Substring(0, 6), Acre_Name = Line.Substring(7);
                    if (ushort.TryParse(Acre_ID_String.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out ushort Acre_ID))
                        Acre_Dictionary.Add(Acre_ID, Acre_Name);
                    else
                        NewMainForm.Debug_Manager.WriteLine("Unable to add Acre: " + Acre_ID_String + " | " + Acre_Name, DebugLevel.Error);
                }
            }
            return Acre_Dictionary;
        }


        public static Dictionary<string, List<byte>> GetFiledAcreData(SaveType Save_Type, string Language = "en")
        {
            StreamReader Contents = null;
            string Acre_DB_Location = NewMainForm.Assembly_Location + "\\Resources\\";
            if (Save_Type == SaveType.Wild_World)
                Acre_DB_Location += "WW_Acres_" + Language + ".txt";
            try { Contents = File.OpenText(Acre_DB_Location); }
            catch (Exception e)
            {
                NewMainForm.Debug_Manager.WriteLine(string.Format("An error occured opening acre database file:\n\"{0}\"\nError Info:\n{1}", Acre_DB_Location, e.Message), DebugLevel.Error);
                return null;
            }
            Dictionary<string, List<byte>> Filed_List = new Dictionary<string, List<byte>>();
            string Line;
            string Current_Acre_Type = "Unsorted";
            while ((Line = Contents.ReadLine()) != null)
            {
                if (Line.Contains("//"))
                {
                    if (Current_Acre_Type != "Unsorted")
                    {
                        if (!Filed_List.ContainsKey(Current_Acre_Type))
                            Filed_List.Add(Current_Acre_Type, new List<byte>());
                    }
                    Current_Acre_Type = Line.Replace("//", "");
                }
                else if (Line.Contains("0x"))
                {
                    if (!Filed_List.ContainsKey(Current_Acre_Type))
                        Filed_List.Add(Current_Acre_Type, new List<byte>());
                    string Acre_ID_String = Line.Substring(0, 4), Acre_Name = Line.Substring(5);
                    if (byte.TryParse(Acre_ID_String.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out byte Acre_ID))
                        Filed_List[Current_Acre_Type].Add(Acre_ID);
                    else
                        NewMainForm.Debug_Manager.WriteLine("Unable to add Acre: " + Acre_ID_String + " | " + Acre_Name, DebugLevel.Error);
                }
            }
            return Filed_List;
        }

        public static Dictionary<string, Dictionary<ushort, string>> GetFiledAcreDataUInt16(SaveType Save_Type, string Language = "en")
        {
            StreamReader Contents = null;
            string Acre_DB_Location = NewMainForm.Assembly_Location + "\\Resources\\";
            if (Save_Type == SaveType.Animal_Crossing)
                Acre_DB_Location += "AC_Acres_" + Language + ".txt";
            else if (Save_Type == SaveType.City_Folk)
                Acre_DB_Location += "CF_Acres_" + Language + ".txt";
            else if (Save_Type == SaveType.New_Leaf)
                Acre_DB_Location += "NL_Acres_" + Language + ".txt";
            else if (Save_Type == SaveType.Welcome_Amiibo)
                Acre_DB_Location += "WA_Acres_" + Language + ".txt";
            try { Contents = File.OpenText(Acre_DB_Location); }
            catch (Exception e)
            {
                NewMainForm.Debug_Manager.WriteLine(string.Format("An error occured opening acre database file:\n\"{0}\"\nError Info:\n{1}", Acre_DB_Location, e.Message), DebugLevel.Error);
                return null;
            }
            Dictionary<string, Dictionary<ushort, string>> Filed_List = new Dictionary<string, Dictionary<ushort, string>>();
            string Line;
            string Current_Acre_Type = "Unsorted";
            while ((Line = Contents.ReadLine()) != null)
            {
                if (Line.Contains("//"))
                {
                    if (Current_Acre_Type != "Unsorted")
                    {
                        if (!Filed_List.ContainsKey(Current_Acre_Type))
                            Filed_List.Add(Current_Acre_Type, new Dictionary<ushort, string>());
                    }
                    Current_Acre_Type = Line.Replace("//", "");
                }
                else if (Line.Contains("0x"))
                {
                    if (!Filed_List.ContainsKey(Current_Acre_Type))
                        Filed_List.Add(Current_Acre_Type, new Dictionary<ushort, string>());
                    string Acre_ID_String = Line.Substring(0, 6), Acre_Name = Line.Substring(7);
                    if (ushort.TryParse(Acre_ID_String.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out ushort Acre_ID))
                        Filed_List[Current_Acre_Type].Add(Acre_ID, Line.Substring(7));
                    else
                        NewMainForm.Debug_Manager.WriteLine("Unable to add Acre: " + Acre_ID_String + " | " + Acre_Name, DebugLevel.Error);
                }
            }
            return Filed_List;
        }

        public static void GetNibbles(byte Byte_to_Split, out byte Lower_Nibble, out byte Upper_Nibble)
        {
            Lower_Nibble = (byte)(Byte_to_Split & 0x0F);
            Upper_Nibble = (byte)((Byte_to_Split & 0xF0) >> 4);
        }

        public static byte CondenseNibbles(byte Lower_Nibble, byte Upper_Nibble)
        {
            return (byte)(((Upper_Nibble & 0x0F) >> 4) + Lower_Nibble & 0x0F);
        }
    }

    public class Save
    {
        public SaveType Save_Type;
        public byte[] Original_Save_Data;
        public byte[] Working_Save_Data;
        public int Save_Data_Start_Offset;
        public string Save_Path;
        public string Save_Name;
        public string Save_Extension;
        public string Save_ID;
        public bool Is_Big_Endian = true;
        private FileStream Save_File;
        private BinaryReader Save_Reader;
        private BinaryWriter Save_Writer;

        public Save(string File_Path)
        {
            if (File.Exists(File_Path))
            {
                if (Save_File != null)
                {
                    Save_Reader.Close();
                    Save_File.Close();
                }
                bool Failed_to_Load = false;
                try { Save_File = new FileStream(File_Path, FileMode.Open); } catch { Failed_to_Load = true; }
                if (Save_File == null || Failed_to_Load || !Save_File.CanWrite)
                {
                    MessageBox.Show(string.Format("Error: File {0} is being used by another process. Please close any process using it before editing!",
                        Path.GetFileName(File_Path)), "File Opening Error");
                    try { Save_File.Close(); } catch { };
                    return;
                }

                Save_Reader = new BinaryReader(Save_File);

                Original_Save_Data = Save_Type == SaveType.Doubutsu_no_Mori ? SaveDataManager.ByteSwap(Save_Reader.ReadBytes((int)Save_File.Length)) : Save_Reader.ReadBytes((int)Save_File.Length);
                Working_Save_Data = new byte[Original_Save_Data.Length];
                Buffer.BlockCopy(Original_Save_Data, 0, Working_Save_Data, 0, Original_Save_Data.Length);

                Save_Type = SaveDataManager.GetSaveType(Original_Save_Data);
                Save_Name = Path.GetFileNameWithoutExtension(File_Path);
                Save_Path = Path.GetDirectoryName(File_Path) + Path.DirectorySeparatorChar;
                Save_Extension = Path.GetExtension(File_Path);
                Save_ID = SaveDataManager.GetGameID(Save_Type);
                Save_Data_Start_Offset = SaveDataManager.GetSaveDataOffset(Save_ID.ToLower(), Save_Extension.Replace(".", "").ToLower());

                if (Save_Type == SaveType.Wild_World || Save_Type == SaveType.New_Leaf || Save_Type == SaveType.Welcome_Amiibo)
                    Is_Big_Endian = false;

                Save_Reader.Close();
                Save_File.Close();
            }
            else
                MessageBox.Show("File doesn't exist!");
        }

        public void Flush()
        {
            string Full_Save_Name = Save_Path + Path.DirectorySeparatorChar + Save_Name + Save_Extension;
            Save_File = new FileStream(Full_Save_Name, FileMode.OpenOrCreate);
            Save_Writer = new BinaryWriter(Save_File);
            if (Save_Type == SaveType.Animal_Crossing)
            {
                Write(Save_Data_Start_Offset + 0x12, Checksum.Calculate(Working_Save_Data.Skip(Save_Data_Start_Offset).Take(0x26000).ToArray(), 0x12), true);
                Working_Save_Data.Skip(Save_Data_Start_Offset).Take(0x26000).ToArray().CopyTo(Working_Save_Data, Save_Data_Start_Offset + 0x26000); //Update second save copy
            }
            else if (Save_Type == SaveType.Wild_World)
            {
                Write(Save_Data_Start_Offset + 0x15FDC, Checksum.Calculate(Working_Save_Data.Skip(Save_Data_Start_Offset).Take(0x15FE0).ToArray(),
                    0x15FDC, true));
                Working_Save_Data.Skip(Save_Data_Start_Offset).Take(0x15FE0).ToArray().CopyTo(Working_Save_Data, Save_Data_Start_Offset + 0x15FE0); //Update both save copies on WW
            }
            else if (Save_Type == SaveType.City_Folk)
            {
                for (int i = 0; i < 4; i++)
                {
                    int Player_Data_Offset = Save_Data_Start_Offset + i * 0x86C0 + 0x1140;
                    uint Player_CRC32 = CRC32.GetCRC32(Working_Save_Data.Skip(Player_Data_Offset + 4).Take(0x759C).ToArray());
                    Write(Player_Data_Offset, Player_CRC32, true);
                }
                Write(Save_Data_Start_Offset + 0x5EC60, CRC32.GetCRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x5EC64).Take(0x1497C).ToArray()), true);
                Write(Save_Data_Start_Offset + 0x5EB04, CRC32.GetCRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x5EB08).Take(0x152).ToArray(), 0x12141018), true);
                Write(Save_Data_Start_Offset + 0x73600, CRC32.GetCRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x73604).Take(0x19BD1C).ToArray()), true);
                Write(Save_Data_Start_Offset, CRC32.GetCRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 4).Take(0x1C).ToArray()), true);
                Write(Save_Data_Start_Offset + 0x20, CRC32.GetCRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x24).Take(0x111C).ToArray()), true);
            }
            else if (Save_Type == SaveType.New_Leaf)
            {
                Write(Save_Data_Start_Offset, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 4).Take(0x1C).ToArray()));
                for (int i = 0; i < 4; i++)
                {
                    int DataOffset = Save_Data_Start_Offset + 0x20 + i * 0x9F10;
                    Write(DataOffset, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(DataOffset + 4).Take(0x6B64).ToArray()));
                    int DataOffset2 = Save_Data_Start_Offset + 0x20 + 0x6B68 + i * 0x9F10;
                    Write(DataOffset2, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(DataOffset2 + 4).Take(0x33A4).ToArray()));
                    
                }
                Write(Save_Data_Start_Offset + 0x27C60, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x27C60 + 4).Take(0x218B0).ToArray()));
                Write(Save_Data_Start_Offset + 0x49520, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x49520 + 4).Take(0x44B8).ToArray()));
                Write(Save_Data_Start_Offset + 0x4D9DC, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x4D9DC + 4).Take(0x1E420).ToArray()));
                Write(Save_Data_Start_Offset + 0x6BE00, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x6BE00 + 4).Take(0x20).ToArray()));
                Write(Save_Data_Start_Offset + 0x6BE24, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x6BE24 + 4).Take(0x13AF8).ToArray()));
            }
            else if (Save_Type == SaveType.Welcome_Amiibo)
            {
                Write(Save_Data_Start_Offset, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 4).Take(0x1C).ToArray()));
                for (int i = 0; i < 4; i++)
                {
                    int DataOffset = Save_Data_Start_Offset + 0x20 + i * 0xA480;
                    Write(DataOffset, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(DataOffset + 4).Take(0x6B84).ToArray()));
                    int DataOffset2 = Save_Data_Start_Offset + 0x20 + 0x6B88 + i * 0xA480;
                    Write(DataOffset2, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(DataOffset2 + 4).Take(0x38F4).ToArray()));
                }
                Write(Save_Data_Start_Offset + 0x29220, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x29220 + 4).Take(0x22BC8).ToArray()));
                Write(Save_Data_Start_Offset + 0x4BE00, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x4BE00 + 4).Take(0x44B8).ToArray()));
                Write(Save_Data_Start_Offset + 0x533A4, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x533A4 + 4).Take(0x1E4D8).ToArray()));
                Write(Save_Data_Start_Offset + 0x71880, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x71880 + 4).Take(0x20).ToArray()));
                Write(Save_Data_Start_Offset + 0x718A4, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x718A4 + 4).Take(0xBE4).ToArray()));
                Write(Save_Data_Start_Offset + 0x738D4, NL_CRC32.Calculate_CRC32(Working_Save_Data.Skip(Save_Data_Start_Offset + 0x738D4 + 4).Take(0x16188).ToArray()));
            }
            Save_Writer.Write(Save_Type == SaveType.Doubutsu_no_Mori ? SaveDataManager.ByteSwap(Working_Save_Data) : Working_Save_Data); //Doubutsu no Mori is dword byteswapped
            Save_Writer.Flush();
            Save_File.Flush();

            Save_Writer.Close();
            Save_File.Close();
        }

        public void Write(int offset, dynamic data, bool reversed = false, int stringLength = 0)
        {
            Type Data_Type = data.GetType();
            NewMainForm.Debug_Manager.WriteLine(string.Format("Writing Data{2} of type {0} to offset {1}", Data_Type.Name, "0x" + offset.ToString("X"), //recasting a value shows it as original type?
                    Data_Type.IsArray ? "" : " with value 0x" + (data.ToString("X"))), DebugLevel.Debug);
            if (!Data_Type.IsArray)
            {
                if (Data_Type == typeof(byte))
                    Working_Save_Data[offset] = (byte)data;
                else if (Data_Type == typeof(string))
                {
                    byte[] String_Byte_Buff = ACString.GetBytes((string)data, stringLength);
                    Buffer.BlockCopy(String_Byte_Buff, 0, Working_Save_Data, offset, String_Byte_Buff.Length);
                }
                else
                {
                    byte[] Byte_Array = BitConverter.GetBytes(data);
                    if (reversed)
                        Array.Reverse(Byte_Array);
                    Buffer.BlockCopy(Byte_Array, 0, Working_Save_Data, offset, Byte_Array.Length);
                }
            }
            else
            {
                dynamic Data_Array = data;
                if (Data_Type == typeof(byte[]))
                    for (int i = 0; i < Data_Array.Length; i++)
                        Working_Save_Data[offset + i] = Data_Array[i];
                else
                {
                    int Data_Size = Marshal.SizeOf(Data_Array[0]);
                    for (int i = 0; i < Data_Array.Length; i++)
                    {
                        byte[] Byte_Array = BitConverter.GetBytes(Data_Array[i]);
                        if (reversed)
                            Array.Reverse(Byte_Array);
                        Byte_Array.CopyTo(Working_Save_Data, offset + i * Data_Size);
                    }
                }
            }
        }

        //Used for replacing instances of player/town names. Possibly other uses?
        public void FindAndReplaceByteArray(int end, byte[] oldarr, byte[] newarr)
        {
            for (int i = Save_Data_Start_Offset; i < Save_Data_Start_Offset + end; i += 2) //Every? dword or larger is aligned
            {
                if (Enumerable.SequenceEqual(ReadByteArray(i, oldarr.Length), oldarr))
                {
                    MessageBox.Show("Match found at offset: 0x" + i.ToString("X"));
                    Write(i, newarr, Is_Big_Endian);
                }
            }
        }

        public byte ReadByte(int offset)
        {
            return Working_Save_Data[offset];
        }

        public byte[] ReadByteArray(int offset, int count, bool reversed = false)
        {
            byte[] Data = new byte[count];
            Buffer.BlockCopy(Working_Save_Data, offset, Data, 0, count);
            if (reversed)
                Array.Reverse(Data);
            return Data;
        }

        public ushort ReadUInt16(int offset, bool reversed = false)
        {
            return BitConverter.ToUInt16(ReadByteArray(offset, 2, reversed), 0);
        }

        public ushort[] ReadUInt16Array(int offset, int count, bool reversed = false)
        {
            ushort[] Returned_Values = new ushort[count];
            for (int i = 0; i < count; i++)
                Returned_Values[i] = ReadUInt16(offset + i * 2, reversed);
            return Returned_Values;
        }

        public uint ReadUInt32(int offset, bool reversed = false)
        {
            return BitConverter.ToUInt32(ReadByteArray(offset, 4, reversed), 0);
        }

        public uint[] ReadUInt32Array(int offset, int count, bool reversed = false)
        {
            uint[] Returned_Values = new uint[count];
            for (int i = 0; i < count; i++)
                Returned_Values[i] = ReadUInt32(offset + i * 4, reversed);
            return Returned_Values;
        }

        public ulong ReadUInt64(int offset, bool reversed = false)
        {
            return BitConverter.ToUInt64(ReadByteArray(offset, 8, reversed), 0);
        }

        public string ReadString(int offset, int length)
        {
             return new ACString(ReadByteArray(offset, length), Save_Type).Trim();
        }

        public string[] ReadStringArray(int offset, int length, int count) //Only useful for strings of matching lengths, possibly "fix" this in the future
        {
            string[] String_Array = new string[count];
            for (int i = 0; i < count; i++)
                String_Array[i] = ReadString(offset + i * length, length);
            return String_Array;
        }

        public string[] ReadStringArrayWithVariedLengths(int offset, int count, byte endCharByte, int maxLength = 10) // ^^ Fixed it ^^
        {
            string[] String_Array = new string[count];
            int lastOffset = 0;
            for (int i = 0; i < count; i++)
            {
                byte lastChar = 0;
                int idx = 0;
                while (lastChar != endCharByte && idx < maxLength)
                {
                    lastChar = ReadByte(offset + lastOffset + idx);
                    idx++;
                }
                String_Array[i] = ReadString(offset + lastOffset, idx);
                lastOffset += idx;
            }
            return String_Array;
        }
    }
}
