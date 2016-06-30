using System;
using System.Collections.Generic;
using System.Text;

namespace APIMonLib
{
    public interface ChannelReceiver 
    {
        void receiveTransferUnits(Queue<TransferUnit> tu_array);

        /// <summary>
        /// Send debugging messages, through this method
        /// </summary>
        /// <param name="message"></param>
        void receiveTextMessage(String message);

        void receiveRemoteHookingException(RemoteHookingException ex);

        APIFullName[] getApiCallsToIntercept();
        void ping();
    }
}
