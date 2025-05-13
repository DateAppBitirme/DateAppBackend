namespace DateApp.Core.Enums
{
    public class ComplaintRequest
    {
        public enum ComplaintType
        {
            User = 1,
            Content = 2,
            Other = 3
        }
        public enum RequestType
        {
            FeatureRequest = 1,
            BugReport = 2,
            Other = 3
        }
    }
}
