namespace Gameboard
{
    public static class DataTypes
    {
        public enum OperatingMode { Debug, Production }

        public enum CompanionContainerSortingTypes
        {
            Stack = 0,
            Fan = 1,
        }

        public enum CardFacingDirections
        {
            FaceUp = 0,
            FaceDown = 1,
        }

        public enum ObjectDisplayStates
        {
            Hidden = 0,
            Displayed = 1,
        }

        public enum TickboxStates
        {
            Off = 0,
            On = 1,
        }

        public enum TrackedBoardObjectTypes
        {
            None = 0,
            Pointer = 1,
            Token = 2,
        }

        public enum GameboardScreenCoordinateSystem
        {
            UNKNOWN = 0,
            TOP_LEFT = 1,
            BUTTOM_LEFT = 2,
        }

        public enum UserPresenceChangeTypes
        {
            UNKNOWN = -1,
            ADD = 0,
            REMOVE = 1,
            CHANGE = 2,
            CHANGE_POSITION = 3
        }

        public enum PresenceType
        {
            DRAWER = 0,
            COMPANION = 1,
            NONE = 2,
        }

        public enum UserType
        {
            NONE = -1, // Value is empty
            USER = 0, // Logged in with a permanent UserID.
            GUEST = 1, // Guest user with a generated userID not associated with a Gameboard account.
            OPEN = 2, // Open seat with no user.
        }

        public enum PointerTypes
        {
            NONE = -1,
            Finger = 0, // Single finger on the board
            Blade = 64, // Blade of the hand on the board
        }

        public enum ScreenSides
        {
            UNKNOWN = 0,
            Forward = 1,
            Back = 2,
            Left = 3,
            Right = 4,
        }

        public enum EventQueueStates
        {
            WaitingToProcess = 0,
            Processing = 1,
            Cancelled = 2,
            TimedOut = 3,
            Completed = 4,
        }

        public enum DeviceTypes
        {
            Gameboard = 0,
            Companion = 1,
        }
    }
}