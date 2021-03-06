﻿/* NormalListToMALXML.cs
 * This class converts a normalized list to MAL XML format.
 * 
 * Copyright (c) 2018 MAL Updater OS X Group, a division of Moy IT Solutions
 * Licensed under MIT License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NekomataCore
{
    public class NormalListToMALXML
    {
        private List<ListEntry> list;
        private List<ListEntry> validlist;
        public List<ListEntry> faillist;
        private EntryType listtype;
        private string Username;
        private Service currentservice;
        public TitleIDConverter tconverter;

        // Update on import
        public bool updateonimportcurrent;
        public bool updateonimportcompleted;
        public bool updateonimportonhold;
        public bool updateonimportdropped;
        public bool updateonimportplanned;

        private const String headerstring = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n\t<!--\n\tCreated by Nekomata\n\tProgrammed by MAL Updater OS X Group Software (James Moy), a division of Moy IT Solutions \n\tNote that not all values are exposed by the API and not all fields will be exported.\n\t--> \n\n\t<myanimelist>";
        private const String footerstring = "\n\n\t</myanimelist>";
        private const String animepretag = "\n\n\t\t<anime>";
        private const String animeendtag = "\n\t\t</anime>";
        private const String mangapretag = "\n\n\t\t<manga>";
        private const String mangaendtag = "\n\t\t</manga>";
        private const String tabformatting = "\n\t\t\t";

        public NormalListToMALXML()
        {
            
        }

        public void ConvertNormalizedListToMAL(List<ListEntry> list, EntryType type, String username, Service listservice)
        {
            this.Username = username;
            this.listtype = type;
            this.list = list;
            this.currentservice = listservice;
            this.validlist = new List<ListEntry>();
            this.faillist = new List<ListEntry>();
            ProcessList();
        }
        public string GenerateXML()
        {
            switch (listtype)
            {
                case EntryType.Anime:
                    return GenerateAnimeXML();
                case EntryType.Manga:
                    return GenerateMangaXML();
                default:
                    return "";
            }
        }
        public bool InvalidEntriesExist()
        {
            return (faillist.Count > 0);
        }
        private void ProcessList()
        {
            foreach (ListEntry entry in list)
            {
                int convertedtitleid = GetConvertedTitleID(entry.titleId);
                if (convertedtitleid > 0)
                {
                    entry.titleId = convertedtitleid;
                    entry.rating = entry.rating / 10;
                    validlist.Add(entry);
                }
                else
                {
                    faillist.Add(entry);
                }
            }
        }
        private string GenerateAnimeXML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(headerstring);
            sb.Append("\n\n\t<myinfo>");
            sb.Append(tabformatting + "<username>" + this.Username + "</username>");
            sb.Append(tabformatting + "<user_export_type>1</user_export_type>");
            sb.Append("\n\t</myinfo>");
            foreach (ListEntry entry in validlist)
            {
                sb.Append(animepretag);
                sb.Append(tabformatting + "<series_animedb_id>" + entry.titleId + "</series_animedb_id>");
                sb.Append(tabformatting + "<series_title><![CDATA[" + entry.title + "]]></series_title>");
                sb.Append(tabformatting + "<series_type>" + entry.mediaFormat + "</series_type>");
                sb.Append(tabformatting + "<series_episodes>" + entry.totalSegment + "</series_episodes>");
                sb.Append(tabformatting + "<my_id>0</my_id>");
                sb.Append(tabformatting + "<my_watched_episodes>" + entry.progress + "</my_watched_episodes>");
                sb.Append(tabformatting + "<my_start_date>" + entry.startDate + "</my_start_date>");
                sb.Append(tabformatting + "<my_finish_date>" + entry.endDate + "</my_finish_date>");
                sb.Append(tabformatting + "<my_rated></my_rated>");
                sb.Append(tabformatting + "<my_score>" + entry.rating + "</my_score>");
                sb.Append(tabformatting + "<my_dvd></my_dvd>");
                sb.Append(tabformatting + "<my_storage></my_storage>");
                sb.Append(tabformatting + "<my_status>" + ConvertNormalizedStatus(entry.entryStatus) + "</my_status>");
                sb.Append(tabformatting + "<my_comments><![CDATA[" + entry.personalComments + "]]></my_comments>");
                sb.Append(tabformatting + "<my_times_watched>" + entry.repeatCount + "</my_times_watched>");
                sb.Append(tabformatting + "<my_rewatch_value></my_rewatch_value>");
                sb.Append(tabformatting + "<my_tags><![CDATA[" + "" + "]]></my_tags>");
                sb.Append(tabformatting + "<my_rewatching>" + (entry.repeating ? "1" : "0") + "</my_rewatching>");
                sb.Append(tabformatting + "<my_rewatching_ep>0</my_rewatching_ep>");
                sb.Append(tabformatting + "<update_on_import>" + this.getUpdateonimport(entry.entryStatus) + "</update_on_import>");
                sb.Append(animeendtag);
            }
            sb.Append(footerstring);
            return sb.ToString();

        }
        private string GenerateMangaXML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(headerstring);
            sb.Append("\n\n\t<myinfo>");
            sb.Append(tabformatting + "<username>" + this.Username + "</username>");
            sb.Append(tabformatting + "<user_export_type>2</user_export_type>");
            sb.Append("\n\t</myinfo>");
            foreach (ListEntry entry in validlist)
            {
                sb.Append(mangapretag);
                sb.Append(tabformatting + "<manga_mangadb_id>" + entry.titleId + "</manga_mangadb_id>");
                sb.Append(tabformatting + "<manga_title><![CDATA[" + entry.title + "]]></manga_title>");
                sb.Append(tabformatting + "<manga_volumes>" + entry.totalVolumes + "</manga_volumes>");
                sb.Append(tabformatting + "<manga_chapters>" + entry.totalSegment + "</manga_chapters>");
                sb.Append(tabformatting + "<my_id>0</my_id>");
                sb.Append(tabformatting + "<my_read_volumes>" + entry.progressVolumes + "</my_read_volumes>");
                sb.Append(tabformatting + "<my_read_chapters>" + entry.progress + "</my_read_chapters>");
                sb.Append(tabformatting + "<my_start_date>" + entry.startDate + "</my_start_date>");
                sb.Append(tabformatting + "<my_finish_date>" + entry.endDate + "</my_finish_date>");
                sb.Append(tabformatting + "<my_scanalation_group><![CDATA[]]></my_scanalation_group>");
                sb.Append(tabformatting + "<my_score>" + entry.rating + "</my_score>");
                sb.Append(tabformatting + "<my_storage></my_storage>");
                sb.Append(tabformatting + "<my_status>" + ConvertNormalizedStatus(entry.entryStatus) + "</my_status>");
                sb.Append(tabformatting + "<my_comments><![CDATA[" + entry.personalComments + "]]></my_comments>");
                sb.Append(tabformatting + "<my_times_read>" + entry.repeatCount + "</my_times_read>");
                sb.Append(tabformatting + "<my_tags><![CDATA[" + "" + "]]></my_tags>");
                sb.Append(tabformatting + "<my_reread_value></my_reread_value>");
                sb.Append(tabformatting + "<update_on_import>" + this.getUpdateonimport(entry.entryStatus) + "</update_on_import>");
                sb.Append(mangaendtag);
            }
            sb.Append(footerstring);
            return sb.ToString();
        }
        private int GetConvertedTitleID(int titleid)
        {
            switch (currentservice)
            {
                case Service.AniList:
                    return tconverter.GetMALIDFromAniListID(titleid, listtype);
                case Service.Kitsu:
                    return tconverter.GetMALIDFromKitsuID(titleid, listtype);
                default:
                    return -1;
            }
        }
        private string ConvertNormalizedStatus(EntryStatus status)
        {
            switch (listtype)
            {
                case EntryType.Anime:
                    {
                        switch (status)
                        {
                            case EntryStatus.current:
                                return "Watching";
                            case EntryStatus.completed:
                                return "Completed";
                            case EntryStatus.dropped:
                                return "Dropped";
                            case EntryStatus.paused:
                                return "On-Hold";
                            case EntryStatus.planning:
                                return "Plan to Watch";
                            default:
                                return "";
                        }
                    }
                case EntryType.Manga:
                    {
                        switch (status)
                        {
                            case EntryStatus.current:
                                return "Reading";
                            case EntryStatus.completed:
                                return "Completed";
                            case EntryStatus.dropped:
                                return "Dropped";
                            case EntryStatus.paused:
                                return "On-Hold";
                            case EntryStatus.planning:
                                return "Plan to Read";
                            default:
                                return "";
                        }
                    }
                default:
                    return "";
            }
        }
        public void cleanup()
        {
            this.list.Clear();
            this.validlist.Clear();
            this.faillist.Clear();
        }
        private int getUpdateonimport(EntryStatus status)
        {
            switch (status)
            {
                case EntryStatus.current:
                    return this.updateonimportcurrent ? 1 : 0;
                case EntryStatus.completed:
                    return this.updateonimportcompleted ? 1 : 0;
                case EntryStatus.dropped:
                    return this.updateonimportdropped ? 1 : 0;
                case EntryStatus.paused:
                    return this.updateonimportonhold ? 1 : 0;
                case EntryStatus.planning:
                    return this.updateonimportplanned ? 1 : 0;
                default:
                    return 0;
            }
        }
    }
}
