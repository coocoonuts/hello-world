using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Speech.Synthesis;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;

namespace CatFacts
{
    public partial class SpeakRandomFact : ServiceBase
    {
        #region Fields
        Random _rand = new Random();

        float _currentVol = AudioControls.Volume;
        bool _currentMute = AudioControls.Mute;

        static SpeechSynthesizer _ss = new SpeechSynthesizer();
        ReadOnlyCollection<InstalledVoice> _voices = _ss.GetInstalledVoices();

        int _speakVolume = Int16.Parse(ConfigurationManager.AppSettings["SpeakVolume"]);
        int _minWaitSeconds = Int16.Parse(ConfigurationManager.AppSettings["MinWaitSeconds"]);
        int _maxWaitSeconds = Int16.Parse(ConfigurationManager.AppSettings["MaxWaitSeconds"]);
        string _greeting = ConfigurationManager.AppSettings["Greeting"];
        #endregion

        /// <summary>
        /// Initialize the service
        /// </summary>
        public SpeakRandomFact()
        {
            InitializeComponent();
        }

        #region Event Handlers
        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            //Ensure our wait range is valid
            if (_minWaitSeconds > _maxWaitSeconds)
            {
                int temp = _maxWaitSeconds;
                _maxWaitSeconds = _minWaitSeconds;
                _minWaitSeconds = temp;
            }

            //Greeting
            Talk(_greeting, _speakVolume, false, 0);

            //Continually educate them, by force
            while (true)
            {
                //Wait random amount of time between facts
                CatNap();

                //EDU-CAT THEM! - the e is missing on purrrrpose, because this is about cats afterall
                Talk(GetFact(), _speakVolume, false, 0);
            }
        }

        protected override void OnStop()
        {
            //Get the now current volume values
            _currentVol = AudioControls.Volume;
            _currentMute = AudioControls.Mute;

            //Make sure they can hear this
            Talk("Learned enough have we?", _speakVolume, false, 0);
        }
        #endregion

        #region Backing functions
        /// <summary>
        /// Get a random cat fact from catfact ninja
        /// </summary>
        /// <returns>A random cat fact</returns>
        private string GetFact()
        {
            //Fact to return
            string fact = string.Empty;

            //Ensure TLS1.2 is used to make the connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //Make the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://catfact.ninja/fact");
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                fact = reader.ReadToEnd().Split('\"')[3]; //This is just for formatting of the response being cast to a string
                return fact;
            }
        }

        /// <summary>
        /// Wait a random time between _minWaitSeconds and _maxWaitSeconds to duke spook'em
        /// </summary>
        private void CatNap()
        {
            //Miliseconds to cat nap
            Thread.Sleep(_rand.Next(_minWaitSeconds, _maxWaitSeconds) * 1000);
        }

        /// <summary>
        /// Text to speech
        /// </summary>
        /// <param name="Message">What to say</param>
        /// <param name="Volume">What volume to say it at</param>
        /// <param name="Mute">To mute the volume or not. True means no sound, false means sound</param>
        /// <param name="Rate">How fast to speak. range is -10 to 10, 0 is "normal"</param>
        private void Talk(string Message, float Volume, bool Mute, int Rate)
        {
            //Get the now current volume values
            _currentVol = AudioControls.Volume;
            _currentMute = AudioControls.Mute;

            //Set the volume such that the message is heard
            AudioControls.ControlVolume(Volume, Mute);

            //Randomize the voice
            _ss.SelectVoice(_voices[_rand.Next(0, _voices.Count)].VoiceInfo.Name);
            _ss.Rate = Rate;

            //Speak
            _ss.Speak(Message);

            //Set the volume back
            AudioControls.ControlVolume(_currentVol, _currentMute);
        }
        #endregion
    }
}
