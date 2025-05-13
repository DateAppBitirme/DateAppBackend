namespace DateApp.Core.Enums
{
    public class ComplaintRequest
    {
        public enum ComplaintType
        {
            User = 0,
            Content = 1,
            Other = 2
        }
        public enum RequestType
        {
            FeatureRequest = 0,
            BugReport = 1,
            Other = 2
        }
    }
}
