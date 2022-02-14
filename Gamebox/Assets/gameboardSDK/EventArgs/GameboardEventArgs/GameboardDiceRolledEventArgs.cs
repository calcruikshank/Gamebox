namespace Gameboard.EventArgs
{
    public class GameboardDiceRolledEventArgs : GameboardIncomingEventArg
    {
        public string userIdWhoRolled;
        public string diceNotation;
        public int[] diceSizesRolledList;
        public int addedModifier;
    }
}