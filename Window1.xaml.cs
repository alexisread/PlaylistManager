using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.JoshSmith.ServiceProviders.UI;
using System.IO;
using System.Linq;
using FORMS = System.Windows.Forms;

namespace PlaylistManager
{
	/// <summary>
	/// Demonstrates how to use the ListViewDragManager class.
	/// </summary>
	public partial class Window1 : System.Windows.Window
	{
		ListViewDragDropManager<Song> dragMgr;
		ListViewDragDropManager<Song> dragMgr2;

		public Window1()
		{
			InitializeComponent();
			this.Loaded += Window1_Loaded;
		}

		#region Window1_Loaded

		void Window1_Loaded( object sender, RoutedEventArgs e )
		{
			// Give the ListView an ObservableCollection of Song 
			// as a data source.  Note, the ListViewDragManager MUST
			// be bound to an ObservableCollection, where the collection's
			// type parameter matches the ListViewDragManager's type
			// parameter (in this case, both have a type parameter of Song).
			//ObservableCollection<Song> Songs = Song.CreateSongs();
			//this.listView.ItemsSource = Songs;
            ObservableCollection<Song> Songs = new ObservableCollection<Song>();
            this.listView.ItemsSource = Songs;

			this.listView2.ItemsSource = new ObservableCollection<Song>();

			// This is all that you need to do, in order to use the ListViewDragManager.
			this.dragMgr = new ListViewDragDropManager<Song>( this.listView );
			this.dragMgr2 = new ListViewDragDropManager<Song>( this.listView2 );

			// Turn the ListViewDragManager on and off. 
			this.chkManageDragging.Checked += delegate { this.dragMgr.ListView = this.listView; };
			this.chkManageDragging.Unchecked += delegate { this.dragMgr.ListView = null; };

			// Show and hide the drag adorner.
			this.chkDragAdorner.Checked += delegate { this.dragMgr.ShowDragAdorner = true; };
			this.chkDragAdorner.Unchecked += delegate { this.dragMgr.ShowDragAdorner = false; };

			// Change the opacity of the drag adorner.
			this.sldDragOpacity.ValueChanged += delegate { this.dragMgr.DragAdornerOpacity = this.sldDragOpacity.Value; };

			// Apply or remove the item container style, which responds to changes
			// in the attached properties of ListViewItemDragState.
			this.chkApplyContStyle.Checked += delegate { this.listView.ItemContainerStyle = this.FindResource( "ItemContStyle" ) as Style; };
			this.chkApplyContStyle.Unchecked += delegate { this.listView.ItemContainerStyle = null; };

			// Use or do not use custom drop logic.
			this.chkSwapDroppedItem.Checked += delegate { this.dragMgr.ProcessDrop += dragMgr_ProcessDrop; };
			this.chkSwapDroppedItem.Unchecked += delegate { this.dragMgr.ProcessDrop -= dragMgr_ProcessDrop; };

			// Show or hide the lower ListView.
			this.chkShowOtherListView.Checked += delegate { this.listView2.Visibility = Visibility.Visible; };
			this.chkShowOtherListView.Unchecked += delegate { this.listView2.Visibility = Visibility.Collapsed; };

			// Hook up events on both ListViews to that we can drag-drop
			// items between them.
			this.listView.DragEnter += OnListViewDragEnter;
			this.listView2.DragEnter += OnListViewDragEnter;
			this.listView.Drop += OnListViewDrop;
			this.listView2.Drop += OnListViewDrop;

            // Hook up file buttons
            this.chooseFolder.Click += ChooseScanFolder;
            this.choosePlaylist.Click += ChoosePlaylist;
            this.chooseCopyFolder.Click += ChooseCopyFolder;
            this.scanFiles.Click += ScanAgain;
            this.sortFiles.Click += SortSonglist;
            this.updateFiles.Click += UpdateSongList;
            this.moveUp.Click += ProcessUp;
            this.moveDown.Click += ProcessDown;
        }

		#endregion // Window1_Loaded

		#region dragMgr_ProcessDrop

		// Performs custom drop logic for the top ListView.
		void dragMgr_ProcessDrop( object sender, ProcessDropEventArgs<Song> e )
		{
			// This shows how to customize the behavior of a drop.
			// Here we perform a swap, instead of just moving the dropped item.

			int higherIdx = Math.Max( e.OldIndex, e.NewIndex );
			int lowerIdx = Math.Min( e.OldIndex, e.NewIndex );

			if( lowerIdx < 0 )
			{
				// The item came from the lower ListView
				// so just insert it.
				e.ItemsSource.Insert( higherIdx, e.DataItem );
			}
			else
			{
				// null values will cause an error when calling Move.
				// It looks like a bug in ObservableCollection to me.
				if( e.ItemsSource[lowerIdx] == null ||
					e.ItemsSource[higherIdx] == null )
					return;

				// The item came from the ListView into which
				// it was dropped, so swap it with the item
				// at the target index.
				e.ItemsSource.Move( lowerIdx, higherIdx );
				e.ItemsSource.Move( higherIdx - 1, lowerIdx );
			}

			// Set this to 'Move' so that the OnListViewDrop knows to 
			// remove the item from the other ListView.
			e.Effects = DragDropEffects.Move;
		}

		#endregion // dragMgr_ProcessDrop

		#region OnListViewDragEnter

		// Handles the DragEnter event for both ListViews.
		void OnListViewDragEnter( object sender, DragEventArgs e )
		{
			e.Effects = DragDropEffects.Move;
		}

		#endregion // OnListViewDragEnter

		#region OnListViewDrop

		// Handles the Drop event for both ListViews.
		void OnListViewDrop( object sender, DragEventArgs e )
		{
			if( e.Effects == DragDropEffects.None )
				return;

			Song Song = e.Data.GetData( typeof( Song ) ) as Song;
			if( sender == this.listView )
			{
				if( this.dragMgr.IsDragInProgress )
					return;

				// An item was dragged from the bottom ListView into the top ListView
				// so remove that item from the bottom ListView.
				(this.listView2.ItemsSource as ObservableCollection<Song>).Remove( Song );
			}
			else
			{
				if( this.dragMgr2.IsDragInProgress )
					return;

				// An item was dragged from the top ListView into the bottom ListView
				// so remove that item from the top ListView.
				(this.listView.ItemsSource as ObservableCollection<Song>).Remove( Song );
			}
		}

		#endregion // OnListViewDrop

        #region Buttons

        void ChooseScanFolder(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == FORMS.DialogResult.OK)
            {
                this.folderLocation.Text = dialog.SelectedPath.ToString();
            }
        }

        void ChoosePlaylist(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".pla"; // Default file extension
            dlg.Filter = "Playlists (*.pla, *.m3u)|*.pla;*.m3u|All files (*.*)|*.*"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                this.playlistLocation.Text = dlg.FileName;
            }
        }

        void ChooseCopyFolder(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == FORMS.DialogResult.OK)
            {
                this.copyLocation.Text = dialog.SelectedPath.ToString();
            }
        }

        void ScanAgain(object sender, EventArgs e)
        {
            ObservableCollection<Song> songs = Song.CreateSongList(this.folderLocation.Text);
            this.listView.ItemsSource = songs;
        }

        void SortSonglist(object sender, RoutedEventArgs e)
        {
            // Read in playlist
            List<Song> playlist = new List<Song>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(this.playlistLocation.Text);
            while ((line = file.ReadLine()) != null)
            {
                string playlistPath = this.playlistLocation.Text.Substring(0, this.playlistLocation.Text.LastIndexOf("\\"));
                string songName = line.Substring(line.LastIndexOf("\\") +1);
                string relSongPath = line.Replace(playlistPath, "").Substring(0, line.LastIndexOf("\\") +1);
                playlist.Add(new Song(songName, ""));
            }
            file.Close();

            // Match against filelist
            ObservableCollection<Song> songs = Song.CreateSongList(this.folderLocation.Text);
            foreach (Song p in playlist)
            {
                Song s = songs.Where(x => x.Name == p.Name).FirstOrDefault();
                if (s != null)
                {
                    p.SongAvailability = SongAvailability.Matched;
                    p.Path = s.Path;
                }
                else
                    p.SongAvailability = SongAvailability.MissingFromFiles;
            }

            // Add on unreferenced files at the end of the playlist
            foreach (Song s in songs)
            {
                if (!playlist.Any(x => x.Name == s.Name))
                    playlist.Add(new Song(s.Name, s.Path, SongAvailability.MissingFromPlaylist));
            }

            // Replace filelist
            this.listView.ItemsSource = new ObservableCollection<Song>(playlist);
        }

        void UpdateSongList(object sender, RoutedEventArgs e)
        {
            // Clear directory as we need to write in order
            Array.ForEach(Directory.GetFiles(this.copyLocation.Text), File.Delete);

            // playlistName is the last folder name
            string playlistName = this.copyLocation.Text.Substring(this.copyLocation.Text.LastIndexOf("\\") +1);
            int i = 1;
            System.IO.StreamWriter file = new System.IO.StreamWriter(this.copyLocation.Text + "\\" + playlistName + ".pla");

            foreach (Song song in this.listView.ItemsSource)
            {
                //TagLib.File f = TagLib.File.Create(this.folderLocation.Text + "\\" + song.Name);
                //f.Tag.Track = (uint)i;
                //f.Tag.Album = "Playlist";
                //f.Tag.Year = (uint)DateTime.Today.Year;
                //f.Tag.TitleSort = i.ToString();
                //f.Tag.AlbumArtists = new string[] {"Playlist"};
                //f.Save();

                //File.SetCreationTime(song.Path + "\\" + song.Name, DateTime.Now.AddDays(-1).AddMinutes(i));
                //File.SetLastWriteTime(song.Path + "\\" + song.Name, DateTime.Now.AddDays(-1).AddMinutes(i));
                //File.SetLastAccessTime(song.Path + "\\" + song.Name, DateTime.Now.AddDays(-1).AddMinutes(i));

                // For the moment, we are flattening the copy
                File.Copy(song.Path + "\\" + song.Name, this.copyLocation.Text + "\\" + song.Name);
 
                //string relSongPath = song.Path.Substring(this.folderLocation.Text.Length);
                //file.WriteLine(relSongPath + "\\" + song.Name);
                file.WriteLine(song.Name);

                i++;
            }
            file.Close();
        }

        void ProcessUp(object sender, RoutedEventArgs e)
        {
            // At top of list already
            if (this.listView.SelectedIndex == 0) return;

            int oldIndex = this.listView.SelectedIndex;
            int newIndex = this.listView.SelectedIndex - 1;
            ObservableCollection<Song> Songs = this.listView.ItemsSource as ObservableCollection<Song>;

            // Let the client code process the drop.
            ProcessDropEventArgs<Song> args = new ProcessDropEventArgs<Song>(Songs, Songs[this.listView.SelectedIndex], oldIndex, newIndex, DragDropEffects.Move);
            dragMgr_ProcessDrop(sender, args);

            // Re-assign index after swap.
            this.listView.SelectedIndex = newIndex;
        }

        void ProcessDown(object sender, RoutedEventArgs e)
        {
            // At bottom of list already
            if (this.listView.SelectedIndex+1 == this.listView.Items.Count) return;

            int oldIndex = this.listView.SelectedIndex;
            int newIndex = this.listView.SelectedIndex+1;
            ObservableCollection<Song> Songs = this.listView.ItemsSource as ObservableCollection<Song>;

            // Let the client code process the drop.
            ProcessDropEventArgs<Song> args = new ProcessDropEventArgs<Song>(Songs, Songs[this.listView.SelectedIndex], oldIndex, newIndex, DragDropEffects.Move);
            dragMgr_ProcessDrop(sender, args);
        }

        void DeleteSubject(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string name = b.CommandParameter.ToString();
            ObservableCollection<Song> songs = this.listView.ItemsSource as ObservableCollection<Song>;
            songs.Remove(songs.Where(x => x.Name == name).FirstOrDefault());
        }

        #endregion // Buttons

        #region Delete Command

        private ICommand removeSubjectCommand;

        public ICommand RemoveSubjectCommand
        {
            get { return removeSubjectCommand ?? (removeSubjectCommand = new RelayCommand(param => this.RemoveSubject(), null)); }
        }

        private void RemoveSubject()
        {
            string test = "got here";
        }

        #endregion // Delete Command
    }
}