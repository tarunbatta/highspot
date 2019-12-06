﻿using System;
using System.Collections.Generic;
using System.Linq;
using highspot.Entity;
using highspot.Helper;
using Newtonsoft.Json;

namespace highspot
{
    class Program
    {
        static void Main(string[] args)
        {
            string file_mixtape = "mixtape-data.json";
            string file_changes = "changes.json";
            string file_output = "output.json";

            if (args != null && args.Length > 0)
            {
                file_mixtape = args.Length > 0 ? args[0] : file_mixtape;
                file_changes = args.Length > 1 ? args[1] : file_changes;
                file_output = args.Length > 2 ? args[2] : file_output;
            }

            string mixTapeData = Utility.ReadFile(Config.folder_assets + file_mixtape);
            Mixtape mixTape = JsonConvert.DeserializeObject<Mixtape>(mixTapeData);

            if (mixTape != null)
            {
                Console.WriteLine("MixTape Data loaded successfully");
            }

            string changesData = Utility.ReadFile(Config.folder_assets + file_changes);
            Change changes = JsonConvert.DeserializeObject<Change>(changesData);

            if (changes != null)
            {
                Console.WriteLine("Changes Data loaded successfully");
            }

            if (mixTape != null && changes != null && changes.changes != null && changes.changes.Count > 0)
            {
                foreach (IChange item in changes.changes)
                {
                    if (item is AddSong)
                    {
                        AddSong i = (AddSong)item;
                        Song isSongPresent = mixTape.songs.Where(x => x.id == i.id).FirstOrDefault();
                        Playlist isPlaylistPresent = mixTape.playlists.Where(x => x.id == i.playlist_id).FirstOrDefault();

                        if (isSongPresent != null && isPlaylistPresent != null)
                        {
                            isPlaylistPresent.song_ids.Add(i.id);
                        }
                    }
                    else if (item is AddPlaylist)
                    {
                        //                        AddPlaylist i = (AddPlaylist)item;
                        User isUserPresent = mixTape.users.Where(x => x.id == ((AddPlaylist)item).user_id).FirstOrDefault();
                        List<Song> isSongsPresent = mixTape.songs.Where(x => ((AddPlaylist)item).song_ids.Contains(x.id)).ToList();

                        if (isUserPresent != null && isSongsPresent != null && isSongsPresent.Count > 0)
                        {
                            mixTape.playlists.Add(new Playlist(((AddPlaylist)item).id, ((AddPlaylist)item).user_id, ((AddPlaylist)item).song_ids));
                        }
                    }
                    else if (item is RemovePlaylist)
                    {
                        RemovePlaylist i = ((RemovePlaylist)item);
                        Playlist isPlaylistPresent = mixTape.playlists.Where(x => x.id == i.id).FirstOrDefault();

                        if (isPlaylistPresent != null)
                        {
                            mixTape.playlists.Remove(isPlaylistPresent);
                        }
                    }
                }
            }

            Utility.WriteFile(Config.folder_assets + file_output, JsonConvert.SerializeObject(mixTape));
            Console.WriteLine("Output Data written successfully");
        }
    }
}
