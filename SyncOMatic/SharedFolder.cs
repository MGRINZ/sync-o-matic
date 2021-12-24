namespace SyncOMatic
{
    public class SharedFolder
    {
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
    }
}