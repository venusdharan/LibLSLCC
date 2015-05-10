﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace FindReplace
{
    /// <summary>
    /// This class ensures that the settings and text to be found is preserved when the find/replace dialog is closed
    /// 
    /// We need two-way binding, otherwise we could just make all properties static properties of the window
    /// </summary>
    public class FindReplaceMgr : DependencyObject
    {
        private FindReplaceDialog _dialog;

        /// <summary>
        /// Instance of the dialog window
        /// </summary>
        private FindReplaceDialog dialog
        {
            get
            {
                if (_dialog == null)
                {
                    _dialog = new FindReplaceDialog(this);
                    _dialog.Closed += delegate { _dialog = null; };
                    if (OwnerWindow != null)
                        _dialog.Owner = OwnerWindow;
                }
                return _dialog;
            }
        }



        public FindReplaceMgr()
        {
            ReplacementText = "";

            SearchIn = SearchScope.CurrentDocument;
            ShowSearchIn = true;
        }




        #region Exposed CommandBindings


        public CommandBinding FindBinding
        {
            get { return new CommandBinding(ApplicationCommands.Find, (s, e) => ShowAsFind()); }
        }

        public CommandBinding FindNextBinding
        {
            get
            {
                return new CommandBinding(NavigationCommands.Search,
                    (s, e) => FindNext(e.Parameter != null));
            }
        }

        public CommandBinding ReplaceBinding
        {
            get
            {
                return new CommandBinding(ApplicationCommands.Replace, (s, e) => { if (AllowReplace) ShowAsReplace(); });
            }
        }


        #endregion




        #region Public Properties


        /// <summary>
        /// The list of editors in which the search should take place.
        /// The elements must either implement the IEditor interface, or 
        /// InterfaceConverter should bne set.
        /// </summary>       
        public IEnumerable Editors
        {
            get { return (IEnumerable) GetValue(EditorsProperty); }
            set { SetValue(EditorsProperty, value); }
        }

        public static readonly DependencyProperty EditorsProperty =
            DependencyProperty.Register("Editors", typeof (IEnumerable), typeof (FindReplaceMgr),
                new PropertyMetadata(null));


        /// <summary>
        /// The editor in which the current search operation takes place.
        /// </summary>
        public object CurrentEditor
        {
            get { return GetValue(CurrentEditorProperty); }
            set { SetValue(CurrentEditorProperty, value); }
        }

        public static readonly DependencyProperty CurrentEditorProperty =
            DependencyProperty.Register("CurrentEditor", typeof (object), typeof (FindReplaceMgr),
                new PropertyMetadata(0));


        /// <summary>
        /// Objects in the Editors list that do not implement the IEditor interface are converted to IEditor using this converter.
        /// </summary>
        public IValueConverter InterfaceConverter
        {
            get { return (IValueConverter) GetValue(InterfaceConverterProperty); }
            set { SetValue(InterfaceConverterProperty, value); }
        }

        public static readonly DependencyProperty InterfaceConverterProperty =
            DependencyProperty.Register("InterfaceConverter", typeof (IValueConverter), typeof (FindReplaceMgr),
                new PropertyMetadata(null));

        public static readonly DependencyProperty TextToFindProperty =
            DependencyProperty.Register("TextToFind", typeof (string),
                typeof (FindReplaceMgr), new UIPropertyMetadata(""));

        public string TextToFind
        {
            get { return (string) GetValue(TextToFindProperty); }
            set { SetValue(TextToFindProperty, value); }
        }

        // public string ReplacementText { get; set; }
        public string ReplacementText
        {
            get { return (string) GetValue(ReplacementTextProperty); }
            set { SetValue(ReplacementTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReplacementText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReplacementTextProperty =
            DependencyProperty.Register("ReplacementText", typeof (string), typeof (FindReplaceMgr),
                new UIPropertyMetadata(""));


        public bool UseWildcards
        {
            get { return (bool) GetValue(UseWildcardsProperty); }
            set { SetValue(UseWildcardsProperty, value); }
        }

        public static readonly DependencyProperty UseWildcardsProperty =
            DependencyProperty.Register("UseWildcards", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(false));

        public bool SearchUp
        {
            get { return (bool) GetValue(SearchUpProperty); }
            set { SetValue(SearchUpProperty, value); }
        }

        public static readonly DependencyProperty SearchUpProperty =
            DependencyProperty.Register("SearchUp", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(false));

        public bool CaseSensitive
        {
            get { return (bool) GetValue(CaseSensitiveProperty); }
            set { SetValue(CaseSensitiveProperty, value); }
        }

        public static readonly DependencyProperty CaseSensitiveProperty =
            DependencyProperty.Register("CaseSensitive", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(false));

        public bool UseRegEx
        {
            get { return (bool) GetValue(UseRegExProperty); }
            set { SetValue(UseRegExProperty, value); }
        }

        public static readonly DependencyProperty UseRegExProperty =
            DependencyProperty.Register("UseRegEx", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(false));

        public bool WholeWord
        {
            get { return (bool) GetValue(WholeWordProperty); }
            set { SetValue(WholeWordProperty, value); }
        }

        public static readonly DependencyProperty WholeWordProperty =
            DependencyProperty.Register("WholeWord", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(false));

        public bool AcceptsReturn
        {
            get { return (bool) GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        public static readonly DependencyProperty AcceptsReturnProperty =
            DependencyProperty.Register("AcceptsReturn", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(false));

        public enum SearchScope
        {
            CurrentDocument,
            AllDocuments
        }

        public SearchScope SearchIn
        {
            get { return (SearchScope) GetValue(SearchInProperty); }
            set { SetValue(SearchInProperty, value); }
        }

        public static readonly DependencyProperty SearchInProperty =
            DependencyProperty.Register("SearchIn", typeof (SearchScope), typeof (FindReplaceMgr),
                new UIPropertyMetadata(SearchScope.CurrentDocument));

        public double WindowLeft
        {
            get { return (double) GetValue(WindowLeftProperty); }
            set { SetValue(WindowLeftProperty, value); }
        }

        public static readonly DependencyProperty WindowLeftProperty =
            DependencyProperty.Register("WindowLeft", typeof (double), typeof (FindReplaceMgr),
                new UIPropertyMetadata(100.0));

        public double WindowTop
        {
            get { return (double) GetValue(WindowTopProperty); }
            set { SetValue(WindowTopProperty, value); }
        }

        public static readonly DependencyProperty WindowTopProperty =
            DependencyProperty.Register("WindowTop", typeof (double), typeof (FindReplaceMgr),
                new UIPropertyMetadata(100.0));

        /// <summary>
        /// Determines whether to display the Search in combo box
        /// </summary>
        public bool ShowSearchIn
        {
            get { return (bool) GetValue(ShowSearchInProperty); }
            set { SetValue(ShowSearchInProperty, value); }
        }

        public static readonly DependencyProperty ShowSearchInProperty =
            DependencyProperty.Register("ShowSearchIn", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(true));


        /// <summary>
        /// Determines whether the "Replace"-page in the dialog in shown or not.
        /// </summary>
        public bool AllowReplace
        {
            get { return (bool) GetValue(AllowReplaceProperty); }
            set { SetValue(AllowReplaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowReplace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowReplaceProperty =
            DependencyProperty.Register("AllowReplace", typeof (bool), typeof (FindReplaceMgr),
                new UIPropertyMetadata(true));


        /// <summary>
        /// The Window that serves as the parent of the Find/Replace dialog
        /// </summary>
        public Window OwnerWindow
        {
            get { return (Window) GetValue(OwnerWindowProperty); }
            set { SetValue(OwnerWindowProperty, value); }
        }

        public static readonly DependencyProperty OwnerWindowProperty =
            DependencyProperty.Register("OwnerWindow", typeof (Window), typeof (FindReplaceMgr),
                new UIPropertyMetadata(null));


        #endregion




        private IEditor GetCurrentEditor()
        {
            if (CurrentEditor == null)
                return null;
            if (CurrentEditor is IEditor)
                return CurrentEditor as IEditor;
            if (InterfaceConverter == null)
                return null;

            return
                InterfaceConverter.Convert(CurrentEditor, typeof (IEditor), null, CultureInfo.CurrentCulture) as IEditor;
        }



        private IEditor GetNextEditor(bool previous = false)
        {
            if (!ShowSearchIn || SearchIn == SearchScope.CurrentDocument || Editors == null)
                return GetCurrentEditor();

            List<object> l = new List<object>(Editors.Cast<object>());
            int i = l.IndexOf(CurrentEditor);
            if (i >= 0)
            {
                i = (i + (previous ? l.Count - 1 : +1))%l.Count;
                CurrentEditor = l[i];
            }
            return GetCurrentEditor();
        }



        [Serializable]
        public class FindReplaceMalformedRegex : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //

            public FindReplaceMalformedRegex()
            {
            }



            public FindReplaceMalformedRegex(string message) : base(message)
            {
            }



            public FindReplaceMalformedRegex(string message, Exception inner) : base(message, inner)
            {
            }



            protected FindReplaceMalformedRegex(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }



        /// <summary>
        /// Constructs a regular expression according to the currently selected search parameters.
        /// </summary>
        /// <param name="ForceLeftToRight"></param>
        /// <returns>The regular expression.</returns>
        public Regex GetRegEx(bool ForceLeftToRight = false)
        {
            try
            {
                Regex r;
                RegexOptions o = RegexOptions.None;
                if (SearchUp && !ForceLeftToRight)
                    o = o | RegexOptions.RightToLeft;
                if (!CaseSensitive)
                    o = o | RegexOptions.IgnoreCase;

                if (UseRegEx)
                    r = new Regex(TextToFind, o);
                else
                {
                    string s = Regex.Escape(TextToFind);
                    if (UseWildcards)
                        s = s.Replace("\\*", ".*").Replace("\\?", ".");
                    if (WholeWord)
                        s = "\\b" + s + "\\b";
                    r = new Regex(s, o);
                }
                return r;
            }
            catch (ArgumentException e)
            {
                throw new FindReplaceMalformedRegex(e.Message, e);
            }
        }



        public void ReplaceAll(bool AskBefore = true)
        {
            try
            {
                IEditor CE = GetCurrentEditor();
                if (CE == null) return;

                if (!AskBefore ||
                    MessageBox.Show(
                        "Do you really want to replace all occurrences of '" + TextToFind + "' with '" + ReplacementText +
                        "'?",
                        "Replace all", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) ==
                    MessageBoxResult.Yes)
                {
                    object InitialEditor = CurrentEditor;
                    // loop through all editors, until we are back at the starting editor                
                    do
                    {
                        Regex r = GetRegEx(true); // force left to right, otherwise indices are screwed up
                        int offset = 0;
                        CE.BeginChange();
                        foreach (Match m in r.Matches(CE.Text))
                        {
                            CE.Replace(offset + m.Index, m.Length, ReplacementText);
                            offset += ReplacementText.Length - m.Length;
                        }
                        CE.EndChange();
                        CE = GetNextEditor();
                    } while (CurrentEditor != InitialEditor);
                }
            }
            catch (FindReplaceMalformedRegex e)
            {
                MessageBox.Show(e.Message, "Regex Syntax Error");
            }
        }



        /// <summary>
        /// Shows this instance of FindReplaceDialog, with the Find page active
        /// </summary>
        public void ShowAsFind()
        {
            dialog.tabMain.SelectedIndex = 0;
            dialog.Show();
            dialog.Activate();
            dialog.txtFind.Focus();
            dialog.txtFind.SelectAll();
        }



        public void ShowAsFind(TextEditor target)
        {
            CurrentEditor = target;
            ShowAsFind();
        }



        /// <summary>
        /// Shows this instance of FindReplaceDialog, with the Replace page active
        /// </summary>
        public void ShowAsReplace()
        {
            dialog.tabMain.SelectedIndex = 1;
            dialog.Show();
            dialog.Activate();
            dialog.txtFind2.Focus();
            dialog.txtFind2.SelectAll();
        }



        public void ShowAsReplace(object target)
        {
            CurrentEditor = target;
            ShowAsReplace();
        }



        //static TextEditor txtCode;
        public void FindNext(object target, bool InvertLeftRight = false)
        {
            CurrentEditor = target;
            FindNext(InvertLeftRight);
        }



        public void FindNext(bool InvertLeftRight = false)
        {
            try
            {
                IEditor CE = GetCurrentEditor();
                if (CE == null) return;
                Regex r;
                if (InvertLeftRight)
                {
                    SearchUp = !SearchUp;
                    r = GetRegEx();
                    SearchUp = !SearchUp;
                }
                else
                    r = GetRegEx();

                Match m = r.Match(CE.Text,
                    r.Options.HasFlag(RegexOptions.RightToLeft)
                        ? CE.SelectionStart
                        : CE.SelectionStart + CE.SelectionLength);
                if (m.Success)
                {
                    CE.Select(m.Index, m.Length);
                }
                else
                {
                    // we have reached the end of the document
                    // start again from the beginning/end,
                    object OldEditor = CurrentEditor;
                    do
                    {
                        if (ShowSearchIn)
                        {
                            CE = GetNextEditor(r.Options.HasFlag(RegexOptions.RightToLeft));
                            if (CE == null) return;
                        }
                        if (r.Options.HasFlag(RegexOptions.RightToLeft))
                            m = r.Match(CE.Text, CE.Text.Length);
                        else
                            m = r.Match(CE.Text, 0);
                        if (m.Success)
                        {
                            CE.Select(m.Index, m.Length);
                            break;
                        }
                    } while (CurrentEditor != OldEditor);
                }
            }
            catch (FindReplaceMalformedRegex e)
            {
                MessageBox.Show(e.Message, "Regex Syntax Error");
            }
        }



        public void FindPrevious()
        {
            FindNext(true);
        }



        public void Replace()
        {
            try
            {
                IEditor CE = GetCurrentEditor();
                if (CE == null) return;

                // if currently selected text matches -> replace; anyways, find the next match
                Regex r = GetRegEx();
                string s = CE.Text.Substring(CE.SelectionStart, CE.SelectionLength); // CE.SelectedText;
                Match m = r.Match(s);
                if (m.Success && m.Index == 0 && m.Length == s.Length)
                {
                    CE.Replace(CE.SelectionStart, CE.SelectionLength, ReplacementText);
                    //CE.SelectedText = ReplacementText;
                }

                FindNext();
            }
            catch (FindReplaceMalformedRegex e)
            {
                MessageBox.Show(e.Message, "Regex Syntax Error");
            }
        }



        /// <summary>
        /// Closes the Find/Replace dialog, if it is open
        /// </summary>
        public void CloseWindow()
        {
            dialog.Close();
        }
    }

    public class SearchScopeToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) value;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FindReplaceMgr.SearchScope) value;
        }
    }

    public class BoolToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
                return 1;
            return 0;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public interface IEditor
    {
        string Text { get; }
        int SelectionStart { get; }
        int SelectionLength { get; }



        /// <summary>
        /// Selects the specified portion of Text and scrolls that part into view.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        void Select(int start, int length);



        void Replace(int start, int length, string ReplaceWith);



        /// <summary>
        /// This method is called before a replace all operation.
        /// </summary>
        void BeginChange();



        /// <summary>
        /// This method is called after a replace all operation.
        /// </summary>
        void EndChange();
    }
}