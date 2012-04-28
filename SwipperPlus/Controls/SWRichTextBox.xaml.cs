using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SwipperPlus.Controls
{
  public partial class SWRichTextBox : UserControl
  {

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(string), typeof(SWRichTextBox), new PropertyMetadata(null, OnTextPropertyChanged));

    private static void OnTextPropertyChanged(DependencyObject re, DependencyPropertyChangedEventArgs e)
    {
      SWRichTextBox richEdit = (SWRichTextBox)re;
      if (richEdit.richTextBox.Xaml != (string)e.NewValue)
      {
        try
        {
          richEdit.richTextBox.Blocks.Clear();

          if (string.IsNullOrEmpty((string)e.NewValue) == false)
          {
            richEdit.richTextBox.Xaml = (string)e.NewValue;
          }
        }
        catch
        {
          richEdit.richTextBox.Blocks.Clear();

          if (string.IsNullOrEmpty((string)e.NewValue) == false)
          {
            richEdit.richTextBox.Selection.Text = (string)e.NewValue;
          }
        }
      }
    }

    public SWRichTextBox()
    {
      InitializeComponent();
    }
  }
}
