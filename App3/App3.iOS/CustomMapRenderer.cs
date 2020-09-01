using App3.iOS;
using App3.Models;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using System.Diagnostics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PartyMap), typeof(CustomMapRenderer))]
namespace App3.iOS
{
    public class CustomMapRenderer : MapRenderer
    {
        UIView customPinView;
        PartyMap formsMap;
        bool newPinSelected;
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap != null)
                {
                    nativeMap.RemoveAnnotations(nativeMap.Annotations);
                    nativeMap.GetViewForAnnotation = null;
                    nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                    nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
                    nativeMap.RegionChanged -= NativeMap_RegionChanged;
                }
            }

            if (e.NewElement != null)
            {
                formsMap = (PartyMap)e.NewElement;

                OverrideUserInterfaceStyle = UIUserInterfaceStyle.Dark;
                var nativeMap = Control as MKMapView;
                nativeMap.ShowsCompass = false;
                nativeMap.ShowsUserLocation = true;

                formsMap.CallToNativeMethod += (sender, ev) =>
                {
                    newPinSelected = true;
                };
                nativeMap.RegionChanged += NativeMap_RegionChanged;
                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }


        private void NativeMap_RegionChanged(object sender, MKMapViewChangeEventArgs e)
        {
            if (newPinSelected)
            {
                SelectCurrentPin();
                newPinSelected = false;
            }

        }

        private void SelectCurrentPin()
        {

            var pin = formsMap.selectedPin;
            if (pin == null) return;
            var nativeMap = Control as MKMapView;
            var pinLocation = new CLLocationCoordinate2D { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };

            var annotations = nativeMap.GetAnnotations(nativeMap.VisibleMapRect);
            MKPointAnnotation annotation = null;
            foreach (NSObject anno in annotations)
            {
                if (anno is MKPointAnnotation an)
                {
                    if (pinLocation.Equals(an.Coordinate))
                    {
                        annotation = an;
                        break;
                    }

                }
            }
            if (annotation == null) Debug.WriteLine("Annotation not found");
            else nativeMap.SelectAnnotation(annotation, true);
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {

            MKAnnotationView annotationView;
            if (annotation == null || annotation.GetTitle() == null) return null;
            if (annotation.GetTitle().Equals("My Location"))
            {
                return null;
            }

            annotationView = mapView.DequeueReusableAnnotation(annotation.GetTitle());
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, annotation.GetTitle());
                annotationView.Image = UIImage.FromFile("heatpin.png");
                annotationView.CalloutOffset = new CGPoint(0, 0);
                ((CustomMKAnnotationView)annotationView).Name = annotation.GetTitle();

            }
            annotationView.CanShowCallout = true;

            return annotationView;
        }

        private void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (!e.View.Selected && customPinView != null)
            {
                customPinView.RemoveFromSuperview();
                customPinView.Dispose();
                customPinView = null;
            }

        }

        private void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {

            //CustomMKAnnotationView customView = e.View as CustomMKAnnotationView;
            //customPinView = new UIView();
            /*
            if (customView.Name.Equals("Xamarin"))
            {
                customPinView.Frame = new CGRect(0, 0, 200, 84);
                var image = new UIImageView(new CGRect(0, 0, 200, 84));
                image.Image = UIImage.FromFile("xamarin.png");
                customPinView.AddSubview(image);
                customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75)); //75
                e.View.AddSubview(customPinView);
            }*/
        }

    }

    internal class CustomMKAnnotationView : MKAnnotationView
    {
        public CustomMKAnnotationView(IMKAnnotation annotation, object name)
        {

            Annotation = annotation;
            Name = name;

        }

        public object Name { get; internal set; }
    }


}