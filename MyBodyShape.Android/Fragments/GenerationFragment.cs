/**********************************************************/
/*************** The generation fragment
/**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using AndroidResource = Android.Resource;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Android.Graphics;
using Android.Content;
using Android.App;
using V4App = Android.Support.V4.App;
using RadialProgress;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MyBodyShape.Android.Helpers;
using MyBodyShape.Android.Listeners;
using System.Threading;
using Android.Support.V4.Content;

namespace MyBodyShape.Android.Fragments
{
    /// <summary>
    /// The generation fragment.
    /// </summary>
    public class GenerationFragment : V4App.Fragment
    {
        /// <summary>
        /// The fragment view.
        /// </summary>
        private View fragmentView;

        /// <summary>
        /// The generate model button.
        /// </summary>
        private Button generateButton;

        /// <summary>
        /// The minus height button.
        /// </summary>
        private Button minusHeightButton;

        /// <summary>
        /// The plus height button.
        /// </summary>
        private Button plusHeightButton;

        /// <summary>
        /// The height text view.
        /// </summary>
        private EditText heightTextEdit;

        /// <summary>
        /// The minus weight button.
        /// </summary>
        private Button minusWeightButton;

        /// <summary>
        /// The plus weight button.
        /// </summary>
        private Button plusWeightButton;

        /// <summary>
        /// The weight text view.
        /// </summary>
        private EditText weightTextEdit;

        /// <summary>
        /// The sent real height.
        /// </summary>
        public int SentHeight { get; set; }

        /// <summary>
        /// The sent pixel height for front picture.
        /// </summary>
        public int PixelHeightFront { get; set; }

        /// <summary>
        /// The sent pixel height for side picture.
        /// </summary>
        public int PixelHeightSide { get; set; }

        /// <summary>
        /// The server url.
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// The constants.
        /// </summary>
        private const int RETRY_BUTTON_ID = 3001;

        /// <summary>
        /// The front and side positions.
        /// </summary>
        public List<CircleArea> FrontSidePositions { get; set; }

        /// <summary>
        /// The OnCreate method.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        /// <summary>
        /// The OnCreateView method.
        /// </summary>
        /// <param name="inflater">The inflater.</param>
        /// <param name="container">The container.</param>
        /// <param name="savedInstanceState">The saved instance state.</param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (fragmentView == null)
            {
                fragmentView = inflater.Inflate(Resource.Layout.Generate, container, false);

                var textViewHeight = fragmentView.FindViewById<TextView>(Resource.Id.textViewHeight);
                textViewHeight.Text = Languages.Resources.Res_Android_Height;
                var textViewWeight = fragmentView.FindViewById<TextView>(Resource.Id.textViewWeight);
                textViewWeight.Text = Languages.Resources.Res_Android_Weight;

                minusHeightButton = fragmentView.FindViewById<Button>(Resource.Id.height_btn_minus);
                plusHeightButton = fragmentView.FindViewById<Button>(Resource.Id.height_btn_plus);
                heightTextEdit = fragmentView.FindViewById<EditText>(Resource.Id.heightText);
                minusHeightButton.Click += OnMinusHeightButton_Click;
                plusHeightButton.Click += OnPlusHeightButton_Click;
                minusHeightButton.SetOnTouchListener(new EditTextRepeatListener(minusHeightButton, heightTextEdit, 1000, 2000, (button) => {}, button => {}, (button, isLongPress) => {}));
                plusHeightButton.SetOnTouchListener(new EditTextRepeatListener(plusHeightButton, heightTextEdit, 1000, 2000, (button) => { }, button => { }, (button, isLongPress) => { }));

                minusWeightButton = fragmentView.FindViewById<Button>(Resource.Id.weight_btn_minus);
                plusWeightButton = fragmentView.FindViewById<Button>(Resource.Id.weight_btn_plus);
                weightTextEdit = fragmentView.FindViewById<EditText>(Resource.Id.weightText);
                minusWeightButton.Click += OnMinusWeightButton_Click;
                plusWeightButton.Click += OnPlusWeightButton_Click;
                minusWeightButton.SetOnTouchListener(new EditTextRepeatListener(minusWeightButton, weightTextEdit, 1000, 2000, (button) => { }, button => { }, (button, isLongPress) => { }));
                plusWeightButton.SetOnTouchListener(new EditTextRepeatListener(plusWeightButton, weightTextEdit, 1000, 2000, (button) => { }, button => { }, (button, isLongPress) => { }));

                heightTextEdit.KeyListener = null;
                weightTextEdit.KeyListener = null;

                generateButton = fragmentView.FindViewById<Button>(Resource.Id.generateButton);
                generateButton.Text = Languages.Resources.Res_Android_GenerateButton;
                generateButton.Click += OnGenerateButton_Click;

                // Extract server url
                var configJsonStream = new System.IO.StreamReader(this.Activity.Assets.Open("config.json"));
                var configJsonStreamString = configJsonStream.ReadToEnd();
                var configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJsonStreamString);
                this.ServerUrl = configData["server_url"];
            }

            return fragmentView;
        }

        /// <summary>
        /// The minus height click.
        /// </summary>
        private void OnMinusHeightButton_Click(object sender, EventArgs e)
        {
            var currentButton = sender as Button;

            int parsedNumber;
            if (int.TryParse(this.heightTextEdit.Text, out parsedNumber))
            {
                if (parsedNumber > 70)
                {
                    heightTextEdit.Text = (parsedNumber - 1).ToString();
                }
            }
        }
        
        /// <summary>
        /// The plus height click.
        /// </summary>
        private void OnPlusHeightButton_Click(object sender, EventArgs e)
        {
            int parsedNumber;
            if (int.TryParse(this.heightTextEdit.Text, out parsedNumber))
            {
                if(parsedNumber < 250)
                {
                    heightTextEdit.Text = (parsedNumber + 1).ToString();
                }
            }
        }

        /// <summary>
        /// The minus weight click.
        /// </summary>
        private void OnMinusWeightButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(weightTextEdit.Text))
            {
                weightTextEdit.Text = "0";
            }

            int parsedNumber;
            if (int.TryParse(this.weightTextEdit.Text, out parsedNumber))
            {
                if (parsedNumber > 0)
                {
                    weightTextEdit.Text = (parsedNumber - 1).ToString();
                }
            }
        }

        /// <summary>
        /// The plus weight click.
        /// </summary>
        private void OnPlusWeightButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(weightTextEdit.Text))
            {
                weightTextEdit.Text = "0";
            }

            int parsedNumber;
            if (int.TryParse(this.weightTextEdit.Text, out parsedNumber))
            {
                if (parsedNumber < 250)
                {
                    weightTextEdit.Text = (parsedNumber + 1).ToString();
                }
            }
        }

        /// <summary>
        /// The generate button event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnGenerateButton_Click(object sender, EventArgs e)
        {
            // Delete content
            var linearLayout = fragmentView.FindViewById<LinearLayout>(Resource.Id.layoutGenerateCenter);
            linearLayout.RemoveAllViewsInLayout();

            // Disable the swipes
            var viewPager = this.Activity.FindViewById<BodyShapeViewPager>(Resource.Id.bodyshapeViewPager);
            viewPager.SetSwipeEnabled(false);

            // Rubik's Cuke.
            WebView webView = new WebView(this.Context);
            webView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            webView.Visibility = ViewStates.Visible;
            webView.LoadUrl(string.Format("file:///android_asset/RubiksCube.html"));
            webView.SetBackgroundColor(Color.ParseColor("#000000"));
            webView.SetLayerType(LayerType.Software, null);

            // Progress bar
            var customDrawable = ContextCompat.GetDrawable(this.Context, Resource.Drawable.custom_progressbar);
            Func<float, string> percentFormat = (x) => string.Format("{0}%", x);
            RadialProgressView progressBar = new RadialProgressView(this.Context, 0, 100, RadialProgressViewStyle.Big, Color.Green);
            progressBar.LabelTextDelegate = percentFormat;
            progressBar.LayoutParameters = new ViewGroup.LayoutParams(500, 500);
            linearLayout.AddView(webView);
            linearLayout.AddView(progressBar);

            // Launch the background task
            ThreadPool.QueueUserWorkItem(p =>
            {
                try
                {
                    // Send generation to API here
                    int enteredWeight;
                    int enteredHeight;
                    bool compareRealWeight = int.TryParse(weightTextEdit.Text, out enteredWeight);
                    bool compareRealHeight = int.TryParse(heightTextEdit.Text, out enteredHeight);
                    this.SentHeight = enteredHeight;
                    if (compareRealWeight)
                    {
                        if (enteredWeight < 25)
                        {
                            enteredWeight = 0;
                            compareRealWeight = false;
                        }
                    }
                    else
                    {
                        enteredWeight = 0;
                    }

                    // Check if pictures were loaded
                    ISharedPreferences prefs = Application.Context.GetSharedPreferences("bodyshape", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    var isPicture1Loaded = prefs.GetBoolean("picture1", false);
                    var isPicture2Loaded = prefs.GetBoolean("picture2", false);
                    if (isPicture1Loaded == false || isPicture2Loaded == false)
                    {
                        var message = new AlertDialog.Builder(this.Activity);
                        message.SetMessage("At least one picture was not loaded. Try again.");
                        message.Show();
                        return;
                    }

                    // Get picture name
                    this.FrontSidePositions = JsonConvert.DeserializeObject<List<CircleArea>>(prefs.GetString("frontpositions", string.Empty));
                    this.FrontSidePositions.AddRange(JsonConvert.DeserializeObject<List<CircleArea>>(prefs.GetString("sidepositions", string.Empty)));
                    var picturesName = prefs.GetString("filename", Guid.NewGuid().ToString());

                    // Get pictures dimensions and positions
                    var frontName = "frontpicture";
                    var pictFrontLeft = prefs.GetInt(frontName + "_left", 0);
                    var pictFrontTop = prefs.GetInt(frontName + "_top", 0);
                    var pictFrontWidth = prefs.GetInt(frontName + "_width", 0);
                    var pictFrontHeight = prefs.GetInt(frontName + "_height", 0);
                    var sideName = "sidepicture";
                    var pictSideLeft = prefs.GetInt(sideName + "_left", 0);
                    var pictSideTop = prefs.GetInt(sideName + "_top", 0);
                    var pictSideWidth = prefs.GetInt(sideName + "_width", 0);
                    var pictSideHeight = prefs.GetInt(sideName + "_height", 0);

                    // Calculate the pixel height
                    this.PixelHeightFront = (int)Math.Abs(this.FrontSidePositions.Where(t => t.Id == "pied1_u3_1").FirstOrDefault().PositionY - this.FrontSidePositions.Where(r => r.Id == "head_u1_1").FirstOrDefault().PositionY);
                    this.PixelHeightSide = (int)Math.Abs(this.FrontSidePositions.Where(t => t.Id == "pied_v3_1").FirstOrDefault().PositionY - this.FrontSidePositions.Where(r => r.Id == "head_v1_1").FirstOrDefault().PositionY);

                    // Delete some data
                    editor.Remove("picture1");
                    editor.Remove("picture2");
                    editor.Remove("filename");
                    editor.Remove("frontpositions");
                    editor.Remove("sidepositions");
                    editor.Remove(frontName + "_left");
                    editor.Remove(frontName + "_top");
                    editor.Remove(frontName + "_width");
                    editor.Remove(frontName + "_height");
                    editor.Remove(sideName + "_left");
                    editor.Remove(sideName + "_top");
                    editor.Remove(sideName + "_width");
                    editor.Remove(sideName + "_height");
                    editor.Apply();

                    // Send the two pictures to server
                    var picturesUriFront = new Uri(this.ServerUrl + $"/Home/UploadFromMobile?rootFileName={ picturesName }Picture_1.png");
                    var picturesUriSide = new Uri(this.ServerUrl + $"/Home/UploadFromMobile?rootFileName={ picturesName }Picture_2.png");
                    var webClient = new BodyShapeWebClient();
                    webClient.Timeout = 30000;
                    webClient.Headers.Add("Content-Type", "binary/octet-streOpenWriteam");

                    var resultSendFileFront = webClient.UploadFile(picturesUriFront, "POST", App1._path == null ? App1._file.AbsolutePath : App1._path);
                    this.Activity.RunOnUiThread(() =>
                    {
                        progressBar.Value = 20;
                    });
                    var resultSendFileSide = webClient.UploadFile(picturesUriSide, "POST", App2._path == null ? App2._file.AbsolutePath : App2._path);
                    this.Activity.RunOnUiThread(() =>
                    {
                        progressBar.Value = 40;
                    });

                    var resultSendFileFrontString = Encoding.UTF8.GetString(resultSendFileFront, 0, resultSendFileFront.Length);
                    var resultSendFileSideString = Encoding.UTF8.GetString(resultSendFileSide, 0, resultSendFileSide.Length);

                    if (!resultSendFileFrontString.Contains(picturesName) || !resultSendFileSideString.Contains(picturesName))
                    {
                        var errorMessage = new AlertDialog.Builder(this.Activity);
                        errorMessage.SetMessage("Sorry, but your pictures may have not been sent due to connection lags.");
                        errorMessage.Show();
                    }

                    // Build the json
                    var jsonObject = this.GenerateDataCoordinates(enteredHeight, enteredWeight, pictFrontLeft, pictFrontTop, pictFrontWidth, pictFrontHeight, pictSideLeft, pictSideTop, pictSideWidth, pictSideHeight, picturesName);
                    var jsonToSend = JsonConvert.SerializeObject(jsonObject);

                    // Send the request
                    var bodyShapeClient = new HttpClient();
                    bodyShapeClient.Timeout = TimeSpan.FromMilliseconds(30000);
                    var uri = new Uri(this.ServerUrl + "/Home/Calculate");
                    var content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");
                    var postResult = bodyShapeClient.PostAsync(uri, content).Result;
                    this.Activity.RunOnUiThread(() =>
                    {
                        progressBar.Value = 80;
                    });

                    // Get the response
                    if (postResult.IsSuccessStatusCode)
                    {
                        var bodyResultString = postResult.Content.ReadAsStringAsync().Result;
                        var bodyResultObject = JsonConvert.DeserializeObject(bodyResultString);
                        var bodyResultObjectSecond = JsonConvert.DeserializeObject(bodyResultObject.ToString());
                        var bodyResult = JObject.FromObject(bodyResultObjectSecond);

                        var resultDictionnary = new Dictionary<string, float>();
                        resultDictionnary.Add("headMass", bodyResult["BodyMass"]["Head"]["Mass"].Value<float>());
                        resultDictionnary.Add("neckMass", bodyResult["BodyMass"]["Neck"]["Mass"].Value<float>());
                        resultDictionnary.Add("thoraxMass", bodyResult["BodyMass"]["Thorax"]["Mass"].Value<float>());
                        resultDictionnary.Add("abdoMass", bodyResult["BodyMass"]["Abdomen"]["Mass"].Value<float>());
                        resultDictionnary.Add("fesseMass", bodyResult["BodyMass"]["Bottom"]["Mass"].Value<float>());

                        resultDictionnary.Add("cuissegaucheMass", bodyResult["BodyMass"]["ThighLeft"]["Mass"].Value<float>());
                        resultDictionnary.Add("cuissedroiteMass", bodyResult["BodyMass"]["ThighRight"]["Mass"].Value<float>());
                        resultDictionnary.Add("jambegaucheMass", bodyResult["BodyMass"]["LegLeft"]["Mass"].Value<float>());
                        resultDictionnary.Add("jambedroiteMass", bodyResult["BodyMass"]["LegRight"]["Mass"].Value<float>());
                        //resultDictionnary.Add("chevillegaucheMass", bodyResult["BodyMass"]["AnkleLeft"]["Mass"].Value<float>());
                        //resultDictionnary.Add("chevilledroiteMass", bodyResult["BodyMass"]["AnkleRight"]["Mass"].Value<float>());
                        resultDictionnary.Add("piedgaucheMass", bodyResult["BodyMass"]["FootLeft"]["Mass"].Value<float>());
                        resultDictionnary.Add("pieddroitMass", bodyResult["BodyMass"]["FootRight"]["Mass"].Value<float>());

                        resultDictionnary.Add("brasgaucheMass", bodyResult["BodyMass"]["ArmLeft"]["Mass"].Value<float>());
                        resultDictionnary.Add("brasdroitMass", bodyResult["BodyMass"]["ArmRight"]["Mass"].Value<float>());
                        resultDictionnary.Add("avantbrasgaucheMass", bodyResult["BodyMass"]["ForeArmLeft"]["Mass"].Value<float>());
                        resultDictionnary.Add("avantbrasdroitMass", bodyResult["BodyMass"]["ForeArmRight"]["Mass"].Value<float>());
                        resultDictionnary.Add("maingaucheMass", bodyResult["BodyMass"]["HandLeft"]["Mass"].Value<float>());
                        resultDictionnary.Add("maindroiteMass", bodyResult["BodyMass"]["HandRight"]["Mass"].Value<float>());

                        resultDictionnary.Add("poidstotal", bodyResult["BodyMass"]["TotalMass"].Value<float>());

                        double error = 0;
                        if (enteredWeight != 0)
                        {
                            error = bodyResult["BodyMass"]["TotalMass"].Value<double>() - enteredWeight;
                            error = error / enteredWeight * 100;
                            error = Math.Round(error, 2);
                        }

                        this.Activity.RunOnUiThread(() =>
                        {
                            progressBar.Value = 100;

                            // Remove rubis cube
                            linearLayout.RemoveAllViewsInLayout();

                            // Enable the swipes
                            viewPager.SetSwipeEnabled(true);
                        });

                        // Send the response data to results fragment (shared preferences)
                        ISharedPreferences resultsPrefs = Application.Context.GetSharedPreferences("bodyshaperesults", FileCreationMode.Private);
                        ISharedPreferencesEditor resultsEditor = resultsPrefs.Edit();
                        this.CacheResults(resultsPrefs, resultsEditor, resultDictionnary, enteredWeight != 0, error);

                        // Swipe to result fragment
                        this.Activity.RunOnUiThread(() =>
                        {
                            // Update this fragment
                            var buttonWidthHeight = Resources.DisplayMetrics.HeightPixels / 6;
                            var retryButton = new Button(this.Context);
                            var retryButtonParams = new LinearLayout.LayoutParams(buttonWidthHeight, ViewGroup.LayoutParams.MatchParent, 0.7f);
                            retryButton.LayoutParameters = retryButtonParams;
                            retryButton.SetBackgroundResource(Resource.Drawable.blue_button);
                            retryButton.Id = RETRY_BUTTON_ID;
                            retryButton.Text = "Retry";
                            retryButton.SetTextAppearance(Resource.Style.button_text);
                            retryButton.Visibility = ViewStates.Visible;
                            retryButton.Click += OnRetryButton_Click;
                            linearLayout.AddView(retryButton);

                            // Disable the picture fragments
                            ((MainActivity)this.Activity).FirstPictureFragment.DisableView();
                            ((MainActivity)this.Activity).SecondPictureFragment.DisableView();

                            // Results fragment
                            ((MainActivity)this.Activity).ResultsFragment.ShowResults((string) bodyResult["Ticket"]);
                        });
                    }
                    else
                    {
                        var errorMessage = new AlertDialog.Builder(this.Activity);
                        errorMessage.SetMessage("An error occured during calculations... Try again later.");
                        errorMessage.Show();

                        // Reload
                        var reloadTransaction = this.FragmentManager.BeginTransaction();
                        reloadTransaction.Detach(this)
                                         .Attach(new GenerationFragment())
                                         .Commit();

                        return;
                    }
                }
                catch (Exception)
                {
                    // Back to main thread
                    this.Activity.RunOnUiThread(() =>
                    {
                        var message = new AlertDialog.Builder(this.Activity);
                        message.SetMessage("An error occured during taking your pictures. Try again later.");
                        message.SetCancelable(false);
                        message.SetPositiveButton("OK", (okSender, okE) =>
                        {
                            this.ReloadBodyShape();
                        });
                        message.Show();   
                    });
                }
            });
        }

        /// <summary>
        /// The retry button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRetryButton_Click(object sender, EventArgs e)
        {
            this.ReloadBodyShape();
        }

        /// <summary>
        /// The cache results method.
        /// </summary>
        private void CacheResults(ISharedPreferences prefs, ISharedPreferencesEditor editor, Dictionary<string, float> values, bool hasWeight, double errorInfo)
        {
            foreach(var value in values)
            {
                var result = prefs.GetFloat(value.Key, 0);
                if (result != 0)
                {
                    editor.Remove(value.Key);
                }
                editor.PutFloat(value.Key, value.Value);
            }
            // Cache error percentage
            var stringKeyError = "generationError";
            if (hasWeight)
            {
                var errorResult = prefs.GetFloat(stringKeyError, 0);
                if (errorResult != 0)
                {
                    editor.Remove(stringKeyError);
                }
                editor.PutFloat(stringKeyError, (float) errorInfo);
            }
            editor.Apply();
        }

        /// <summary>
        /// The pixel to real left method.
        /// </summary>
        private double PixelToRealLeft(string pixel)
        {
            int realHeight;
            if(pixel.Contains("_u"))
            {
                realHeight = this.PixelHeightFront;
            }
            else
            {
                realHeight = this.PixelHeightSide;
            }
            var foundPoint = this.FrontSidePositions.Where(u => u.Id == pixel).FirstOrDefault();
            var result = Math.Round(foundPoint.PositionX) * this.SentHeight / realHeight;
            return result;
        }
        
        /// <summary>
        /// The pixel to real top method.
        /// </summary>
        private double PixelToRealTop(string pixel, string pixel2)
        {
            int realHeight;
            if (pixel.Contains("_u"))
            {
                realHeight = this.PixelHeightFront;
            }
            else
            {
                realHeight = this.PixelHeightSide;
            }
            var foundFirstPoint = this.FrontSidePositions.Where(u => u.Id == pixel).FirstOrDefault();
            var foundSecondPoint = this.FrontSidePositions.Where(u => u.Id == pixel2).FirstOrDefault();
            var result = (Math.Round(foundFirstPoint.PositionY) + Math.Round(foundSecondPoint.PositionY)) * this.SentHeight / realHeight / 2;
            return result;
        }

        /// <summary>
        /// The generate data coordinates function.
        /// </summary>
        private object GenerateDataCoordinates(int height, int weight, int pictureFrontLeft, int pictureFrontTop, int pictureFrontWidth, int pictureFrontHeight, int pictureSideLeft, int pictureSideTop, int pictureSideWidth, int pictureSideHeight, string pictureName)
        {
            object myJsonData = new
            {
                Height = height,
                Weight = weight,
                Picture_1 = pictureName + "Picture_1.png",
                PictureWidth_1 = pictureFrontWidth,
                PictureHeight_1 = pictureFrontHeight,
                PictureLeft_1 = pictureFrontLeft,
                PictureTop_1 = pictureFrontTop,
                Picture_2 = pictureName + "Picture_2.png",
                PictureWidth_2 = pictureSideWidth,
                PictureHeight_2 = pictureSideHeight,
                PictureLeft_2 = pictureSideLeft,
                PictureTop_2 = pictureSideTop,
                Abdomen = new
                {
                    U1 = Math.Abs(this.PixelToRealLeft("abdo_u1_2") - this.PixelToRealLeft("abdo_u1_1")),
                    U2 = Math.Abs(this.PixelToRealLeft("abdo_u2_2") - this.PixelToRealLeft("abdo_u2_1")),
                    U3 = Math.Abs(this.PixelToRealLeft("abdo_u3_2") - this.PixelToRealLeft("abdo_u3_1")),
                    V1 = Math.Abs(this.PixelToRealLeft("abdo_v1_2") - this.PixelToRealLeft("abdo_v1_1")),
                    V2 = Math.Abs(this.PixelToRealLeft("abdo_v2_2") - this.PixelToRealLeft("abdo_v2_1")),
                    V3 = Math.Abs(this.PixelToRealLeft("abdo_v3_2") - this.PixelToRealLeft("abdo_v3_1")),
                    Z1 = 0,
                    Z2 = Math.Abs(this.PixelToRealTop("abdo_u3_1", "abdo_u3_2") - this.PixelToRealTop("abdo_u1_1", "abdo_u1_2")) / 2,
                    Z3 = Math.Abs(this.PixelToRealTop("abdo_u3_1", "abdo_u3_2") - this.PixelToRealTop("abdo_u1_1", "abdo_u1_2"))
                },
                Thorax = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("neck_u2_2") - this.PixelToRealLeft("neck_u2_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("thorax_u2_2") - this.PixelToRealLeft("thorax_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("thorax_u3_2") - this.PixelToRealLeft("thorax_u3_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("thorax_v1_2") - this.PixelToRealLeft("thorax_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("thorax_v2_2") - this.PixelToRealLeft("thorax_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("thorax_v3_2") - this.PixelToRealLeft("thorax_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("thorax_u3_1", "thorax_u3_2") - this.PixelToRealTop("neck_u2_1", "neck_u2_2")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("thorax_u3_1", "thorax_u3_2") - this.PixelToRealTop("neck_u2_1", "neck_u2_2"))
                    },
                    Neck = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("head_u3_2") - this.PixelToRealLeft("head_u3_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("neck_u1_2") - this.PixelToRealLeft("neck_u1_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("neck_u2_2") - this.PixelToRealLeft("neck_u2_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("neck_v1_2") - this.PixelToRealLeft("neck_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("neck_v2_2") - this.PixelToRealLeft("neck_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("neck_v3_2") - this.PixelToRealLeft("neck_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("neck_u2_1", "neck_u2_2") - this.PixelToRealTop("head_u3_1", "head_u3_2")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("neck_u2_1", "neck_u2_2") - this.PixelToRealTop("head_u3_1", "head_u3_2"))
                    },
                    Head = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("head_u1_2") - this.PixelToRealLeft("head_u1_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("head_u2_2") - this.PixelToRealLeft("head_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("head_u3_2") - this.PixelToRealLeft("head_u3_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("head_v1_2") - this.PixelToRealLeft("head_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("head_v2_2") - this.PixelToRealLeft("head_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("head_v3_2") - this.PixelToRealLeft("head_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("head_u3_1", "head_u3_2") - this.PixelToRealTop("head_u1_1", "head_u1_2")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("head_u3_1", "head_u3_2") - this.PixelToRealTop("head_u1_1", "head_u1_2"))
                    },
                    Bottom = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("fesse_u1_2") - this.PixelToRealLeft("fesse_u1_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("fesse_u2_2") - this.PixelToRealLeft("fesse_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("fesse_u3_2") - this.PixelToRealLeft("fesse_u3_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("fesse_v1_2") - this.PixelToRealLeft("fesse_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("fesse_v2_2") - this.PixelToRealLeft("fesse_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("fesse_v3_2") - this.PixelToRealLeft("fesse_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("fesse_u3_1", "fesse_u3_2") - this.PixelToRealTop("fesse_u1_1", "fesse_u1_2")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("fesse_u3_1", "fesse_u3_2") - this.PixelToRealTop("fesse_u1_1", "fesse_u1_2"))
                    },
                    Thigh = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("cuisse1_u1_1") - this.PixelToRealLeft("cuisse2_u1_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("cuisse1_u2_1") - this.PixelToRealLeft("cuisse2_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("cuisse1_u3_1") - this.PixelToRealLeft("cuisse2_u3_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("cuisse_v1_2") - this.PixelToRealLeft("cuisse_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("cuisse_v2_2") - this.PixelToRealLeft("cuisse_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("cuisse_v3_2") - this.PixelToRealLeft("cuisse_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("cuisse1_u3_1", "cuisse2_u3_1") - this.PixelToRealTop("cuisse1_u1_1", "cuisse2_u1_1")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("cuisse1_u3_1", "cuisse2_u3_1") - this.PixelToRealTop("cuisse1_u1_1", "cuisse2_u1_1"))
                    },
                    Leg = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("jambe1_u1_1") - this.PixelToRealLeft("jambe2_u1_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("jambe1_u2_1") - this.PixelToRealLeft("jambe2_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("jambe1_u3_1") - this.PixelToRealLeft("jambe2_u3_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("jambe_v1_2") - this.PixelToRealLeft("jambe_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("jambe_v2_2") - this.PixelToRealLeft("jambe_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("jambe_v3_2") - this.PixelToRealLeft("jambe_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("jambe1_u3_1", "jambe2_u3_1") - this.PixelToRealTop("jambe1_u1_1", "jambe2_u1_1")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("jambe1_u3_1", "jambe2_u3_1") - this.PixelToRealTop("jambe1_u1_1", "jambe2_u1_1"))
                    },
                    Foot = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("pied1_u1_1") - this.PixelToRealLeft("pied2_u1_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("pied1_u2_1") - this.PixelToRealLeft("pied2_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("pied1_u3_1") - this.PixelToRealLeft("pied2_u3_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("pied_v1_2") - this.PixelToRealLeft("pied_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("pied_v2_2") - this.PixelToRealLeft("pied_v2_1")) / 2,
                        V3 = Math.Abs(this.PixelToRealLeft("pied_v3_2") - this.PixelToRealLeft("pied_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("pied_v1_1", "pied_v1_2") - this.PixelToRealTop("pied_v3_1", "pied_v3_2")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("pied_v1_1", "pied_v1_2") - this.PixelToRealTop("pied_v3_1", "pied_v3_2"))
                    },
                    Hand = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("maingauche_u1_2") - this.PixelToRealLeft("maingauche_u1_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("maingauche_u2_2") - this.PixelToRealLeft("maingauche_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("maingauche_u4_2") - this.PixelToRealLeft("maingauche_u4_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("main_v1_2") - this.PixelToRealLeft("main_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("main_v2_2") - this.PixelToRealLeft("main_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("main_v3_2") - this.PixelToRealLeft("main_v3_1")),
                        Z1 = 0,
                        Z2 = Math.Abs(this.PixelToRealTop("maingauche_u4_1", "maingauche_u4_2") - this.PixelToRealTop("maingauche_u1_1", "maingauche_u1_2")) / 2,
                        Z3 = Math.Abs(this.PixelToRealTop("maingauche_u4_1", "maingauche_u4_2") - this.PixelToRealTop("maingauche_u1_1", "maingauche_u1_2"))
                    },
                    ForeArm = new
                    {
                U1 = Math.Abs(this.PixelToRealLeft("avantbrasgauche_u1_2") - this.PixelToRealLeft("avantbrasgauche_u1_1")),
                        U2 = Math.Abs(this.PixelToRealLeft("avantbrasgauche_u2_2") - this.PixelToRealLeft("avantbrasgauche_u2_1")),
                        U3 = Math.Abs(this.PixelToRealLeft("avantbrasgauche_u3_2") - this.PixelToRealLeft("avantbrasgauche_u3_1")),
                        V1 = Math.Abs(this.PixelToRealLeft("avantbras_v1_2") - this.PixelToRealLeft("avantbras_v1_1")),
                        V2 = Math.Abs(this.PixelToRealLeft("avantbras_v2_2") - this.PixelToRealLeft("avantbras_v2_1")),
                        V3 = Math.Abs(this.PixelToRealLeft("avantbras_v3_2") - this.PixelToRealLeft("avantbras_v3_1")),
                        Z1= 0,
                        Z2= Math.Abs(this.PixelToRealTop("avantbrasgauche_u3_1", "avantbrasgauche_u3_2") - this.PixelToRealTop("avantbrasgauche_u1_1", "avantbrasgauche_u1_2")) / 2,
                        Z3= Math.Abs(this.PixelToRealTop("avantbrasgauche_u3_1", "avantbrasgauche_u3_2") - this.PixelToRealTop("avantbrasgauche_u1_1", "avantbrasgauche_u1_2"))
                    },
                    Arm = new
                    {
                        U1= Math.Abs(this.PixelToRealLeft("brasgauche_u2_2") - this.PixelToRealLeft("brasgauche_u2_1")),
                        U2= Math.Abs(this.PixelToRealLeft("brasgauche_u3_2") - this.PixelToRealLeft("brasgauche_u3_1")),
                        U3= Math.Abs(this.PixelToRealLeft("brasgauche_u4_2") - this.PixelToRealLeft("brasgauche_u4_1")),
                        V1= Math.Abs(this.PixelToRealLeft("bras_v4_2") - this.PixelToRealLeft("bras_v4_1")),
                        V2= Math.Abs(this.PixelToRealLeft("bras_v2_2") - this.PixelToRealLeft("bras_v2_1")),
                        V3= Math.Abs(this.PixelToRealLeft("bras_v3_2") - this.PixelToRealLeft("bras_v3_1")),
                        Z1= 0,
                        Z2= Math.Abs(this.PixelToRealTop("brasgauche_u1_2", "brasgauche_u1_1") - this.PixelToRealTop("brasgauche_u4_2", "brasgauche_u4_1")) / 2,
                        Z3= Math.Abs(this.PixelToRealTop("brasgauche_u1_2", "brasgauche_u1_1") - this.PixelToRealTop("brasgauche_u4_2", "brasgauche_u4_1"))
                    },
                };

            return myJsonData;
        }

        /// <summary>
        /// The reload bodyshape app.
        /// </summary>
        private void ReloadBodyShape()
        {
            Intent i = this.Activity.PackageManager.GetLaunchIntentForPackage(this.Context.PackageName);
            i.AddFlags(ActivityFlags.ClearTop);
            this.StartActivity(i);
        }
    }
}