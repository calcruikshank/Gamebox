namespace Gameboard.EventArgs
{
    public class GameboardIncomingEventArg
    {
        /// <summary>
        /// All incoming events have a ConnectionID associated with them, which is the connection it came from. Each device (Gameboard, Companion) has a unique ConnectionID for that
        /// device, which only changes if the device disconnects and reconnects.
        /// </summary>
        public string connectionId;
    }
}