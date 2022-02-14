using static Gameboard.DataTypes;

namespace Gameboard.EventArgs
{
    public class CompanionMessageResponseArgs
    {
        // These come in from the Companion Server
        public int versionTag = 1;
        public int errorId = 0;

        // These are locally defined
        public bool wasSuccessful { get { return errorId == 0 && errorResponse == null; } }
        public CompanionErrorResponse errorResponse;
    }
}