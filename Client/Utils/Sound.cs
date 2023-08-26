using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Playables;
using static ProjectM.ServerRuntimeSettings;
using UnityEngine.Rendering;

namespace BloodyShop.Client.Utils
{
    internal class Sound
    {
        private static MemoryStream ms;
        private static WaveStream ws;
        private static WaveOutEvent output;

        public static void Play(UnmanagedMemoryStream sound)
        {
            if(!Plugin.Sounds.Value) { return; }
            ms = new MemoryStream(UseStreamDotReadMethod(sound));

            ws = new WaveFileReader(ms);

            WaveOutEvent output = new WaveOutEvent();
            output.Volume = 0.2f;
            output.Init(ws);
            output.Play();
        }

        private static byte[] UseStreamDotReadMethod(Stream stream)
        {
            byte[] bytes;
            List<byte> totalStream = new();
            byte[] buffer = new byte[32];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalStream.AddRange(buffer.Take(read));
            }
            bytes = totalStream.ToArray();
            return bytes;
        }
    }
}
