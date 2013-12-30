using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MS.Internal.Ink;

namespace WdbCreatureCacheReader
{
    class Program
    {
        const string fileName = "creaturecache.wdb";
        static void Main(string[] args)
        {
            if (!System.IO.File.Exists(fileName))
                throw new System.IO.FileNotFoundException();

            int record_count = 0;

            using (var wdb_stream = new BinaryReader(File.OpenRead(fileName)))
            {
                var signature = wdb_stream.ReadBytes(4);
                var build     = wdb_stream.ReadUInt32();
                var locale    = Encoding.UTF8.GetString(wdb_stream.ReadBytes(4).Reverse().ToArray());
                var unk_h1    = wdb_stream.ReadInt32();
                var unk_h2    = wdb_stream.ReadInt32();
                var version   = wdb_stream.ReadInt32();

                Console.WriteLine(fileName);
                Console.WriteLine("Locale: " + locale);
                Console.WriteLine("Build: " + build);

                using (var writer = File.CreateText(fileName + ".sql"))
                {
                    while (wdb_stream.BaseStream.Position != wdb_stream.BaseStream.Length)
                    {
                        var entry = wdb_stream.ReadInt32();
                        var size  = wdb_stream.ReadInt32();

                        // eof
                        if (entry == 0 && size == 0)
                            break;

                        ++record_count;

                        #region reader

                        var row_bytes = wdb_stream.ReadBytes(size);
                        var reader    = new BitStreamReader(row_bytes);

                        // entry = wdb_stream.ReadInt32();
                        int subname_len         = (int)reader.ReadUInt32(11);
                        int unk_text_len        = (int)reader.ReadUInt32(11);
                        int icon_name_len       = (int)reader.ReadUInt32(6);

                        var racial_leader = reader.ReadBit() ? 1 : 0; // racial lesder

                        int[] male_names_len    = new int[4];
                        int[] female_name_len   = new int[4];
                        string[] male_names     = new string[4];
                        string[] female_names   = new string[4];

                        for (int i2 = 0; i2 < 4; ++i2)
                        {
                            male_names_len[i2]  = (int)reader.ReadUInt32(11);// name size
                            female_name_len[i2] = (int)reader.ReadUInt32(11);// unk size
                        }

                        for (int i2 = 0; i2 < 4; ++i2)
                        {
                            male_names[i2]      = reader.ReadEsqapedSqlString2(male_names_len[i2]);
                            female_names[i2]    = reader.ReadEsqapedSqlString2(female_name_len[i2]);
                        }

                        var type_flags          = reader.ReadUInt32();
                        var unk_541             = reader.ReadInt32(); // flag ???

                        var type                = reader.ReadInt32();
                        var family              = reader.ReadInt32();
                        var rank                = reader.ReadInt32();

                        var kill_kredit1        = reader.ReadInt32();
                        var kill_kredit2        = reader.ReadInt32();

                        var modelid1            = reader.ReadInt32();
                        var modelid2            = reader.ReadInt32();
                        var modelid3            = reader.ReadInt32();
                        var modelid4            = reader.ReadInt32();

                        var HealthModifier      = reader.ReadFloat();
                        var PowerModifier       = reader.ReadFloat();

                        var quest_item_count    = reader.ReadInt32();
                        var movement_id         = reader.ReadInt32();
                        var unk543              = reader.ReadInt32(); // addon version ???

                        var sub_name            = reader.ReadEsqapedSqlString2(subname_len);
                        var unk_text            = reader.ReadEsqapedSqlString2(unk_text_len); // have 3 records: entry in (61665 66201 70323)

                        var icon_name           = reader.ReadEsqapedSqlString2(icon_name_len);

                        int[] QuestItem = new int[6];
                        for (int i = 0; i < quest_item_count; ++i)
                            QuestItem[i] = reader.ReadInt32();

                        if (reader.Buffer.Length != reader.Index)
                            Console.WriteLine(reader.Buffer.Length - reader.Index);

                        #endregion

                        #region SQL
                        writer.WriteLine("REPLACE INTO `creaturecache` VALUES (\'"+locale+"\', {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, {37}, {38}, {39}, {40}, {41}, {42}, {43}, {44}, {45});",
                            entry,
                            subname_len,
                            unk_text_len,
                            icon_name_len,
                            racial_leader,

                            male_names_len[0], female_name_len[0],
                            male_names_len[1], female_name_len[1],
                            male_names_len[2], female_name_len[2],
                            male_names_len[3], female_name_len[3],

                            male_names[0],
                            female_names[0],

                            male_names[1],
                            female_names[1],

                            male_names[2],
                            female_names[2],

                            male_names[3],
                            female_names[3],

                            type_flags, unk_541,
                            type, family, rank,
                            kill_kredit1, kill_kredit2,
                            modelid1, modelid2, modelid3, modelid4,

                            HealthModifier.ToString(CultureInfo.InvariantCulture),
                            PowerModifier.ToString(CultureInfo.InvariantCulture),

                            quest_item_count,
                            movement_id,
                            unk543,

                            sub_name,
                            unk_text,
                            icon_name,
                        
                            QuestItem[0],
                            QuestItem[1],
                            QuestItem[2],
                            QuestItem[3],
                            QuestItem[4],
                            QuestItem[5]
                            );
                        #endregion
                    }
                    writer.Flush();
                }
            }
            Console.WriteLine("Reading {0} records", record_count);
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
