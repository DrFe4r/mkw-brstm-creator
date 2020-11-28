﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace brstm_maker
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Select a track:");
            
            foreach(KeyValuePair<string, string> kvp in Tracks.trackNames)
            {
                Console.WriteLine(kvp.Key);
            }

            Console.WriteLine("--------");
            string userSelection = Console.ReadLine();
            string trackFilename = "";
            try 
            {
                trackFilename = Tracks.getFilename(userSelection);
            }
            catch(System.ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            Console.WriteLine("Enter a YouTube URL or a .wav filename in the current directory: ");
            string input = Console.ReadLine();
            string path = "";
            if(input.Contains("http"))
            {
                string id = input.Split('=')[1];
                path = await YoutubeHandler.downloadAudio(id, trackFilename);
                path = await AudioHandler.convertToWav(path);
            }
            else if(File.Exists($".\\{input}"))
            {
                string current = Directory.GetCurrentDirectory();
                if(!Directory.Exists(current + "\\brstms"))
                {
                    Directory.CreateDirectory(current + "\\brstms");
                }
                path = current + "\\" + input;
                File.Move(path, current + "\\brstms\\" + trackFilename + "_temp.wav");
                path = current + "\\brstms\\" + trackFilename + "_temp.wav";
            }
            else
            {
                Console.WriteLine("Something went wrong. That file either doesn't exist, or you just inputted garbage.");
                Environment.Exit(1);
            }

            Console.WriteLine("How much do you want to increase the volume? (Enter a value from 0-10 (dB))");
            int decibelIncrease = Int32.Parse(Console.ReadLine());
            path = AudioHandler.adjustVolume(path, decibelIncrease);
            path = AudioHandler.adjustChannels(path, Tracks.getChannelCount(userSelection));
            Console.WriteLine("Speed factor for final lap? (Enter a value from 1.05-1.30)");
            double speedFactor = Double.Parse(Console.ReadLine());            
            string finalpath = AudioHandler.finalLapMaker(path, speedFactor);
            AudioHandler.convertToBrstm(path);
            AudioHandler.convertToBrstm(finalpath);
            
        }
    }
}
