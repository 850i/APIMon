using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIMonLib;
using APIMonLib.Hooks.ntdll.dll;
using APIMonLib.Hooks.kernel32.dll;
using APIMonLib.Hooks.ws2_32.dll;
using APIMonLib.Hooks;
using CPN;
using System.Diagnostics;

namespace APIMon {
	public class CPNetBuilder {

		#region Useful functions defenitions
		public static Func<object, string> to_string = CPNetBlocks.to_string;
		public static Func<object, int> to_int = CPNetBlocks.to_int;
		public static Func<object, string, bool> begins_with_string = CPNetBlocks.begins_with_string;
		public static Func<object, string, bool> ends_with_string = CPNetBlocks.ends_with_string;
		public static Func<object, object, bool> strings_equal_ignore_case = CPNetBlocks.strings_equal_ignore_case;
		public static Func<object, string> get_file_extention = CPNetBlocks.get_file_extention;

		public Func<Token, Token, bool> EQUAL(string field_name_1, string field_name_2) {
			return (token1, token2) => token1[field_name_1].Equals(token2[field_name_2]);
		}

		public Func<Token, Token, bool> FIRST_ENDS_WITH_SECOND_STRING(string field_name_1, string field_name_2) {
			return (token1, token2) => ends_with_string(token1[field_name_1], to_string(token2[field_name_2]));
		}

		public Func<Tuple, Token> STANDARD_GENEXP(Place base_place) {
			return tuple =>
			{
				Token result = new Token(tuple[base_place]);
				result.prevTuple = tuple;
				return result;
			};
		}

		public Func<Tuple, Token> SIMPLE_GENEXP() {
			return tuple =>
			{
				Token result = new Token();
				result.prevTuple = tuple;
				return result;
			};
		}

		//private bool value_in_set

		private Func<Token, bool> TRUE = token => true;
		private Func<Token, Token, bool> TRUE_2 = (token1, token2) => true;

		#endregion

        public void injectLibraryIntoFollowupProcess(Place place, Token token) {
            Process p = Process.GetProcessById((int)token[Hook_CreateProcessInternalW.Color.ProcessId]);
            MessageServer.instance.injectLibrary(p, (int)token[Hook_CreateProcessInternalW.Color.FirstThreadId]);
        }

		public void assembleCPN() {

			#region API places definition
            //ApiPlace ldr_load_dll = new ApiPlace(typeof(Hook_LdrLoadDll));

			#region Object managment API
			//ApiPlace zw_duplicate_object_api = new ApiPlace(typeof(Hook_ZwDuplicateObject));
			#endregion

			#region Process management API
			ApiPlace create_process_w_api = new ApiPlace(typeof(Hook_CreateProcessInternalW));
            create_process_w_api.addPutReaction(new Place.Reaction(PrintReactionProvider.getProvider(ConsoleColor.Red).advancedPrintToken));

            if (Configuration.FOLLOW_PROCESS_TREE) create_process_w_api.addPutReaction(new Place.Reaction(injectLibraryIntoFollowupProcess));
			//ApiPlace exit_process_api = new ApiPlace(typeof(Hook_ExitProcess));
			//ApiPlace zw_terminate_process_api = new ApiPlace(typeof(Hook_ZwTerminateProcess));
			//ApiPlace ldr_shutdown_process_api = new ApiPlace(typeof(Hook_LdrShutdownProcess));
			#endregion
			#region Network API
			ApiPlace wsa_socket_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_WSASocketW), Hook_WSASocketW.Color.Handle);
			ApiPlace wsa_connect_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_WSAConnect), Hook_WSAConnect.Color.Handle);
			ApiPlace connect_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_connect), Hook_connect.Color.Handle);
			ApiPlace bind_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_bind), Hook_bind.Color.Handle);
			ApiPlace listen_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_listen), Hook_listen.Color.Handle);
			ApiPlace wsa_accept_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_WSAAccept), Hook_WSAAccept.Color.Handle);
			ApiPlace accept_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_accept), Hook_accept.Color.Handle);
			ApiPlace send_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_send), Hook_send.Color.Handle);
			ApiPlace wsa_send_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_WSASend), Hook_WSASend.Color.Handle);

			#endregion
			#region Named Pipes API
			ApiPlace create_named_pipe_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_CreateNamedPipeW), Hook_CreateNamedPipeW.Color.PipeHandle);
            create_named_pipe_api.addPutReaction(new Place.Reaction(PrintReactionProvider.getProvider(ConsoleColor.Yellow).simplePrintToken));
			ApiPlace connect_named_pipe_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_ConnectNamedPipe), Hook_ConnectNamedPipe.Color.PipeHandle);
			#endregion
			#region File API
			ApiPlace zw_create_file_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_ZwCreateFile), "FileHandle");
			ApiPlace zw_open_file_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_ZwOpenFile), "FileHandle");
			ApiPlace zw_read_file_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_ZwReadFile), "FileHandle");
			ApiPlace zw_write_file_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_ZwWriteFile), "FileHandle");
			ApiPlace zw_create_section_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_ZwCreateSection), "FileHandle");
			CPNetBlocks.assembleCloseHandleSubNet(zw_create_section_api, "SectionHandle");// in addition section can be closed by SectionHandle
			ApiPlace zw_map_view_of_section_api = CPNetBlocks.getApiPlaceClosedByZwClose(typeof(Hook_ZwMapViewOfSection), "SectionHandle");

			//Place zw_create_file_api = CPNetBlocks.getPlaceClosedByZwClose("3", "2");
			//Place zw_open_file_api = CPNetBlocks.getPlaceClosedByZwClose("33", "2");
			//Place zw_read_file_api = CPNetBlocks.getPlaceClosedByZwClose("333", "2");
			//Place zw_write_file_api = CPNetBlocks.getPlaceClosedByZwClose("4", "2");
			//Place zw_create_section_api = CPNetBlocks.getPlaceClosedByZwClose("44", "2");
			//CPNetBlocks.assembleCloseHandleSubNet(zw_create_section_api, "SectionHandle");// in addition section can be closed by SectionHandle
			//Place zw_map_view_of_section_api = CPNetBlocks.getPlaceClosedByZwClose("444", "2");

			#endregion
			#endregion

			#region Filesystem predefinitions
			Place joined_create_open_file_api = CPNetBlocks.getPlaceClosedByZwClose("Joined_create_&_open", "FileHandle");
			CPNetBlocks.unconditionallyJoinPlacesIntoOne(zw_create_file_api, zw_open_file_api, joined_create_open_file_api);
			#endregion

			#region Network predefinitions
			Place joined_connect_api = CPNetBlocks.getPlaceClosedByZwClose("Joined_connect_&_WSAConnect", Hook_connect.Color.Handle);
			CPNetBlocks.unconditionallyJoinPlacesIntoOne(wsa_connect_api, connect_api, joined_connect_api);

			Place joined_send_api = CPNetBlocks.getPlaceClosedByZwClose("Joined_send_&_WSASend", Hook_send.Color.Handle);
			CPNetBlocks.unconditionallyJoinPlacesIntoOne(send_api, wsa_send_api, joined_send_api);

			Place joined_accept_api = CPNetBlocks.getPlaceClosedByZwClose("Joined_accept_&_WSAAccept", Hook_accept.Color.Handle);
			CPNetBlocks.unconditionallyJoinPlacesIntoOne(accept_api, wsa_accept_api, joined_accept_api);

			Place socket_connected = CPNetBlocks.getPlaceClosedByZwClose("Socket_connected", Hook_connect.Color.Handle);

			CPNetBlocks.assembleDiInputStructure(
									wsa_socket_api,
									Delete.No,
									joined_connect_api,
									Delete.Yes,
									socket_connected,
									EQUAL(Hook_WSASocketW.Color.Handle, Hook_connect.Color.Handle)).generating_expression = STANDARD_GENEXP(joined_connect_api);

			#endregion

			#region File read/write self_read

			Place file_mapping = Detect_FileMapping(joined_create_open_file_api, zw_create_section_api, zw_map_view_of_section_api);
			const int MIN_TO_READ = 1600;
			Place read_from_file = AccumulatedBytesCheckAssemble(joined_create_open_file_api, zw_read_file_api, MIN_TO_READ, "BytesRead", "read");
			const int MIN_TO_WRITE = 1600;
			Place written_to_file = AccumulatedBytesCheckAssemble(joined_create_open_file_api, zw_write_file_api, MIN_TO_WRITE, "BytesWritten", "write");

			Place self_read_or_map = new Place("Self_read_or_map_file");

			Place read_from_file_mirror = CPNetBlocks.mirror(read_from_file);
			CPNetBlocks.assembleMonoInputStructure(read_from_file_mirror,
													Arc.MAX_WEIGHT,
													Delete.Yes,
													self_read_or_map,
													token => CPNetBlocks.checkProcessStartedFromTheModule(token.PID, to_string(token["ObjectName"]))
													).generating_expression = STANDARD_GENEXP(read_from_file_mirror);

			CPNetBlocks.assembleMonoInputStructure(read_from_file_mirror,
													Arc.MIN_WEIGHT,
													Delete.Yes,
													CPNetBlocks.getPlaceDestroingTokens(),
													TRUE).generating_expression = STANDARD_GENEXP(read_from_file_mirror);

			Place map_file_mirror = CPNetBlocks.mirrorDumped(file_mapping);

			CPNetBlocks.assembleMonoInputStructure(map_file_mirror,
													Arc.MAX_WEIGHT,
													Delete.Yes,
													self_read_or_map,
													token => CPNetBlocks.checkProcessStartedFromTheModule(token.PID, to_string(token["ObjectName"]))
													).generating_expression = STANDARD_GENEXP(map_file_mirror);

			#region Definition of executable file extentions
			HashSet<string> executables_extentions_set = new
		HashSet<string>() {	
									"ADE", //- Microsoft Access Project Extension 
									"ADP", //- Microsoft Access Project 
									"BAS", //- Visual Basic Class Module 
									"BAT", //- Batch File 
									"CHM", //- Compiled HTML Help File 
									"CMD", //- Windows NT Command Script 
									"COM", //- MS-DOS Application 
									"CPL", //- Control Panel Extension 
									"CRT", //- Security Certificate 
									"DLL", //- Dynamic Link Library 
									"EXE", //- Application 
									"HLP", //- Windows Help File 
									"HTA", //- HTML Applications 
									"INF", //- Setup Information File 
									"INS", //- Internet Communication Settings 
									"ISP", //- Internet Communication Settings 
									"JSE", //- JScript Encoded Script File 
									"LNK", //- Shortcut 
									"MDB", //- Microsoft Access Application 
									"MDE", //- Microsoft Access MDE Database 
									"MSC", //- Microsoft Common Console Document 
									"MSI", //- Windows Installer Package 
									"MSP", //- Windows Installer Patch 
									"MST", //- Visual Test Source File 
									"OCX", //- ActiveX Objects 
									"PCD", //- Photo CD Image 
									"PIF", //- Shortcut to MS-DOS Program 
									"POT", //- PowerPoint Templates 
									"PPT", //- PowerPoint Files 
									"REG", //- Registration Entries 
									"SCR", //- Screen Saver 
									"SCT", //- Windows Script Component 
									"SHB", //- Document Shortcut File 
									"SHS", //- Shell Scrap Object 
									"SYS", //- System Config/Driver 
									"URL", //- Internet Shortcut (Uniform Resource Locator) 
									"VBE", //- VBScript Encoded Script File 
									"VBS", //- VBScript Script File 
									"WSC", //- Windows Script Component 
									"WSF", //- Windows Script File 
									"WSH" //- Windows Scripting Host Settings File 
									}; 
			#endregion

			Place write_to_executable = new Place("write_to_executable");//.addPutReaction(new Place.Reaction(CPNetBlocks.getPrintReactionProvider(ConsoleColor.Yellow).advancedPrintToken));
			Place written_to_file_mirror_1=CPNetBlocks.mirrorDumped(written_to_file);
			CPNetBlocks.assembleMonoInputStructure(written_to_file_mirror_1,
													Delete.Yes,
													write_to_executable,
													token => executables_extentions_set.Contains(get_file_extention(token[Hook_ZwCreateFile.Color.ObjectName]).ToUpper())
													).generating_expression = STANDARD_GENEXP(written_to_file_mirror_1);


			//do self read -> write to EXE action detection

			Place self_read_write_to_executable = new DetectionPlace("self_read_write_to_executable");

			CPNetBlocks.assembleDiInputStructure(CPNetBlocks.mirror(self_read_or_map),
													Delete.No,
													CPNetBlocks.mirrorDumped(write_to_executable),
													Delete.Yes,
													self_read_write_to_executable,
													TRUE_2).generating_expression = SIMPLE_GENEXP();
			#endregion

			#region Download & execute

			Place start_process_from_edited_or_created_executable = new DetectionPlace("start_process_from_edited_or_created_executable");

			Place create_process_w_api_mirror_1 = CPNetBlocks.mirrorDumped(create_process_w_api);

			CPNetBlocks.assembleDiInputStructure(CPNetBlocks.mirror(write_to_executable),
												Delete.No,
												create_process_w_api_mirror_1,
												Delete.Yes,
												start_process_from_edited_or_created_executable,
                                                FIRST_ENDS_WITH_SECOND_STRING(Hook_ZwCreateFile.Color.ObjectName, Hook_CreateProcessInternalW.Color.ApplicationName)
												).generating_expression = STANDARD_GENEXP(create_process_w_api_mirror_1);

			Place socket_has_been_connected_fact = new Place("fact_that_socket_has_been_connected");
			CPNetBlocks.assembleEliminateDuplicateItems(CPNetBlocks.mirrorWithRemoval(socket_connected), socket_has_been_connected_fact, TRUE_2);

			Place download_and_execute_detected = new DetectionPlace("download_and_execute_detected");

			CPNetBlocks.assembleDiInputStructure(socket_has_been_connected_fact,
													Delete.No,
													CPNetBlocks.mirrorDumped(start_process_from_edited_or_created_executable),
													Delete.Yes,
													download_and_execute_detected,
													TRUE_2).generating_expression = SIMPLE_GENEXP();			
			#endregion

			#region Self mail detection

			Place smtp_detected = Detect_SMTP(socket_connected, joined_send_api);

			Place self_mail_detected = new DetectionPlace("self_mail_detected");
			CPNetBlocks.assembleDiInputStructure(CPNetBlocks.mirrorDumped(smtp_detected),
													Delete.Yes,
													self_read_or_map,
													Delete.No,
													self_mail_detected,
													TRUE_2).generating_expression = STANDARD_GENEXP(self_read_or_map);

			#endregion

			Detect_NamedPipeShell(create_named_pipe_api, connect_named_pipe_api, create_process_w_api);
			Detect_BindListenAcceptShell(create_process_w_api, assembleBindListenAcceptLine(wsa_socket_api, bind_api, listen_api, joined_accept_api));
		}

		private Place Detect_BindListenAcceptShell(Place create_process_w_api, Place accepted_sockets) {
			Place create_process_w_api_mirror = CPNetBlocks.mirror(create_process_w_api);

			Place socket_listen_accept_in_shell_detected = new Place("Socket_listen_accept_connected_to_stdin");
			Place socket_listen_accept_out_shell_detected = new Place("Socket_listen_accept_connected_to_stdout");
			Place bind_shell_via_listen_accept_detected = new DetectionPlace("Bind_shell_via_listen_accept_detected");

			CPNetBlocks.assembleDiInputStructure(accepted_sockets,
													Delete.No,
													create_process_w_api_mirror,
													Delete.No,
													socket_listen_accept_in_shell_detected,
													EQUAL(Hook_WSAAccept.Color.Handle,Hook_CreateProcessInternalW.Color.StdInHandle)
												).generating_expression = STANDARD_GENEXP(create_process_w_api_mirror);

			CPNetBlocks.assembleDiInputStructure(accepted_sockets,
													Delete.No,
													create_process_w_api_mirror,
													Delete.No,
													socket_listen_accept_out_shell_detected,
													EQUAL(Hook_WSAAccept.Color.Handle,Hook_CreateProcessInternalW.Color.StdOutHandle)
												).generating_expression = STANDARD_GENEXP(create_process_w_api_mirror);

			CPNetBlocks.assembleDiInputStructure(socket_listen_accept_in_shell_detected,
													Delete.Yes,
													socket_listen_accept_out_shell_detected,
													Delete.Yes,
													bind_shell_via_listen_accept_detected,
													EQUAL(Hook_CreateProcessInternalW.Color.ProcessId,Hook_CreateProcessInternalW.Color.ProcessId)
												).generating_expression = tuple =>
												{
													Token result = new Token();
													result.loadAllFields("In.", tuple[socket_listen_accept_in_shell_detected]);
													result.loadAllFields("Out.", tuple[socket_listen_accept_out_shell_detected]);
													//result.prevTuple = tuple;
													return result;
												};
			return bind_shell_via_listen_accept_detected;
		}



		/// <summary>
		/// Assembles standart socket line bind->listen->accept detection.
		/// </summary>
		/// <param name="wsa_socket_api">No delete</param>
		/// <param name="bind_api">DELETE</param>
		/// <param name="listen_api">DELETE</param>
		/// <param name="joined_accept_api">DELETE</param>
		/// <returns>Place with accepted sockets</returns>
		private Place assembleBindListenAcceptLine(Place wsa_socket_api, Place bind_api, Place listen_api, Place joined_accept_api) {
			//Assemble line socket bind listen accept
			//Actually it is possible to listen w/o bind. But then port is random and getsockname or netstat is needed
			Place socket_bound = CPNetBlocks.getPlaceClosedByZwClose("Socket_bound", Hook_bind.Color.Handle);
			Place socket_listening = CPNetBlocks.getPlaceClosedByZwClose("Socket_listening", Hook_listen.Color.Handle);
			Place accepted_sockets = CPNetBlocks.getPlaceClosedByZwClose("Accepted_sockets", Hook_accept.Color.Handle);

			CPNetBlocks.assembleDiInputStructure(wsa_socket_api,
													Delete.No,
													bind_api,
													Delete.Yes,
													socket_bound,
													EQUAL(Hook_WSASocketW.Color.Handle, Hook_bind.Color.Handle)
												).generating_expression = STANDARD_GENEXP(bind_api);
			CPNetBlocks.assembleDiInputStructure(socket_bound,
													Delete.No,
													listen_api,
													Delete.Yes,
													socket_listening,
													EQUAL(Hook_bind.Color.Handle, Hook_listen.Color.Handle)
												).generating_expression = STANDARD_GENEXP(listen_api);
            socket_listening.addPutReaction(new Place.Reaction(PrintReactionProvider.getProvider(ConsoleColor.Yellow).advancedPrintToken));
			CPNetBlocks.assembleDiInputStructure(socket_listening,
													Delete.No,
													joined_accept_api,
													Delete.Yes,
													accepted_sockets,
													EQUAL(Hook_listen.Color.Handle, Hook_accept.Color.ListeningSocketHandle)
												).generating_expression = STANDARD_GENEXP(joined_accept_api);
			return accepted_sockets;
		}

		/// <summary>
		/// Detects when two named pipes are created and attached to the input of the process.
		/// </summary>
		/// <param name="create_named_pipe_api">NO Delete</param>
		/// <param name="connect_named_pipe_api">Mirrored </param>
		/// <param name="create_process_w_api">Mirrored</param>
		private Place Detect_NamedPipeShell(Place create_named_pipe_api, Place connect_named_pipe_api, Place create_process_w_api) {
			Place pipe_connected = CPNetBlocks.getPlaceClosedByZwClose("Pipe_created_&_connected", Hook_CreateNamedPipeW.Color.PipeHandle);

			CPNetBlocks.assembleDiInputStructure(create_named_pipe_api,
													Delete.No,
													CPNetBlocks.mirrorWithRemoval(connect_named_pipe_api),
													Delete.Yes,
													pipe_connected,
													EQUAL(Hook_CreateNamedPipeW.Color.PipeHandle, Hook_ConnectNamedPipe.Color.PipeHandle)
												).generating_expression = STANDARD_GENEXP(create_named_pipe_api);

			Place pipe_shell_in_detected = CPNetBlocks.getPlaceClosedByZwClose("Pipe_connnected_to_process_input_detected", Hook_CreateNamedPipeW.Color.PipeHandle);
			//pipe_shell_in_detected.addPutReaction(new Place.Reaction(CPNetBlocks.getPrintReactionProvider(ConsoleColor.Green).advancedPrintToken));

			Place pipe_shell_out_detected = CPNetBlocks.getPlaceClosedByZwClose("Pipe_connnected_to_process_output_detected", Hook_CreateNamedPipeW.Color.PipeHandle);
			//pipe_shell_out_detected.addPutReaction(new Place.Reaction(CPNetBlocks.getPrintReactionProvider(ConsoleColor.Blue).advancedPrintToken));

			Place create_process_w_api_mirror = CPNetBlocks.mirrorDumped(create_process_w_api);

			CPNetBlocks.assembleDiInputStructure(pipe_connected,
													Delete.No,
													create_process_w_api_mirror,
													Delete.No,
													pipe_shell_in_detected,
													EQUAL(Hook_CreateNamedPipeW.Color.PipeHandle,Hook_CreateProcessInternalW.Color.StdInHandle)
												).generating_expression = tuple =>
												{
													Token result = new Token(tuple[create_process_w_api_mirror]);
													result.loadFields(new string[] { Hook_CreateNamedPipeW.Color.PipeHandle }, string.Empty, tuple[pipe_connected]);
													result.prevTuple = tuple;
													return result;
												};

			CPNetBlocks.assembleDiInputStructure(pipe_connected,
													Delete.No,
													create_process_w_api_mirror,
													Delete.No,
													pipe_shell_out_detected,
													EQUAL(Hook_CreateNamedPipeW.Color.PipeHandle,Hook_CreateProcessInternalW.Color.StdOutHandle)
												).generating_expression = tuple =>
												{
													Token result = new Token(tuple[create_process_w_api_mirror]);
													result.loadFields(new string[] { Hook_CreateNamedPipeW.Color.PipeHandle }, string.Empty, tuple[pipe_connected]);
													result.prevTuple = tuple;
													return result;
												};

			Place bind_shell_via_named_pipes_detected = new DetectionPlace("Bind_shell_via_named_pipes_detected");

			//Pipe shell is obvious when in and out std handles of a process are connected to pipes
			//so here we check that both std streams of a process are connected to pipes
			CPNetBlocks.assembleDiInputStructure(pipe_shell_in_detected,
													Delete.Yes,
													pipe_shell_out_detected,
													Delete.Yes,
													bind_shell_via_named_pipes_detected,
													EQUAL(Hook_CreateProcessInternalW.Color.ProcessId,Hook_CreateProcessInternalW.Color.ProcessId)
												).generating_expression = tuple =>
												{
													Token result = new Token();
													result.loadAllFields("In.", tuple[pipe_shell_in_detected]);
													result.loadAllFields("Out.", tuple[pipe_shell_out_detected]);
													//result.prevTuple = tuple;
													return result;
												};			
			return bind_shell_via_named_pipes_detected;
		}

		/// <summary>
        /// Detects that program does't network interaction accordin to SMTP e-mail protocol.
		/// Fully mirrored
		/// </summary>
		/// <param name="socket_connected_out">Mirrored</param>
		/// <param name="joined_send_api">Mirrored</param>
		/// /// <returns>Place with unique set of objects from where more than min_to_read bytes were read by program. This procedure DELETES from {place with read behavior}</returns>
		private Place Detect_SMTP(Place socket_connected, Place joined_send_api) {
			Place joined_send_api_mirror = CPNetBlocks.mirrorWithRemoval(joined_send_api);
			
			string[] smtp_prefixes_array = new string[] { "MAIL FROM:", "RCPT TO:", "DATA" };

			Place smtp_sequencer = CPNetBlocks.getPlaceClosedByZwClose("SMTP_sequencer", "SocketHandle").setPrintLevel(Place.PrintLevel.Low);

			CPNetBlocks.assembleDiInputStructure(	joined_send_api_mirror, 
													Delete.Yes, 
													CPNetBlocks.mirrorWithRemoval(socket_connected), 
													Delete.No, 
													smtp_sequencer,
										(token1, token2) => token1["SocketHandle"].Equals(token2["SocketHandle"]) && (CPNetBlocks.begins_with_string(token1["buffer"], "helo") || (CPNetBlocks.begins_with_string(token1["buffer"], "ehlo")))
									 ).generating_expression = tuple =>
									 {
										 Token result = new Token(tuple[joined_send_api_mirror]);
										 result.prevTuple = tuple;
										 result["index"] = 0;
										 result["smtp_prefixes"] = smtp_prefixes_array;
										 return result;
									 };

			Place smtp_buffers_sent = new Place("SMTP_buffers_sent");

			Transition t_cycle_through_smtp_prefixes = new Transition("t_cycle_through_smtp_prefixes");
			t_cycle_through_smtp_prefixes.addBinaryExpression(joined_send_api_mirror + smtp_sequencer,
																(token1, token2) => token1["SocketHandle"].Equals(token2["SocketHandle"]) &&
																					CPNetBlocks.begins_with_string(token1["buffer"], ((string[])token2["smtp_prefixes"])[to_int(token2["index"])])
															  );
			new Arc(joined_send_api_mirror, t_cycle_through_smtp_prefixes).enableTokenRemovalFromInput();
			new Arc(smtp_sequencer, t_cycle_through_smtp_prefixes).enableTokenRemovalFromInput();
			Arc output_arc_1 = new Arc(t_cycle_through_smtp_prefixes, smtp_sequencer);
			output_arc_1.filtering_expression = tuple => to_int(tuple[smtp_sequencer]["index"]) < smtp_prefixes_array.Length - 1;
			output_arc_1.generating_expression = tuple =>
			{
				Token result = new Token(tuple[joined_send_api_mirror]);
				result.prevTuple = tuple;
				result["index"] = to_int(tuple[smtp_sequencer]["index"]) + 1;
				result["smtp_prefixes"] = smtp_prefixes_array;
				return result;
			};
			Arc output_arc_2 = new Arc(t_cycle_through_smtp_prefixes, smtp_buffers_sent);
			output_arc_2.filtering_expression = tuple => to_int(tuple[smtp_sequencer]["index"]) == smtp_prefixes_array.Length - 1;
			output_arc_2.generating_expression = tuple => { Token result = new Token(tuple[joined_send_api_mirror]); result.prevTuple = tuple; return result; };

			//			smtp_buffers_sent.addPutReaction(new Place.Reaction(CPNetBlocks.getPrintReactionProvider(ConsoleColor.Yellow).advancedPrintToken));

			return smtp_buffers_sent;
		}

		/// <summary>
		/// Assembles file mapping detection
		/// </summary>
		/// <param name="joined_create_open_file">No delete</param>
		/// <param name="zw_create_section_api">DELETE</param>
		/// <param name="zw_map_view_of_section_api">DELETE</param>
		/// <returns></returns>
		private Place Detect_FileMapping(Place joined_create_open_file, Place zw_create_section_api, Place zw_map_view_of_section_api) {
			Place file_mapping_created = CPNetBlocks.getPlaceClosedByZwClose("File_mapping_created", "FileHandle");

			CPNetBlocks.assembleDiInputStructure(joined_create_open_file,
										Delete.No,
										zw_create_section_api,
										Delete.Yes,
										file_mapping_created,
										EQUAL(Hook_ZwCreateFile.Color.FileHandle, Hook_ZwCreateSection.Color.FileHandle)
									).generating_expression = tuple =>
									{
										Token result = new Token(tuple[zw_create_section_api]);
										result.loadFields(new string[] { "ObjectName" }, "Zw(Create/Open)File.", tuple[joined_create_open_file]);
										result.prevTuple = tuple;
										return result;
									};
			//file_mapping_created.addReaction(new Place.Reaction(printCauseToken));
			Place file_mapping_created_and_mapped = new Place("File_mapping_created_&_mapped");
			CPNetBlocks.assembleCloseHandleSubNet(file_mapping_created_and_mapped, Hook_ZwCreateSection.Color.FileHandle);
			CPNetBlocks.assembleCloseHandleSubNet(file_mapping_created_and_mapped, Hook_ZwCreateSection.Color.SectionHandle);

			CPNetBlocks.assembleDiInputStructure(file_mapping_created,
										Delete.No,
										zw_map_view_of_section_api,
										Delete.Yes,
										file_mapping_created_and_mapped,
										EQUAL(Hook_ZwCreateSection.Color.SectionHandle,Hook_ZwMapViewOfSection.Color.SectionHandle)
									 ).generating_expression = tuple =>
									 {
										 Token result = new Token(tuple[zw_map_view_of_section_api]);
										 result.loadFields(new string[] { "FileHandle", "ObjectName", "Zw(Create/Open)File.ObjectName" }, string.Empty, tuple[file_mapping_created]);
										 result.prevTuple = tuple;
										 return result;
									 };
			Place file_mapping_created_and_mapped_unique = new Place("File_mapping_created_and_mapped_unique");
			CPNetBlocks.assembleEliminateDuplicateItems(file_mapping_created_and_mapped, file_mapping_created_and_mapped_unique, (token1, token2) =>
			{
				return strings_equal_ignore_case(token1["Zw(Create/Open)File.ObjectName"], token2["Zw(Create/Open)File.ObjectName"]);
			});

			//file_mapping_created_and_mapped_unique.addPutReaction(new Place.Reaction(new SimpleReactionProvider(ConsoleColor.Green).advancedPrintTokenReaction));
			return file_mapping_created_and_mapped_unique;
		}

		/// <summary>
		/// Assembles repeated action detection. If program reads/writes/sends more than  min_to_transfer  the created/opened object is remebered in output place
		/// </summary>
		/// <param name="create_or_open_file">place whith object created/opened</param>
		/// <param name="zw_read_file_api">place with read behavior. DELETE</param>
		/// <param name="min_to_transfer">Min number of bytes to transfer for detection</param>
		/// <returns>Place with unique set of objects from where more than min_to_read bytes were read by program. This procedure DELETES from {place with read behavior}</returns>
		private Place AccumulatedBytesCheckAssemble(Place create_or_open_object, Place action_api, int min_to_transfer, string accumulator_field_name, string human_readable_prefix) {

			Place transfer_data_from_object = CPNetBlocks.getPlaceClosedByZwClose(human_readable_prefix + "_transfer_data_from_object", "FileHandle").setPrintLevel(Place.PrintLevel.Low);

			CPNetBlocks.assembleDiInputStructure(
									create_or_open_object,
									Arc.DEFAULT_WEIGHT,
									Delete.No,
									action_api,
									Arc.MIN_WEIGHT,
									Delete.Yes,
									transfer_data_from_object,
									EQUAL(Hook_ZwCreateFile.Color.FileHandle,"FileHandle")
							).generating_expression = tuple =>
							{
								Token result = new Token(tuple[action_api]);
								result.loadFields(new string[] { "ObjectName" }, string.Empty, tuple[create_or_open_object]);
								result.prevTuple = tuple;
								return result;
							};

			//add read more than limit subnet from the first time

			CPNetBlocks.assembleDiInputStructure(
							action_api,
							Delete.Yes,
							transfer_data_from_object,
							Delete.Yes,
							transfer_data_from_object,
							(token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"]) && to_int(token2[accumulator_field_name]) < min_to_transfer
						).generating_expression = tuple =>
						{
							Token result = new Token(tuple[transfer_data_from_object]);
							result[accumulator_field_name] = to_int(tuple[transfer_data_from_object][accumulator_field_name]) + to_int(tuple[action_api][accumulator_field_name]);
							return result;
						};

			Place excessive_tranfers = CPNetBlocks.getPlaceClosedByZwClose(human_readable_prefix + "_excessive_transfers", "FileHandle").setPrintLevel(Place.PrintLevel.Low);

			CPNetBlocks.assembleDiInputStructure(
							action_api,
							Arc.MAX_WEIGHT,
							Delete.Yes,
							transfer_data_from_object,
							Arc.DEFAULT_WEIGHT,
							Delete.No,
							excessive_tranfers,
							(token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"]) && to_int(token2[accumulator_field_name]) >= min_to_transfer
						).generating_expression = tuple => new Token(tuple[transfer_data_from_object]);


			Place remember_transfers = new Place(human_readable_prefix + "_remember_transfers").setPrintLevel(Place.PrintLevel.Low);
			//remember_file_read.addReaction(new Place.Reaction(printCauseToken));

			CPNetBlocks.assembleMonoInputStructure(
							transfer_data_from_object,
							Arc.MIN_WEIGHT,
							Delete.No,
							remember_transfers,
							(token) => to_int(token[accumulator_field_name]) >= min_to_transfer
						).generating_expression = tuple => new Token(tuple[transfer_data_from_object]);

			Place remember_transfers_unique = new Place(human_readable_prefix + "_remember_transfers_unique");
			//remember_file_read_unique.addReaction(new Place.Reaction(printCauseToken));
			CPNetBlocks.assembleEliminateDuplicateItems(remember_transfers, remember_transfers_unique, (token1, token2) => CPNetBlocks.strings_equal_ignore_case(token1["ObjectName"], token2["ObjectName"]));
			return remember_transfers_unique;
		}


		#region commmented out
		//private void ReadFileCheckAssemble(Place zw_create_file_api, ApiPlace zw_read_file_api) {
		//    const int MIN_TO_READ = 1600;
		//    Place read_from_file = new Place("read_from_file");
		//    CPNetBlocks.assembleCloseHandleSubNet(read_from_file, "FileHandle");
		//    //read_from_file.addReaction(new Place.Reaction(printCauseToken));

		//    Transition t_first_read = new Transition("first_read_transition");
		//    t_first_read.addBinaryExpression(zw_create_file_api + zw_read_file_api,
		//                                                (token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"])
		//                                              );

		//    new Arc(zw_create_file_api, t_first_read);
		//    new Arc(zw_read_file_api, t_first_read, Arc.MIN_WEIGHT).enableTokenRemovalFromInput();
		//    Arc output_arc_3 = new Arc(t_first_read, read_from_file);
		//    output_arc_3.generating_expression = tuple =>
		//    {
		//        Token result = new Token(tuple[zw_read_file_api]);
		//        result.loadFields(new string[] { "ObjectName" }, string.Empty, tuple[zw_create_file_api]);
		//        result.prevTuple = tuple;
		//        return result;
		//    };

		//    //add read more than limit subnet from the first time

		//    Transition t_next_read = new Transition("next_read_transition");
		//    t_next_read.addBinaryExpression(zw_read_file_api + read_from_file,
		//                                       (token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"]) &&
		//                                           to_int(token2["BytesRead"]) < MIN_TO_READ
		//                                    );

		//    new Arc(zw_read_file_api, t_next_read, Arc.DEFAULT_WEIGHT).enableTokenRemovalFromInput();
		//    new Arc(read_from_file, t_next_read).enableTokenRemovalFromInput();
		//    new Arc(t_next_read, read_from_file).generating_expression = tuple =>
		//    {
		//        Token result = new Token(tuple[read_from_file]);
		//        result["BytesRead"] = to_int(tuple[read_from_file]["BytesRead"]) + to_int(tuple[zw_read_file_api]["BytesRead"]);
		//        return result;
		//    };

		//    Place excessive_reads = new Place("excessive_reads");
		//    CPNetBlocks.assembleCloseHandleSubNet(excessive_reads, "FileHandle");

		//    Transition t_excessive_read = new Transition("excessive_read_transition");
		//    t_excessive_read.addBinaryExpression(zw_read_file_api + read_from_file,
		//                                       (token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"]) &&
		//                                           to_int(token2["BytesRead"]) >= MIN_TO_READ
		//                                    );
		//    new Arc(zw_read_file_api, t_excessive_read, Arc.MAX_WEIGHT).enableTokenRemovalFromInput();
		//    new Arc(read_from_file, t_excessive_read);
		//    new Arc(t_excessive_read, excessive_reads).generating_expression = tuple => new Token(tuple[read_from_file]);
		//    //excessive_reads.addReaction(new Place.Reaction(printCauseToken));

		//    Place remember_file_read = new Place("remember_file_reads");
		//    //remember_file_read.addReaction(new Place.Reaction(printCauseToken));

		//    Transition t_remember_file_read = new Transition("Remeber that particular file has been read transition");
		//    t_remember_file_read.addUnaryExpression(read_from_file, (token) => to_int(token["BytesRead"]) >= MIN_TO_READ);

		//    new Arc(read_from_file, t_remember_file_read, Arc.MIN_WEIGHT);
		//    new Arc(t_remember_file_read, remember_file_read).generating_expression = tuple => new Token(tuple[read_from_file]);

		//    Place remember_file_read_unique = new Place("remember_file_read_unique");
		//    //remember_file_read_unique.addReaction(new Place.Reaction(printCauseToken));
		//    CPNetBlocks.assembeEliminateDuplicateItems(remember_file_read, remember_file_read_unique, (token1, token2) => strings_equal_ignore_case(token1["ObjectName"], token2["ObjectName"]));
		//}

		//private void WriteFileCheckAssemble(Place zw_create_file_api, ApiPlace zw_write_file_api) {
		//    const int MIN_TO_WRITE = 1600;
		//    Place write_from_file = new Place("write_from_file");
		//    CPNetBlocks.assembleCloseHandleSubNet(write_from_file, "FileHandle");
		//    //write_from_file.addReaction(new Place.Reaction(printCauseToken));

		//    Transition t_first_write = new Transition("first_write_transition");
		//    t_first_write.addBinaryExpression(zw_create_file_api + zw_write_file_api,
		//                                                (token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"])
		//                                              );

		//    new Arc(zw_create_file_api, t_first_write);
		//    new Arc(zw_write_file_api, t_first_write, Arc.MIN_WEIGHT).enableTokenRemovalFromInput();
		//    Arc output_arc_3 = new Arc(t_first_write, write_from_file);
		//    output_arc_3.generating_expression = tuple =>
		//    {
		//        Token result = new Token(tuple[zw_write_file_api]);
		//        result.loadFields(new string[] { "ObjectName" }, string.Empty, tuple[zw_create_file_api]);
		//        result.prevTuple = tuple;
		//        return result;
		//    };

		//    //add write more than limit subnet from the first time

		//    Transition t_next_write = new Transition("next_write_transition");
		//    t_next_write.addBinaryExpression(zw_write_file_api + write_from_file,
		//                                       (token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"]) &&
		//                                           to_int(token2["BytesWritten"]) < MIN_TO_WRITE
		//                                    );

		//    new Arc(zw_write_file_api, t_next_write, Arc.DEFAULT_WEIGHT).enableTokenRemovalFromInput();
		//    new Arc(write_from_file, t_next_write).enableTokenRemovalFromInput();
		//    new Arc(t_next_write, write_from_file).generating_expression = tuple =>
		//    {
		//        Token result = new Token(tuple[write_from_file]);
		//        result["BytesWritten"] = to_int(tuple[write_from_file]["BytesWritten"]) + to_int(tuple[zw_write_file_api]["BytesWritten"]);
		//        return result;
		//    };

		//    Place excessive_writes = new Place("excessive_writes");
		//    CPNetBlocks.assembleCloseHandleSubNet(excessive_writes, "FileHandle");

		//    Transition t_excessive_write = new Transition("excessive_write_transition");
		//    t_excessive_write.addBinaryExpression(zw_write_file_api + write_from_file,
		//                                       (token1, token2) => token1["FileHandle"].Equals(token2["FileHandle"]) &&
		//                                           to_int(token2["BytesWritten"]) >= MIN_TO_WRITE
		//                                    );
		//    new Arc(zw_write_file_api, t_excessive_write, Arc.MAX_WEIGHT).enableTokenRemovalFromInput();
		//    new Arc(write_from_file, t_excessive_write);
		//    new Arc(t_excessive_write, excessive_writes).generating_expression = tuple => new Token(tuple[write_from_file]);
		//    //excessive_writes.addReaction(new Place.Reaction(printCauseToken));

		//    Place remember_file_write = new Place("remember_file_writes");
		//    //remember_file_write.addReaction(new Place.Reaction(printCauseToken));

		//    Transition t_remember_file_write = new Transition("Remeber that particular file has been write transition");
		//    t_remember_file_write.addUnaryExpression(write_from_file, (token) => to_int(token["BytesWritten"]) >= MIN_TO_WRITE);

		//    new Arc(write_from_file, t_remember_file_write, Arc.MIN_WEIGHT);
		//    new Arc(t_remember_file_write, remember_file_write).generating_expression = tuple => new Token(tuple[write_from_file]);

		//    Place remember_file_write_unique = new Place("remember_file_write_unique");
		//    //remember_file_write_unique.addReaction(new Place.Reaction(printCauseToken));
		//    CPNetBlocks.assembeEliminateDuplicateItems(remember_file_write, remember_file_write_unique, (token1, token2) => strings_equal_ignore_case(token1["ObjectName"], token2["ObjectName"]));
		//}
		#endregion
	}
}
