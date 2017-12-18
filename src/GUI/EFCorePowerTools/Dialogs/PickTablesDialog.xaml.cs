using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ErikEJ.SqlCeToolbox.Helpers;
using Microsoft.Win32;

namespace ErikEJ.SqlCeToolbox.Dialogs
{
    public partial class PickTablesDialog
    {
        public PickTablesDialog(DatabaseInfo dbInf)
        {
            Telemetry.TrackPageView(nameof(PickTablesDialog));
            InitializeComponent();
            Background = VsThemes.GetWindowBackground();
            IncludeViews = false; //default unless when loading previous selection
            DBInfo = dbInf;
        }

        public bool IncludeTables { get; set; }
        public bool IncludeViews { get; set; }

        private List<CheckListItem> itemsTab = new List<CheckListItem>();
        private List<CheckListItem> itemsView = new List<CheckListItem>();
        private List<CheckListItem> itemsSp = new List<CheckListItem>();
        private List<CheckListItem> itemsOther = new List<CheckListItem>();

        public List<string> Tables { get; set; }
        public List<string> Views { get; set; }
        public List<string> SP { get; set; }
        public List<string> Other { get; set; }

        public DatabaseInfo DBInfo { get; set; }

        public void LoadAll()
        {
            LoadTables();
            LoadViews();
            LoadSp();
            LoadOthers();
        }

        /// <summary>
        /// Load tables and views
        /// </summary>
        public void LoadTables()
        {
            using (var repository = RepositoryHelper.CreateRepository(DBInfo))
            {
                var tabList = new List<string>();
                tabList  = repository.GetAllTableNamesForExclusion();
                Tables = tabList;
            }
            BindTables();
        }

        public void LoadViews()
        {
            using (var repository = RepositoryHelper.CreateRepository(DBInfo))
            {
                var tabList = new List<string>();
                tabList.AddRange(repository.GetAllViewNamesForExclusion());
                Views = tabList;
            }
            BindViews();
        }

        public void LoadSp()
        {
            using (var repository = RepositoryHelper.CreateRepository(DBInfo))
            {
                //ToDo:
                var tabList = new List<string>();
                //tabList.AddRange(repository.GetAllViewNamesForExclusion());
                SP = tabList;
            }
            BindSP();
        }

        public void LoadOthers()
        {
            using (var repository = RepositoryHelper.CreateRepository(DBInfo))
            {
                //ToDo:
                var tabList = new List<string>();
                //tabList.AddRange(repository.GetAllViewNamesForExclusion());
                Other = tabList;
            }
            BindOther();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAll();
        }

        private void BindTables()
        {
            var defViewChecked = chkViewsClear.IsChecked == true; //For now have the views un-checked
            itemsTab = new List<CheckListItem>();
            foreach (var table in Tables)
            {
                var isChecked = !table.StartsWith("__");
                isChecked = isChecked && !table.StartsWith("dbo.__");
                itemsTab.Add(new CheckListItem { IsChecked = (isChecked && chkClear.IsChecked == true), Label = table });
            }
            chkTables.ItemsSource = itemsTab;
        }

        private void BindViews()
        {
            itemsView = new List<CheckListItem>();
            foreach (var table in Views)
            {
                var isChecked = !table.StartsWith("__");
                itemsView.Add(new CheckListItem { IsChecked = (isChecked && chkViewsClear.IsChecked == true), Label = table });
            }
            chkViews.ItemsSource = itemsView;
            tabViews.IsEnabled = itemsView.Count > 0;
        }

        private void BindSP()
        {
            itemsSp = new List<CheckListItem>();
            foreach (var table in SP)
            {
                var isChecked = !table.StartsWith("__");
                isChecked = isChecked && !table.StartsWith("dbo.__");
                itemsSp.Add(new CheckListItem { IsChecked = (isChecked && chkSPClear.IsChecked == true), Label = table });
            }
            chkSp.ItemsSource = itemsSp;
            tabSp.IsEnabled = itemsSp.Count > 0;
            tabSp.Visibility = itemsSp.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void BindOther()
        {
            itemsOther = new List<CheckListItem>();
            foreach (var table in Other)
            {
                var isChecked = !table.StartsWith("__");
                isChecked = isChecked && !table.StartsWith("dbo.__");
                itemsOther.Add(new CheckListItem { IsChecked = (isChecked && chkOtherClear.IsChecked == true), Label = table });
            }
            chkOther.ItemsSource = itemsOther;
            tabOther.IsEnabled = itemsOther.Count > 0;
            tabOther.Visibility = itemsOther.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Tables.Clear();
            foreach (var item in chkTables.Items)
            {
                var checkItem = (CheckListItem)item;
                if ((!checkItem.IsChecked && !IncludeTables) 
                    || (checkItem.IsChecked && IncludeTables))
                {
                    Tables.Add(checkItem.Label);
                }
            }
            Views.Clear();
            foreach (var item in chkViews.Items)
            {
                var checkItem = (CheckListItem)item;
                if ((!checkItem.IsChecked && !IncludeTables)
                    || (checkItem.IsChecked && IncludeTables))
                {
                    Views.Add(checkItem.Label);
                }
            }
            SP.Clear();
            foreach (var item in chkSp.Items)
            {
                var checkItem = (CheckListItem)item;
                if ((!checkItem.IsChecked && !IncludeTables)
                    || (checkItem.IsChecked && IncludeTables))
                {
                    SP.Add(checkItem.Label);
                }
            }
            Other.Clear();
            foreach (var item in chkOther.Items)
            {
                var checkItem = (CheckListItem)item;
                if ((!checkItem.IsChecked && !IncludeTables)
                    || (checkItem.IsChecked && IncludeTables))
                {
                    Other.Add(checkItem.Label);
                }
            }
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void chkClear_Click(object sender, RoutedEventArgs e)
        {
            if (chkClear.IsChecked != null && chkClear.IsChecked.Value)
            {
                foreach (var item in itemsTab)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                }
            }
            else
            {
                foreach (var item in itemsTab)
                {
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                    }
                }
            }
            chkTables.ItemsSource = null;
            chkTables.ItemsSource = itemsTab;
        }

        private void chkView_Click(object sender, RoutedEventArgs e)
        {
            if (chkViewsClear.IsChecked != null && chkViewsClear.IsChecked.Value)
            {
                foreach (var item in itemsView)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                }
            }
            else
            {
                foreach (var item in itemsView)
                {
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                    }
                }
            }
            chkViews.ItemsSource = null;
            chkViews.ItemsSource = itemsView;
        }

        private void chkSp_Click(object sender, RoutedEventArgs e)
        {
            if (chkSPClear.IsChecked != null && chkSPClear.IsChecked.Value)
            {
                foreach (var item in itemsSp)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                }
            }
            else
            {
                foreach (var item in itemsSp)
                {
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                    }
                }
            }
            chkSp.ItemsSource = null;
            chkSp.ItemsSource = itemsSp;
        }

        private void chkOther_Click(object sender, RoutedEventArgs e)
        {
            if (chkOtherClear.IsChecked != null && chkOtherClear.IsChecked.Value)
            {
                foreach (var item in itemsOther)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                }
            }
            else
            {
                foreach (var item in itemsOther)
                {
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                    }
                }
            }
            chkOther.ItemsSource = null;
            chkOther.ItemsSource = itemsOther;
        }

        private void BtnSaveSelection_OnClick(object sender, RoutedEventArgs e)
        {
            var tableList = string.Empty;
            foreach (var item in chkTables.Items)
            {
                var checkItem = (CheckListItem)item;
                if ((checkItem.IsChecked))
                {
                    tableList += checkItem.Label + Environment.NewLine;
                }
            }

            var sfd = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|All Files(*.*)|*.*",
                ValidateNames = true,
                Title = "Save list of tables as"
            };
            if (sfd.ShowDialog() != true) return;
            File.WriteAllText(sfd.FileName, tableList, Encoding.UTF8);
        }

        private void BtnLoadSelection_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|All Files(*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false,
                Title = "Select list of tables to load"
            };
            if (ofd.ShowDialog() != true) return;

            var lines = File.ReadAllLines(ofd.FileName);
            foreach (var item in itemsTab)
            {
                item.IsChecked = lines.Contains(item.Label);
            }
            chkTables.ItemsSource = null;
            chkTables.ItemsSource = itemsTab;
        }
    }
}
