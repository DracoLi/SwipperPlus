using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace SwipperPlus.Views
{
  public partial class ImageViewer : PhoneApplicationPage
  {
    // This works as expected and includes all the bells and whistles.

    public ImageViewer()
    {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      // Load image if not loaded
      if (ImgZoom.Source == null)
      {
        // Show loading screen
        showLoadingScreen();

        string largeLink = NavigationContext.QueryString["large"];
        ImgZoom.Source = new BitmapImage(new Uri(largeLink));
        ImgZoom.ImageOpened += new EventHandler<RoutedEventArgs>(ImgZoom_ImageOpened);
        ImgZoom.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(ImgZoom_ImageFailed);
      }
    }

    void ImgZoom_ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
      // Show error and go back
      MessageBoxResult rs = MessageBox.Show("Image Failed To Load");
      if (rs == MessageBoxResult.OK)
        NavigationService.GoBack();
    }

    void ImgZoom_ImageOpened(object sender, RoutedEventArgs e)
    {
      // Stop loading screen
      stopLoadingScreen();
    }

    void showLoadingScreen()
    {
      progressBar.IsIndeterminate = true;
    }

    void stopLoadingScreen()
    {
      progressBar.IsIndeterminate = false;
    }

    // these two fields fully define the zoom state:
    private double TotalImageScale = 1d;
    private Point ImagePosition = new Point(0, 0);


    private const double MAX_IMAGE_ZOOM = 5;
    private Point _oldFinger1;
    private Point _oldFinger2;
    private double _oldScaleFactor;


    #region Event handlers

    /// <summary>
    /// Initializes the zooming operation
    /// </summary>
    private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
    {
      _oldFinger1 = e.GetPosition(ImgZoom, 0);
      _oldFinger2 = e.GetPosition(ImgZoom, 1);
      _oldScaleFactor = 1;
    }

    /// <summary>
    /// Computes the scaling and translation to correctly zoom around your fingers.
    /// </summary>
    private void OnPinchDelta(object sender, PinchGestureEventArgs e)
    {
      var scaleFactor = e.DistanceRatio / _oldScaleFactor;
      if (!IsScaleValid(scaleFactor))
        return;

      var currentFinger1 = e.GetPosition(ImgZoom, 0);
      var currentFinger2 = e.GetPosition(ImgZoom, 1);

      var translationDelta = GetTranslationDelta(
          currentFinger1,
          currentFinger2,
          _oldFinger1,
          _oldFinger2,
          ImagePosition,
          scaleFactor);

      _oldFinger1 = currentFinger1;
      _oldFinger2 = currentFinger2;
      _oldScaleFactor = e.DistanceRatio;

      UpdateImageScale(scaleFactor);
      UpdateImagePosition(translationDelta);
    }

    /// <summary>
    /// Moves the image around following your finger.
    /// </summary>
    private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
    {
      var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

      if (IsDragValid(1, translationDelta))
        UpdateImagePosition(translationDelta);
    }

    /// <summary>
    /// Resets the image scaling and position
    /// </summary>
    private void OnDoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
    {
      ResetImagePosition();
    }

    #endregion

    #region Utils

    /// <summary>
    /// Computes the translation needed to keep the image centered between your fingers.
    /// </summary>
    private Point GetTranslationDelta(
        Point currentFinger1, Point currentFinger2,
        Point oldFinger1, Point oldFinger2,
        Point currentPosition, double scaleFactor)
    {
      var newPos1 = new Point(
       currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
       currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

      var newPos2 = new Point(
       currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
       currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

      var newPos = new Point(
          (newPos1.X + newPos2.X) / 2,
          (newPos1.Y + newPos2.Y) / 2);

      return new Point(
          newPos.X - currentPosition.X,
          newPos.Y - currentPosition.Y);
    }

    /// <summary>
    /// Updates the scaling factor by multiplying the delta.
    /// </summary>
    private void UpdateImageScale(double scaleFactor)
    {
      TotalImageScale *= scaleFactor;
      ApplyScale();
    }

    /// <summary>
    /// Applies the computed scale to the image control.
    /// </summary>
    private void ApplyScale()
    {
      ((CompositeTransform)ImgZoom.RenderTransform).ScaleX = TotalImageScale;
      ((CompositeTransform)ImgZoom.RenderTransform).ScaleY = TotalImageScale;
    }

    /// <summary>
    /// Updates the image position by applying the delta.
    /// Checks that the image does not leave empty space around its edges.
    /// </summary>
    private void UpdateImagePosition(Point delta)
    {
      var newPosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

      if (newPosition.X > 0) newPosition.X = 0;
      if (newPosition.Y > 0) newPosition.Y = 0;

      if ((ImgZoom.ActualWidth * TotalImageScale) + newPosition.X < ImgZoom.ActualWidth)
        newPosition.X = ImgZoom.ActualWidth - (ImgZoom.ActualWidth * TotalImageScale);

      if ((ImgZoom.ActualHeight * TotalImageScale) + newPosition.Y < ImgZoom.ActualHeight)
        newPosition.Y = ImgZoom.ActualHeight - (ImgZoom.ActualHeight * TotalImageScale);

      ImagePosition = newPosition;

      ApplyPosition();
    }

    /// <summary>
    /// Applies the computed position to the image control.
    /// </summary>
    private void ApplyPosition()
    {
      ((CompositeTransform)ImgZoom.RenderTransform).TranslateX = ImagePosition.X;
      ((CompositeTransform)ImgZoom.RenderTransform).TranslateY = ImagePosition.Y;
    }

    /// <summary>
    /// Resets the zoom to its original scale and position
    /// </summary>
    private void ResetImagePosition()
    {
      TotalImageScale = 1;
      ImagePosition = new Point(0, 0);
      ApplyScale();
      ApplyPosition();
    }

    /// <summary>
    /// Checks that dragging by the given amount won't result in empty space around the image
    /// </summary>
    private bool IsDragValid(double scaleDelta, Point translateDelta)
    {
      if (ImagePosition.X + translateDelta.X > 0 || ImagePosition.Y + translateDelta.Y > 0)
        return false;

      if ((ImgZoom.ActualWidth * TotalImageScale * scaleDelta) + (ImagePosition.X + translateDelta.X) < ImgZoom.ActualWidth)
        return false;

      if ((ImgZoom.ActualHeight * TotalImageScale * scaleDelta) + (ImagePosition.Y + translateDelta.Y) < ImgZoom.ActualHeight)
        return false;

      return true;
    }

    /// <summary>
    /// Tells if the scaling is inside the desired range
    /// </summary>
    private bool IsScaleValid(double scaleDelta)
    {
      return (TotalImageScale * scaleDelta >= 1) && (TotalImageScale * scaleDelta <= MAX_IMAGE_ZOOM);
    }

    #endregion
  }
}