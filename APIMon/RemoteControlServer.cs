using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIMonShared;
using APIMonLib;
using System.Runtime.Remoting;
using EasyHook;
using System.IO;
using System.Diagnostics;
using CPN;
using System.Threading;

namespace APIMon {
    public class RemoteControlServer : RemoteControlInterface {

        private BlockingQueue response_queue = new BlockingQueue(100);

        private static RemoteControlServer remote_control_server_instance = null;

        /// <summary>
        /// Reference to the single instance of TransferUnitReceiver class
        /// </summary>
        public static RemoteControlServer instance { get { return getInstance(); } }

        /// <summary>
        /// Returns single instance of this class in the system
        /// </summary>
        /// <returns></returns>
        private static RemoteControlServer getInstance() {
            initialize();
            return remote_control_server_instance;
        }

        /// <summary>
        /// Creates new instance of the server if it doesn't exist
        /// </summary>
        public static void initialize() {
            if (remote_control_server_instance == null) remote_control_server_instance = new RemoteControlServer();
        }

        const string REPORT_DIRECTORY_BASE = "C:\\Reports\\";
        const string MALICIOUS_LIST_FILE_NAME = "detected_malicious.txt";
        const string MALICOUS_LIST_FILE_PATH = REPORT_DIRECTORY_BASE + MALICIOUS_LIST_FILE_NAME;

        private static string createReportDirectory() {
            DateTime dt = DateTime.Now;
            string report_dir = REPORT_DIRECTORY_BASE + dt.Year + "." + dt.Month + "." + dt.Day + "." + dt.Hour + "." + +dt.Minute + "." + dt.Second + "\\";
            if (!Directory.Exists(report_dir)) {
                Directory.CreateDirectory(report_dir);
            }
            return report_dir;
        }

        private static string generateFileName(Process p) {
            string result = string.Empty;
            DateTime dt = DateTime.Now;
            result += dt.Minute + "." + dt.Second + "." + (int)dt.Millisecond / 100;
            result = p.MainModule.ModuleName + "_" + result + ".csv";
            return result;
        }

        private String channel_name = Configuration.REMOTE_CONTROL_CHANNEL_NAME;

        string report_directory = createReportDirectory();

        private RemoteControlServer() {
            RemoteHooking.IpcCreateServer<RemoteControlStsukoProxy>(
                ref channel_name, WellKnownObjectMode.SingleCall,
                new System.Security.Principal.WellKnownSidType[]{
                    System.Security.Principal.WellKnownSidType.BuiltinUsersSid
                                                                });
        }

        private class Experiment {
            private ProgramStartDescription _program_start_description = null;
            private MessageServer message_server = MessageServer.instance;
            string _report_directory = null;
            BlockingQueue _response_queue = null;
            public Experiment(ProgramStartDescription program_start_description, RemoteControlServer suka_ssilka_nah) {
                _program_start_description = new ProgramStartDescription(program_start_description);
                _report_directory = suka_ssilka_nah.report_directory;
                _response_queue = suka_ssilka_nah.response_queue;
            }

            public void run() {
                StreamWriter malicious_list_sw = new StreamWriter(new FileStream(MALICOUS_LIST_FILE_PATH, FileMode.Append));
                try {
                    try {
                        Process process = message_server.startProcessAndInject(_program_start_description);

                        string report_file_path = _report_directory + generateFileName(process);
                        StreamWriter sw = new StreamWriter(new FileStream(report_file_path, FileMode.CreateNew));
                        try {
                            Console.WriteLine("Waiting for process to end...");
                            bool result = message_server.waitForProcessToEnd(process, _program_start_description.max_running_time);
                            Console.WriteLine("Writting report to file: " + report_file_path);
                            #region Write  a report to file
                            message_server.waitForTheEndOfProcessing();
                            sw.WriteLine(_program_start_description);
                            Place.writeStatistics(Place.PrintLevel.Medium, sw);

                            //Check if executable exposed any detectable malicious functionality
                            // and write path to it into report file.
                            IEnumerable<Place> detection_places = Place.getDetectionPlaces();
                            foreach (Place place in detection_places) {
                                if (!place.isVirgin()) {
                                    Console.WriteLine("We have detected malicious activity " + place + " for this program");
                                    malicious_list_sw.WriteLine(_program_start_description.image_path);
                                    ProgramResponseDescription response = new ProgramResponseDescription(_program_start_description);
                                    response.desciption = "Detected " + place + " functionality";
                                    _response_queue.Enqueue(response);
                                    break;
                                }
                            }
                            malicious_list_sw.Flush();
                            sw.WriteLine(result ? "Exited" : "Killed");
                            sw.Flush();
                        } catch (Exception e) {
                            Console.WriteLine("APIMonMain.runExperiment Error while processing");
                            Console.WriteLine(e);
                        } finally {
                            sw.Close();
                        }
                            #endregion
                        //cleaning up
                        Place.clearAllPlaces();
                        System.GC.Collect();
                    } catch (Exception ExtInfo) {
                        Console.WriteLine("There was an error while running target: " + _program_start_description.image_path + "\r\n{0}", ExtInfo.ToString());
                        //throw ExtInfo;
                    }

                } finally {
                    malicious_list_sw.Close();
                }
            }
        }


        private void shutdownJob() {
            Thread.Sleep(1000);
            MessageServer.instance.closeEnvironment();
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        #region RemoteControlInterface Members

        public void launchProgram(ProgramStartDescription start_description) {
            Experiment experiment = new Experiment(start_description, this);
            ThreadStart job = new ThreadStart(experiment.run);
            Thread thread = new Thread(job);
            thread.Start();
        }

        public void ping() {
            Console.Write("...");
        }


        public void exit() {
            ThreadStart job = new ThreadStart(shutdownJob);
            Thread thread = new Thread(job);
            thread.Start();
        }

        /// <summary>
        /// Tries to create a block of responses
        /// </summary>
        /// <returns>Queue of responses. Queue might be empty</returns>
        private Queue<ProgramResponseDescription> getBlockToSend() {
            //check how many tu-s we have
            int queue_count = response_queue.Count;
            Queue<ProgramResponseDescription> response_block = new Queue<ProgramResponseDescription>(queue_count + 1);
            for (int i = 0; i < queue_count; i++) {
                //we don't expect it to block here since it is guaranteed that we have enough tu-s
                ProgramResponseDescription response = (ProgramResponseDescription)response_queue.Dequeue();
                //Console.WriteLine("Response prepared ID=" + response.id + "\n" + response);
                response_block.Enqueue(response);
            }
            return response_block;
        }

        public ProgramResponseDescription[] getLaunchResults() {
            //Console.WriteLine("Results requested");
            ProgramResponseDescription[] r=getBlockToSend().ToArray();
            return r;
        }

        public bool isLaunchResultsAvailable() {
            bool result = (response_queue.Count != 0);
            //Console.WriteLine("isLaucnhResultsAvailable="+result);
            return result;
        }

        #endregion
    }
}
