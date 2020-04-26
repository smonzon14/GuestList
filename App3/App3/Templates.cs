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
        public static ViewCell partyCell()
        {
            Color defaultBGColor = Color.Aqua;
            Color defaultTextColor = Color.White;
            int defaultFontSize = 30;

            Label nameLabel = new Label();
            nameLabel.SetBinding(Label.TextProperty, "name");
            nameLabel.TextColor = defaultTextColor;
            nameLabel.FontSize = defaultFontSize;
            nameLabel.Padding = 20;

            Label descriptionLabel = new Label();
            descriptionLabel.SetBinding(Label.TextProperty, "description");
            descriptionLabel.TextColor = defaultTextColor;
            descriptionLabel.Padding = 20;
            descriptionLabel.Margin = new Thickness(0, 40, 0, 0);

            Label maxInvitesLabel = new Label();
            maxInvitesLabel.SetBinding(Label.TextProperty, "maxInvites");
            maxInvitesLabel.TextColor = defaultTextColor;
            maxInvitesLabel.Padding = 20;
            maxInvitesLabel.Margin = new Thickness(0, 80, 0, 0);

            GradientView gradient = new GradientView
            {

                VerticalOptions = LayoutOptions.FillAndExpand,
                GradientSource = new CssGradientSource { Stylesheet = "linear-gradient(to right, #614385, #516395)" }

            };

            BoxView boxView = new BoxView();
            //boxView.BackgroundColor = Color.Black;
            boxView.Opacity = 0;

            ViewCell cell = new ViewCell()
            {

                View = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 5, 0, 0),
                    Children =
                            {


                                new Frame
                                {
                                    BackgroundColor=defaultBGColor,
                                    //Opacity=0,
                                    CornerRadius=20,
                                    Padding=0,
                                    WidthRequest=300,
                                    VerticalOptions=LayoutOptions.StartAndExpand,
                                    Content = new Grid
                                    {

                                        BackgroundColor=defaultBGColor,
                                        Opacity=0,
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
        public static View partyCell(Party party)
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
                return partyCell();
            });
        }
    }
    
}
