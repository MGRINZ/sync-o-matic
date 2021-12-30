using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace SyncOMatic
{
    public class AddEditDeleteWindow : Window
    {
        public AddEditDeleteWindow() : base() { }

        protected void AddItem(IAddEditDelete dialog, IList itemList)
        {
            Window dialogWindow = (Window)dialog;
            dialogWindow.Owner = this;
            dialogWindow.ShowDialog();
            if (dialog.GetItem() != null)
                itemList.Add(dialog.GetItem());
        }

        protected void EditItem(IAddEditDelete dialog, ListBox listBox, IList itemList)
        {
            Window dialogWindow = (Window)dialog;
            dialogWindow.Owner = this;

            if (listBox.SelectedItem == null)
                return;

            dialog.SelectedIndex = listBox.SelectedIndex;
            dialog.SetItem(listBox.SelectedItem);
            dialogWindow.ShowDialog();

            if (dialog.GetItem() != null)
                itemList[listBox.SelectedIndex] = dialog.GetItem();
        }

        protected void DeleteItem(ListBox listBox, IList itemList)
        {
            if (listBox.SelectedItem != null)
                itemList.Remove(listBox.SelectedItem);
        }
    }
}
