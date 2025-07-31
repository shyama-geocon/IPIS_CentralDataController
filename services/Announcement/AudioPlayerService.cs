using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IpisCentralDisplayController.managers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.models.Announcement;
using NAudio.Wave;

namespace IpisCentralDisplayController.services.Announcement
{
    public class AudioPlayerService
    {
        public ActiveTrain train { get; set; }

        public string workspaceFolderPath { get; set; }

        public string audioFolderPath { get; set; }

        public AnnFormat selectedFormat { get; set; }

        public List<AudioElement> audioPacket { get; set; }


        #region Punjabi
        public void AddPunjabiETA()
        {

            if (selectedFormat == AnnFormat._24_hr)
            {

                //Expected Arrival Time HOUR
                string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours}.mp3");
                audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));


                if (train.ETA_Minutes != 00)
                {
                    //" बजकर"
                    string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                    //Expected Arrival Time MINUTE
                    string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                    //"minute"
                    string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                }
                else
                {
                    //" बजे"
                    string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                }

            }

            else if (selectedFormat == AnnFormat._12_hr)
            {
                // raat 12
                if (train.ETA_Hours == 0)
                {
                    //"raat"
                    string ਰਾਤ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਰਾਤ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਰਾਤ));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\12.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }


                }

                //"raat"
                else if (train.ETA_Hours >= 20 || train.ETA_Hours <= 4)
                {
                    //"raat"
                    string ਰਾਤ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਰਾਤ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਰਾਤ));

                    if (train.ETA_Hours >= 20)
                    {
                        //Expected Arrival Time HOUR
                        string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours - 12}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    }
                    else if (train.ETA_Hours <= 4)
                    {
                        //Expected Arrival Time HOUR
                        string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    }

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }


                }

                //"subah"
                else if (train.ETA_Hours >= 5 && train.ETA_Hours <= 11)
                {
                    //"subah"
                    string ਸਵੇਰੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਸਵੇਰੇ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਸਵੇਰੇ));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //dopahar 12 
                else if (train.ETA_Hours == 12)
                {
                    //"dopahar"
                    string ਦੁਪਹਿਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਦੁਪਹਿਰ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਦੁਪਹਿਰ));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //"dopahar"
                else if (train.ETA_Hours >= 13 && train.ETA_Hours <= 16)
                {
                    //"dopahar"
                    string ਦੁਪਹਿਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਦੁਪਹਿਰ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਦੁਪਹਿਰ));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //"shaam"
                else if (train.ETA_Hours >= 15 && train.ETA_Hours <= 19)
                {
                    //"shaam"
                    string ਸ਼ਾਮ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਸ਼ਾਮ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਸ਼ਾਮ));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //raat
                else
                {
                    //"raat"
                    string ਰਾਤ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਰਾਤ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਰਾਤ));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

            }
        }

        public void AddPunjabiETD()
        {

            if (selectedFormat == AnnFormat._24_hr)
            {

                //Expected Departure Time HOUR
                string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours}.mp3");
                audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));


                if (train.ETA_Minutes != 00)
                {
                    //" बजकर"
                    string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                    //Expected Departure Time MINUTE
                    string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                    //"minute"
                    string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                }
                else
                {
                    //" बजे"
                    string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                }

            }

            else if (selectedFormat == AnnFormat._12_hr)
            {
                // raat 12
                if (train.ETD_Hours == 0)
                {
                    //"raat"
                    string ਰਾਤ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਰਾਤ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਰਾਤ));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\12.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }


                }

                //"raat"
                else if (train.ETD_Hours >= 16 || train.ETD_Hours <= 4)
                {
                    //"raat"
                    string ਰਾਤ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਰਾਤ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਰਾਤ));

                    if (train.ETD_Hours >= 16)
                    {
                        //Expected Departure Time HOUR
                        string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours - 12}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    }
                    else if (train.ETD_Hours <= 4)
                    {
                        //Expected Departure Time HOUR
                        string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    }

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }


                }

                //"subah"
                else if (train.ETD_Hours >= 5 && train.ETD_Hours <= 11)
                {
                    //"subah"
                    string ਸਵੇਰੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਸਵੇਰੇ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਸਵੇਰੇ));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //dopahar 12 
                else if (train.ETD_Hours == 12)
                {
                    //"dopahar"
                    string ਦੁਪਹਿਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਦੁਪਹਿਰ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਦੁਪਹਿਰ));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //"dopahar"
                else if (train.ETD_Hours >= 13 && train.ETD_Hours <= 16)
                {
                    //"dopahar"
                    string ਦੁਪਹਿਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਦੁਪਹਿਰ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਦੁਪਹਿਰ));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //"shaam"
                else if (train.ETD_Hours >= 15 && train.ETD_Hours <= 19)
                {
                    //"shaam"
                    string ਸ਼ਾਮ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਸ਼ਾਮ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਸ਼ਾਮ));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

                //raat
                else
                {
                    //"raat"
                    string ਰਾਤ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਰਾਤ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਰਾਤ));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string ਬਜਕਰ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਬਜਕਰ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਬਜਕਰ));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਮਿੰਟ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));
                    }
                    else
                    {
                        //" बजे"
                        string ਵਜੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਵਜੇ.mp3");
                        audioPacket.Add(AudioElement.FromFile(ਵਜੇ));
                    }

                }

            }
        }

        public void AddPunjabiAnnouncement()
        {
            #region Punjabi

            //Chime
            string Chime = string.Concat(audioFolderPath, "\\Chimes\\ann-chime.mp3");
            audioPacket.Add(AudioElement.FromFile(Chime));

            //"Attention Passengers"
            string AP = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\AP.mp3");
            audioPacket.Add(AudioElement.FromFile(AP));

            //"Train Number"
            string TN = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\TN.mp3");
            audioPacket.Add(AudioElement.FromFile(TN));

            //Train Number
            #region 
            int digit1 = 0;
            int digit2 = 0;
            int digit3 = 0;
            int digit4 = 0;
            int digit5 = 0;

            if (int.TryParse(train.TrainNumber, out int number))
            {
                digit1 = number / 10000 % 10; // 1
                digit2 = number / 1000 % 10;  // 2
                digit3 = number / 100 % 10;   // 3
                digit4 = number / 10 % 10;    // 4
                digit5 = number % 10;         // 5

            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
            int[] digits = new int[5];

            digits[0] = digit1;
            digits[1] = digit2;
            digits[2] = digit3;
            digits[3] = digit4;
            digits[4] = digit5;

            for (int i = 0; i < 5; i++)
            {
                string numberdigit = string.Concat(audioFolderPath, $"\\numbers\\female\\{digits[i]}-pa-IN.mp3");//actually female

                audioPacket.Add(AudioElement.FromFile(numberdigit));

            }

            #endregion

            //Source Station
            string SourceStation = string.Concat(audioFolderPath, $"\\Stations\\{train.SrcCode}\\Punjabi\\Female\\{train.SrcCode}.mp3"); //It is actually female
            audioPacket.Add(AudioElement.FromFile(SourceStation));

            //"tou"
            string FROM = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\FROM.mp3");
            audioPacket.Add(AudioElement.FromFile(FROM));

            //Destination Station
            string DestinationStation = string.Concat(audioFolderPath, $"\\Stations\\{train.DestCode}\\Punjabi\\Female\\{train.DestCode}.mp3"); //It is actually female
            audioPacket.Add(AudioElement.FromFile(DestinationStation));

            //"wal jaane waali"
            string TO = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\TO.mp3");
            audioPacket.Add(AudioElement.FromFile(TO));

            //Train Name
            if (int.Parse(train.TrainNumber) <= 9999)
            {
                string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\Punjabi\\0{train.TrainNumber}.mp3");
                audioPacket.Add(AudioElement.FromFile(TrainName));

            }
            else
            {
                string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\Punjabi\\{train.TrainNumber}.mp3");
                audioPacket.Add(AudioElement.FromFile(TrainName));

            }
            //Train name is accessed by th e train number and it must always be a 5 digit number and also check if the file actually exists or not in all the fields

            //Running right time
            if (train.StatusByte == 0x01)
            {
                //"samay te chale rahi hai"
                string ONTIME = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ONTIME.mp3");
                audioPacket.Add(AudioElement.FromFile(ONTIME));

                //"anumanit pahauchn da samay aa"
                string ETA = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ETA.mp3");
                audioPacket.Add(AudioElement.FromFile(ETA));

                AddPunjabiETA();
            }

            //Will arrive shortly
            else if (train.StatusByte == 0x02)
            {
                //"jaldi hi platform number"
                string SOON = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\SOON.mp3");
                audioPacket.Add(AudioElement.FromFile(SOON));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"pe pahauchan waali hai"
                string REACHING = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\REACHING.mp3");
                audioPacket.Add(AudioElement.FromFile(REACHING));

            }

            //Is arriving on
            else if (train.StatusByte == 0x03)
            {
                //"platform sankhya"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"te aa rahi hai"
                string ARRIVING = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ARRIVING.mp3");
                audioPacket.Add(AudioElement.FromFile(ARRIVING));

            }

            //Has arrived on 
            else if (train.StatusByte == 0x04)
            {
                //"platform sankhya"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"te aa chuki hai"
                string ARRIVED = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\ARRIVED.mp3");
                audioPacket.Add(AudioElement.FromFile(ARRIVED));

                //"Yeh Gaadi"
                string TRAIN_WILL_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\TRAIN_WILL_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(TRAIN_WILL_DEPART));

                AddPunjabiETA();

                //"te rawana hovegi"
                string DEPART_AT = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DEPART_AT.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_AT));


            }

            //Running late
            else if (train.StatusByte == 0x05)
            {
                if (train.LateByHours != 00)
                {
                    //Late Time HOUR
                    string LateTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.LateByHours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeHOUR));

                    //"ਘੰਟੇ" ghante
                    string ਘੰਟੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਘੰਟੇ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਘੰਟੇ));

                }

                if (train.LateByMinutes != 00)
                {
                    //Late Time MINUTE
                    string LateTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.LateByMinutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeMINUTE));

                    //"ਮਿੰਟ" //minute
                    string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\मिਮਿੰਟ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));

                }

                //"di deeri ho rahi hai"
                string DELAY_TIME = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\DELAY_TIME.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_TIME));

                //"anumanit pahauchan da samay hai"
                string ETA = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\ETA.mp3");
                audioPacket.Add(AudioElement.FromFile(ETA));

                AddPunjabiETA();


            }

            //Cancelled || Cancelled:D
            else if (train.StatusByte == 0x06 || train.StatusByte == 0x0B)
            {
                //"raad kitti gayi hai"
                string CANCELLED = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\CANCELLED.mp3");
                audioPacket.Add(AudioElement.FromFile(CANCELLED));

                //"kripiya karke aur jaankari naal puch teech counter na samparg karo "
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));

            }

            //Indefinite Late
            else if (train.StatusByte == 0x07)
            {
                //"ananit samay di deeri hui hai"
                string INDEF_DELAY = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\INDEF_DELAY.mp3");
                audioPacket.Add(AudioElement.FromFile(INDEF_DELAY));

                //"kripiya karke aur jaankari naal puch teech counter na samparg karo "
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));
            }

            //Terminated At
            else if (train.StatusByte == 0x08)
            {
                //"mukamal ho gayi hai"
                string TERMINATED = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\TERMINATED.mp3");
                audioPacket.Add(AudioElement.FromFile(TERMINATED));

                //"kripiya karke aur jaankari naal puch teech counter na samparg karo "
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));

            }

            //Platform Changed 
            if (train.StatusByte == 0x09 || train.StatusByte == 0x12)
            {
                //"da platform"
                string PLATFORM_NO = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //"platform sankhya"
                string PLATFORM_NOSANKHYA = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NOSANKHYA));

                //Platform Number OLD
                string PlatformNumberOld = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.OldPFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumberOld));

                //"toh badal ke"
                string CHANGED_FROM = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\CHANGED_FROM.mp3");
                audioPacket.Add(AudioElement.FromFile(CHANGED_FROM));               

                //"platform sankhya"
                string PLATFORM_NO1 = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"kar dita gaya hai"
                string CHANGED_TO = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\CHANGED_TO.mp3");
                audioPacket.Add(AudioElement.FromFile(CHANGED_TO));


            }

            //Running right time: DEPATURE 
            else if (train.StatusByte == 0x0A)
            {
                //"samay te chale rahi hai"
                string ONTIME = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\ONTIME.mp3");
                audioPacket.Add(AudioElement.FromFile(ONTIME));

                //"Hun yeh Gaadi"
                string DEPART_NOW = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DEPART_NOW.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_NOW));

                AddPunjabiETD();

                //"te rawana hove gi"
                string DEPARTURE_TIME = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DEPARTURE_TIME.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPARTURE_TIME));


            }

            //Is Ready To leave
            else if (train.StatusByte == 0x0C)
            {
                //"platform number"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"toh rawana honle taiyaar hai"
                string READY_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\READY_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(READY_DEPART));


            }

            //Is on Platform
            else if (train.StatusByte == 0x0D)
            {
                //"iss vele"
                string CURRENTLY_ON = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\CURRENTLY_ON.mp3");
                audioPacket.Add(AudioElement.FromFile(CURRENTLY_ON));

                //"platform number"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"te hai"
                string STANDING = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\STANDING.mp3");
                audioPacket.Add(AudioElement.FromFile(STANDING));
            }

            //Departed
            else if (train.StatusByte == 0x0E)
            {
                //"platform number"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"toh rawana ho chuki hai"
                string DEPARTED = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DEPARTED.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPARTED));
            }

            //Rescheduled
            else if (train.StatusByte == 0x0F)
            {
                //"ki rawangi wich "
                string DELAY_IN_DEPARTURE = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DELAY_IN_DEPARTURE.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_IN_DEPARTURE));

                if (train.LateByHours != 00)
                {
                    //Late Time HOUR
                    string LateTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.LateByHours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeHOUR));

                    //"ਘੰਟੇ" ghante
                    string ਘੰਟੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਘੰਟੇ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਘੰਟੇ));

                }

                if (train.LateByMinutes != 00)
                {
                    //Late Time MINUTE
                    string LateTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.LateByMinutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeMINUTE));

                    //"ਮਿੰਟ" //minute
                    string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\मिਮਿੰਟ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));

                }

                //"di deeri ho rahi hai mein "
                string DELAY_TIME = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DELAY_TIME.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_TIME));

                //"Hun yeh Gaadi"
                string DEPART_NOW = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DEPART_NOW.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_NOW));

                AddPunjabiETD();


                //"nu chalegi hogi"
                string WILL_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\WILL_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(WILL_DEPART));

            }

            //Diverted
            else if (train.StatusByte == 0x10)
            {
                //"ko"
                string DIVERTED_TO = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DIVERTED_TO.mp3");
                audioPacket.Add(AudioElement.FromFile(DIVERTED_TO));

                //Diverted station name
                string StationName = string.Concat(audioFolderPath, $"\\Stations\\{train.SplStationCode}\\EnHindiglish\\Female\\{train.SplStationCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(StationName));

                //"nu modd dita gaya hai"
                string DIVERTED = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DIVERTED.mp3");
                audioPacket.Add(AudioElement.FromFile(DIVERTED));

                //"kripiya karke aur jaankari naal puch teech counter na samparg karo "
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Punjabi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));
            }

            // Delayed Departure
            else if (train.StatusByte == 0x11)
            {

                //"ki rawangi wich "
                string DELAY_IN_DEPARTURE = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DELAY_IN_DEPARTURE.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_IN_DEPARTURE));

                if (train.LateByHours != 00)
                {
                    //Late Time HOUR
                    string LateTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.LateByHours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeHOUR));

                    //"ਘੰਟੇ" ghante
                    string ਘੰਟੇ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\ਘੰਟੇ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਘੰਟੇ));

                }

                if (train.LateByMinutes != 00)
                {
                    //Late Time MINUTE
                    string LateTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\{train.LateByMinutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeMINUTE));

                    //"ਮਿੰਟ" //minute
                    string ਮਿੰਟ = string.Concat(audioFolderPath, $"\\Time\\Punjabi\\Female\\मिਮਿੰਟ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ਮਿੰਟ));

                }

                //"di deeri ho rahi hai mein "
                string DELAY_TIME = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DELAY_TIME.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_TIME));

                //"Hun yeh Gaadi"
                string DEPART_NOW = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\DEPART_NOW.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_NOW));

                AddPunjabiETD();


                //"nu chalegi hogi"
                string WILL_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Punjabi\\Female\\WILL_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(WILL_DEPART));

            }

            #endregion

        }

        #endregion

        #region Hindi
        public void AddHindiETA()
        {

            if (selectedFormat == AnnFormat._24_hr)
            {

                //Expected Arrival Time HOUR
                string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours}.mp3");
                audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));


                if (train.ETA_Minutes != 00)
                {
                    //" बजकर"
                    string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                    audioPacket.Add(AudioElement.FromFile(बजकर));

                    //Expected Arrival Time MINUTE
                    string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                    //"minute"
                    string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                    audioPacket.Add(AudioElement.FromFile(मिनट));
                }
                else
                {
                    //" बजे"
                    string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                    audioPacket.Add(AudioElement.FromFile(बजे));
                }

            }

            else if (selectedFormat == AnnFormat._12_hr)
            {
                // raat 12
                if (train.ETA_Hours == 0)
                {
                    //"raat"
                    string रात = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\रात.mp3");
                    audioPacket.Add(AudioElement.FromFile(रात));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\12.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }


                }

                //"raat"
                else if (train.ETA_Hours >= 20 || train.ETA_Hours <= 4)
                {
                    //"raat"
                    string रात = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\रात.mp3");
                    audioPacket.Add(AudioElement.FromFile(रात));

                    if (train.ETA_Hours >= 20)
                    {
                        //Expected Arrival Time HOUR
                        string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours - 12}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    }
                    else if (train.ETA_Hours <= 4)
                    {
                        //Expected Arrival Time HOUR
                        string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    }

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }


                }

                //"subah"
                else if (train.ETA_Hours >= 5 && train.ETA_Hours <= 11)
                {
                    //"subah"
                    string सुबह = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\सुबह.mp3");
                    audioPacket.Add(AudioElement.FromFile(सुबह));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //dopahar 12 
                else if (train.ETA_Hours == 12)
                {
                    //"dopahar"
                    string दोपहर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\दोपहर.mp3");
                    audioPacket.Add(AudioElement.FromFile(दोपहर));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //"dopahar"
                else if (train.ETA_Hours >= 13 && train.ETA_Hours <= 16)
                {
                    //"dopahar"
                    string दोपहर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\दोपहर.mp3");
                    audioPacket.Add(AudioElement.FromFile(दोपहर));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //"shaam"
                else if (train.ETA_Hours >= 15 && train.ETA_Hours <= 19)
                {
                    //"shaam"
                    string शाम = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\शाम.mp3");
                    audioPacket.Add(AudioElement.FromFile(शाम));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //raat
                else
                {
                    //"raat"
                    string रात = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\रात.mp3");
                    audioPacket.Add(AudioElement.FromFile(रात));

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

            }
        }

        public void AddHindiETD()
        {

            if (selectedFormat == AnnFormat._24_hr)
            {

                //Expected Departure Time HOUR
                string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours}.mp3");
                audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));


                if (train.ETA_Minutes != 00)
                {
                    //" बजकर"
                    string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                    audioPacket.Add(AudioElement.FromFile(बजकर));

                    //Expected Departure Time MINUTE
                    string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                    //"minute"
                    string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                    audioPacket.Add(AudioElement.FromFile(मिनट));
                }
                else
                {
                    //" बजे"
                    string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                    audioPacket.Add(AudioElement.FromFile(बजे));
                }

            }

            else if (selectedFormat == AnnFormat._12_hr)
            {
                // raat 12
                if (train.ETD_Hours == 0)
                {
                    //"raat"
                    string रात = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\रात.mp3");
                    audioPacket.Add(AudioElement.FromFile(रात));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\12.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }


                }

                //"raat"
                else if (train.ETD_Hours >= 16 || train.ETD_Hours <= 4)
                {
                    //"raat"
                    string रात = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\रात.mp3");
                    audioPacket.Add(AudioElement.FromFile(रात));

                    if (train.ETD_Hours >= 16)
                    {
                        //Expected Departure Time HOUR
                        string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours - 12}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    }
                    else if (train.ETD_Hours <= 4)
                    {
                        //Expected Departure Time HOUR
                        string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    }

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }


                }

                //"subah"
                else if (train.ETD_Hours >= 5 && train.ETD_Hours <= 11)
                {
                    //"subah"
                    string सुबह = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\सुबह.mp3");
                    audioPacket.Add(AudioElement.FromFile(सुबह));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //dopahar 12 
                else if (train.ETD_Hours == 12)
                {
                    //"dopahar"
                    string दोपहर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\दोपहर.mp3");
                    audioPacket.Add(AudioElement.FromFile(दोपहर));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //"dopahar"
                else if (train.ETD_Hours >= 13 && train.ETD_Hours <= 16)
                {
                    //"dopahar"
                    string दोपहर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\दोपहर.mp3");
                    audioPacket.Add(AudioElement.FromFile(दोपहर));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //"shaam"
                else if (train.ETD_Hours >= 15 && train.ETD_Hours <= 19)
                {
                    //"shaam"
                    string शाम = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\शाम.mp3");
                    audioPacket.Add(AudioElement.FromFile(शाम));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

                //raat
                else
                {
                    //"raat"
                    string रात = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\रात.mp3");
                    audioPacket.Add(AudioElement.FromFile(रात));

                    //Expected Departure Time HOUR
                    string ExpectedDepartureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //" बजकर"
                        string बजकर = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजकर.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजकर));

                        //Expected Departure Time MINUTE
                        string ExpectedDepartureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepartureTimeMINUTE));

                        //"minute"
                        string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                        audioPacket.Add(AudioElement.FromFile(मिनट));
                    }
                    else
                    {
                        //" बजे"
                        string बजे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\बजे.mp3");
                        audioPacket.Add(AudioElement.FromFile(बजे));
                    }

                }

            }
        }

        public void AddHindiAnnouncement()
        {
            #region Hindi

            //Chime
            string Chime = string.Concat(audioFolderPath, "\\Chimes\\ann-chime.mp3");
            audioPacket.Add(AudioElement.FromFile(Chime));

            //"Attention Passengers"
            string AP = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\AP.mp3");
            audioPacket.Add(AudioElement.FromFile(AP));


            //"Train Number"
            string TN = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\TN.mp3");
            audioPacket.Add(AudioElement.FromFile(TN));

            //Train Number
            #region 
            int digit1 = 0;
            int digit2 = 0;
            int digit3 = 0;
            int digit4 = 0;
            int digit5 = 0;

            if (int.TryParse(train.TrainNumber, out int number))
            {
                digit1 = number / 10000 % 10; // 1
                digit2 = number / 1000 % 10;  // 2
                digit3 = number / 100 % 10;   // 3
                digit4 = number / 10 % 10;    // 4
                digit5 = number % 10;         // 5

            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
            int[] digits = new int[5];

            digits[0] = digit1;
            digits[1] = digit2;
            digits[2] = digit3;
            digits[3] = digit4;
            digits[4] = digit5;

            for (int i = 0; i < 5; i++)
            {
                string numberdigit = string.Concat(audioFolderPath, $"\\numbers\\male\\{digits[i]}-hi-IN.mp3");//actually female

                audioPacket.Add(AudioElement.FromFile(numberdigit));

            }

            #endregion

            //Source Station
            string SourceStation = string.Concat(audioFolderPath, $"\\Stations\\{train.SrcCode}\\Hindi\\Male\\{train.SrcCode}.mp3"); //It is actually female
            audioPacket.Add(AudioElement.FromFile(SourceStation));

            //"se"
            string FROM = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\FROM.mp3");
            audioPacket.Add(AudioElement.FromFile(FROM));

            //Destination Station
            string DestinationStation = string.Concat(audioFolderPath, $"\\Stations\\{train.DestCode}\\Hindi\\Male\\{train.DestCode}.mp3"); //It is actually female
            audioPacket.Add(AudioElement.FromFile(DestinationStation));

            //"ki auor jaane waali"
            string TOWARDS = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\TOWARDS.mp3");
            audioPacket.Add(AudioElement.FromFile(TOWARDS));

            //Train Name
            if (int.Parse(train.TrainNumber) <= 9999)
            {
                string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\Hindi\\0{train.TrainNumber}.mp3");
                audioPacket.Add(AudioElement.FromFile(TrainName));

            }
            else
            {
                string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\Hindi\\{train.TrainNumber}.mp3");
                audioPacket.Add(AudioElement.FromFile(TrainName));

            }
            //Train name is accessed by th e train number and it must always be a 5 digit number and also check if the file actually exists or not in all the fields

            //Running right time
            if (train.StatusByte == 0x01)
            {
                //"samay par chale rahi hai"
                string ONTIME = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ONTIME.mp3");
                audioPacket.Add(AudioElement.FromFile(ONTIME));

                //"anumanit aadman samay hai"
                string ETA = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ETA.mp3");
                audioPacket.Add(AudioElement.FromFile(ETA));


                AddHindiETA();
            }

            //Will arrive shortly
            else if (train.StatusByte == 0x02)
            {
                //"shigrah hi platform sankhya"
                string ARR_SHORTLY = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ARR_SHORTLY.mp3");
                audioPacket.Add(AudioElement.FromFile(ARR_SHORTLY));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"par aa rahi hai"
                string ARRIVING = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ARRIVING.mp3");
                audioPacket.Add(AudioElement.FromFile(ARRIVING));

            }

            //Is arriving on
            else if (train.StatusByte == 0x03)
            {
                //"platform sankhya"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"par aa rahi hai"
                string REACHING = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\REACHING.mp3");
                audioPacket.Add(AudioElement.FromFile(REACHING));

            }

            //Has arrived on 
            else if (train.StatusByte == 0x04)
            {
                //"platform number"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"par aa chuki hai"
                string ARRIVED = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ARRIVED.mp3");
                audioPacket.Add(AudioElement.FromFile(ARRIVED));

                //"Yeh Gaadi"
                string TRAIN_WILL_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\TRAIN_WILL_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(TRAIN_WILL_DEPART));

                AddHindiETD();

                //"par rawana hogi"
                string DEPART_AT = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DEPART_AT.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_AT));


            }

            //Running late
            else if (train.StatusByte == 0x05)
            {

                if (train.LateByHours != 00)
                {

                    //Late Time HOUR
                    string LateTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.LateByHours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeHOUR));

                    //"घंटे"
                    string घंटे = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\घंटे.mp3");
                    audioPacket.Add(AudioElement.FromFile(घंटे));

                }

                if (train.LateByMinutes != 00)
                {
                    //Late Time MINUTE
                    string LateTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.LateByMinutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeMINUTE));

                    //"मिनट"
                    string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                    audioPacket.Add(AudioElement.FromFile(मिनट));

                }

                //"deeri se chal rahi hai"
                string RUNNING_LATE = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\RUNNING_LATE.mp3");
                audioPacket.Add(AudioElement.FromFile(RUNNING_LATE));

                //"anumanit aadman samay hai"
                string ETA = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ETA.mp3");
                audioPacket.Add(AudioElement.FromFile(ETA));

                AddHindiETA();


            }

            //Cancelled || Cancelled:D
            else if (train.StatusByte == 0x06 || train.StatusByte == 0x0B)
            {
                //"has been cancelled"
                string CANCELLED = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\CANCELLED.mp3");
                audioPacket.Add(AudioElement.FromFile(CANCELLED));

                //"kripiya adhik jaankari ke liye puch tach counter se samparg kare"
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));

            }

            //Indefinite Late
            else if (train.StatusByte == 0x07)
            {
                //"anischit kal ke liye vilambit hai"
                string INDEF_DELAY = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\INDEF_DELAY.mp3");
                audioPacket.Add(AudioElement.FromFile(INDEF_DELAY));

                //"kripiya adhik jaankari ke liye puch tach counter se samparg kare"
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));
            }

            //Terminated At
            else if (train.StatusByte == 0x08)
            {
                //"has been terminated"
                string TERMINATED = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\TERMINATED.mp3");
                audioPacket.Add(AudioElement.FromFile(TERMINATED));

                //"kripiya adhik jaankari ke liye puch tach counter se samparg kare"
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));

            }

            //Platform Changed 
            if (train.StatusByte == 0x09 || train.StatusByte == 0x12)
            {
                //"ka platform"
                string PLATFORM_CHANGE = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\PLATFORM_CHANGE.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_CHANGE));

                //"platform sankhya"
                string PLATFORM_NO = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number OLD
                string PlatformNumberOld = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.OldPFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumberOld));

                //"se badal kar"
                string FROM_PLATFORM = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\FROM_PLATFORM.mp3");
                audioPacket.Add(AudioElement.FromFile(FROM_PLATFORM));

                //"platform sankhya"
                string PLATFORM_NO1 = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"kar diya gaya hai"
                string CHANGED_TO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\CHANGED_TO.mp3");
                audioPacket.Add(AudioElement.FromFile(CHANGED_TO));


            }

            //Running right time: DEPATURE 
            else if (train.StatusByte == 0x0A)
            {
                //"samay par chale rahi hai"
                string ONTIME = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\ONTIME.mp3");
                audioPacket.Add(AudioElement.FromFile(ONTIME));

                //"Yeh Gaadi"
                string TRAIN_WILL_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\TRAIN_WILL_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(TRAIN_WILL_DEPART));

                AddHindiETD();

                //"par rawana hogi"
                string DEPART_AT = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DEPART_AT.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_AT));


            }

            //Is Ready To leave
            else if (train.StatusByte == 0x0C)
            {
                //"platform number"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"se prasthan karne ke liye taiyaar hai"
                string READY_TO_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\READY_TO_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(READY_TO_DEPART));


            }

            //Is on Platform
            else if (train.StatusByte == 0x0D)
            {
                //"vartman mein platform sankhya"
                string CURRENTLY_ON = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\CURRENTLY_ON.mp3");
                audioPacket.Add(AudioElement.FromFile(CURRENTLY_ON));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"par khadi hai"
                string STANDING = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\STANDING.mp3");
                audioPacket.Add(AudioElement.FromFile(STANDING));
            }

            //Departed
            else if (train.StatusByte == 0x0E)
            {
                //"platform number"
                string PLATFORM_NO = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\PLATFORM_NO.mp3");
                audioPacket.Add(AudioElement.FromFile(PLATFORM_NO));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                //"se prasthan kar chuki hai"
                string DEPARTED = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DEPARTED.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPARTED));
            }

            //Rescheduled
            else if (train.StatusByte == 0x0F)
            {
                //"ki prasthan mein "
                string DELAY_IN_DEPARTURE = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DELAY_IN_DEPARTURE.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_IN_DEPARTURE));

                if (train.LateByHours != 00)
                {
                    //Late Time HOUR
                    string LateTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.LateByHours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeHOUR));

                    //"घंटे"
                    string घंटे1 = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\घंटे.mp3");
                    audioPacket.Add(AudioElement.FromFile(घंटे1));
                }

                if (train.LateByMinutes != 00)
                {
                    //Late Time MINUTE
                    string LateTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.LateByMinutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeMINUTE));

                    //"मिनट"
                    string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                    audioPacket.Add(AudioElement.FromFile(मिनट));

                }

                //"ki deeri ho rahi hai mein "
                string DELAY_TIME = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DELAY_TIME.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_TIME));

                //"Yeh Gaadi"
                string TRAIN_WILL_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\TRAIN_WILL_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(TRAIN_WILL_DEPART));

                AddHindiETD();


                //"Par rawana hogi"
                string DEPART_AT = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DEPART_AT.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_AT));

            }

            //Diverted
            else if (train.StatusByte == 0x10)
            {
                //"ko"
                string DIVERTED_TO = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DIVERTED_TO.mp3");
                audioPacket.Add(AudioElement.FromFile(DIVERTED_TO));

                //Diverted station name
                string StationName = string.Concat(audioFolderPath, $"\\Stations\\{train.SplStationCode}\\EnHindiglish\\Female\\{train.SplStationCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(StationName));

                //"ki auor modd diya gaya hai"
                string DIVERTED_TOWARDS = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DIVERTED_TOWARDS.mp3");
                audioPacket.Add(AudioElement.FromFile(DIVERTED_TOWARDS));

                //"kripiya adhik jaankari ke liye puch tach counter se samparg kare"
                string ENQUIRY = string.Concat(audioFolderPath, $"\\Announcements\\Hindi\\Female\\ENQUIRY.mp3");
                audioPacket.Add(AudioElement.FromFile(ENQUIRY));
            }

            // Delayed Departure
            else if (train.StatusByte == 0x11)
            {

                //"ki prasthan mein "
                string DELAY_IN_DEPARTURE = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DELAY_IN_DEPARTURE.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_IN_DEPARTURE));

                if (train.LateByHours != 00)
                {
                    //Late Time HOUR
                    string LateTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.LateByHours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeHOUR));

                    //"घंटे"
                    string घंटे1 = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\घंटे.mp3");
                    audioPacket.Add(AudioElement.FromFile(घंटे1));
                }

                if (train.LateByMinutes != 00)
                {
                    //Late Time MINUTE
                    string LateTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\{train.LateByMinutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateTimeMINUTE));

                    //"मिनट"
                    string मिनट = string.Concat(audioFolderPath, $"\\Time\\Hindi\\Female\\मिनट.mp3");
                    audioPacket.Add(AudioElement.FromFile(मिनट));

                }

                //"ki deeri ho rahi hai mein "
                string DELAY_TIME = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DELAY_TIME.mp3");
                audioPacket.Add(AudioElement.FromFile(DELAY_TIME));

                //"Yeh Gaadi"
                string TRAIN_WILL_DEPART = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\TRAIN_WILL_DEPART.mp3");
                audioPacket.Add(AudioElement.FromFile(TRAIN_WILL_DEPART));

                AddHindiETD();

                //"Par rawana hogi"
                string DEPART_AT = string.Concat(audioFolderPath, "\\Announcements\\Hindi\\Female\\DEPART_AT.mp3");
                audioPacket.Add(AudioElement.FromFile(DEPART_AT));

            }

            #endregion

        }
        #endregion

        #region English
        public void AddEnglishETA()
        {
            if (selectedFormat == AnnFormat._24_hr)
            {

                //Expected Arrival Time HOUR
                string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Hours}.mp3");
                audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                //"hours"
                string hours = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\hours.mp3");
                audioPacket.Add(AudioElement.FromFile(hours));

                if (train.ETA_Minutes != 00)
                {
                    //Expected Arrival Time MINUTE
                    string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Minutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));

                    //"mintues"
                    string mintues = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\minutes.mp3");
                    audioPacket.Add(AudioElement.FromFile(mintues));

                }
                //else
                //{
                //    //o'clock
                //    string o_clock = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\o'clock.mp3");
                //    audioPacket.Add(AudioElement.FromFile(o_clock));
                //}
            }

            else if (selectedFormat == AnnFormat._12_hr)
            {

                if (train.ETA_Hours == 0)
                {
                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\12.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));
                    }
                    //else
                    //{
                    //    //o'clock
                    //    string o_clock = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\o'clock.mp3");
                    //    audioPacket.Add(AudioElement.FromFile(o_clock));
                    //}

                    //"am"
                    string AM = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\AM.mp3");
                    audioPacket.Add(AudioElement.FromFile(AM));
                }

                if (train.ETA_Hours >= 12)
                {
                    //pm


                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));
                    }
                    //else
                    //{
                    //    //o'clock
                    //    string o_clock = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\o'clock.mp3");
                    //    audioPacket.Add(AudioElement.FromFile(o_clock));
                    //}

                    //"am"
                    string PM = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\PM.mp3");
                    audioPacket.Add(AudioElement.FromFile(PM));

                }

                else
                {
                    //am

                    //Expected Arrival Time HOUR
                    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));

                    if (train.ETA_Minutes != 00)
                    {
                        //Expected Arrival Time MINUTE
                        string ExpectedArrivalTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTE));
                    }
                    //else
                    //{
                    //    //o'clock
                    //    string o_clock = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\o'clock.mp3");
                    //    audioPacket.Add(AudioElement.FromFile(o_clock));
                    //}

                    //"am"
                    string AM = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\AM.mp3");
                    audioPacket.Add(AudioElement.FromFile(AM));

                }
            }

        }

        public void AddEnglishETD()
        {
            if (selectedFormat == AnnFormat._24_hr)
            {

                //Expected Depature Time HOUR
                string ExpectedDepatureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETD_Hours}.mp3");
                audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeHOUR));

                //"hours"
                string hours = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\hours.mp3");
                audioPacket.Add(AudioElement.FromFile(hours));

                if (train.ETD_Minutes != 00)
                {
                    //Expected Depature Time MINUTE
                    string ExpectedDepatureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETD_Minutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeMINUTE));

                    //"mintues"
                    string mintues = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\minutes.mp3");
                    audioPacket.Add(AudioElement.FromFile(mintues));

                }
            }

            else if (selectedFormat == AnnFormat._12_hr)
            {

                if (train.ETD_Hours == 0)
                {
                    //Expected Depature Time HOUR
                    string ExpectedDepatureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\12.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeHOUR));

                    if (train.ETD_Minutes != 00)
                    {
                        //Expected Depature Time MINUTE
                        string ExpectedDepatureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeMINUTE));
                    }
                    //else
                    //{
                    //    //o'clock
                    //    string o_clock = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\o'clock.mp3");
                    //    audioPacket.Add(AudioElement.FromFile(o_clock));
                    //}

                    //"am"
                    string AM = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\AM.mp3");
                    audioPacket.Add(AudioElement.FromFile(AM));
                }

                if (train.ETD_Hours >= 12)
                {
                    //pm


                    //Expected Depature Time HOUR
                    string ExpectedDepatureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETD_Hours - 12}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeHOUR));

                    if (train.ETD_Minutes != 00)
                    {
                        //Expected Depature Time MINUTE
                        string ExpectedDepatureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeMINUTE));
                    }
                    //else
                    //{
                    //    //o'clock
                    //    string o_clock = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\o'clock.mp3");
                    //    audioPacket.Add(AudioElement.FromFile(o_clock));
                    //}

                    //"am"
                    string PM = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\PM.mp3");
                    audioPacket.Add(AudioElement.FromFile(PM));

                }

                else
                {
                    //am

                    //Expected Depature Time HOUR
                    string ExpectedDepatureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETD_Hours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeHOUR));

                    if (train.ETD_Minutes != 00)
                    {
                        //Expected Depature Time MINUTE
                        string ExpectedDepatureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETD_Minutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(ExpectedDepatureTimeMINUTE));
                    }
                    //else
                    //{
                    //    //o'clock
                    //    string o_clock = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\o'clock.mp3");
                    //    audioPacket.Add(AudioElement.FromFile(o_clock));
                    //}

                    //"am"
                    string AM = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\AM.mp3");
                    audioPacket.Add(AudioElement.FromFile(AM));

                }
            }

        }

        public void AddEnglishAnnouncement()
        {
            #region English

            //Chime
            string Chime = string.Concat(audioFolderPath, "\\Chimes\\ann-chime.mp3");
            audioPacket.Add(AudioElement.FromFile(Chime));

            //"Attention Passengers"
            string AP = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\AP.mp3");
            audioPacket.Add(AudioElement.FromFile(AP));

            //platform changed
            if (train.StatusByte == 0x09 || train.StatusByte == 0x12) //Platform Changed 
            {
                //The: NO CLIP FOUND

                //"Platform for"
                string PLTFM_CHG = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\PLTFM_CHG.mp3");
                audioPacket.Add(AudioElement.FromFile(PLTFM_CHG));

                //"Train Number"
                string TN = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TN.mp3");
                audioPacket.Add(AudioElement.FromFile(TN));

                //Train Number
                #region 
                int digit1 = 0;
                int digit2 = 0;
                int digit3 = 0;
                int digit4 = 0;
                int digit5 = 0;

                if (int.TryParse(train.TrainNumber, out int number))
                {
                    digit1 = number / 10000 % 10; // 1
                    digit2 = number / 1000 % 10;  // 2
                    digit3 = number / 100 % 10;   // 3
                    digit4 = number / 10 % 10;    // 4
                    digit5 = number % 10;         // 5

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
                int[] digits = new int[5];

                digits[0] = digit1;
                digits[1] = digit2;
                digits[2] = digit3;
                digits[3] = digit4;
                digits[4] = digit5;

                for (int i = 0; i < 5; i++)
                {
                    string numberdigit = string.Concat(audioFolderPath, $"\\numbers\\female\\{digits[i]}-en-IN.mp3");

                    audioPacket.Add(AudioElement.FromFile(numberdigit));

                }

                #endregion

                //"travelling from"
                string TF = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TF.mp3");
                audioPacket.Add(AudioElement.FromFile(TF));

                //Source Station
                string SourceStation = string.Concat(audioFolderPath, $"\\Stations\\{train.SrcCode}\\English\\Female\\{train.SrcCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(SourceStation));

                //"to"
                string To = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TO.mp3");
                audioPacket.Add(AudioElement.FromFile(To));

                //Destination Station
                string DestinationStation = string.Concat(audioFolderPath, $"\\Stations\\{train.DestCode}\\English\\Female\\{train.DestCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(DestinationStation));

                //Train Name
                if (int.Parse(train.TrainNumber) <= 9999)
                {
                    string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\English\\0{train.TrainNumber}.mp3");
                    audioPacket.Add(AudioElement.FromFile(TrainName));

                }
                else
                {
                    string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\English\\{train.TrainNumber}.mp3");
                    audioPacket.Add(AudioElement.FromFile(TrainName));

                }

                //"has been changed from"
                string CHG_FROM = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\CHG_FROM.mp3");
                audioPacket.Add(AudioElement.FromFile(CHG_FROM));

                //"platform number"
                string PN = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\PN.mp3");
                audioPacket.Add(AudioElement.FromFile(PN));

                //Platform Number OLD
                string PlatformNumberOld = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.OldPFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumberOld));

                //"to"
                string TO = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TO.mp3");
                audioPacket.Add(AudioElement.FromFile(TO));

                //"platform number"
                string PN1 = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\PN.mp3");
                audioPacket.Add(AudioElement.FromFile(PN1));

                //Platform Number
                string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.PFNo}.mp3");
                audioPacket.Add(AudioElement.FromFile(PlatformNumber));



            }

            // Delayed Departure
            else if (train.StatusByte == 0x11) // Delayed Departure
            {

                //"The departure of "
                string DEP_OF = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\DEP_OF.mp3");
                audioPacket.Add(AudioElement.FromFile(DEP_OF));


                //"Train Number"
                string TN = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TN.mp3");
                audioPacket.Add(AudioElement.FromFile(TN));

                //Train Number
                #region 
                int digit1 = 0;
                int digit2 = 0;
                int digit3 = 0;
                int digit4 = 0;
                int digit5 = 0;

                if (int.TryParse(train.TrainNumber, out int number))
                {
                    digit1 = number / 10000 % 10; // 1
                    digit2 = number / 1000 % 10;  // 2
                    digit3 = number / 100 % 10;   // 3
                    digit4 = number / 10 % 10;    // 4
                    digit5 = number % 10;         // 5

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
                int[] digits = new int[5];

                digits[0] = digit1;
                digits[1] = digit2;
                digits[2] = digit3;
                digits[3] = digit4;
                digits[4] = digit5;

                for (int i = 0; i < 5; i++)
                {
                    string numberdigit = string.Concat(audioFolderPath, $"\\numbers\\female\\{digits[i]}-en-IN.mp3");

                    audioPacket.Add(AudioElement.FromFile(numberdigit));

                }

                #endregion

                //"travelling from"
                string TF = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TF.mp3");
                audioPacket.Add(AudioElement.FromFile(TF));

                //Source Station
                string SourceStation = string.Concat(audioFolderPath, $"\\Stations\\{train.SrcCode}\\English\\Female\\{train.SrcCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(SourceStation));

                //"to"
                string TO = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TO.mp3");
                audioPacket.Add(AudioElement.FromFile(TO));

                //Destination Station
                string DestinationStation = string.Concat(audioFolderPath, $"\\Stations\\{train.DestCode}\\English\\Female\\{train.DestCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(DestinationStation));

                //Train Name
                if (int.Parse(train.TrainNumber) <= 9999)
                {
                    string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\English\\0{train.TrainNumber}.mp3");
                    audioPacket.Add(AudioElement.FromFile(TrainName));

                }
                else
                {
                    string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\English\\{train.TrainNumber}.mp3");
                    audioPacket.Add(AudioElement.FromFile(TrainName));

                }


                //"is delayed by"
                string DL_BY = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\DL_BY.mp3");
                audioPacket.Add(AudioElement.FromFile(DL_BY));

                if (train.LateByHours != 00)
                {
                    //Late Depature Time HOUR
                    string LateDepatureTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.LateByHours}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateDepatureTimeHOUR));

                    //"hours"
                    string hours1 = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\hours.mp3");
                    audioPacket.Add(AudioElement.FromFile(hours1));

                }


                if (train.LateByMinutes != 00)
                {
                    //Late Time MINUTE
                    string LateDepatureTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.LateByMinutes}.mp3");
                    audioPacket.Add(AudioElement.FromFile(LateDepatureTimeMINUTE));

                    //"mintues"
                    string mintues = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\minutes.mp3");
                    audioPacket.Add(AudioElement.FromFile(mintues));

                }

                //"The train will now depart at"
                string ND = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ND.mp3");
                audioPacket.Add(AudioElement.FromFile(ND));

                AddEnglishETD();

            }

            else
            {

                //"Train Number"
                string TN = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TN.mp3");
                audioPacket.Add(AudioElement.FromFile(TN));

                //Train Number
                #region 
                int digit1 = 0;
                int digit2 = 0;
                int digit3 = 0;
                int digit4 = 0;
                int digit5 = 0;

                if (int.TryParse(train.TrainNumber, out int number))
                {
                    digit1 = number / 10000 % 10; // 1
                    digit2 = number / 1000 % 10;  // 2
                    digit3 = number / 100 % 10;   // 3
                    digit4 = number / 10 % 10;    // 4
                    digit5 = number % 10;         // 5

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
                int[] digits = new int[5];

                digits[0] = digit1;
                digits[1] = digit2;
                digits[2] = digit3;
                digits[3] = digit4;
                digits[4] = digit5;

                for (int i = 0; i < 5; i++)
                {
                    string numberdigit = string.Concat(audioFolderPath, $"\\numbers\\female\\{digits[i]}-en-IN.mp3");

                    audioPacket.Add(AudioElement.FromFile(numberdigit));

                }

                #endregion

                //"travelling from"
                string TF = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TF.mp3");
                audioPacket.Add(AudioElement.FromFile(TF));

                //Source Station
                string SourceStation = string.Concat(audioFolderPath, $"\\Stations\\{train.SrcCode}\\English\\Female\\{train.SrcCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(SourceStation));

                //"to"
                string To = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TO.mp3");
                audioPacket.Add(AudioElement.FromFile(To));

                //Destination Station
                string DestinationStation = string.Concat(audioFolderPath, $"\\Stations\\{train.DestCode}\\English\\Female\\{train.DestCode}.mp3"); //It is actually female
                audioPacket.Add(AudioElement.FromFile(DestinationStation));

                //Train Name
                if (int.Parse(train.TrainNumber) <= 9999)
                {
                    string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\English\\0{train.TrainNumber}.mp3");
                    audioPacket.Add(AudioElement.FromFile(TrainName));

                }
                else
                {
                    string TrainName = string.Concat(audioFolderPath, $"\\Trains\\NDLS\\female\\English\\{train.TrainNumber}.mp3");
                    audioPacket.Add(AudioElement.FromFile(TrainName));

                }
                //Train name is accessed by th e train number and it must always be a 5 digit number and also check if the file actually exists or not in all the fields


                //Running right time
                if (train.StatusByte == 0x01)
                {
                    //"is running on time"
                    string ONT = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\ONT.mp3");
                    audioPacket.Add(AudioElement.FromFile(ONT));

                    //"The expected arrival time is"
                    string ETA = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\ETA.mp3");
                    audioPacket.Add(AudioElement.FromFile(ETA));

                    AddEnglishETA();

                }

                //Will arrive shortly
                else if (train.StatusByte == 0x02)
                {
                    //"will ahortly arrive on"
                    string WSA = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\WSA.mp3");
                    audioPacket.Add(AudioElement.FromFile(WSA));

                    //"platform number"
                    string PN = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\PN.mp3");
                    audioPacket.Add(AudioElement.FromFile(PN));

                    //Platform Number
                    string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.PFNo}.mp3");
                    audioPacket.Add(AudioElement.FromFile(PlatformNumber));
                }

                //Is arriving on
                else if (train.StatusByte == 0x03)
                {
                    //"is arriving on"
                    string ARR = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\ARR.mp3");
                    audioPacket.Add(AudioElement.FromFile(ARR));

                    //"platform number"
                    string PN = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\PN.mp3");
                    audioPacket.Add(AudioElement.FromFile(PN));

                    //Platform Number
                    string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.PFNo}.mp3");
                    audioPacket.Add(AudioElement.FromFile(PlatformNumber));
                }

                //Has arrived on 
                else if (train.StatusByte == 0x04)
                {
                    //"is arriving on"
                    string ARRVD = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\ARRVD.mp3");
                    audioPacket.Add(AudioElement.FromFile(ARRVD));

                    //"platform number"
                    string PN = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\PN.mp3");
                    audioPacket.Add(AudioElement.FromFile(PN));

                    //Platform Number
                    string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.PFNo}.mp3");
                    audioPacket.Add(AudioElement.FromFile(PlatformNumber));

                    //"The train will depart at "
                    string TDA = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TDA.mp3");
                    audioPacket.Add(AudioElement.FromFile(TDA));

                    AddEnglishETD();


                }

                //Running late
                else if (train.StatusByte == 0x05)
                {
                    //"is runnig late by"
                    string RL = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\RL.mp3");
                    audioPacket.Add(AudioElement.FromFile(RL));

                    if (train.LateByHours != 00)
                    {
                        //Late Time HOUR
                        string LateTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.LateByHours}.mp3");
                        audioPacket.Add(AudioElement.FromFile(LateTimeHOUR));

                        //"hours"
                        string hours1 = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\hours.mp3");
                        audioPacket.Add(AudioElement.FromFile(hours1));

                    }


                    if (train.LateByMinutes != 00)
                    {
                        //Late Time MINUTE
                        string LateTimeMINUTE = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.LateByMinutes}.mp3");
                        audioPacket.Add(AudioElement.FromFile(LateTimeMINUTE));

                        //"mintues"
                        string mintues = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\minutes.mp3");
                        audioPacket.Add(AudioElement.FromFile(mintues));

                    }

                    //"The expected arrival time is"
                    string ETA = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ETA.mp3");
                    audioPacket.Add(AudioElement.FromFile(ETA));

                    AddEnglishETA();

                }

                //Cancelled || Cancelled:D
                else if (train.StatusByte == 0x06 || train.StatusByte == 0x0B)
                {
                    //"has been cancelled"
                    string CANCELLED = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\CANCELLED.mp3");
                    audioPacket.Add(AudioElement.FromFile(CANCELLED));

                    //"please contact the enquiry counter for any further details"
                    string ENQ = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ENQ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ENQ));

                }

                //Indefinite Late
                else if (train.StatusByte == 0x07)
                {
                    //"is indefinitely delayed"
                    string ID = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ID.mp3");
                    audioPacket.Add(AudioElement.FromFile(ID));

                    //"please contact the enquiry counter for any further details"
                    string ENQ = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ENQ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ENQ));
                }

                //Terminated At
                else if (train.StatusByte == 0x08)
                {
                    //"has been terminated"
                    string TERMINATED = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\TERMINATED.mp3");
                    audioPacket.Add(AudioElement.FromFile(TERMINATED));


                    //"please contact the enquiry counter for any further details"
                    string ENQ = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ENQ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ENQ));

                }

                //Running right time: DEPATURE 
                else if (train.StatusByte == 0x0A)
                {
                    //"is running on time"
                    string ONT = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\ONT.mp3");
                    audioPacket.Add(AudioElement.FromFile(ONT));

                    //"The train will depart at "
                    string TDA = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TDA.mp3");
                    audioPacket.Add(AudioElement.FromFile(TDA));

                    AddEnglishETD();


                }

                //Is Ready To leave
                else if (train.StatusByte == 0x0C)
                {
                    //"is ready to depart from"
                    string RDY_DEP = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\RDY_DEP.mp3");
                    audioPacket.Add(AudioElement.FromFile(RDY_DEP));

                    //"platform number"
                    string PN = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\PN.mp3");
                    audioPacket.Add(AudioElement.FromFile(PN));

                    //Platform Number
                    string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.PFNo}.mp3");
                    audioPacket.Add(AudioElement.FromFile(PlatformNumber));
                }

                //Is on Platform
                else if (train.StatusByte == 0x0D)
                {
                    //"is currently on platform number"
                    string CURR_ON = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\CURR_ON.mp3");
                    audioPacket.Add(AudioElement.FromFile(CURR_ON));

                    //Platform Number
                    string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.PFNo}.mp3");
                    audioPacket.Add(AudioElement.FromFile(PlatformNumber));
                }

                //Departed
                else if (train.StatusByte == 0x0E)
                {
                    //"has departed from"
                    string DEPARTED = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\DEPARTED.mp3");
                    audioPacket.Add(AudioElement.FromFile(DEPARTED));

                    //"platform number"
                    string PN = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\PN.mp3");
                    audioPacket.Add(AudioElement.FromFile(PN));

                    //Platform Number
                    string PlatformNumber = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.PFNo}.mp3");
                    audioPacket.Add(AudioElement.FromFile(PlatformNumber));
                }

                //Rescheduled
                else if (train.StatusByte == 0x0F)
                {
                    //"has been rescheduled to"
                    string RESCHD = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\RESCHD.mp3");
                    audioPacket.Add(AudioElement.FromFile(RESCHD));

                    AddEnglishETD();

                    //"please contact the enquiry counter for any further details"
                    string ENQ = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ENQ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ENQ));

                }

                //Diverted
                else if (train.StatusByte == 0x10)
                {
                    //"has been diverted"
                    string DIVERTED = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\DIVERTED.mp3");
                    audioPacket.Add(AudioElement.FromFile(DIVERTED));

                    //"to"
                    string TO = string.Concat(audioFolderPath, "\\Announcements\\English\\Female\\TO.mp3");
                    audioPacket.Add(AudioElement.FromFile(TO));

                    //Diverted station name
                    string StationName = string.Concat(audioFolderPath, $"\\Stations\\{train.SplStationCode}\\English\\Female\\{train.SplStationCode}.mp3"); //It is actually female
                    audioPacket.Add(AudioElement.FromFile(StationName));

                    //"please contact the enquiry counter for any further details"
                    string ENQ = string.Concat(audioFolderPath, $"\\Announcements\\English\\Female\\ENQ.mp3");
                    audioPacket.Add(AudioElement.FromFile(ENQ));
                }

            }

            #endregion


        }
        #endregion


        public List<AudioElement> AudioSequenceBuilder(ActiveTrain Train, string WorkspaceFolderPath)
        {
            train = Train;
            workspaceFolderPath = WorkspaceFolderPath;

             audioPacket = new List<AudioElement>();

            audioFolderPath = string.Concat(workspaceFolderPath, "\\Audio");

            AddEnglishAnnouncement();
            AddHindiAnnouncement();
            AddPunjabiAnnouncement();

            return audioPacket;

        }

        //public async Task PlayAudioSequenceAsync(List<AudioElement> sequence, bool annInProgress)
        public async Task PlayAudioSequenceAsync( List<AudioElement> sequence,
                                                  AnnFormat SelectedFormat,

                                                  Func<bool> getPlayAudio,
                                                  Action<bool> setPlayAudio,

                                                  Func<bool> getPauseAudio,
                                                  Action<bool> setPauseAudio,

                                                  Func<bool> getStopAudio,
                                                  Action<bool> setStopAudio,

                                                  Func<bool> getMuteAudio,
                                                  Action<bool> setMuteAudio
            )
        { 
       


            selectedFormat =SelectedFormat;

            foreach (var element in sequence)
            {
                if (element.PauseDurationMs.HasValue)
                {
                    await Task.Delay(element.PauseDurationMs.Value);
                }
                else if (!string.IsNullOrEmpty(element.FilePath))
                {
                    await PlayMp3Async(element.FilePath);
                }

               // for stop
                if (getStopAudio() == true)
                    {
                        break;
                    }

                while (getPauseAudio())
                {
                    await Task.Delay(100); // async-friendly delay
                    if (getStopAudio() == true)
                    {
                        //break;
                        return;
                    }
                }
            }

            setStopAudio(true) ;
        }

        private Task PlayMp3Async(string filePath)
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                using var audioFile = new AudioFileReader(filePath);
                using var outputDevice = new WaveOutEvent();

                outputDevice.Init(audioFile);
                outputDevice.Play();

                outputDevice.PlaybackStopped += (s, e) =>
                {
                    tcs.TrySetResult(true);
                };

                // Wait until the playback finishes
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100); // small delay to prevent tight loop
                }
            });

            return tcs.Task;
        }

    }

}


////Expected Arrival Time HOUR
//if (train.ETA_Hours <= 12)
//{
//    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Hours}.mp3");
//    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));
//}
//else
//{
//    string ExpectedArrivalTimeHOUR = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{(train.ETA_Hours) - 12}.mp3");
//    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeHOUR));
//}


//if (train.ETA_Minutes != 00)
//{
//    //Expected Arrival Time MINUTES
//    string ExpectedArrivalTimeMINUTES = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\0{train.ETA_Minutes}.mp3");
//    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTES));
//}
//else
//{
//    //Expected Arrival Time MINUTES
//    string ExpectedArrivalTimeMINUTES = string.Concat(audioFolderPath, $"\\Time\\English\\Female\\{train.ETA_Minutes}.mp3");
//    audioPacket.Add(AudioElement.FromFile(ExpectedArrivalTimeMINUTES));
//}