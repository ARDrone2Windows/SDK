﻿using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using Windows.Networking.Connectivity;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;

using ARDrone2Client.Common;
using ARDrone2Client.Common.Configuration;
using ARDrone2Client.Common.Input;
using ARDrone2Client.Common.Navigation;
using ARDrone2Client.Common.ViewModel;
using ARDrone2Client.WP8.Video;
using ARDrone2.WP8Sample.Resources;

namespace ARDrone2.WP8Sample
{
    public partial class FlyPage : PhoneApplicationPage
    {
        private DroneClient _droneClient;
        SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
        SpeechRecognizer _speechRecognizer = null;

        public FlyPage()
        {
            InitializeComponent();

            _droneClient = DroneClient.Instance;
            _droneClient.Messages.CollectionChanged += Messages_CollectionChanged;

            this.DataContext = _droneClient;
         }

        void mediaElem_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(mediaElem.CurrentState);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            mediaElem.SetSource(new ARDroneStreamSource("192.168.1.1"));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (await _droneClient.ConnectAsync())
            {
                TakeOffButton.IsEnabled = true;

                await Task.Run(async () =>
                {
                    try
                    {
                        if (_speechRecognizer == null)
                        {
                            _speechRecognizer = new SpeechRecognizer();
                            _speechRecognizer.Grammars.AddGrammarFromList("DroneCommands", new List<string>
                            {
                                "Take off", "Take off the drone", "Take the drone off", 
                                "Land", "Land the drone"
                            });
                        }
                        try
                        {
                            while (true)
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    if (TextRecognizer.Text != "Voice: listening...")
                                    {
                                        TextRecognizer.Foreground = new SolidColorBrush(Colors.Green);
                                        TextRecognizer.Text = "Voice: listening...";
                                    }
                                });
                                
                                var result = await _speechRecognizer.RecognizeAsync();

                                //if (result.TextConfidence == SpeechRecognitionConfidence.High)
                                if (result.Text.Length > 0)
                                {
                                    Dispatcher.BeginInvoke(() =>
                                    {
                                        switch (result.Text)
                                        {
                                            case "Take off":
                                            case "Take off the drone":
                                            case "Take the drone off":
                                                _droneClient.TakeOff();
                                                break;
                                            case "Land":
                                            case "Land the drone":
                                                _droneClient.Land();
                                                break;
                                        }
                                    });
                                    await Task.Delay(1000);
                                }
                                else
                                    await Task.Delay(500);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

                        Dispatcher.BeginInvoke(() =>
                        {
                            TextRecognizer.Foreground = new SolidColorBrush(Colors.Red);
                            TextRecognizer.Text = "Voice: unknown";
                        });

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_droneClient.IsFlying)
            {
                _droneClient.Land();
                TakeOffButton.Content = "Take off";

            }
            else
            {
                _droneClient.TakeOff();
                TakeOffButton.Content = "Land";
            }
        }

        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (e.NewItems != null)
            //    foreach (var item in e.NewItems)
            //    {
            //        try
            //        {
            //            _speechSynthesizer.CancelAll();
            //            await _speechSynthesizer.SpeakTextAsync(((Message)item).Content);
            //        }
            //        catch(Exception) 
            //        { }
            //    }
        }
    }
}