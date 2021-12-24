using System.Collections.ObjectModel;

namespace SyncOMatic
{
    public partial class SyncRule
    {
        public bool IsActive { get; set; }
        public string LocalDir { get; set; }
        public string RemoteDir { get; set; }
        public SyncMethod SyncMethod { get; set; }
        public ObservableCollection<FileExclusion> FileExclusions { get; set; }
    }
}