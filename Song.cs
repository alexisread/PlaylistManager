using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media;

namespace PlaylistManager
{
    enum SongAvailability
    {
        Matched,
        MissingFromPlaylist,
        MissingFromFiles
    }

	class Song
	{
		public static ObservableCollection<Song> CreateSongList(string directory)
		{
			ObservableCollection<Song> list = new ObservableCollection<Song>();
            
            // Exclude playlists, this app might then be useful for multiple music types mp3, wma etc.
            List<FileInfo> files = DirSearch(directory)
                                    .Where(x => !x.Name.Contains(".pla") && !x.Name.Contains(".m3u"))
                                    .OrderBy(p => p.CreationTime).ToList();
            foreach (FileInfo file in files)
            {
                list.Add(new Song(file.Name, file.DirectoryName, SongAvailability.Matched));
            }
			return list;
		}

		string name;
        string path;
        SongAvailability availability;
        Brush highlightColor;

		public Song( string name, string path )
		{
			this.name = name;
            this.path = path;
		}

        public Song(string name, string path, SongAvailability availability)
        {
            this.name = name;
            this.path = path;
            this.SongAvailability = availability;
        }

		public string Name
		{
			get { return this.name; }
		}

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public SongAvailability SongAvailability
        {
            get { return this.availability; }
            set
            {
                if (value == SongAvailability.MissingFromFiles) this.highlightColor = Brushes.Red;
                else if (value == SongAvailability.MissingFromPlaylist) this.highlightColor = Brushes.Orange;
                else this.highlightColor = Brushes.Green;

                this.availability = value;
            }
        }

        public Brush HighlightColor
        {
            get { return this.highlightColor; }
        }

        private static List<FileInfo> DirSearch(string sDir)
        {
            List<FileInfo> files = new List<FileInfo>();
            try
            {
                DirectoryInfo info = new DirectoryInfo(sDir);
                foreach (FileInfo f in info.GetFiles())
                    files.Add(f);

                // We need an assigment for files, as the Linq statement is just a select
                foreach (DirectoryInfo d in info.GetDirectories())
                    files = files.Concat(DirSearch(sDir + "\\" + d.Name)).ToList();
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return files;
        }

	}
}
