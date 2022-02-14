namespace Gameboard.EventArgs
{
    public class CompanionDiceRollEventArgs : CompanionMessageResponseArgs
    {
        public string userIdWhoRolled;
        public int[] diceSizesRolled = new int[0];
        public int addedModifier = 0;
    }
}