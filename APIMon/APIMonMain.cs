using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using EasyHook;
using APIMonLib;
using APIMonShared;
using System.Diagnostics;
using System.Threading;

using CPN;
using System.IO.Pipes;
using System.IO;
using System.Reflection;

namespace APIMon {
	class APIMonMain {

		static char requestKey() {
			return Console.ReadKey(true).KeyChar;
		}

		private static Func<int, string, string> expand = (num, what) => num <= 0 ? what : expand(num - 1, what) + what;

		private CPNetBuilder cpn_builder = new CPNetBuilder();
		private MessageServer message_server = MessageServer.instance;

		private static string generateFileName(Process p) {
			string result = string.Empty;
			DateTime dt = DateTime.Now;
			result += dt.Minute + "." + dt.Second + "." + (int)dt.Millisecond / 100;
			result = p.MainModule.ModuleName + "_" + result + ".csv";
			return result;
		}

		const string REPORT_DIRECTORY_BASE = "C:\\Reports\\";

		private static string createReportDirectory() {
			DateTime dt = DateTime.Now;
			string report_dir = REPORT_DIRECTORY_BASE + dt.Year + "." + dt.Month + "." + dt.Day + "."+dt.Hour+"." + +dt.Minute + "." + dt.Second + "\\";
			if (!Directory.Exists(report_dir)) {
				Directory.CreateDirectory(report_dir);
			}
			return report_dir;
		}

        const string MALICIOUS_LIST_FILE_NAME = "detected_malicious.txt";

        const string MALICOUS_LIST_FILE_PATH = REPORT_DIRECTORY_BASE + MALICIOUS_LIST_FILE_NAME;

		private void ThreadJob() {
			bool do_exit = false;
			while (!do_exit) {
				Thread.Sleep(100);
				switch (requestKey()) {
					case 'q': { do_exit = true; break; }
					case 'p': {
							Console.WriteLine("(g)eneral or (s)pecial " + expand(2, "=>"));
							switch (requestKey()) {
								case 'g': { Place.printStatistics(Place.PrintLevel.Low); break; }
								case 's': {
										Console.WriteLine("Enter the place id to print:");
										string str_id = Console.ReadLine();
										try {
											int place_id = int.Parse(str_id);
											foreach (Place place in Place.places_created) {
												if (place.id == place_id) {
													Console.WriteLine("\n Report for place: " + place.name_);
													Console.WriteLine(place.toString());
												}
											}
										} catch {
										}
										break;
									}
							}
							break;
						}
				}
			}
		}

		private Thread startUIThread() {
			ThreadStart job = new ThreadStart(ThreadJob);
			Thread thread = new Thread(job);
			thread.Start();
            return thread;
		}


		private void runExperiment(ProgramStartList descr_list) {
			string report_directory = createReportDirectory();

            StreamWriter malicious_list_sw = new StreamWriter(new FileStream(MALICOUS_LIST_FILE_PATH, FileMode.Append)); 
            try {
                foreach (ProgramStartDescription program_start_decr in descr_list) {
                    try {
                        Process process = message_server.startProcessAndInject(program_start_decr);

                        string report_file_path = report_directory + generateFileName(process);
                        StreamWriter sw = new StreamWriter(new FileStream(report_file_path, FileMode.CreateNew));
                        try {
                            Console.WriteLine("Waiting for process to end...");
                            bool result = message_server.waitForProcessToEnd(process, program_start_decr.max_running_time);
                            Console.WriteLine("Writting report to file: " + report_file_path);
                            #region Write  a report to file
                            message_server.waitForTheEndOfProcessing();
                            sw.WriteLine(program_start_decr);
                            Place.writeStatistics(Place.PrintLevel.Medium, sw);

                            //Check if executable exposed any detectable malicious functionality
                            // and write path to it into report file.
                            IEnumerable<Place> detection_places = Place.getDetectionPlaces();
                            foreach (Place place in detection_places) {
                                if (!place.isVirgin()) {
                                    Console.WriteLine("We have detected something for the program launched");
                                    malicious_list_sw.WriteLine(program_start_decr.image_path);
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
                        Console.WriteLine("There was an error while running target: " + program_start_decr.image_path + "\r\n{0}", ExtInfo.ToString());
                        //throw ExtInfo;
                    }
                }
            } finally {
                malicious_list_sw.Close();
            }
		}


        private void MainAutoStartPorcesses() {
            const int max_running_time = 20;

            Thread thread = startUIThread();

            runExperiment(malwareStartList(max_running_time));
            Console.WriteLine("\nPress q to finish.");
            thread.Join();
            message_server.closeEnvironment();
            Environment.Exit(0);
        }

        private void MainNoProcessStart() {
            message_server.watchProcesses(new string[] { 
														"msimn", 
														"smallftpd", 
														"Test_AcceptCon", 
														"Test_CreateProcess", 
														"BindShell_Pipe", 
														"iexplore", 
														"opera", 
														"far", 
													    "Test_Files_Handles"
														//,"cmd" 
														},
                                            false,
                                            null
                                        );
            ThreadJob();
        }

        /// <summary>
        /// This method launches remote controlled session s.t. launch operations are controlled by remote program through RemoteControlInterface
        /// </summary>
        private void RemoteControlledStartProcesses() {
            Thread thread = startUIThread();
            RemoteControlServer.initialize();            
        }

        /// <summary>
        /// This method of launch starts processes automatically according to the ProgramStartList provided
        /// </summary>
        private void MainAutoStartPorcesses(ProgramStartList start_list) {
            Thread thread = startUIThread();
            runExperiment(start_list);
            Console.WriteLine("\nPress q to finish.");
            thread.Join();
            message_server.closeEnvironment();
            Environment.Exit(0);
        }

        /// <summary>
        /// Work begins from here. We create CPN and initialize environment.
        /// </summary>
        public APIMonMain() {
            cpn_builder.assembleCPN();
            message_server.initEnvironment();
            //AssemblyName an = AssemblyName.GetAssemblyName(".\\AlmostNativeHooks.dll");
            //System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(an);
            //System.Reflection.Assembly assembly_1 = System.Reflection.Assembly.Load("AlmostNativeHooks, Version=1.0.1.1, Culture=neutral, PublicKeyToken=d34a061f079be347");
        }

		static void Main(string[] args) {

			APIMonMain api_mon = new APIMonMain();
			if (args.Length >= 1) {
				if (args[0].Equals("/noprocess")) {
					api_mon.MainNoProcessStart();
					return;
				}
                if ((args.Length >= 1) && (args[0].Equals("/legal"))) {
                    const int max_running_time = 20;
                    api_mon.MainAutoStartPorcesses(windowsProgramStartList(max_running_time));
                    return;
                }
                if ((args.Length >= 1) && (args[0].Equals("/malware"))) {
                    const int max_running_time = 20;
                    api_mon.MainAutoStartPorcesses(malwareStartList(max_running_time));
                    return;
                }
                if ((args.Length >= 2)&&(args[0].Equals("/remote"))){//remotely controlled instance
                    api_mon.RemoteControlledStartProcesses();
                    return;
                }
			}
            api_mon.RemoteControlledStartProcesses();

		}

        static ProgramStartList windowsProgramStartList(int max_running_time)
        {
            ProgramStartList descr_list = new ProgramStartList();
            #region Program start description for windows programs cmd msimn etc
            descr_list.last().image_dir = "C:\\Windows\\system32";
            descr_list.last().image_filename = "cmd.exe";
            descr_list.last().command_line = " /c dir";
            descr_list.last().max_running_time = max_running_time;
            descr_list += " /c dir c:\\Windows\\system32";
            descr_list += " /c dir c:\\Windows";
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\Outlook Express";
            descr_list.last().image_filename = "msimn.exe";
            descr_list.last().command_line = "";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\Internet Explorer";
            descr_list.last().image_filename = "iexplore.exe";
            descr_list.last().command_line = " http://bbc.com";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\Opera";
            descr_list.last().image_filename = "opera.exe";
            descr_list.last().command_line = " http://bbc.com";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\Far";
            descr_list.last().image_filename = "far.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\Messenger";
            descr_list.last().image_filename = "msmsgs.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\Movie Maker";
            descr_list.last().image_filename = "moviemk.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\MSN\\MSNCoreFiles";
            descr_list.last().image_filename = "msn6.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\Windows Media Player";
            descr_list.last().image_filename = "wmplayer.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Program Files\\WinRar";
            descr_list.last().image_filename = "WinRar.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Windows";
            descr_list.last().image_filename = "explorer.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += descr_list.last().getExecutables();
            descr_list += new ProgramStartDescription();
            descr_list.last().image_dir = "C:\\Windows\\system32";
            descr_list.last().image_filename = "accwiz.exe";
            descr_list.last().command_line = " ";
            descr_list.last().max_running_time = max_running_time;
            descr_list += descr_list.last().getExecutables();
            //descr_list += new ProgramStartDescription();
            //descr_list.last().image_dir = "C:\\Documents and Settings\\amd\\Desktop";
            //descr_list.last().image_filename = "Test_Files_Handles.exe";
            ////descr_list.last().command_line = " 50 10000";
            //descr_list.last().command_line = " ";
            //descr_list.last().max_running_time = 60 * 4; 
            #endregion
            return descr_list;
        }

        static ProgramStartList malwareStartList(int max_running_time)
        {
            ProgramStartList descr_list;
            
            #region Program start description for windows programs cmd msimn etc
            descr_list = new ProgramStartList();
            descr_list.last().image_dir = "Z:\\Malware\\Launch\\Virus.Win32.Parite.a";
            descr_list.last().image_filename = "Virus.Win32.Parite.a_spooIsv.exe";
            descr_list.last().max_running_time = max_running_time;
            descr_list += ProgramStartDescription.findExecutablesRecursive("z:\\Malware\\Launch");

            //descr_list.last().command_line = "";

            //descr_list += new ProgramStartDescription();
            //descr_list.last().image_dir = "C:\\Malware\\Virus.Win32.Virut.av";
            //descr_list.last().image_filename = "explorer.exe";
            //descr_list.last().command_line = "";

            //descr_list.last().image_dir = "C:\\Malware\\Worm.Win32.AutoRun.afdh";
            //descr_list.last().image_filename = "sEtuP.exe";
            //descr_list.last().command_line = "";
            //descr_list.last().max_running_time = max_running_time;
            //descr_list += new ProgramStartDescription();
            //descr_list.last().image_dir = "C:\\Program Files\\Internet Explorer";
            //descr_list.last().image_filename = "iexplore.exe";
            //descr_list.last().command_line = " http://bbc.com";
            //descr_list.last().max_running_time = max_running_time;
            #endregion
            return descr_list;
        }

	}
}




#region Commented out

//AnonymousPipeServerStream forStdIn = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
//AnonymousPipeServerStream forStdOut = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
//AnonymousPipeServerStream forStdErr = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

//Process process = message_server.startProcessAndInject("C:\\Documents and Settings\\amd\\Desktop\\Test_Files_Handles.exe", " 10 10000 50",
//    forStdIn.ClientSafePipeHandle.DangerousGetHandle(), forStdOut.ClientSafePipeHandle.DangerousGetHandle(), forStdErr.ClientSafePipeHandle.DangerousGetHandle());

//Process process = message_server.startProcessAndInject("C:\\Windows\\system32\\mspaint.exe", " ",
//    forStdIn.ClientSafePipeHandle.DangerousGetHandle(), forStdOut.ClientSafePipeHandle.DangerousGetHandle(), forStdErr.ClientSafePipeHandle.DangerousGetHandle());

//Process process = message_server.startProcessAndInject("C:\\Windows\\system32\\netstat.exe", " -ano",
//    forStdIn.ClientSafePipeHandle.DangerousGetHandle(), forStdOut.ClientSafePipeHandle.DangerousGetHandle(), forStdErr.ClientSafePipeHandle.DangerousGetHandle());


//Important:
//We need to close local handles. In that way when child process closes pipe we get EOF. If we don't close local child handles we never get EOF and the last read blocks forever
//forStdIn.DisposeLocalCopyOfClientHandle();
//forStdOut.DisposeLocalCopyOfClientHandle();
//forStdErr.DisposeLocalCopyOfClientHandle();

//StreamWriter swr = new StreamWriter(forStdIn);
//StreamReader srd = new StreamReader(forStdOut);
//StreamReader srderr = new StreamReader(forStdErr);



//Other type of code


//Process process = new Process();
//ProcessStartInfo psi = new ProcessStartInfo();
//psi.FileName = "C:\\Windows\\system32\\cmd.exe";
//psi.Arguments = " ";
////psi.RedirectStandardError = true;
////psi.RedirectStandardInput = true;
////psi.RedirectStandardOutput = true;
//psi.UseShellExecute = false;
//psi.CreateNoWindow = true;
//process.StartInfo = psi;
//process.EnableRaisingEvents = false;
//process.Start();

//message_server.injectLibrary(process); 
#endregion