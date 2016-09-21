using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;
namespace System_Monitor
{
    class Program
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        static void Main(string[] args)
        {
            // List of messsages that will be selected at random 
            List<string> cpuMaxedOutMessages = new List<string>();
            cpuMaxedOutMessages.Add("Warning: CPU is max percentage");
            cpuMaxedOutMessages.Add("Warning: you are running your CPU too hard");
            cpuMaxedOutMessages.Add("Warning: stop what your doing the CPU does not like it");
            cpuMaxedOutMessages.Add("Warning: the CPU hates you");
            cpuMaxedOutMessages.Add("Warning: The CPU is about to die");

            Random rand = new Random();

            
            synth.Speak("Welcome to the System Monitor!");

            #region Performance Counters
            // Tells us current CPU usage in %
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();

            // Tells us current available memory in MB
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            // Tells us system uptime in seconds
            PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            #endregion

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
            string systemUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds",
                (int)uptimeSpan.TotalDays,
                (int)uptimeSpan.Hours,
                (int)uptimeSpan.Minutes,
                (int)uptimeSpan.Seconds
                );
            // Tell user what the current system uptime is
            NishuSpeak(systemUptimeMessage, VoiceGender.Male, 2);

            int speechSpeed = 1;
            bool isChromeOpenedAlready = false;
            
            while (true)
            {
                
                // Get the current performance counter values 
                int currentCpuPercentage = (int)perfCpuCount.NextValue();
                int currentAvailableMemory = (int)perfMemCount.NextValue();
                // Every 1 second print CPU load in % to the screen

                Console.WriteLine("CPU Load        : {0}%", currentCpuPercentage);
                Console.WriteLine("Available Memory: {0}MB", currentAvailableMemory);

                

                // Only tell us when CPU is above 80%
                #region Logic
                if (currentCpuPercentage > 80)
                {
                    if (currentCpuPercentage == 100)
                    {
                        // Designed to pervent speech from exceeding 5x normal
                        if (speechSpeed < 5)
                        {
                            speechSpeed++;
                        }
                        string cpuLoadVocalMessage = cpuMaxedOutMessages[rand.Next(5)];
                        

                        if (isChromeOpenedAlready == false)
                        {
                            OpenWebsite("http://www.youtube.com");
                            isChromeOpenedAlready = true;
                        }
                        NishuSpeak(cpuLoadVocalMessage, VoiceGender.Male, speechSpeed);
                    }
                    else
                    {
                        
                        string cpuLoadVocalMessage = String.Format("The current CPU load is {0} percent,", currentCpuPercentage);
                        NishuSpeak(cpuLoadVocalMessage, VoiceGender.Female, 5);
                    }
                }
                #endregion

                // Only tell us when memory is below one gigabyte
                if (currentAvailableMemory < 1024)
                {
                   
                    string memAvailableVocalMessage = String.Format("You currently have {0} megabytes of memory available", currentAvailableMemory);
                    NishuSpeak(memAvailableVocalMessage, VoiceGender.Male, 10);
                }

                // Speak to user with text to speech to tell them what current values are


                Thread.Sleep(1000);
            } // end of loop
        }
        /// <summary>
        /// Speaks with a selected voice
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        public static void NishuSpeak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
        }
        /// <summary>
        /// Speaks with a selected voice at a selected speed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        /// <param name="rate"></param>
        public static void NishuSpeak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            NishuSpeak(message, voiceGender);
        }
       
        // Open a website
        public static void OpenWebsite(string URL)
        {
            Process p1 = new Process();
            p1.StartInfo.FileName = "chrome.exe";
            p1.StartInfo.Arguments = URL;
            p1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p1.Start();
        }
    }
}