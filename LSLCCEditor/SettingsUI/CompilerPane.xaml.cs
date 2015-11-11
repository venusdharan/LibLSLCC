﻿#region FileInfo

// 
// File: CompilerPane.xaml.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Eric A. Blundell
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibLSLCC.Collections;
using LibLSLCC.Compilers;
using LSLCCEditor.Settings;

namespace LSLCCEditor.SettingsUI
{
    /// <summary>
    /// Interaction logic for CompilerPane.xaml
    /// </summary>
    public partial class CompilerPane : UserControl, ISettingsPane
    {
        public ObservableCollection<string> CompilerConfigurations { get; set; }


        public static readonly DependencyProperty SelectedCompilerConfigurationProperty = DependencyProperty.Register(
            "SelectedCompilerConfiguration", typeof (string), typeof (CompilerPane),
            new PropertyMetadata(default(string), SelectedCompilerConfigurationChanged));

        private static void SelectedCompilerConfigurationChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var pane = ((CompilerPane) dependencyObject);


            pane.CurrentCompilerConfiguration =
                (CompilerSettingsNode)
                AppSettings.Settings.CompilerConfigurations[dependencyPropertyChangedEventArgs.NewValue.ToString()].Clone();


            pane.ConstructorAccessibilityLevel =
                pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedConstructorAccessibility;


            pane.NamespaceImports = new ObservableCollection<NamespaceImport>(pane.CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedNamespaceImports);
        }



        public static readonly DependencyProperty NamespaceImportsProperty = DependencyProperty.Register(
            "NamespaceImports", typeof (ObservableCollection<NamespaceImport>), typeof (CompilerPane), new PropertyMetadata(default(ObservableCollection<NamespaceImport>)));

        public ObservableCollection<NamespaceImport> NamespaceImports
        {
            get { return (ObservableCollection<NamespaceImport>) GetValue(NamespaceImportsProperty); }
            set { SetValue(NamespaceImportsProperty, value); }
        }

        public static readonly DependencyProperty CurrentCompilerConfigurationProperty = DependencyProperty.Register(
            "CurrentCompilerConfiguration", typeof (CompilerSettingsNode), typeof (CompilerPane),
            new PropertyMetadata(default(CompilerSettingsNode)));



        public CompilerSettingsNode CurrentCompilerConfiguration
        {
            get { return (CompilerSettingsNode) GetValue(CurrentCompilerConfigurationProperty); }
            set { SetValue(CurrentCompilerConfigurationProperty, value); }
        }


        public string SelectedCompilerConfiguration
        {
            get { return (string) GetValue(SelectedCompilerConfigurationProperty); }
            set { SetValue(SelectedCompilerConfigurationProperty, value); }
        }


        public CompilerPane()
        {
            this.DataContext = this;

            Title = "Compiler Settings";

            CompilerConfigurations = new ObservableCollection<string>(ApplicationSettings.CompilerConfigurations.Keys);
            NamespaceImports = new ObservableCollection<NamespaceImport>();


            InitializeComponent();
            

            SelectedCompilerConfiguration = CompilerConfigurations.First();
            
        }


        public static readonly DependencyProperty ConstructorAccessibility = DependencyProperty.Register(
            "ConstructorAccessibility", typeof (AccessibilityLevel), typeof (CompilerPane), new PropertyMetadata(default(AccessibilityLevel)));

        public AccessibilityLevel ConstructorAccessibilityLevel
        {
            get { return (AccessibilityLevel) GetValue(ConstructorAccessibility); }
            set { SetValue(ConstructorAccessibility, value); }
        }


        public SettingsNode ApplicationSettings
        {
            get { return AppSettings.Settings; }
        }

        public int Priority
        {
            get { return 1; }
        }

        public string Title { get; private set; }

        public void Init(SettingsWindow window)
        {

        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedNamespaceImports = new ObservableHashSet<NamespaceImport>(this.NamespaceImports);
            AppSettings.Settings.CompilerConfigurations[SelectedCompilerConfiguration] = CurrentCompilerConfiguration;
            AppSettings.Save();
        }

        private void RevertButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentCompilerConfiguration =
                (CompilerSettingsNode)AppSettings.Settings.CompilerConfigurations[SelectedCompilerConfiguration].Clone();
        }

 
        private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var ns = e.Row.Item as NamespaceImport;

            if (string.IsNullOrWhiteSpace(ns.Name))
            {
                e.Cancel = true;
            }
            else if (CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedNamespaceImports.Contains(ns))
            {
                e.Cancel = true;
            }
            else
            {
                CurrentCompilerConfiguration.OpenSimCompilerSettings.GeneratedNamespaceImports.Add(ns);
            }
        }

    }





}