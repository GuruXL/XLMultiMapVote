namespace XLMultiMapVote.Data
{
    public static class Labels
    {
        // Pop Ups
        public const string popUpMessage = "Vote for the next map!";

        public const string changeMapMessage = "Voting in progress... The map will change when the timer ends.";

        public const string changetoMessage = "Map changing to: "; 

        // UI
        public const string addMapText = "Select Map To Add";

        public const string menuButtonLabel = "Vote For Next Map";

        public const string cancelButtonLabel = "Cancel Vote";

        //Error
        public const string invalidMapError = "Cannot Change Maps: Map is Invald";

        public const string hostError = "Voting can only be setup by the lobby host";

        public const string voteInProgressError = "Please wait until current vote is complete";

        public const string voteCancelError = "Voting has been cancelled - the map will no longer change";

        public const string mapListEmptyError = "Cannot queue vote - Map list must have a minimun of 2 maps.";
    }
}
