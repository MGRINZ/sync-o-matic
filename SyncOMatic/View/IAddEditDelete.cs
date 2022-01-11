namespace SyncOMatic.View
{
    public interface IAddEditDelete
    {
        public int SelectedIndex { get; set; }

        public object GetItem();
        public void SetItem(object item);
    }
}