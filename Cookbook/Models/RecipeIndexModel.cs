namespace Cookbook.Models
{
    public class RecipeIndexModel
    {
        public int Id{ get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CreationTimeFormattedString { get; set; }
        public string AuthorName { get; set; }
        public FileInfo? Image { get; set; }
        public bool Liked { get; set; }

        public RecipeIndexModel()
        {
            CreationTimeFormattedString = FormatDateTime(DateTime.Now);
        }

        public RecipeIndexModel(int id, string title, string description, DateTime creationTime, string authorName, FileInfo image, bool liked)
        {
            Id = id;
            Title = title;
            Description = description;
            CreationTimeFormattedString = FormatDateTime(creationTime);
            AuthorName = authorName;
            Image = image;
            Liked = liked;
        }

        private string FormatDateTime(DateTime creationTime)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - creationTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * 60)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * 60)
                return "1 minute ago";

            if (delta < 45 * 60)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * 60)
                return "1 hour ago";

            if (delta < 24 * 60 * 60)
                return ts.Hours + " hours ago";

            if (delta < 48 * 60 * 60)
                return "1 day ago";

            if (delta < 30 * 24 * 60 * 60)
                return ts.Days + " days ago";

            if (delta < 12 * 30 * 24 * 60 * 60)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "1 year ago" : years + " years ago";
            }
        }

    }
}
