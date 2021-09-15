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
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
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

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDJbgsQ6V6URc8GRdgnQdnuer_EWWogDKQ",
                ApplicationName = "MusicMan"
            });

            var searchListRequerst = youtubeService.Search.List("snippet");
            searchListRequerst.Q = "Metallica";
            searchListRequerst.MaxResults = 5;

            SearchListResponse response = new SearchListResponse();

            try
            {
                response = await ExecuteSearch(searchListRequerst);
            }
            catch
            {
                
            }

            var youtube = new YoutubeClient();
            YoutubeExplode.Videos.Video video = null;

            try
            {
                video = await GetMetaData("https://www.youtube.com/watch?v=SrdthGuaXNk", youtube);
            }
            catch 
            {

            }

            var title = video.Title; 
            var author = video.Author.Title; 
            var duration = video.Duration;

            StreamManifest manifest = null;

            try
            {
                manifest = await GetStreamManifest("SrdthGuaXNk", youtube);
            }
            catch
            {

            }


            var streamInfo = manifest.GetVideoOnlyStreams().Where(s => s.Container == Container.Mp4).GetWithHighestVideoQuality();
            Stream videoStream = null;

            try
            {
                videoStream = await GetActualStream(youtube, streamInfo);
            }
            catch
            {

            }

            try
            {
                await DonwloadVideo(youtube, streamInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task<SearchListResponse> ExecuteSearch(SearchResource.ListRequest request)
        {
            return await request.ExecuteAsync();
        }

        private async Task<YoutubeExplode.Videos.Video> GetMetaData(string url, YoutubeClient client)
        {
            return await client.Videos.GetAsync(url);
        }

        private async Task<YoutubeExplode.Videos.Streams.StreamManifest> GetStreamManifest(string videoId, YoutubeClient client)
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
