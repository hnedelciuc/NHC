/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2020 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Application entry point.
/********************************************************************************************************************/

using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;

namespace Crypt
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            HelperService.compressionLevel = System.IO.Compression.CompressionLevel.Fastest;

            HelperService.ParseArgs(args);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            HelperService.AppInitialization1();

            do
            {
                HelperService.AppInitialization2();
                try
                {
                    Application.Run(new CryptForm());
                }
                catch (Exception ex)
                {
                    var newLn = Environment.NewLine + Environment.NewLine;
                    var retryCancelMsg = "Click 'Retry' if you want the application to attempt to ignore the problem and stay alive. Clicking 'Cancel' will exit the application immediately.";

                    if (MessageBox.Show("An error has occured." + newLn + ex.Message + newLn + (ex.InnerException == null ? retryCancelMsg : ex.InnerException.Message + newLn + retryCancelMsg),
                        "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                        HelperService.exitOnClose = false;
                    else Environment.Exit(1);
                }
            }
            while (!HelperService.exitOnClose);
        }
    }

    internal delegate void ProgressDelegate(int CurrentValue);
    internal delegate void CurrentFileProcessedDelegate(string currentFileProcessed);
    internal delegate void NumberOfEntriesProcessedDelegate(int numberOfFilesExtracted);
    internal delegate void CurrentDateTimeDelegate(DateTime currentDateTime);
}

/********************************************************************************************************************/
