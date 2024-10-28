namespace XLMultiMapVote.Data
{
    public static class Labels
    {
        //Popups
        public const string popUpMessage = "Vote for the next map!";

        public const string voteStartedMessage = "Voting in progress... The map will change when the timer ends.";

        public const string mapChangedMessage = "Map Changed By Vote To : ";

        public const string tiedMapMessage = "There has been a tie! A random map will be chosen from the winners.";

        // UI
        public const string addMapText = "Select Map to Add";

        public const string menuButtonLabel = "Vote for Next Map";

        public const string cancelButtonLabel = "Cancel Vote";

        // Error
        public const string invalidMapError = "Cannot Change Maps: Map is Invalid";

        public const string hostError = "Voting can only be set up by the lobby host";

        public const string voteInProgressError = "Please wait until the current vote is complete";

        public const string voteCancelError = "Voting has been cancelled - the map will no longer change";

        public const string mapListEmptyError = "Cannot queue vote - Map list must have a minimum of 2 maps.";

        public const string addMapError = "Cannot add current map to voting list";

    }
}
