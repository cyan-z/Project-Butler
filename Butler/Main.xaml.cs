﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Butler {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        CommandLine CMD;
        public bool Ready = false;

        #region Hotkey Registration
        HotkeyHandler kHandler;
        private void Main_HotkeyRegister(object sender, EventArgs e) {
            kHandler = new HotkeyHandler(this);
            kHandler.HotkeyPressed += HotkeyWasPressed;
        }
        private void Main_HotkeyDeregister(object sender, System.ComponentModel.CancelEventArgs e) {
            kHandler.Dispose();
        }
        #endregion
        //Current Hotkey = WinKey + Escape
        private void HotkeyWasPressed() {
            switch(CMD.Visibility) {
                case Visibility.Visible:
                CMD.Hide();
                break;
                case Visibility.Hidden:
                case Visibility.Collapsed:
                CMD.Show();
                break;
            }

        }


        public MainWindow() {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            this.SourceInitialized += Main_HotkeyRegister;  //Inside Hotkey Registration Region
            this.Closing += Main_HotkeyDeregister;          //Inside Hotkey Registration Region

            this.Closing += (sender, e) => {
                TaskbarIconManager.Dispose();
                System.Windows.Application.Current.Shutdown(0);
            };

            this.Loaded += SetupComponents;
            this.StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e) {
            switch(this.WindowState) {
                case WindowState.Minimized: ShowInTaskbar = false;
                    break;
                case WindowState.Maximized:
                case WindowState.Normal: ShowInTaskbar = true;
                    break;
            }
        }

        private void SetupComponents(object sender, RoutedEventArgs e) {
            CMD = CommandLine.GetInstance();

            //Taskbar notification icon
            TaskbarIconManager.AddItem("Show", () => {
                WindowState = WindowState.Normal;
            });
            TaskbarIconManager.AddItem("Exit", () => {
                System.Windows.Application.Current.Shutdown(0);
            });

            TaskbarIconManager.CommitItems();
            TaskbarIconManager.SetActive(true);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            ModuleLoader.LoadAll();
            ModuleDataGrid.ItemsSource = ModuleLoader.ModuleLoadOrder;
        }

    }
}