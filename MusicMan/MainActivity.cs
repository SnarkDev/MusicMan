using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using System.IO;

namespace MusicMan
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Button downloadButton = FindViewById<Button>(Resource.Id.DownloadButton);
            downloadButton.Click += DownloadButton_Click;
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            string link = FindViewById<EditText>(Resource.Id.UrlEditText).Text;

            var youtube = new YoutubeClient();
            Video video = null;
            StreamManifest manifest = null;
            Stream videoStream = null;

            //try
            //{
            //    video = await GetMetaData("https://www.youtube.com/watch?v=SrdthGuaXNk", youtube);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

            //var title = video.Title; 
            //var author = video.Author.Title; 
            //var duration = video.Duration;

            try
            {
                manifest = await GetStreamManifest("SrdthGuaXNk", youtube);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var videoStreamInfo = manifest.GetVideoOnlyStreams().Where(s => s.Container == Container.Mp4).GetWithHighestVideoQuality();
            var audioStreamInfo = manifest.GetAudioStreams().GetWithHighestBitrate();

            //try
            //{
            //    videoStream = await GetActualStream(youtube, streamInfo);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

            try
            {
                await DonwloadVideo(youtube, videoStreamInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                await DonwloadAudio(youtube, audioStreamInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("** Downloaded Data **");
        }

        private async Task<Video> GetMetaData(string url, YoutubeClient client)
        {
            return await client.Videos.GetAsync(url);
        }

        private async Task<StreamManifest> GetStreamManifest(string videoId, YoutubeClient client)
        {
            return await client.Videos.Streams.GetManifestAsync(videoId);
        }

        private async Task<Stream> GetActualStream(YoutubeClient client, IStreamInfo streamInfo)
        {
            return await client.Videos.Streams.GetAsync(streamInfo);
        }

        private async Task DonwloadVideo(YoutubeClient client, IStreamInfo streamInfo)
        {
            string testPath = Application.Context.GetExternalFilesDir("").AbsolutePath + $"test.mp4";
            await client.Videos.Streams.DownloadAsync(streamInfo, testPath);
        }

        private async Task DonwloadAudio(YoutubeClient client, IStreamInfo streamInfo)
        {
            string musicPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath + $"/test.mp3";
            await client.Videos.Streams.DownloadAsync(streamInfo, musicPath);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
