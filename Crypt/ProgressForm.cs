//********************************************************************************************************************//
// Needle in a Haystack in a Crypt v1.0.
// Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
//********************************************************************************************************************//
// Progress bar / result window.
//********************************************************************************************************************//

using GracefulDynamicDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Crypt
{
    internal partial class ProgressForm : Form
    {
        /********************************************************************************************************************/
        internal ProgressForm(HelperService.CryptionOptions cryptionOptions, HelperService.CryptionAlgorithm cryptionAlgorithm, ArrayList items, CompressionLevel compressionLevel, string outputFileName, string keyFileName, long splitArchiveSize, string password = null)
        {
            InitializeComponent();
            SetWindowColor();
            this.cryptionOptions = cryptionOptions;
            this.cryptionAlgorithm = cryptionAlgorithm;
            this.items = items;
            this.compressionLevel = compressionLevel;
            this.outputFileName = outputFileName;
            this.keyFileName = keyFileName;
            this.splitArchiveSize = splitArchiveSize;
            this.password = password;
        }

        /********************************************************************************************************************/

        internal ProgressForm(HelperService.CryptionOptions cryptionOptions, HelperService.CryptionAlgorithm cryptionAlgorithm, ArrayList items, string[] itemsToBeExtracted, string[] itemsToBeExtractedCorrespondingArchive, bool? extractAll, HelperService.OverwriteFilesSetting overwriteFilesSetting, CompressionLevel compressionLevel, string outputFileName, string keyFileName, long splitArchiveSize, string password = null)
        {
            InitializeComponent();
            SetWindowColor();
            this.cryptionOptions = cryptionOptions;
            this.cryptionAlgorithm = cryptionAlgorithm;
            this.compressionLevel = compressionLevel;
            this.items = items;
            this.itemsToBeExtracted = itemsToBeExtracted;
            this.itemsToBeExtractedCorrespondingArchive = itemsToBeExtractedCorrespondingArchive;
            this.extractAll = extractAll;
            this.overwriteFilesSetting = overwriteFilesSetting;
            this.outputFileName = outputFileName;
            this.keyFileName = keyFileName;
            this.splitArchiveSize = splitArchiveSize;
            this.password = password;
        }

        /********************************************************************************************************************/

        // Incoming parameter variables
        private HelperService.CryptionOptions cryptionOptions;
        private HelperService.CryptionAlgorithm cryptionAlgorithm;
        private ArrayList items;
        private string[] itemsToBeExtracted;
        private string[] itemsToBeExtractedCorrespondingArchive;
        private bool? extractAll;
        private HelperService.OverwriteFilesSetting overwriteFilesSetting;
        private string outputFileName;
        private string keyFileName;

        /********************************************************************************************************************/

        // Local variables
        private bool workerIsDone = false;
        private byte[] keyByte;
        private byte[] keyArray;
        private byte[] IV;
        private long splitArchiveSize;
        private string loadKeyResult = String.Empty;
        private string inputFileName = String.Empty;
        private string savedFileName = String.Empty;
        private string resultMessage = String.Empty;
        private bool compressEncryptSuccess = false;
        private int decryptCount = 0;
        private int encryptCount = 0;
        private string currentFileProcessed = String.Empty;
        private long numberOfEntriesProcessed = 0;
        private DateTime startDateTime;
        private DateTime currentDateTime;
        private string password;
        private string tempDirectory = null;
        private CompressionLevel compressionLevel;

        /********************************************************************************************************************/

        private void Progress_Load(object sender, System.EventArgs e)
        {
            if (!worker.IsBusy && !workerIsDone)
            {
                worker.RunWorkerAsync();
            }
            if (cryptionOptions == HelperService.CryptionOptions.Encrypt)
            {
                labelCompressEncrypt.Text = "Creating Archive...";
            }
            else if (cryptionOptions == HelperService.CryptionOptions.Decrypt)
            {
                labelCompressEncrypt.Text = "Extracting Archive...";
            }
            else if (cryptionOptions == HelperService.CryptionOptions.Update)
            {
                labelCompressEncrypt.Text = "Updating Archive Contents...";
            }
        }

        /********************************************************************************************************************/

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (cryptionOptions)
            {
                case HelperService.CryptionOptions.Encrypt:
                    {
                        startDateTime = DateTime.Now;

                        CryptionService.LoadKey(null, out keyArray, out IV, HelperService.CryptionAlgorithm.NeedleCryptKey, password);
                        loadKeyResult = CryptionService.LoadKey(keyFileName, out keyByte, out IV, HelperService.cryptionAlgorithm, password);

                        if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey ||
                            cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
                            keyArray = keyByte;

                        if (loadKeyResult != "Success")
                        {
                            resultMessage = "There was an error while loading cryption key file." +
                                Environment.NewLine + Environment.NewLine + loadKeyResult;
                            workerIsDone = true;
                        }
                        else
                        {
                            try
                            {
                                if (File.Exists(outputFileName))
                                {
                                    File.Delete(outputFileName);
                                }

                                MainService.CompressEncryptFiles(
                                    items,
                                    outputFileName,
                                    HelperService.cryptionAlgorithm,
                                    compressionLevel,
                                    password,
                                    keyByte,
                                    keyArray,
                                    IV,
                                    splitArchiveSize,
                                    (current) => worker.ReportProgress(current),
                                    (current) => currentFileProcessed = current,
                                    (nr) => ++numberOfEntriesProcessed,
                                    (current) => currentDateTime = current
                                    );

                                if (HelperService.backgroundWorkerClosePending)
                                    throw new Exception("The task was canceled.");

                                worker.ReportProgress(100);

                                compressEncryptSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                resultMessage = "There was an error during archiving." +
                                    Environment.NewLine + Environment.NewLine + ex.Message;
                                compressEncryptSuccess = false;
                            }
                            finally
                            {
                                workerIsDone = true;
                            }


                            if (compressEncryptSuccess == true)
                            {
                                // Displays the Success Message.
                                resultMessage = "Success" + Environment.NewLine + Environment.NewLine +
                                    items.Count + " entries have been archived successfully.";
                            }
                            else
                            {
                                // Displays the Fail Message.
                                resultMessage = "Fail" + Environment.NewLine + Environment.NewLine +
                                    $"{numberOfEntriesProcessed} out of {items.Count} entries have been archived." +
                                    Environment.NewLine + Environment.NewLine + resultMessage;
                            }

                        }

                        break;
                    }

                case HelperService.CryptionOptions.Decrypt:
                    {
                        startDateTime = DateTime.Now;
                        HelperService.totalGlobalPos = 0;
                        var itemsList = new List<dynamic>();

                        try
                        {
                            bool isNHCArchive;

                            // filtering out items that are not NHC archives
                            for (dynamic i = 0; i < items.Count; i++)
                            {
                                try { isNHCArchive = items[i].cryptionAlgorithm != null; }
                                catch { isNHCArchive = false; }

                                if (isNHCArchive) { itemsList.Add(items[i]); }
                            }

                            for (int i = 0; i < itemsList.Count; i++)
                            {
                                dynamic item = itemsList[i];
                                inputFileName = item.fullPath;
                                

                                CryptionService.LoadKey(null, out keyArray, out IV, HelperService.CryptionAlgorithm.NeedleCryptKey, item.password);
                                loadKeyResult = CryptionService.LoadKey(item.keyFilePath, out keyByte, out IV, item.cryptionAlgorithm, item.password);

                                if (item.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey ||
                                    item.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
                                    keyArray = keyByte;

                                if (loadKeyResult != "Success")
                                {
                                    resultMessage = "There was an error while loading cryption key file." +
                                        Environment.NewLine + Environment.NewLine + loadKeyResult;
                                    workerIsDone = true;
                                }
                                else
                                {
                                    MainService.DecompressDecryptArchive(
                                            outputFileName,
                                            inputFileName,
                                            itemsToBeExtracted,
                                            itemsToBeExtractedCorrespondingArchive,
                                            extractAll,
                                            keyByte,
                                            keyArray,
                                            IV,
                                            (string)item.password,
                                            (HelperService.CryptionAlgorithm)item.cryptionAlgorithm,
                                            overwriteFilesSetting,
                                            false,
                                            (current) => worker.ReportProgress(current),
                                            (current) => currentFileProcessed = current,
                                            (nr) => numberOfEntriesProcessed = nr,
                                            items.Count,
                                            i,
                                            (current) => currentDateTime = current
                                            );

                                    if (HelperService.backgroundWorkerClosePending)
                                        throw new Exception("The task was canceled.");

                                    decryptCount++;
                                }

                                worker.ReportProgress(100);
                            }

                        }
                        catch (InvalidDataException ex)
                        {
                            resultMessage = "The file " + inputFileName +
                                    " is either not a valid .NHC file, or is corrupted, or an incorrect key or password was provided." +
                                Environment.NewLine + Environment.NewLine + ex.Message;
                        }
                        catch (Exception ex)
                        {
                            resultMessage = "The file " + inputFileName + " could not be extracted." +
                                Environment.NewLine + Environment.NewLine + ex.Message;
                        }
                        finally
                        {
                            workerIsDone = true;
                        }

                        if (decryptCount == itemsList.Count && !HelperService.backgroundWorkerClosePending)
                        {
                            // Displays the Success Message.
                            resultMessage = "Success" + Environment.NewLine + Environment.NewLine +
                                itemsList.Count + " archived files have been extracted successfully. " +
                                Environment.NewLine + Environment.NewLine + numberOfEntriesProcessed +
                                " entries have been extracted.";
                        }
                        else
                        if (decryptCount > 0)
                        {
                            // Displays the Success Message.
                            resultMessage = "Partial Success / Partial Fail" + Environment.NewLine + Environment.NewLine +
                                decryptCount + " archived files have been extracted successfully, while " +
                                (itemsList.Count - decryptCount) + " have failed, and the failure occured at " +
                                inputFileName + " file." + Environment.NewLine + Environment.NewLine +
                                resultMessage + Environment.NewLine + Environment.NewLine +
                                numberOfEntriesProcessed + " entries have been extracted.";
                        }
                        else
                        {
                            resultMessage = "Fail" + Environment.NewLine + Environment.NewLine +
                                "Extracting entries has stopped." +
                            Environment.NewLine + Environment.NewLine + resultMessage +
                            Environment.NewLine + Environment.NewLine + numberOfEntriesProcessed + " entries have been extracted.";
                        }

                        HelperService.selectedPaths.Clear();
                        HelperService.entriesProcessed.Clear();
                        HelperService.numberOfEntriesProcessed = 0;
                        numberOfEntriesProcessed = 0;

                        break;
                    }

                case HelperService.CryptionOptions.Update:
                    {
                        startDateTime = DateTime.Now;
                        HelperService.totalGlobalPos = 0;

                        try
                        {
                            for (int i = 0; i < items.Count; i++)
                            {
                                tempDirectory = ((dynamic)HelperService.selectedPaths[0]).fullPath.Replace("\\" + ((dynamic)HelperService.selectedPaths[0]).relativePath, string.Empty);

                                dynamic item = items[i];
                                inputFileName = item.fullPath;
                                bool isNHCArchive = false;
                                string outputDirectory;

                                try { isNHCArchive = item.cryptionAlgorithm != null; } catch { }

                                if (!isNHCArchive)
                                {
                                    outputDirectory = LongDirectory.GetDirectoryName(inputFileName);
                                    HelperService.selectedPaths.Add(new DDict(new Dictionary<string, dynamic>()
                                    {
                                        { "relativePath", item.relativePath },
                                        { "fullPath", item.fullPath },
                                        { "isDirectory", item.isDirectory },
                                        { "currentArchiveName", "" }
                                    }));
                                    continue;
                                }
                                else outputDirectory = tempDirectory.Clone().ToString();

                                CryptionService.LoadKey(null, out keyArray, out IV, HelperService.CryptionAlgorithm.NeedleCryptKey, item.password);
                                loadKeyResult = CryptionService.LoadKey(item.keyFilePath, out keyByte, out IV, item.cryptionAlgorithm, item.password);

                                if (item.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey ||
                                    item.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
                                    keyArray = keyByte;

                                if (loadKeyResult != "Success")
                                {
                                    resultMessage = "There was an error while loading cryption key file." +
                                        Environment.NewLine + Environment.NewLine + loadKeyResult;
                                    workerIsDone = true;
                                }
                                else
                                {
                                    MainService.DecompressDecryptArchive(
                                            outputDirectory,
                                            inputFileName,
                                            itemsToBeExtracted,
                                            itemsToBeExtractedCorrespondingArchive,
                                            extractAll,
                                            keyByte,
                                            keyArray,
                                            IV,
                                            (string)item.password,
                                            (HelperService.CryptionAlgorithm)item.cryptionAlgorithm,
                                            overwriteFilesSetting,
                                            true,
                                            (current) => worker.ReportProgress(current / 2),
                                            (current) => currentFileProcessed = current,
                                            (nr) =>  numberOfEntriesProcessed = nr - 1, // -1 because the temp folder is counted too, but we don't need to reflect that here
                                            items.Count,
                                            i,
                                            (current) => currentDateTime = current
                                            );

                                    if (HelperService.backgroundWorkerClosePending)
                                        throw new Exception("The task was canceled.");

                                    decryptCount++;
                                }
                            }

                            numberOfEntriesProcessed = 0;
                            HelperService.numberOfEntriesProcessed = 0;

                            if (LongFile.Exists(outputFileName))
                            {
                                LongFile.Delete(outputFileName);
                            }

                            CryptionService.LoadKey(null, out keyArray, out IV, HelperService.CryptionAlgorithm.NeedleCryptKey, password);
                            loadKeyResult = CryptionService.LoadKey(keyFileName, out keyByte, out IV, cryptionAlgorithm, password);

                            if (cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey ||
                                cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
                                keyArray = keyByte;

                            if (loadKeyResult != "Success")
                            {
                                resultMessage = "There was an error while loading cryption key file." +
                                    Environment.NewLine + Environment.NewLine + loadKeyResult;
                                workerIsDone = true;
                            }
                            else
                            {
                                MainService.CompressEncryptFiles(
                                HelperService.selectedPaths,
                                outputFileName,
                                cryptionAlgorithm,
                                compressionLevel,
                                password,
                                keyByte,
                                keyArray,
                                IV,
                                splitArchiveSize,
                                (current) => worker.ReportProgress(50 + (current / 2)),
                                (current) => currentFileProcessed = current,
                                (nr) => numberOfEntriesProcessed = nr,
                                (current) => currentDateTime = current
                                );

                                if (HelperService.backgroundWorkerClosePending)
                                    throw new Exception("The task was canceled.");

                                worker.ReportProgress(100);

                                encryptCount = HelperService.selectedPaths.ToArray().Count(p => ((dynamic)p).currentArchiveName == ""); // if no exception thrown thus far, then all these files have been encrypted
                            }
                        }
                        catch (InvalidDataException ex)
                        {
                            resultMessage = "The file " + inputFileName +
                                    " is either not a valid .NHC file, or is corrupted, or an incorrect key or password was provided." +
                                Environment.NewLine + Environment.NewLine + ex.Message;
                        }
                        catch (Exception ex)
                        {
                            resultMessage = ex.Message;
                        }
                        finally
                        {
                            try { LongDirectory.Delete(tempDirectory, true); } catch { }

                            workerIsDone = true;
                        }

                        if (string.IsNullOrEmpty(resultMessage) && (encryptCount + decryptCount) == items.Count && !HelperService.backgroundWorkerClosePending)
                        {
                            // Displays the Success Message.
                            resultMessage = "Success" + Environment.NewLine + Environment.NewLine +
                                outputFileName + " archive has been updated successfully. " +
                                Environment.NewLine + Environment.NewLine + numberOfEntriesProcessed +
                                " entries have been added.";
                        }
                        else
                        {
                            resultMessage = "Fail" + Environment.NewLine + Environment.NewLine +
                                "Could not update " + outputFileName + " archive." +
                            Environment.NewLine + Environment.NewLine + resultMessage +
                            Environment.NewLine + Environment.NewLine + numberOfEntriesProcessed + " entries have been added.";
                        }

                        HelperService.selectedPaths.Clear();
                        HelperService.entriesProcessed.Clear();
                        HelperService.numberOfEntriesProcessed = 0;
                        numberOfEntriesProcessed = 0;

                        break;
                    }
            }
        }

        /********************************************************************************************************************/

        private void worker_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            progressBar1.Value = e.ProgressPercentage > 100 ? 99 : e.ProgressPercentage;
            // Set the text.
            this.Text = "Progress - " + e.ProgressPercentage.ToString() + "%";
            labelPercentage.Text = e.ProgressPercentage.ToString() + "%";
            labelResultMessage.Text = currentFileProcessed;
            
            TimeSpan timeDifference = currentDateTime.Subtract(startDateTime);

            StringBuilder elapsedTime = new StringBuilder();
            elapsedTime.Append("Elapsed time: ");
            if (timeDifference.Days > 0) { elapsedTime.Append(timeDifference.Days + " days, "
                + timeDifference.Hours + " hours, " + timeDifference.Minutes + " minutes, "); }
            else
            if (timeDifference.Hours > 0) { elapsedTime.Append(timeDifference.Hours + " hours, "
                + timeDifference.Minutes + " minutes, "); }
            else
            if (timeDifference.Minutes > 0) { elapsedTime.Append(timeDifference.Minutes + " minutes, "); }
            elapsedTime.Append(timeDifference.Seconds + " seconds");

            labelElapsedTime.Text = elapsedTime.ToString();
        }

        /********************************************************************************************************************/

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            workerIsDone = true;
            btnClose.Visible = true;
            labelResultMessage.Text = resultMessage;
            this.Size = new Size(380 * HelperService.scaling / 100, tableLayoutPanelProgress.Size.Height + 110 * HelperService.scaling / 100);
            HelperService.backgroundWorkerClosePending = false;
        }

        /********************************************************************************************************************/

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (worker.IsBusy)
            {
                HelperService.backgroundWorkerClosePending = true;
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }

        /********************************************************************************************************************/

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /********************************************************************************************************************/

        private void SetWindowColor()
        {
            switch (HelperService.windowColorSetting)
            {
                case HelperService.WindowColorSetting.LightSteelBlue:
                    this.BackColor = System.Drawing.Color.LightSteelBlue;
                    break;
                case HelperService.WindowColorSetting.DarkGray:
                    this.BackColor = System.Drawing.Color.DarkGray;
                    break;
                case HelperService.WindowColorSetting.White:
                    this.BackColor = System.Drawing.Color.White;
                    break;
                default:
                    this.BackColor = System.Drawing.Color.LightSteelBlue;
                    break;
            }
        }
    }
}

/********************************************************************************************************************/
