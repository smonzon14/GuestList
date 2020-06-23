using App3.Models;
using MagicGradients;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace App3
{
    public static class Templates
    {
        static Random rand = new Random();
        public static List<string> GetRandomHexColor()
        {
            int hue = rand.Next(0, 255);
            Color c1 = Color.FromHsla(
                (hue / 255.0f),
                0.5f,
                0.5f);

            hue -= 10;
            if (hue < 0) hue += 255;
            
            Color c2 = Color.FromHsla(
                (hue / 255.0f),
                0.5f,
                0.5f, 0.5);
            return new List<string> {c1.ToHex(),c2.ToHex() };
        }
        private static ViewCell friendCell()
        {

            Label nameLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                FontSize = 20

            };
            nameLabel.SetBinding(Label.TextProperty, "name");

            Label activity = new Label
            {
                FontAttributes = FontAttributes.Italic,
                TextColor = Color.Gray,
                FontSize = 15

            };
            activity.SetBinding(Label.TextProperty, "activity");

            StackLayout info = new StackLayout
            {
                Padding = new Thickness(20,0,0,0),
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    nameLabel,
                    activity
                }
            };

            Image img = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = "Profile"
            };

            Frame profileImage = new Frame
            {
                WidthRequest = 50,
                HeightRequest = 50,
                CornerRadius = 25,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0,
                IsClippedToBounds = true,
                Content = img
            };


            return new ViewCell
            {

                View = new StackLayout
                {
                    Padding = 10,
                    BackgroundColor = Color.Transparent,
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        profileImage,
                        info
                    }
                }
            };
        }
        
        private static ViewCell commentCell()
        {
            Label nameLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                FontSize = 12

            };
            nameLabel.SetBinding(Label.TextProperty, "username");

            Label comment = new Label
            {
                TextColor = Color.Gray,
                FontSize = 12
            };
            comment.SetBinding(Label.TextProperty, "comment");
            
            StackLayout info = new StackLayout
            {
                Padding = 0,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    nameLabel,
                    comment
                }
            };

            Image img = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = "Profile"
            };

            Frame profileImage = new Frame
            {
                WidthRequest = 20,
                HeightRequest = 20,
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0,
                IsClippedToBounds = true,
                Content = img
            };


            return new ViewCell
            {

                View = new StackLayout
                {
                    Padding = 10,
                    BackgroundColor = Color.Transparent,
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        profileImage,
                        info
                    }
                }
            };
        }

        private static StackLayout partyLayout()
        {
            System.Diagnostics.Debug.WriteLine("PartyLayout");
            Color defaultBGColor = Color.Aqua;
            Color defaultTextColor = Color.White;
            int defaultFontSize = 25;

            Label nameLabel = new Label();
            nameLabel.SetBinding(Label.TextProperty, "name");
            nameLabel.TextColor = defaultTextColor;
            nameLabel.FontSize = defaultFontSize;
            nameLabel.Padding = 20;


            Label whereLabel = new Label()
            {
                FontAttributes = FontAttributes.Italic,
                FontSize = 18,
                TextColor = defaultTextColor,
                Padding = new Thickness(20, 0, 20, 0),
                LineBreakMode= LineBreakMode.TailTruncation
            };
            whereLabel.SetBinding(Label.TextProperty, "address");

            Label descriptionLabel = new Label();
            descriptionLabel.SetBinding(Label.TextProperty, "description");
            descriptionLabel.TextColor = defaultTextColor;
            descriptionLabel.Padding = new Thickness(20,10,20,0);
            descriptionLabel.LineBreakMode = LineBreakMode.TailTruncation;
            descriptionLabel.MaxLines = 2;

            Label date = new Label();
            date.SetBinding(Label.TextProperty, new Binding("time", stringFormat: "{0:d} @ {0:h:mm tt}"));
            date.TextColor = Color.White;
            date.FontAttributes = FontAttributes.Bold;
            date.Padding = new Thickness(20, 0, 20, 0);
            date.FontSize = 18;

            List<string> colors = GetRandomHexColor();
            string c3 = "#00000000";//GetRandomColor().ToHex().ToString();
            string style = "linear-gradient(135deg, " + colors[0] + " 0%, " + colors[1] + " 50%, " + c3 + " 90%)";
            //string style = "linear-gradient(to top, #7f000000, #7f000000)";
            GradientView gradient = new GradientView
            {

                VerticalOptions = LayoutOptions.FillAndExpand,
                GradientSource = new CssGradientSource { Stylesheet = style }

            };



            RowDefinitionCollection infoGridRowDefinitions = new RowDefinitionCollection();
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });

            ColumnDefinitionCollection infoGridColumnDefinitions = new ColumnDefinitionCollection();
            infoGridColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            infoGridColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            Grid infoGrid = new Grid
            {
                RowSpacing = 0,
                RowDefinitions = infoGridRowDefinitions,
                ColumnDefinitions = infoGridColumnDefinitions,
                BackgroundColor = defaultBGColor,
                //Opacity=0,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0,


            };
            
            var img = new Image
            {
                HorizontalOptions = LayoutOptions.Fill,
                Aspect=Aspect.AspectFill,
                
                Source = "https://blogmedia.evbstatic.com/wp-content/uploads/wpmulti/sites/8/shutterstock_199419065.jpg"
            };

            infoGrid.Children.Add(img, 0, 2, 0, 4);
            infoGrid.Children.Add(gradient, 0, 2, 0, 4);

            infoGrid.Children.Add(nameLabel, 0, 0);
            infoGrid.Children.Add(date, 0, 2, 1, 2);
            infoGrid.Children.Add(whereLabel, 0, 2, 2, 3);
            infoGrid.Children.Add(descriptionLabel, 0, 2, 3, 4);

            return new StackLayout
            {
                
                
                Children = { new Frame
                    {

                        HasShadow = false,
                        BackgroundColor = defaultBGColor,
                        //Opacity=0,
                        CornerRadius = 30,
                        Padding = 0,
                        WidthRequest = 350,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.StartAndExpand,
                        Content = infoGrid
                    }
                }
            };

        }
        public static DataTemplate PartyObjectUI()
        {
            
            return new DataTemplate(() => { return partyLayout(); });
        }
        public static DataTemplate friendDescriptionLayout()
        {
            return new DataTemplate(() => { return friendCell(); });
        }
        public static DataTemplate commentLayout()
        {
            return new DataTemplate(() =>
            {
                return commentCell();
            });
        }

    }

}
