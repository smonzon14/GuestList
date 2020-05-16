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
                0.7f,
                0.5f);

            hue -= 50;
            if (hue < 0) hue += 255;
            
            Color c2 = Color.FromHsla(
                (hue / 255.0f),
                0.7f,
                0.5f, 0.75);
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
                Padding = 0,
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
                    BackgroundColor = Color.Black,
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
                    BackgroundColor = Color.Black,
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
            int defaultFontSize = 30;

            Label nameLabel = new Label();
            nameLabel.SetBinding(Label.TextProperty, "name");
            nameLabel.TextColor = defaultTextColor;
            nameLabel.FontSize = defaultFontSize;
            nameLabel.Padding = new Thickness(20, 30, 20, 20);
            nameLabel.FontAttributes = FontAttributes.Bold;


            Label whereLabel = new Label()
            {
                FontAttributes = FontAttributes.Italic,
                TextColor = defaultTextColor,
                Padding = 20


            };
            whereLabel.SetBinding(Label.TextProperty, "address");

            Label descriptionLabel = new Label();
            descriptionLabel.SetBinding(Label.TextProperty, "description");
            descriptionLabel.TextColor = defaultTextColor;
            descriptionLabel.Padding = 20;



            List<string> colors = GetRandomHexColor();
            string c3 = "#00000000";//GetRandomColor().ToHex().ToString();
            string style = "linear-gradient(135deg, " + colors[0] + " 0%, " + colors[1] + " 50%, " + c3 + " 100%)";
            GradientView gradient = new GradientView
            {

                VerticalOptions = LayoutOptions.FillAndExpand,
                GradientSource = new CssGradientSource { Stylesheet = style }

            };



            RowDefinitionCollection infoGridRowDefinitions = new RowDefinitionCollection();
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
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

            Frame frame = new Frame
            {
                
                HasShadow = false,
                BackgroundColor = defaultBGColor,
                //Opacity=0,
                CornerRadius = 30,
                Padding = 0,
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Content = infoGrid

            };
            
            var img = new Image
            {
                HorizontalOptions = LayoutOptions.Fill,
                Aspect=Aspect.AspectFill,
                
                Source = "https://blogmedia.evbstatic.com/wp-content/uploads/wpmulti/sites/8/shutterstock_199419065.jpg"
            };

            var count = new Label {
                FontSize = 12,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = defaultTextColor,
                Padding = new Thickness(10, 10, 0, 10)
            };
            count.SetBinding(Label.TextProperty, "numPeopleGoing");

            var goingCountStack = new StackLayout
            {
                BackgroundColor = Color.Black,
                Orientation = StackOrientation.Horizontal,
                Padding = 0,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    count,
                    new Label{ Text = "going", 
                        FontSize = 12,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = defaultTextColor}
                    
                }
            };

            infoGrid.Children.Add(img, 0, 2, 0, 3);
            infoGrid.Children.Add(gradient, 0, 2, 0, 3);
            infoGrid.Children.Add(nameLabel, 0, 0);
            infoGrid.Children.Add(goingCountStack, 1, 0);
            infoGrid.Children.Add(whereLabel, 0, 2, 1, 2);
            infoGrid.Children.Add(descriptionLabel, 0, 2, 2, 3);
            return new StackLayout
            {
                
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Padding = new Thickness(10, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Center,
                Children = { frame }
            };

        }
        public static DataTemplate PartyObjectUI()
        {
            return new DataTemplate(() => { return partyLayout(); });
        }
        public static DataTemplate friendDescriptionLayout()
        {
            return new DataTemplate(() =>
            {
                return friendCell();
            });
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
