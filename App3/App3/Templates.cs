using App3.Models;
using MagicGradients;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace App3
{
    public static class Templates
    {
        static Random rand = new Random();
        public static Color GetRandomColor()
        {
            int hue = rand.Next(0,255);
            Color color = Color.FromHsla(
                (hue / 255.0f),
                0.7f,
                0.5f);
            return color;
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
                FontAttributes=FontAttributes.Italic,
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
            
            

            string c1 = GetRandomColor().ToHex().ToString();
            string c2 = GetRandomColor().ToHex().ToString();
            string style = "linear-gradient(to right, " + c1 + ", " + c2 + ")";
            GradientView gradient = new GradientView
            {

                VerticalOptions = LayoutOptions.FillAndExpand,
                GradientSource = new CssGradientSource { Stylesheet = style}

            };
            
            

            RowDefinitionCollection infoGridRowDefinitions = new RowDefinitionCollection();
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            infoGridRowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

            Grid infoGrid = new Grid
            {
                RowDefinitions = infoGridRowDefinitions,
                BackgroundColor = defaultBGColor,
                //Opacity=0,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0,
                

            };
            infoGrid.Children.Add(gradient, 0, 1, 0, 3);
            infoGrid.Children.Add(nameLabel, 0, 0);
            infoGrid.Children.Add(whereLabel, 0, 1);
            infoGrid.Children.Add(descriptionLabel, 0, 2);
            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Padding = new Thickness(10, 20, 0, 0),
                HorizontalOptions = LayoutOptions.Center,
                Children =
                        {


                            new Frame
                            {
                                HasShadow=false,
                                BackgroundColor=defaultBGColor,
                                //Opacity=0,
                                CornerRadius=30,
                                Padding=0,
                                WidthRequest=300,
                                HorizontalOptions=LayoutOptions.Center,
                                VerticalOptions=LayoutOptions.StartAndExpand,
                                Content = infoGrid


                            }

                        }
            };
            
        }
        private static View partyCell(Party party)
        {
            Color defaultBGColor = Color.Aqua;
            Color defaultTextColor = Color.White;
            int defaultFontSize = 30;

            Label nameLabel = new Label();
            nameLabel.Text = party.name;
            nameLabel.TextColor = defaultTextColor;
            nameLabel.FontSize = defaultFontSize;
            nameLabel.Padding = 20;

            Label descriptionLabel = new Label();
            descriptionLabel.Text = party.description;
            descriptionLabel.TextColor = defaultTextColor;
            descriptionLabel.Padding = 20;
            descriptionLabel.Margin = new Thickness(0, 40, 0, 0);

            Label maxInvitesLabel = new Label();
            maxInvitesLabel.Text = party.maxInvites.ToString();
            maxInvitesLabel.TextColor = defaultTextColor;
            maxInvitesLabel.Padding = 20;
            maxInvitesLabel.Margin = new Thickness(0, 80, 0, 0);

            GradientView gradient = new GradientView
            {

                VerticalOptions = LayoutOptions.FillAndExpand,
                GradientSource = new CssGradientSource { Stylesheet = "linear-gradient(to right, #614385, #516395)" }

            };

            BoxView boxView = new BoxView();
            boxView.BackgroundColor = Color.Black;

            View cell = new ContentView()
            {
                
                Content = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 5, 0, 0),
                    Children =
                            {


                                new Frame
                                {
                                    BackgroundColor=defaultBGColor,

                                    CornerRadius=20,
                                    Padding=0,
                                    WidthRequest=300,
                                    VerticalOptions=LayoutOptions.StartAndExpand,
                                    Content = new Grid
                                    {

                                        BackgroundColor=defaultBGColor,
                                        VerticalOptions = LayoutOptions.Center,
                                        Padding = 0,

                                        Children =
                                        {
                                            gradient,
                                            nameLabel,
                                            descriptionLabel,
                                            maxInvitesLabel

                                        }

                                    }


                                },
                                boxView

                            }
                }
            };

            return cell;
        }
        public static DataTemplate PartyObjectUI()
        {
            
            return new DataTemplate(() =>
            {
                
                return partyLayout();
                
            });
            
        }
        public static DataTemplate friendDescriptionLayout()
        {
            return new DataTemplate(() =>
            {
                return friendCell();
            });
        }
        
    }
    
}
