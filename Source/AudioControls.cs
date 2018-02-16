using System;
using System.Runtime.InteropServices;

namespace CatFacts
{
    class AudioControls
    {
        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IAudioEndpointVolume
        {
            // f(), g(), ... are unused COM method slots. Define these if you care
            int f(); int g(); int h(); int i();
            int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
            int j();
            int GetMasterVolumeLevelScalar(out float pfLevel);
            int k(); int l(); int m(); int n();
            int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, Guid pguidEventContext);
            int GetMute(out bool pbMute);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMMDevice
        {
            int Activate(ref Guid id, int clsCtx, int activationParams, out IAudioEndpointVolume aev);
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMMDeviceEnumerator
        {
            int f(); // Unused
            int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice endpoint);
        }

        [ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]

        class MMDeviceEnumeratorComObject { }

        static IAudioEndpointVolume Vol()
        {
            var enumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
            IMMDevice dev = null;
            Marshal.ThrowExceptionForHR(enumerator.GetDefaultAudioEndpoint(/*eRender*/ 0, /*eMultimedia*/ 1, out dev));
            IAudioEndpointVolume epv = null;
            var epvid = typeof(IAudioEndpointVolume).GUID;
            Marshal.ThrowExceptionForHR(dev.Activate(ref epvid, /*CLSCTX_ALL*/ 23, 0, out epv));
            return epv;
        }

        /// <summary>
        /// The volume of the main volume object.
        /// </summary>
        public static float Volume
        {
            get { float v = -1; Marshal.ThrowExceptionForHR(Vol().GetMasterVolumeLevelScalar(out v)); return v; }
            set
            {
                if (value > 1)
                {
                    while (value > 1)
                    {
                        value /= 10;
                    }
                }

                else if (value < 0)
                {
                    value = 0;
                }

                Marshal.ThrowExceptionForHR(Vol().SetMasterVolumeLevelScalar(value, Guid.Empty));
            }
        }

        /// <summary>
        /// Is the volume muted or not
        /// </summary>
        public static bool Mute
        {
            get { bool mute; Marshal.ThrowExceptionForHR(Vol().GetMute(out mute)); return mute; }
            set { Marshal.ThrowExceptionForHR(Vol().SetMute(value, Guid.Empty)); }
        }

        /// <summary>
        /// Set the volume and mute values
        /// </summary>
        /// <param name="Volume">Main audio component volume value</param>
        /// <param name="Mute">To mute or not, regardless of volume value</param>
        public static void ControlVolume(float Volume, bool Mute)
        {
            //Set things
            AudioControls.Volume = Volume;
            AudioControls.Mute = Mute;
        }
    }
}
