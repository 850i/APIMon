using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using EasyHook;

namespace APIMonLib {
	/// <summary>
	/// Instance of this class should reside in the sending entity.
	/// It uses bufferisation and in general thread safe
	/// But all safe hooking guidelines should be followed before using that.
	/// </summary>
	public class TransferUnitSender_New {

		/// <summary>
		/// Maximum number of transfer untis buffered w/o sending. If the number is
		/// over the call will be blocked.
		/// </summary>
		private const int MAX_TU_BUFFER_SIZE = 500;

		/// <summary>
		/// On timeout send operation is aborted and sender gets exception
		/// </summary>
		private const int TU_BUFFER_TIMEOUT = 5000;

		/// <summary>
		/// Transfer Units are grouped into blocks of at most this size
		/// </summary>
		private const int MAX_SEND_BLOCK_SIZE = 50;

		/// <summary>
		/// How much time to wait for the block to send being completely filled. Otherwise fail.
		/// </summary>
		private const int MAX_WAIT_FOR_BLOCK_FILL_TIMEOUT = 100;

		private const int PING_PERIOD = 1000;

		private Thread processing_thread = null;

		private ChannelReceiver remote_receiver = null;

		private bool _sending_enabled = false;
		private object _sending_enabled_sync = new object();

		public bool sending_enabled {
			 get {
				bool tmp;
				lock (_sending_enabled_sync) {
					tmp = _sending_enabled; 
				}
				return tmp; 
			}
		}

		/// <summary>
		/// Call this method before sending any transfer units.
		/// Otherwise everything you send will be discarded
		/// </summary>
		public void enableTransferUnitSend() {
			lock (_sending_enabled_sync) {
				_sending_enabled = true;
			}
		}

		public TransferUnitSender_New(String channel_name) {
			remote_receiver = (ChannelReceiver)RemoteHooking.IpcConnectClient<MarshalByRefObject>(channel_name);
			remote_receiver.ping();
			startSenderThread();
		}

		/// <summary>
		///  Uses separate synchronous channel for sending exceptions back to the
		/// host installed this hook
		/// </summary>
		/// <param name="ex"></param>
		public void sendException(RemoteHookingException ex) {
			remote_receiver.receiveRemoteHookingException(ex);
		}

		/// <summary>
		/// Uses separate synchronous channel for sending text messages
		/// </summary>
		/// <param name="message"></param>
		public void sendTextMessage(String message) {
			remote_receiver.receiveTextMessage(message);
		}

		public APIFullName[] getApiCallsToIntercept() {
			return remote_receiver.getApiCallsToIntercept();
		}

		/// <summary>
		/// Connectivity testing
		/// </summary>
		public void ping() {
			remote_receiver.ping();
		}

		private BlockingQueue transfer_units_blocking_queue = new BlockingQueue(MAX_TU_BUFFER_SIZE);

		private Queue<TransferUnit> transfer_units_queue = new Queue<TransferUnit>();

		private int _buffer_send_period = 50;

		//private const int MAX_QUEUE_LENGTH = 5000;

		private Random random = new Random();

		/// <summary>
		/// Time between two consequent data send procedures (buffer flushes).
		/// </summary>
		public int buffer_send_period {
			get { return _buffer_send_period; }
			set { _buffer_send_period = value; }
		}

		private bool _stopped;
		private Object sync_stopped = new Object();

		/// <summary>
		/// Returns true until channel completed execution
		/// </summary>
		public bool is_running {
			get {
				bool result;
				lock (sync_stopped) { result = _stopped; }
				return !result;
			}
		}

		/// <summary>
		/// Sets appropriate field that channel is not running anymore
		/// </summary>
		private void setIsNotRunningAnymore() {
			lock (sync_stopped) { _stopped = true; }
		}

		private bool _keep_running = true;
		private Object sync_keep_running = new Object();

		/// <summary>
		/// this property returns false, when channel is scheduled for stop
		/// </summary>
		private bool keep_running {
			get {
				bool result;
				lock (sync_keep_running) { result = _keep_running; }
				return result;
			}
		}

		/// <summary>
		/// Schedules channel for stop
		/// </summary>
		public void stopSender() {
			lock (sync_keep_running) { _keep_running = false; }
		}

		/// <summary>
		/// Schedules transfer unit for sending.
		/// Sending of transfer units should be enabled  by enableTransferUnitSend before sending.
		/// Any thread that attemts to send something before enabling will be blacklisted.
		/// </summary>
		/// <param name="tu"></param>
		public void sendTransferUnit(TransferUnit tu) {
			if (!sending_enabled) {
				Console.WriteLine("Sending is not enabled. Blacklisting current thread ID=" + blacklistCurrentThread());
				return; 
			}
			if (!is_running) throw new Exception("Sending of messages has been interrupted. Probably no receiver");

			try {
				transfer_units_blocking_queue.Enqueue(tu, TU_BUFFER_TIMEOUT);
			} catch (QueueTimeoutException qe) {
				throw new ProcessingException("Queueing timeout. Sending of messages has been interrupted. Probably no receiver", qe);
			}
			 catch (AccessViolationException ave) {
			    Console.WriteLine("++++++++++++++++++++++++++++++++++++Access violation++++++++++++++++++++++++++++++++");
				//Console.WriteLine("Blacklisting current thread ID=" + blacklistCurrentThread()+" due to memory access error");
			    Console.WriteLine(ave);
			}
		}


		/// <summary>
		/// Tries to create a block of transfer units
		/// </summary>
		/// <returns>null when nothing to send. Or array of TranferUnits to send</returns>
		private Queue<TransferUnit> getBlockToSend() {
			//check how many tu-s we have
			int queue_count = transfer_units_blocking_queue.Count;
			if (queue_count <= 0) return null;
			Queue<TransferUnit> tu_block=new Queue<TransferUnit>(queue_count+1);
			for (int i = 0; (i < MAX_SEND_BLOCK_SIZE) && (i<queue_count); i++) {
				//we don't expect it to block here since it is guaranteed that we have enough tu-s
				TransferUnit tu = (TransferUnit)transfer_units_blocking_queue.Dequeue(MAX_WAIT_FOR_BLOCK_FILL_TIMEOUT);
				tu_block.Enqueue(tu);
			}
			return tu_block;
		}

		private void ThreadJob() {
			int cumulative_idle_time = 0;
			try {
				while (keep_running) {
					Queue<TransferUnit> transfer_untis = getBlockToSend();
					if (transfer_untis != null) {
						remote_receiver.receiveTransferUnits(transfer_untis);
						transfer_untis.Clear();
						transfer_untis = null;
						cumulative_idle_time = 0;
						if (random.Next(10000) < 10) {
							System.GC.Collect();
						}
					} else {
						try {
							Thread.Sleep(buffer_send_period);
							cumulative_idle_time += buffer_send_period;
						} catch { }
						if (cumulative_idle_time > PING_PERIOD) {
							cumulative_idle_time = 0;
							remote_receiver.ping();
						}
						if (random.Next(10000) < 5) {
							System.GC.Collect();
						}
					}
				}
			} catch {
				Console.WriteLine("Error in processing thread");
			}
			finally {
				setIsNotRunningAnymore();
			}
		}

		private void startSenderThread()
		{
		    ThreadStart job = new ThreadStart(ThreadJob);
		    processing_thread = new Thread(job);
			processing_thread.Priority = ThreadPriority.Highest;
			processing_thread.Start();
		}

		private List<int> thread_black_list = new List<int>();

		private int blacklistCurrentThread() {
			int id = AppDomain.GetCurrentThreadId();
			thread_black_list.Add(id);
			int[] black_list = thread_black_list.ToArray();
			LocalHook.GlobalThreadACL.SetExclusiveACL(black_list);
			return id;
		}

		public void blockUntilFinishedProcessing() {
			processing_thread.Join();
			//throw new NotImplementedException();
		}
	}
}
