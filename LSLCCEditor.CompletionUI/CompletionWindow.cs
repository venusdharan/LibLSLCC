﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace LSLCCEditor.CompletionUI
{
    /// <summary>
    /// The code completion window.
    /// </summary>
    public class CompletionWindow : CompletionWindowBase
    {

        public static readonly DependencyProperty SelectedItemBackgroundProperty = DependencyProperty.Register(
            "SelectedItemBackground", typeof (Brush), typeof (CompletionWindow), new PropertyMetadata(default(Brush)));

        public Brush SelectedItemBackground
        {
            get { return (Brush) GetValue(SelectedItemBackgroundProperty); }
            set { SetValue(SelectedItemBackgroundProperty, value); }
        }


        public static readonly DependencyProperty SelectedItemForegroundProperty = DependencyProperty.Register(
            "SelectedItemForeground", typeof(Brush), typeof(CompletionWindow), new PropertyMetadata(default(Brush)));

        public Brush SelectedItemForeground
        {
            get { return (Brush)GetValue(SelectedItemForegroundProperty); }
            set { SetValue(SelectedItemForegroundProperty, value); }
        }


        public static readonly DependencyProperty SelectedItemBorderBrushProperty = DependencyProperty.Register(
    "SelectedItemBorderBrush", typeof(Brush), typeof(CompletionWindow), new PropertyMetadata(default(Brush)));

        public Brush SelectedItemBorderBrush
        {
            get { return (Brush)GetValue(SelectedItemBorderBrushProperty); }
            set { SetValue(SelectedItemBorderBrushProperty, value); }
        }




        readonly CompletionList completionList = new CompletionList();
        ToolTip toolTip = new ToolTip();

        /// <summary>
        /// Gets the completion list used in this completion window.
        /// </summary>
        public CompletionList CompletionList
        {
            get { return completionList; }
        }


        public static readonly DependencyProperty ToolTipHorizontalOffsetProperty = DependencyProperty.Register(
            "ToolTipHorizontalOffset", typeof (int), typeof (CompletionWindow), new PropertyMetadata(default(int), ToolTipHorizontalOffsetChanged));

        private static void ToolTipHorizontalOffsetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var self = (CompletionWindow)dependencyObject;
            self.toolTip.HorizontalOffset = (int)dependencyPropertyChangedEventArgs.NewValue;
        }

        public int ToolTipHorizontalOffset
        {
            get { return (int) GetValue(ToolTipHorizontalOffsetProperty); }
            set { SetValue(ToolTipHorizontalOffsetProperty, value); }
        }


        public static readonly DependencyProperty ToolTipVerticalOffsetProperty = DependencyProperty.Register(
            "ToolTipVerticalOffset", typeof (int), typeof (CompletionWindow), new PropertyMetadata(default(int), ToolTipVerticalOffsetChanged));

        private static void ToolTipVerticalOffsetChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var self = (CompletionWindow) dependencyObject;
            self.toolTip.VerticalOffset = (int)dependencyPropertyChangedEventArgs.NewValue;
        }

        public int ToolTipVerticalOffset
        {
            get { return (int) GetValue(ToolTipVerticalOffsetProperty); }
            set { SetValue(ToolTipVerticalOffsetProperty, value); }
        }


        /// <summary>
        /// Creates a new code completion window.
        /// </summary>
        public CompletionWindow(TextArea textArea) : base(textArea)
        {
            // keep height automatic
            CloseAutomatically = true;
            SizeToContent = SizeToContent.Height;
            MaxHeight = 300;
            Width = 175;
            Content = completionList;
            // prevent user from resizing window to 0x0
            MinHeight = 15;
            MinWidth = 30;

            toolTip.PlacementTarget = this;
            toolTip.Placement = PlacementMode.Right;
            toolTip.Closed += toolTip_Closed;
            

            AttachEvents();
        }

        #region ToolTip handling
        void toolTip_Closed(object sender, RoutedEventArgs e)
        {
            // Clear content after tooltip is closed.
            // We cannot clear is immediately when setting IsOpen=false
            // because the tooltip uses an animation for closing.
            if (toolTip != null)
                toolTip.Content = null;
        }

        void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = completionList.SelectedItem;
            if (item == null)
                return;
            object description = item.Description;
            if (description != null)
            {
                string descriptionText = description as string;
                if (descriptionText != null)
                {
                    toolTip.Content = new TextBlock
                    {
                        Text = descriptionText,
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    toolTip.Content = description;
                }

                InitToolTipSettings();

                toolTip.IsOpen = true;
            }
            else
            {
                toolTip.IsOpen = false;
            }
        }

        private void InitToolTipSettings()
        {
            if (ToolTipForeground != null)
            {
                toolTip.Foreground = ToolTipForeground;
            }

            if (ToolTipBackground != null)
            {
                toolTip.Background = ToolTipBackground;
            }

            if (ToolTipBorderBrush != null)
            {
                toolTip.BorderBrush = ToolTipBorderBrush;
            }

        }

        public static readonly DependencyProperty ToolTipBorderBrushProperty = DependencyProperty.Register(
            "ToolTipBorderBrush", typeof (Brush), typeof (CompletionWindow), new PropertyMetadata(default(Brush)));

        public Brush ToolTipBorderBrush
        {
            get { return (Brush) GetValue(ToolTipBorderBrushProperty); }
            set { SetValue(ToolTipBorderBrushProperty, value); }
        }


        public static readonly DependencyProperty ToolTipBackgroundProperty = DependencyProperty.Register(
            "ToolTipBackground", typeof (Brush), typeof (CompletionWindow), new PropertyMetadata(default(Brush)));

        public Brush ToolTipBackground
        {
            get { return (Brush) GetValue(ToolTipBackgroundProperty); }
            set { SetValue(ToolTipBackgroundProperty, value); }
        }


        public static readonly DependencyProperty ToolTipForegroundProperty = DependencyProperty.Register(
            "ToolTipForeground", typeof (Brush), typeof (CompletionWindow), new PropertyMetadata(default(Brush)));

        public Brush ToolTipForeground
        {
            get { return (Brush) GetValue(ToolTipForegroundProperty); }
            set { SetValue(ToolTipForegroundProperty, value); }
        }


        #endregion

        void completionList_InsertionRequested(object sender, EventArgs e)
        {
            Close();
            // The window must close before Complete() is called.
            // If the Complete callback pushes stacked input handlers, we don't want to pop those when the CC window closes.
            var item = completionList.SelectedItem;
            if (item != null)
                item.Complete(TextArea, new AnchorSegment(TextArea.Document, StartOffset, EndOffset - StartOffset), e);
        }

        void AttachEvents()
        {
            completionList.InsertionRequested += completionList_InsertionRequested;
            completionList.SelectionChanged += completionList_SelectionChanged;
            completionList.CompletionDataAdded += CompletionListOnCompletionDataAdded;
            TextArea.Caret.PositionChanged += CaretPositionChanged;
            TextArea.MouseWheel += textArea_MouseWheel;
            TextArea.PreviewTextInput += textArea_PreviewTextInput;
        }


        public static readonly DependencyProperty SizeToCompletionTextBlockContentProperty = DependencyProperty.Register(
            "SizeToCompletionTextBlockContent", typeof (bool), typeof (CompletionWindow), new PropertyMetadata(default(bool)));

        public bool SizeToCompletionTextBlockContent
        {
            get { return (bool) GetValue(SizeToCompletionTextBlockContentProperty); }
            set { SetValue(SizeToCompletionTextBlockContentProperty, value); }
        }


        private void CompletionListOnCompletionDataAdded(object sender, CompletionDataAddedEventArgs completionDataAddedEventArgs)
        {
            if (!SizeToCompletionTextBlockContent) return;

            var tBox = completionDataAddedEventArgs.CompletionData.Content as TextBlock;
            if (tBox == null) return;


            var formattedText = new FormattedText(
                tBox.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tBox.FontFamily, tBox.FontStyle, tBox.FontWeight, tBox.FontStretch),
                tBox.FontSize,
                Brushes.Black);

            if (double.IsNaN(Width) || Width < formattedText.Width)
            {
                Width = formattedText.Width + 80;
            }
        }

        /// <inheritdoc/>
        protected override void DetachEvents()
        {
            completionList.InsertionRequested -= completionList_InsertionRequested;
            completionList.SelectionChanged -= completionList_SelectionChanged;
            completionList.CompletionDataAdded -= CompletionListOnCompletionDataAdded;
            TextArea.Caret.PositionChanged -= CaretPositionChanged;
            TextArea.MouseWheel -= textArea_MouseWheel;
            TextArea.PreviewTextInput -= textArea_PreviewTextInput;
            base.DetachEvents();
        }

        /// <inheritdoc/>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                toolTip = null;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                completionList.HandleKey(e);
            }
        }

        void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
                                       new TextCompositionEventArgs(e.Device, e.TextComposition));
        }

        void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = RaiseEventPair(GetScrollEventTarget(),
                                       PreviewMouseWheelEvent, MouseWheelEvent,
                                       new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
        }

        UIElement GetScrollEventTarget()
        {
            if (completionList == null)
                return this;
            return completionList.ScrollViewer ?? completionList.ListBox ?? (UIElement)completionList;
        }

        /// <summary>
        /// Gets/Sets whether the completion window should close automatically.
        /// The default value is true.
        /// </summary>
        public bool CloseAutomatically { get; set; }

        /// <inheritdoc/>
        protected override bool CloseOnFocusLost
        {
            get { return CloseAutomatically; }
        }

        /// <summary>
        /// When this flag is set, code completion closes if the caret moves to the
        /// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
        /// but not in dot-completion.
        /// Has no effect if CloseAutomatically is false.
        /// </summary>
        public bool CloseWhenCaretAtBeginning { get; set; }

        void CaretPositionChanged(object sender, EventArgs e)
        {
            int offset = TextArea.Caret.Offset;
            if (offset == StartOffset)
            {
                if (CloseAutomatically && CloseWhenCaretAtBeginning)
                {
                    if (!InsertingIntoSelection)
                    {
                        Close();
                    }
                }
                else
                {
                    completionList.SelectItem(string.Empty);
                }
                return;
            }
            if (offset < StartOffset || offset > EndOffset)
            {
                if (CloseAutomatically)
                {
                    Close();
                }
            }
            else
            {
                TextDocument document = TextArea.Document;
                if (document != null)
                {
                    completionList.SelectItem(document.GetText(StartOffset, offset - StartOffset));
                }
            }
        }
    }
}