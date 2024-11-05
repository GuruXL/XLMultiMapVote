namespace XLMultiMapVote.Data
{
    public static class Labels
    {
        //Popups
        public const string popUpMessage = "Vote for the next map!";

        public const string voteStartedMessage = "Voting in progress... The map will change when the timer ends."; 

        public const string voteCompleteMessage = "Voting complete! The selected map is: ";

        public const string tiedMapMessage = "There has been a tie! A random map will be chosen from the winners.";

        public const string mapChangedMessage = "Map changed by vote to: ";

        // UI
        public const string addMapText = "Select Map to Add";

        public const string menuButtonLabel = "Set Up Map Vote";

        public const string cancelButtonLabel = "Cancel Vote";

        // Error
        public const string invalidMapError = "Can not Change Maps: Map is Invalid";

        public const string hostError = "Voting can only be set up by the lobby host";

        public const string voteInProgressError = "Please wait until the current vote is complete";

        public const string voteNotInProgressError = "No vote in progress";

        public const string voteCancelError = "Voting has been cancelled - the map will no longer change"; 

        public const string mapListEmptyError = "Can not queue vote - Map list must have a minimum of 2 maps.";

        public const string addMapError = "Can not add current map to voting list";

        public const string disableVoteAsHostError = "Can not disable voting when host of the room";

    }
}
