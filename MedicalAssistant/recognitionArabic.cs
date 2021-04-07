﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace MedicalAssistant
{
    class recognitionArabic
    {

        static WaveOutEvent MixAudioFiles(string speechMp3)
        {

            Mp3FileReader reader = new Mp3FileReader(speechMp3);
            WaveOutEvent waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();

            return waveOut;


        }

        static void CloudTextToSpeech(string outFileName, string text, string lang, string gender, double speed = 0.5, double pitch = 0.5, double rate = 0.5, double maxresults = 1, double xjerr = 1)
        {
            const string key = "AIzaSyBOti4mM-6x9WDnZIjIeyEU21OpBXqWBgw";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows)");
                int txtLen = text.Length;
                text = "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(text)).Replace("-", "%");
                string url = $"https://www.google.com/speech-api/v2/synthesize?ie=UTF-8enc=mpeg&client=chromium&key={key}&text={text}&xjerr={xjerr}&lang={lang}&gender={gender}&speed ={speed}&pitch={pitch}&maxresults={maxresults}";
                client.DownloadFile(url, outFileName);
            }
        }
        string[] ParseJson(string json)
        {
            List<string> list = new List<string>();
            try
            {
                string[] lines = json.Split(new[] { "\"transcript\":\"" }, StringSplitOptions.RemoveEmptyEntries);


                for (int i = 1; i < lines.Length; i++)
                    list.Add(lines[i].Substring(0, lines[i].IndexOf("\"", StringComparison.Ordinal)));
            }
            catch
            {

            }

            return list.ToArray();
        }
        public string SpeakRecognition()
        {

            WebRequest request = WebRequest.Create("https://www.google.com/speech-api/v2/recognize?output=json&lang=AR-eg&key=AIzaSyBOti4mM-6x9WDnZIjIeyEU21OpBXqWBgw");
            //
            request.Method = "POST";
            byte[] byteArray = File.ReadAllBytes("test.wav");
            request.ContentType = "audio/l16; rate=16000"; //"16000";
            request.ContentLength = byteArray.Length;
            request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string str = reader.ReadToEnd();
            string[] strs = ParseJson(str);
            string phrase = "";

            if (strs.Length > 0) phrase = strs[0].ToLower();

            Console.WriteLine(phrase);
            return phrase;
        }
        public WaveOutEvent CloudTextToSpeech(string text, string gender)
        {
            string lang = "ar";
            double pitch = 0.6;
            double rate = 0.5;
            double speed = 0.5;
            double maxresults = 1;
            double xjerr = 5;
            //string text = textBox1.Text;
            Console.WriteLine($"\tprocessing phrase: \"{text}\" ");
            var timestamp = (DateTime.Now.ToFileTime()).ToString();
            //Console.WriteLine(timestamp);

            CloudTextToSpeech(timestamp, text, lang, gender, speed, pitch, rate, maxresults, xjerr);
            return MixAudioFiles(timestamp);

        }


    }
}
