﻿using System;
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

        private List<CheckListItem> items = new List<CheckListItem>();

        public List<string> Tables { get; set; }

        public DatabaseInfo DBInfo { get; set; }

        /// <summary>
        /// Load tables and views
        /// </summary>
        public void LoadTables()
        {
            using (var repository = RepositoryHelper.CreateRepository(DBInfo))
            {
                var tabList = new List<string>();
                tabList  = repository.GetAllTableNamesForExclusion();
                if (IncludeViews)
                {
                    tabList.AddRange(repository.GetAllViewNamesForExclusion());
                }
                Tables = tabList;
            }
            BindTables();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTables();
        }

        private void BindTables()
        {
            items = new List<CheckListItem>();
            foreach (var table in Tables)
            {
                var isChecked = !table.StartsWith("__");
                isChecked = !table.StartsWith("dbo.__");
                items.Add(new CheckListItem { IsChecked = isChecked, Label = table });
            }
            chkTables.ItemsSource = items;
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
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void chkView_Click(object sender, RoutedEventArgs e)
        {
            IncludeViews = chkViews.IsChecked == true;
            if (IncludeViews)
                chkClear.IsChecked = true;
            LoadTables();
        }

        private void chkClear_Click(object sender, RoutedEventArgs e)
        {
            if (chkClear.IsChecked != null && chkClear.IsChecked.Value)
            {
                foreach (var item in items)
                {
                    if (!item.IsChecked)
                    {
                        item.IsChecked = true;
                    }
                }
            }
            else
            {
                foreach (var item in items)
                {
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                    }
                }
            }
            chkTables.ItemsSource = null;
            chkTables.ItemsSource = items;
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
            foreach (var item in items)
            {
                item.IsChecked = lines.Contains(item.Label);
            }
            chkTables.ItemsSource = null;
            chkTables.ItemsSource = items;
        }
    }
}
