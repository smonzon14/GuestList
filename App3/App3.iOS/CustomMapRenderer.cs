using App3.iOS;
using App3.Maps;
using App3.Models;
using CoreGraphics;
using MapKit;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PartyMap), typeof(CustomMapRenderer))]
namespace App3.iOS
{
    public class CustomMapRenderer : MapRenderer
    {
        UIView customPinView;
        List<PartyPin> customPins;
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
                }
            }

            if (e.NewElement != null)
            {
                var formsMap = (PartyMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                customPins = formsMap.partyPins;

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }
        
        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;
            if (annotation is MKUserLocation)
                return null;
            
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
            if (!e.View.Selected)
            {
                customPinView.RemoveFromSuperview();
                customPinView.Dispose();
                customPinView = null;
            }
            
        }

        private void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            CustomMKAnnotationView customView = e.View as CustomMKAnnotationView;
            customPinView = new UIView();

            if (customView.Name.Equals("Xamarin"))
            {
                customPinView.Frame = new CGRect(0, 0, 200, 84);
                var image = new UIImageView(new CGRect(0, 0, 200, 84));
                image.Image = UIImage.FromFile("xamarin.png");
                customPinView.AddSubview(image);
                customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75)); //75
                e.View.AddSubview(customPinView);
            }
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